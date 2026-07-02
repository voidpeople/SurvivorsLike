using UnityEngine;


namespace SurvivorsLike
{
    // 젬 드랍(①) → 자석 감지(③) → 수집 → 경험치 적립(④)을 중앙에서 일괄 처리.
    // EnemyManager / ProjectileManager 와 동일한 "배열 + _activeCount + 중앙 Update" 패턴.
    public class GemManager : MonoBehaviour
    {
        private const int MaxGems = 512;
        private const float DefaultCollectRadius = 0.5f;

        // ─── Active 목록 ─────────────────────────────────────────────────────
        private readonly Gem[] _activeGems = new Gem[MaxGems];
        private int _activeCount;

        // ─── 협력자 (InGameController에서 주입) ─────────────────────────────
        private Transform _playerTransform;
        private PlayerLevelSystem _levelSystem;
        private GemDataSO _gemDataSO;

        // ─── 자석/이동 파라미터 ───────────────────────────────────────────────
        private float _attractRadius;
        private float _collectRadius;
        private float _startMoveSpeed;
        private float _acceleration;
        private float _maxMoveSpeed;

        // sqrMagnitude 비교용 캐시 (sqrt 연산 제거)
        private float _attractSqr;
        private float _collectSqr;


        // InGameController에서 플레이어 스폰 후 호출
        public void Init(Transform playerTransform, PlayerLevelSystem levelSystem, GemDataSO gemDataSO)
        {
            Debug.Assert(playerTransform != null, $"{nameof(GemManager)}::Init=> playerTransform is null");
            Debug.Assert(levelSystem != null,     $"{nameof(GemManager)}::Init=> levelSystem is null");
            Debug.Assert(gemDataSO != null && gemDataSO.DataList != null && gemDataSO.DataList.Count > 0,
                         $"{nameof(GemManager)}::Init=> gemDataSO is null/empty");

            _playerTransform = playerTransform;
            _levelSystem     = levelSystem;
            _gemDataSO       = gemDataSO;
            _activeCount     = 0;

            // 기본값: 자석 없음 — 플레이어 콜라이더 반경에 접촉하면 즉시 픽업
            float collectRadius = DefaultCollectRadius;
            if (playerTransform.TryGetComponent(out CharacterController cc))
                collectRadius = cc.radius * 2f;

            SetMagnetParameters(
                attractRadius:  0f,
                collectRadius:  collectRadius,
                startMoveSpeed: 0f,
                acceleration:   0f,
                maxMoveSpeed:   0f);
        }

        // 자석 스킬 도입 시 외부에서 재호출하여 파라미터 덮어씀
        public void SetMagnetParameters(
            float attractRadius, float collectRadius,
            float startMoveSpeed, float acceleration, float maxMoveSpeed)
        {
            _attractRadius  = attractRadius;
            _collectRadius  = collectRadius;
            _startMoveSpeed = startMoveSpeed;
            _acceleration   = acceleration;
            _maxMoveSpeed   = maxMoveSpeed;

            _attractSqr = _attractRadius * _attractRadius;
            _collectSqr = _collectRadius * _collectRadius;
        }

        // ① 드랍 결정 + ② 스폰 — EnemyManager.OnEnemyKilled 구독 대상
        public void HandleEnemyKilled(EnemyKilledEvent evt)
        {
            SpawnGem(evt.Position, evt.DropGemType);
        }

        private void SpawnGem(Vector3 pos, GemType gemType)
        {
            // GemType.None은 드랍 없음을 의미
            if (gemType == GemType.None)
                return;

            if (_activeCount >= MaxGems)
                return;

            int idx = (int)gemType;
            if (idx < 0 || idx >= _gemDataSO.DataList.Count)
            {
                Debug.LogError($"{nameof(GemManager)}::SpawnGem=> GemData not found. - GemType: {gemType}");
                return;
            }

            GemData data = _gemDataSO.DataList[idx];
            Gem gem = PoolManager.Instance.Get<Gem>(data.PrefabKey);
            if (gem == null)
                return;

            pos.y = 0.4f;  // 바닥에서 살짝 띄움
            gem.Init(pos, data.ExpReward);
            _activeGems[_activeCount++] = gem;
        }

        // ③ 접촉 픽업(+자석 활성 시 끌림·이동) — 중앙 일괄
        private void Update()
        {
            if (_playerTransform == null)
                return;
            if (!InGameStateManager.Instance.IsPlaying)
                return;
            if (_activeCount == 0)
                return;

            float dt = Time.deltaTime;
            Vector3 playerPos = _playerTransform.position;

            for (int ii = 0; ii < _activeCount; /* 조건부 증가 */)
            {
                Gem gem = _activeGems[ii];

                Vector3 toGem = gem.Position - playerPos;
                toGem.y = 0f;                        // XZ 수평 거리로만 판정 (Y 높이차 무시)
                float sqrDist = toGem.sqrMagnitude;  // ★ Distance() 대신 sqrMagnitude

                if (sqrDist <= _collectSqr)          // 플레이어 반경 접촉 → 즉시 픽업
                {
                    Collect(gem);
                    RemoveAt(ii);                    // swap-back — ii 증가 없이 재검사
                    continue;
                }

                // _attractSqr = 0(기본)이면 끌림 발생 안 함
                if (gem.IsAttracting || (_attractSqr > 0f && sqrDist <= _attractSqr))
                {
                    gem.SetAttracting();
                    float speed = Mathf.Min(_startMoveSpeed + _acceleration * dt, _maxMoveSpeed);
                    gem.SetPosition(Vector3.MoveTowards(gem.Position, playerPos, speed * dt));
                }

                ++ii;
            }
        }

        // ④ 경험치 적립 + 풀 회수
        private void Collect(Gem gem)
        {
            _levelSystem.AddExp(gem.ExpValue);
            gem.ReturnToPool();
        }

        // EnemyManager.UnRegister와 동일한 swap-back 제거
        private void RemoveAt(int index)
        {
            _activeGems[index] = _activeGems[--_activeCount];
            _activeGems[_activeCount] = null;
        }

        private void OnDestroy()
        {
            for (int ii = 0; ii < _activeCount; ++ii)
            {
                _activeGems[ii] = null;
            }
            _activeCount = 0;
        }
    }
}
