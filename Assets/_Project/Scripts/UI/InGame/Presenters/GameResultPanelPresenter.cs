using System;
using UnityEngine;


namespace SurvivorsLike
{
    public class GameResultPanelPresenter : IDisposable
    {
        private readonly GameResultPanelModel _model;
        private readonly GameResultPanelView _view;

        public GameResultPanelPresenter(
            GameResultPanelModel model,
            GameResultPanelView view)
        {
            _model = model;
            _view = view;

            _view.OnResultConfirmed += OnResultConfirmed;
        }

        private void OnResultConfirmed()
        {
            //결과창은 그대로 남겨 놓은 상태에서 InGameController 통보하면
            //InGameController가 로비씬을 로딩한다.
        }

        public void Dispose()
        {
        }
    }
}
