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

        //아무 값도 필요 없다면 Subject<Unit>
        //int 값이 필요 하다면 Subject<int>
        //여러 값이 필요 하다면 Subject<struct>        
        public static readonly Subject<int> OnLevelUp = new();          //새 레벨
        public static readonly Subject<int> OnSkillUp = new();          //스킬 업그레이드
        public static readonly Subject<int> OnKillCountChanged = new(); //누적 킬(집계)
        public static readonly Subject<int> OnExpChanged = new();       //경험치 갱신
        public static readonly Subject<int> OnGoldChanged = new();      //골드량 갱신
        public static readonly Subject<int> OnBossSpawned = new();      //보스 스폰 이벤트
        public static readonly Subject<int> OnBossDefeated = new();     //보스 클리어 이벤트
    }
}
