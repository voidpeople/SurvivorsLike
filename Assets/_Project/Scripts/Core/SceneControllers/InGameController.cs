using Cysharp.Threading.Tasks;
using SurvivorsLike;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class InGameController : MonoBehaviour
{
    [Header("결과창 뷰")]
    [SerializeField] private GameResultPanelView _resultPanelView;

    private GameResultPanelModel     _resultModel;
    private GameResultPanelPresenter _resultPresenter;    

    private void Awake()
    {
        Init();
    }

    void Start()
    {
        GameManager.Instance.SetGameState(GaemState.InGame);
    }

    private void OnDestroy()
    {
        Destroy();
    }

    private void Init()
    {
        _resultModel = new GameResultPanelModel();
        _resultPanelView.Init();
        _resultPresenter = new GameResultPanelPresenter(
            _resultModel,
            _resultPanelView,
            sceneName => GameManager.Instance.LoadScene(sceneName));
    }

    private void Destroy()
    {
        _resultPresenter?.Dispose();
        _resultPanelView?.Destroy();
    }
}
