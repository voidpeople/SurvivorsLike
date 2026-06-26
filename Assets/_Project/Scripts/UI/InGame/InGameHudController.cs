using System;
using System.Collections.Generic;
using UnityEngine;


namespace SurvivorsLike
{
    public class InGameHudController : MonoBehaviour
    {
        [Header("위젯 View")]
        [SerializeField] private ExpBarView _expBarView;

        private ExpBarPresenter _expBarpresenter;

        public void Init(PlayerLevelSystem levelSystem)
        {
            _expBarpresenter = new ExpBarPresenter(_expBarView, levelSystem);
        }

        private void OnDestroy()
        {
            _expBarpresenter.Dispose();
        }
    }
}
