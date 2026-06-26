using R3;
using System;
using UnityEngine;


namespace SurvivorsLike
{
    public class ExpBarPresenter : IDisposable
    {
        private readonly ExpBarView _view;
        private readonly CompositeDisposable _disposables = new();

        public ExpBarPresenter(ExpBarView view, PlayerLevelSystem levelSystem)
        {
            _view = view;

            //Observable.CombineLatest 함수는 여러 개의 Observable 스트림을 하나로 합쳐서,
            //그중 어느 하나라도 값이 바뀔 때마다
            //모든 스트림의 가장 최신(latest) 값들을 묶어서 새로운 값을 반환~
            Observable.CombineLatest(
                levelSystem.CurrentExp,
                levelSystem.RequiredExp,
                (cur, req) => req > 0 ? (float)cur / req : 0f)
                .Subscribe(ratio => _view.SetExpRatio(ratio)) //ratio은 "(cur, req) => req > 0 ? (float)cur / req : 0f)"의 계산값~
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
