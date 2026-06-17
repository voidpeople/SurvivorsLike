using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.U2D;


namespace SurvivorsLike
{
    public class LobbyController : MonoBehaviour
    {
        [Header("뷰 참조")]
        [SerializeField] private LobbyTabView _tabView;
        [SerializeField] private BattlePanelView _battlePanelView;
        [SerializeField] private ChapterSelectPanelView _chapterSelectPanelView;

        [Header("아틀라스")]
        [SerializeField] private SpriteAtlas _lobbyChapterAtlas;

        private LobbyTabModel     _tabModel;
        private LobbyTabPresenter _tabPresenter;

        private BattlePanelModel     _battlePanelModel;
        private BattlePanelPresenter _battlePanelPresenter;

        private ChapterSelectPanelModel _chapterSelectPanelModel;
        private ChapterSelectPanelPresenter _chapterSelectPanelPresenter;

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

            _chapterSelectPanelModel = new ChapterSelectPanelModel(
                DataManager.Instance.ChapterDataSOList,
                UserDataManager.Instance.UserData);
            _chapterSelectPanelView.Init();
            _chapterSelectPanelPresenter = new ChapterSelectPanelPresenter(
                _chapterSelectPanelModel,
                _chapterSelectPanelView,
                OnSelectChapter);


            //챕터 패널 버튼의 이미지를 현재 선택된 챕터의 이미지로 설정하기
            _battlePanelModel = new BattlePanelModel();
            _battlePanelView.Init(_chapterSelectPanelModel.SelectedChapterData.ThumbnailSprite);
            _battlePanelPresenter = new BattlePanelPresenter(
                _battlePanelView,
                _battlePanelModel,
                _chapterSelectPanelPresenter,
                OnGameStart,
                (sceneName) => GameManager.Instance.LoadSceneAsync(sceneName),
                _ct);
        }

        private void OnSelectChapter(ChapterDataSO chapterData)
        {
            Sprite s = _lobbyChapterAtlas.GetSprite(chapterData.DisplaySpriteName);
            _battlePanelView.SetChapterPanelButtonImage(s);

            Debug.Log($"{nameof(LobbyController)}::OnSelectChapter=> Chapter selected: {chapterData.DisplayName}");
        }

        private void OnGameStart()
        {
            GameManager.Instance.ClearGameSessionData();

            if (!DataManager.Instance.PlayerDataDic.TryGetValue(1001, out PlayerData playerData))
            {
                Debug.LogError($"{nameof(LobbyController)}::OnGameStart=> PlayerData does not exist. - PlayerId: {1001}");
                return;
            }

            GameManager.Instance.CreateGameSessionData(_chapterSelectPanelModel.SelectedChapterData, playerData);
        }

        private void Destroy()
        {
            _tabModel.Dispose();

            _tabPresenter.Dispose();
            _tabView.Destroy();

            _battlePanelPresenter.Dispose();
            _battlePanelView.Destroy();

            _chapterSelectPanelPresenter.Dispose();
            _chapterSelectPanelView.Destroy();
        }
    }
}
