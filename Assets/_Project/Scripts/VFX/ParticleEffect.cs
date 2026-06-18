using SurvivorsLike;
using UnityEngine;


namespace SurvivorsLike
{
    public class ParticleEffect : MonoBehaviour, IPoolable
    {
        private ParticleSystem _ps;
        

        private void Awake()
        {
            _ps = GetComponentInChildren<ParticleSystem>();
            Debug.Assert(_ps != null, $"{nameof(ParticleEffect)}::Awake=> ParticleSystem not found in children");

            ParticleStoppedBridge bridge = _ps.gameObject.GetComponent<ParticleStoppedBridge>();
            Debug.Assert(bridge != null, $"{nameof(ParticleEffect)}::Awake=> ParticleStoppedBridge not found");
            bridge.Init(this);
        }

        // 자연 종료 시 엔진이 자동 호출 — Inspector: Stop Action → Callback 필수
        private void OnParticleSystemStopped()
        {
            ReturnToPool();
        }

        public void Play()
        {
            if (_ps == null)
                return;
            // 이전 재생 잔여 파티클 제거 후 새로 재생
            _ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            _ps.Play();
        }

        public void Play(Vector3 pos)
        {
            if (_ps == null)
                return;

             transform.position = pos;

            // 이전 재생 잔여 파티클 제거 후 새로 재생
            _ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            _ps.Play();
        }

        #region IPoolable
        public void OnSpawn()
        {
        }

        public void OnDespawn()
        {
        }

        public void ReturnToPool()
        {
            PoolManager.Instance.Return(this);
        }
        #endregion
    }
}
