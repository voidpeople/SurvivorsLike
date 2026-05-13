using SurvivorsLike;
using UnityEngine;
using UnityEngine.UIElements;

public class InGameController : MonoBehaviour
{
    [Header("결과창 뷰")]
    [SerializeField] private GameResultPanelView _resultView;

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
        _resultView.Init();
        _resultPresenter = new GameResultPanelPresenter(
            _resultModel,
            _resultView);
    }

    private void Destroy()
    {
        _resultPresenter?.Dispose();
        _resultView?.Destroy();
    }
}
