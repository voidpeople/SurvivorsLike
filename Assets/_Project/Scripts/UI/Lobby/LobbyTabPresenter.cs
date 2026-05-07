using R3;
using System;
using UnityEngine;


namespace SurvivorsLike
{
    //MVP 패턴 - Presenter 

    //명시적으로 자원을 해제 해야 할 것이 있다면 IDisposable 인터페이스를 상속받아서
    //오버라이드 한 Dispose()함수에서 자원의 해제를 해준다.
    //여기서는 R3의 Subscribe 구독을 명시적으로 해제하기 위해 사용~
    public class LobbyTabPresenter : IDisposable
    {
        private readonly LobbyTabModel _model;
        private readonly LobbyTabView _view;

        //R3 Subscribe 구독들을 한 바구니에 담아서 한 번에 해제하는 컨테이너 변수 선언
        private readonly CompositeDisposable _disposables = new();

        //생성자
        public LobbyTabPresenter(LobbyTabView view, LobbyTabModel model)
        {
            _view = view;
            _model = model;

            //모델의 CurrentTab 깂이 수정되면
            //뷰의 SetTabSelect함수를 호출하도록 설정
            //_disposables을 추가하는 이유는 인스턴스 소멸시 _disposables을 통해
            //_model의 R3 Subscribe 구독을 해제하기 위해서 추가~
            _model.CurrentTab
                .Subscribe(tabType => _view.SetTabSelect(tabType))
                .AddTo(_disposables);

            //사용자의 입력으로 뷰가 갱신되면 OnTabClickedHandler 함수로 통보 받는다.
            _view.OnTabClicked += OnTabClickedHandler;
        }

        //코드로 탭 전환시 이 함수를 호추한다. 그러면 모델의 값이 수정되고
        //모델의 값이 수정되면 모델의 CurrentTab Subscribe을 통해
        //뷰의 SetTabSelect함수를 통해 뷰를 갱신하게 된다.
        //모델 => 뷰
        public void SelectTab(LobbyTabType tabType)
            => _model.SelectTab(tabType);

        //사용자의 입력에 위해 뷰가 갱신되면 OnTabClickedHandler함수로 통보 받는다.
        private void OnTabClickedHandler(LobbyTabType tabType)
            => _model.SelectTab(tabType);


        //Dispose()함수는 인스턴스 종료시 외부에서 명시적으로 호출해 주어야 한다.
        //인스턴스 종료시 자원들 해제하기
        public void Dispose()
        {
            _view.OnTabClicked -= OnTabClickedHandler;
            _disposables.Dispose();
        }
    }
}
