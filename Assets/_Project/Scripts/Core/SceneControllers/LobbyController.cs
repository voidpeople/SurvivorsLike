using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;


namespace SurvivorsLike
{
    public class LobbyController : MonoBehaviour
    {
        [Header("탭 UI")]
        [SerializeField] private LobbyTabView _tabView;

        private LobbyTabModel     _tabModel;
        private LobbyTabPresenter _tabPresenter;

        private void Awake()
        {
            Init();
        }

        private async UniTaskVoid Start()
        {
            GameManager.Instance.SetGameState(GaemState.Lobby);

            //await UniTask.Delay(TimeSpan.FromSeconds(2.0f));

            _tabPresenter.SelectTab(LobbyTabType.Battle);
        }

        private void OnDestroy()
        {
            Destroy();
        }

        void Init()
        {
            _tabModel = new LobbyTabModel();
            _tabView.Init();
            _tabPresenter = new LobbyTabPresenter(_tabView, _tabModel);
        }

        private void Destroy()
        {
            _tabPresenter?.Dispose();
            _tabView?.Destroy();
        }
    }
}
