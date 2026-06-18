using SurvivorsLike;
using System;
using UnityEngine;


namespace SurvivorsLike
{
    public class ProjectileManager : MonoBehaviour
    {
        private const int MaxProjectiles = 256;
        private readonly ITickable[] _activeProjectiles = new Projectile[MaxProjectiles];
        private int _activeCount;

        private void Awake()
        {
            _activeCount = 0;
        }

        public void Register(ITickable p)
        {
            if(_activeCount >= MaxProjectiles)
                return;

            _activeProjectiles[_activeCount++] = p;
        }

        public void UnRegister(ITickable p)
        {
            for(int ii = 0; ii < _activeCount; ++ii)
            {
                if(_activeProjectiles[ii] == p)
                {
                    _activeProjectiles[ii] = _activeProjectiles[--_activeCount];
                    _activeProjectiles[_activeCount] = null;
                    return;
                }
            }
        }

        public void Clear()
        {
            for (int ii = 0; ii < _activeCount; ++ii)
            {
                _activeProjectiles[ii] = null;
            }
        }

        void Update()
        {
            if (!InGameStateManager.Instance.IsPlaying)
                return;

            float dt = Time.deltaTime;
            for (int ii = 0; ii < _activeCount; ++ii)
            {
                _activeProjectiles[ii].Tick(dt);
            }
        }

        public void Destroy()
        {
            for (int ii = 0; ii < _activeCount; ++ii)
            {
                _activeProjectiles[ii] = null;
            }
        }

        protected void OnDestroy()
        {
            Destroy();
        }
    }
}
