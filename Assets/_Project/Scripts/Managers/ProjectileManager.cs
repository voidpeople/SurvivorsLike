using SurvivorsLike;
using System;
using UnityEngine;


namespace SurvivorsLike
{
    public class ProjectileManager : SingletonMonoBehaviour<ProjectileManager>
    {
        private const int MaxProjectiles = 256;
        private readonly ProjectileBase[] _activeProjectiles = new ProjectileBase[MaxProjectiles];
        private int _activeCount;

        protected override void ChildAwake()
        {
            _activeCount = 0;
        }

        public void Register(ProjectileBase p)
        {
            if(_activeCount >= MaxProjectiles)
                return;

            _activeProjectiles[_activeCount++] = p;
        }

        public void UnRegister(ProjectileBase p)
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
            for (int ii = 0; ii < _activeCount; ++ii)
            {
            }
        }
    }
}
