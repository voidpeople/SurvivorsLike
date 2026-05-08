using Cysharp.Threading.Tasks;
using UnityEngine;


namespace SurvivorsLike
{
    public class LobbyController : MonoBehaviour
    {
        [Header("탭 UI")]
        [SerializeField] private LobbyTabView _tabView;

        [Header("전투 패널 UI")]
        [SerializeField] private BattlePanelView _battlePanelView;

        private LobbyTabModel     _tabModel;
        private LobbyTabPresenter _tabPresenter;

        private BattlePanelModel     _battlePanelModel;
        private BattlePanelPresenter _battlePanelPresenter;

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
            _battlePanelPresenter = new BattlePanelPresenter(_battlePanelView, _battlePanelModel);
        }

        private void Destroy()
        {
            _tabPresenter?.Dispose();
            _tabView?.Destroy();

            _battlePanelPresenter?.Dispose();
            _battlePanelView?.Destroy();
        }
    }
}
