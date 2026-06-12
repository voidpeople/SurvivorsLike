using R3;


namespace SurvivorsLike
{

    //인게임 씬의 주요 게임 흐름 이벤트들을 한 곳에 모아놓은 이벤트 버스 클래스
    //GameManager에서 이벤트 발행 할 경우 - InGameEventBus.OnInGameStart.OnNext(Unit.Default);
    //EnemyController등에서 이벤트 구독할 경우 - InGameEventBus.OnInGameStart
    //                                             .Subscribe(_ => HandleGameStart())
    //                                             .AddTo(this);   // ← MonoBehaviour가 파괴되면 자동 해제
    public static class InGameEventBus
    {
        //Subject<T> - 이벤트를 발행하고 구독도 받을 수 있는 이벤트 채널 객체
        //Unit - void와 같은 타입으로 전달할 데이터가 없을 때 사용

        //플레이어가 죽을 경우 이벤트~
        //발행 시점: 플레이어 HP가 0이 되거나 생존 시간이 초과될 때
        //구독 대상: 게임오버 UI, 결과 화면 전환, 적 스폰 중단 등
        public static readonly Subject<Unit> OnPlayerDied = new Subject<Unit>();
    }
}
