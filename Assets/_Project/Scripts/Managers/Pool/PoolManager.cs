using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;


namespace SurvivorsLike
{
    public class PoolManager : SingletonMonoBehaviour<PoolManager>
    {
        // ─── private 필드 ────────────────────────────────────────────────────
        private Dictionary<string, ObjectPool<GameObject>> _poolDic = new();
        private Dictionary<GameObject, string> _poolKeyDic = new();
        //나중에 Release을 위해 핸들 보관
        private Dictionary<string, AsyncOperationHandle<GameObject>> _asyncOpHandleDic = new();
        // 풀마다 독립적인 IPoolable 캐시 — GetComponent 반복 호출 방지
        private Dictionary<GameObject, IPoolable> _iPoolableCacheDic = new();
        // createFunc 클로저 캡처 대신 프리팹 캐시 — createFunc와 프리웜(PreCreateAsync)이 공유
        private Dictionary<string, GameObject> _prefabDic = new();
        // PreCreateAsync에서 maxSize 초과 생성(Release 즉시 Destroy 낭비) 방지용
        private Dictionary<string, int> _poolMaxSizeDic = new();


        // ─── Unity Lifecycle ─────────────────────────────────────────────────
        protected override void OnDestroy()
        {
            foreach (string poolKey in new List<string>(_poolDic.Keys))
            {
                ReleasePool(poolKey);
            }

            base.OnDestroy();
        }


        // ─── Public Methods ───────────────────────────────────────────────────
        public GameObject Get(string poolKey)
        {
            if (_poolDic.TryGetValue(poolKey, out var pool) == false)
            {
                Debug.LogError($"{nameof(PoolManager)}::Get => Unregistered key: {poolKey}");
                return null;
            }

            GameObject obj = pool.Get();
            //OnSpawn은 진짜 스폰 경로에서만 호출됨 — 프리웜(PreCreateAsync)은 이 메서드를 거치지 않음
            _iPoolableCacheDic[obj].OnSpawn();
            return obj;
        }

        //EnemyController enemy = PoolManager.Instance.Get<EnemyController>("enemy/spiderBot");
        public T Get<T>(string poolKey) where T : Component
        {
            GameObject obj = Get(poolKey);
            if (obj != null)
                return obj.GetComponent<T>();

            return null;
        }

        public void Return(IPoolable poolable)
        {
            MonoBehaviour mono = poolable as MonoBehaviour;
            //MonoBehaviour가 아닌 구현체 + 이미 파괴된 오브젝트(fake-null) 모두 방어
            if (mono == null)
            {
                Debug.LogError($"{nameof(PoolManager)}::Return=> Object is not a MonoBehaviour or has already been destroyed.");
                return;
            }

            GameObject obj = mono.gameObject;
            if (_poolKeyDic.TryGetValue(obj, out string key))
            {
                if (_poolDic.TryGetValue(key, out var pool))
                {
                    //OnDespawn은 진짜 회수 경로에서만 호출됨
                    poolable.OnDespawn();
                    pool.Release(obj);
                }
                else
                {
                    //풀이 이미 해제된 경우 — 엔트리 정리 후 파괴 (잔존 시 메모리 릭)
                    _iPoolableCacheDic.Remove(obj);
                    _poolKeyDic.Remove(obj);
                    Object.Destroy(obj);
                    Debug.LogError($"{nameof(PoolManager)}::Return=> Attempting to return an object with no existing ObjectPool - PoolKey: {key}, GameObject: {obj.name}");
                }
            }
            else
            {
                Object.Destroy(obj);
                Debug.LogError($"{nameof(PoolManager)}::Return=> Attempting to return an object with no registered PoolKey - GameObject: {obj.name}");
            }
        }

        //TODO: 추후에 대규모 오브젝트 생성시 드랍 발생할 경우 Time Budget 방식으로 수정할 것~
        //사전에 오브젝트 생성 — Get/Release 왕복 없이 "직접 생성 → 풀에 적재" 단일 패스
        //OnSpawn/OnDespawn 경로를 구조적으로 거치지 않으므로 프리웜 플래그가 필요 없음
        //매 반복이 "생성 즉시 적재"로 완결되므로 중간에 취소돼도 풀은 항상 일관 상태
        public async UniTask PreCreateAsync(
            string poolKey,
            int count,
            int batchSize = 10,
            CancellationToken ct = default)
        {
            if (_poolDic.TryGetValue(poolKey, out var pool) == false)
            {
                Debug.LogError($"{nameof(PoolManager)}::PreCreateAsync=> PreCreateAsync failed — unregistered key: {poolKey}");
                return;
            }

            //maxSize 초과분은 Release 즉시 Destroy되므로 생성 낭비 — 풀의 빈 슬롯만큼만 생성
            count = Mathf.Min(count, _poolMaxSizeDic[poolKey] - pool.CountInactive);

            for (int ii = 0; ii < count; ++ii)
            {
                //actionOnRelease의 SetActive(false)만 실행됨 — 게임 로직 콜백 없음
                pool.Release(CreateInstance(poolKey));

                //일단 기본적으로 한 프레임에 10개씩 생성~
                if ((ii + 1) % batchSize == 0)
                    await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }
        }

        //풀 생성~
        public async UniTask CreatePoolAsync(
            string poolKey,
            int defaultCapacity = 100,
            int maxSize = 300,
            CancellationToken ct = default)
        {
            //중복 방지
            if (_poolDic.ContainsKey(poolKey))
                return;

            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(poolKey);

            GameObject prefab = null;
            try
            {
                prefab = await handle.ToUniTask(cancellationToken: ct);
            }
            catch (System.OperationCanceledException)
            {
                //취소 — 핸들 해제 후 호출자에게 전파 (UniTask 관례)
                Addressables.Release(handle);
                throw;
            }
            catch (System.Exception e)
            {
                //로드 실패 — 실패한 핸들도 Release가 필요함 (Addressables 계약)
                Addressables.Release(handle);
                Debug.LogError($"{nameof(PoolManager)}::CreatePoolAsync=> Prefab load failed: {poolKey}\n{e}");
                return;
            }

            _asyncOpHandleDic.Add(poolKey, handle);
            _prefabDic.Add(poolKey, prefab);
            _poolMaxSizeDic.Add(poolKey, maxSize);

            ObjectPool<GameObject> newPool = new ObjectPool<GameObject>(
                createFunc: () => CreateInstance(poolKey),
                //풀 델리게이트는 엔진 레벨 처리(SetActive)만 담당~
                //게임 로직 콜백(OnSpawn/OnDespawn)은 Get()/Return() 공개 메서드에서 호출됨
                actionOnGet: obj => obj.SetActive(true),
                actionOnRelease: obj => obj.SetActive(false),
                actionOnDestroy: obj =>
                {
                    _iPoolableCacheDic.Remove(obj);
                    _poolKeyDic.Remove(obj);
                    Object.Destroy(obj);
                },
                //이중 Return을 에디터에서 예외로 검출 (에디터 전용 체크 — 빌드 성능 영향 없음)
                collectionCheck: true,
                //풀의 인스턴스 생성 후 생성하여 채워야 할 오브젝트 갯수
                defaultCapacity: defaultCapacity,
                //풀이 보유할 수 있는 최대 오브젝트의 수 (이 사이즈를 초과하는 오브젝트는 자동으로 Destroy가 된다.)
                maxSize: maxSize);
            _poolDic.Add(poolKey, newPool);
        }

        // ─── Private Methods ──────────────────────────────────────────────────
        //생성 로직 단일화 — createFunc(풀 부족 시)와 PreCreateAsync(프리웜)가 공유
        private GameObject CreateInstance(string poolKey)
        {
            GameObject obj = Object.Instantiate(_prefabDic[poolKey]);
            IPoolable poolable = obj.GetComponent<IPoolable>();
            if (poolable == null)
            {
                Object.Destroy(obj);
                //프리팹 구성 오류는 조용히 null을 흘리지 않고 즉시 실패 (fail-fast)
                throw new System.InvalidOperationException(
                    $"PoolManager::CreateInstance - IPoolable가 없는 프리팹~ : poolKey - {poolKey}");
            }

            _poolKeyDic.Add(obj, poolKey);
            _iPoolableCacheDic.Add(obj, poolable);

            return obj;
        }

        //인게임에서 나올 떄 해당 챕터의 오브젝트들 해제~
        public void ReleasePool(string poolKey)
        {
            if (_poolDic.TryGetValue(poolKey, out var pool))
            {
                //풀 내(비활성) 오브젝트 Destroy — actionOnDestroy가 딕셔너리 정리까지 수행
                pool.Dispose();
                _poolDic.Remove(poolKey);
            }

            //Dispose 후 남은 엔트리 = 필드에 나가 있는 활성 인스턴스
            //핸들 해제 전에 모두 파괴해야 서브에셋(메시/머티리얼) 언로드로 인한 미싱 참조 방지
            List<GameObject> aliveObjs = new();
            foreach (var pair in _poolKeyDic)
            {
                if (pair.Value == poolKey)
                    aliveObjs.Add(pair.Key);
            }
            foreach (GameObject obj in aliveObjs)
            {
                _iPoolableCacheDic.Remove(obj);
                _poolKeyDic.Remove(obj);
                Object.Destroy(obj);
            }

            if (_asyncOpHandleDic.TryGetValue(poolKey, out var handle))
            {
                //메모리 해제
                Addressables.Release(handle);
                _asyncOpHandleDic.Remove(poolKey);
            }

            _prefabDic.Remove(poolKey);
            _poolMaxSizeDic.Remove(poolKey);
        }
    }
}
