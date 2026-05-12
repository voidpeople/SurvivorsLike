using Cysharp.Threading.Tasks;
using UnityEngine;


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

        private LobbyTabModel     _tabModel;
        private LobbyTabPresenter _tabPresenter;

        private BattlePanelModel     _battlePanelModel;
        private BattlePanelPresenter _battlePanelPresenter;

        private ChapterSelectPanelModel _chapterSelectPanelModel;
        private ChapterSelectPanelPresenter _chapterSelectPanelPresenter;

        private void Awake()
        {
            Init();
        }

        private async UniTaskVoid Start()
        {
            GameManager.Instance.SetGameState(GaemState.Lobby);
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

            _battlePanelModel = new BattlePanelModel();
            _battlePanelView.Init();
            _battlePanelPresenter = new BattlePanelPresenter(_battlePanelView, _battlePanelModel,
                sceneName => GameManager.Instance.LoadScene(sceneName));

            _chapterSelectPanelModel = new ChapterSelectPanelModel(DataManager.Instance.ChapterDataSOList);
            _chapterSelectPanelView.Init();
            _chapterSelectPanelPresenter = new ChapterSelectPanelPresenter(
                _chapterSelectPanelModel,
                _chapterSelectPanelView,
                OnSelectChapter
                );
        }

        private void OnSelectChapter(ChapterDataSO chapData)
        {

        }

        private void Destroy()
        {
            _tabPresenter?.Dispose();
            _tabView?.Destroy();

            _battlePanelPresenter?.Dispose();
            _battlePanelView?.Destroy();

            _chapterSelectPanelPresenter?.Dispose();
            _chapterSelectPanelView?.Destroy();
        }
    }
}
