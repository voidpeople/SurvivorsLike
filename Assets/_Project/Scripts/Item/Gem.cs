using UnityEngine;

namespace SurvivorsLike
{
    // 젬 본체 — 순수 데이터/상태만 보유. 이동 로직을 갖지 않는다(매니저가 처리).
    // 스스로 플레이어를 감지하지 않는다(Update 없음). 감지·이동·수집은 GemManager가 일괄 처리.
    public class Gem : ItemBase, IPoolable
    {
        // ─── Runtime State ───────────────────────────────────────────────────
        private int  _expValue;
        private bool _isAttracting;

        // ─── Properties ──────────────────────────────────────────────────────
        public int     ExpValue     => _expValue;
        public Vector3 Position     => transform.position;
        public bool    IsAttracting => _isAttracting;

        // GemManager가 풀에서 꺼낸 직후 초기화
        public void Init(Vector3 pos, int expValue)
        {
            transform.position = pos;
            _expValue          = expValue;
            _isAttracting      = false;
        }

        public void SetAttracting() => _isAttracting = true;

        // 위치 갱신 — 이동 계산은 GemManager.Update가 담당
        public void SetPosition(Vector3 pos) => transform.position = pos;

        #region IPoolable
        public void OnSpawn() { }

        public void OnDespawn()
        {
            _isAttracting = false;
        }

        public void ReturnToPool()
        {
            PoolManager.Instance.Return(this);
        }
        #endregion
    }
}
