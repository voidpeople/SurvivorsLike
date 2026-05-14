using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.U2D;


namespace SurvivorsLike
{
    public class LobbyController : MonoBehaviour
    {
        [Header("탭 버튼 뷰")]
        [SerializeField] private LobbyTabView _tabView;

        [Header("전투 패널 뷰")]
        [SerializeField] private BattlePanelView _battlePanelView;

        [Header("챕터 선택 패널 뷰")]
        [SerializeField] private ChapterSelectPanelView _chapterSelectPanelView;

        [Header("아틀라스")]
        [SerializeField] private SpriteAtlas _lobbyChapterAtlas;

        private LobbyTabModel     _tabModel;
        private LobbyTabPresenter _tabPresenter;

        private BattlePanelModel     _battlePanelModel;
        private BattlePanelPresenter _battlePanelPresenter;

        private ChapterSelectPanelModel _chapterSelectPanelModel;
        private ChapterSelectPanelPresenter _chapterSelectPanelPresenter;

        //게임을 시작할 때 GameSessionData에 저장할 챕터 데이터
        //MVP클래스 같에 상호 참조를 허용하지 않기 때문에 상위 클래스를 통해 데이터 전달
        private ChapterDataSO _selectedChapterData;

        private CancellationToken _ct;

        private void Awake()
        {
            _ct = this.GetCancellationTokenOnDestroy();
            Init();
        }

        private void Start()
        {
            GameManager.Instance.SetGameState(GameState.Lobby);
            _tabPresenter.SelectTab(LobbyTabType.Battle);
        }

        private void OnDestroy()
        {
            Destroy();
        }

        private void Init()
        {
            _tabModel = new LobbyTabModel();
            _tabView.Init();
            _tabPresenter = new LobbyTabPresenter(_tabView, _tabModel);

            _chapterSelectPanelModel = new ChapterSelectPanelModel(DataManager.Instance.ChapterDataSOList);
            _chapterSelectPanelView.Init();
            _chapterSelectPanelPresenter = new ChapterSelectPanelPresenter(
                _chapterSelectPanelModel,
                _chapterSelectPanelView,
                OnSelectChpter);

            _battlePanelModel = new BattlePanelModel();
            _battlePanelView.Init();
            _battlePanelPresenter = new BattlePanelPresenter(
                _battlePanelView,
                _battlePanelModel,
                _chapterSelectPanelPresenter,
                OnGameStart,
                (sceneName, ct) => GameManager.Instance.LoadSceneAsync(sceneName, ct),
                _ct);
        }

        private void OnSelectChpter(ChapterDataSO chapterData)
        {
            Sprite s = _lobbyChapterAtlas.GetSprite(chapterData.displaySpriteName);
            _battlePanelView.SetChapterPanelButtonImage(s);

            //게임 시작 버튼을 클릭하면 이 챕터 데이터를 GameSessionData에 저장~
            _selectedChapterData = chapterData;

            Debug.Log($"챕터 선택 - {chapterData.displayName}");
        }

        private void OnGameStart()
        {
            GameManager.Instance.SessionData.Clear();
            GameManager.Instance.SessionData.Init(_selectedChapterData);
        }

        private void Destroy()
        {
            _tabPresenter.Dispose();
            _tabView.Destroy();

            _battlePanelPresenter.Dispose();
            _battlePanelView.Destroy();

            _chapterSelectPanelPresenter.Dispose();
            _chapterSelectPanelView.Destroy();
        }
    }
}
