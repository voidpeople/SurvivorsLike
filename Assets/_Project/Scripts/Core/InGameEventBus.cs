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

        //인게임 시작 이벤트
        //발행 시점: 스테이지가 시작되고 게임 루프가 본격적으로 돌기 직전
        //구독 대상: EnemySpawner, HUD, BGM 매니저 등
        public static readonly Subject<Unit> OnInGameStart = new Subject<Unit>();

        //게임 일시정지 이벤트
        //발행 시점: 사용자가 일시정지 버튼을 누르거나 앱이 백그라운드로 전환될 때
        //구독 대상: Time.timeScale 제어, 일시정지 UI 표시, 물리/애니메이션 중단 등
        public static readonly Subject<Unit> OnInGamePause = new Subject<Unit>();

        //게임 재개 이벤트
        //발행 시점: 일시정지 상태에서 게임으로 복귀할 때
        //구독 대상: Time.timeScale 복구, 일시정지 UI 숨김 등
        public static readonly Subject<Unit> OnInGameResume = new Subject<Unit>();

        //게임 오버 이벤트
        //발행 시점: 플레이어 HP가 0이 되거나 생존 시간이 초과될 때
        //구독 대상: 게임오버 UI, 결과 화면 전환, 적 스폰 중단 등
        public static readonly Subject<Unit> OnInGameOver = new Subject<Unit>();

        //스테이지 클리어 이벤트
        //발행 시점: 스테이지의 목표(보스 처치, 생존 시간 달성 등)를 달성했을 때
        //구독 대상: 클리어 UI, 보상 처리, 다음 스테이지 로드 등
        public static readonly Subject<Unit> OnStageClear = new Subject<Unit>();
    }
}
