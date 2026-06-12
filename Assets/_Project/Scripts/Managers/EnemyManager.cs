using UnityEngine;


namespace SurvivorsLike
{
    public class EnemyManager : SingletonMonoBehaviour<PopupManager>
    {
        private const int MaxEnemys = 350;
        private readonly EnemyController[] _activeEnemys = new EnemyController[MaxEnemys];
        private int _activeCount;

        protected override bool UseDontDestroyOnLoad => false;

        protected override void ChildAwake()
        {
            _activeCount = 0;
        }

        public void Register(EnemyController p)
        {
            if (_activeCount >= MaxEnemys)
                return;

            _activeEnemys[_activeCount++] = p;
        }

        public void UnRegister(EnemyController p)
        {
            for (int ii = 0; ii < _activeCount; ++ii)
            {
                if (_activeEnemys[ii] == p)
                {
                    _activeEnemys[ii] = _activeEnemys[--_activeCount];
                    _activeEnemys[_activeCount] = null;
                    return;
                }
            }
        }

        public void Clear()
        {
            for (int ii = 0; ii < _activeCount; ++ii)
            {
                _activeEnemys[ii] = null;
            }
        }

        void Update()
        {
            float dt = Time.deltaTime;
            for (int ii = 0; ii < _activeCount; ++ii)
            {
                _activeEnemys[ii].Tick(dt);
            }
        }
    }
}
