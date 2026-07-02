using System;
using UnityEngine;


namespace SurvivorsLike
{   
    public class EnemyManager : MonoBehaviour
    {
        private const int MaxEnemys = 350;
        private readonly ITickable[] _activeEnemys = new ITickable[MaxEnemys];
        private int _activeCount;
        public int ActiveCount => _activeCount;

        public event Action<EnemyKilledEvent> OnEnemyKilled;

        private void Awake()
        {
            _activeCount = 0;
        }

        void Update()
        {
            if (!InGameStateManager.Instance.IsPlaying)
                return;

            float dt = Time.deltaTime;
            for (int ii = 0; ii < _activeCount; ++ii)
            {
                _activeEnemys[ii].Tick(dt);
            }
        }

        protected void OnDestroy()
        {
            for (int ii = 0; ii < _activeCount; ++ii)
            {
                _activeEnemys[ii] = null;
            }
        }

        public void Register(ITickable enemy)
        {
            if (_activeCount >= MaxEnemys)
                return;

            _activeEnemys[_activeCount++] = enemy;
        }

        public void UnRegister(ITickable enemy)
        {
            for (int ii = 0; ii < _activeCount; ++ii)
            {
                if (_activeEnemys[ii] == enemy)
                {
                    _activeEnemys[ii] = _activeEnemys[--_activeCount];
                    _activeEnemys[_activeCount] = null;
                    return;
                }
            }
        }

        //적 캐릭터 사망시~
        public void NotifyKilled(in EnemyKilledEvent evt)
        {
            OnEnemyKilled?.Invoke(evt);
        }
    }
}
