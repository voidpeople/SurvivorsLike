using UnityEngine;
using R3;                              // UniRx 대신 R3
using Cysharp.Threading.Tasks;         // UniTask 2.5.10 그대로 유지
using System.Threading;

public enum PanelType : byte
{ 
    None = 0, 
    Patch = 1, 
    Title = 2 
}

public class BootstrapController : MonoBehaviour
{
    [SerializeField] private GameObject _patchPanel;
    [SerializeField] private GameObject _titlePanel;

    //ReactiveProperty<T>는 “값 + 이벤트(변경 알림)”를 하나로 묶은 클래스
    //즉, 단순 변수에서 끝나는 게 아니라 값이 바뀌는 순간 자동으로 구독자에게 통지해 준다.
    private readonly ReactiveProperty<PanelType> _currentPanel
        = new ReactiveProperty<PanelType>(PanelType.None);

    //DisposableBag은 여러 구독을 한번에 관리한다.
    private DisposableBag _disposables;

    void Start()
    {
        Init();

        //1.패치 서버와 통신하여 패치 여부 검사
        if (true == CanPatch())
        {
            StartPatch();
        }
        else
        {
            //타이틀 화면 띄우기
        }
    }

    void Init()
    {
        BindPanelUI();
    }

    void BindPanelUI()
    {
        //Subscribe()은 _currentPanel 값이 변경 되면 자동 호출되는 로직
        _currentPanel
            .Subscribe(
                onNext: panel =>
                {
                    _patchPanel.SetActive(panel == PanelType.Patch);
                    _titlePanel.SetActive(panel == PanelType.Title);
                },
                onErrorResume: error =>
                {
                    // 오류 발생 시에도 구독이 유지됨 (R3의 핵심 철학)
                    Debug.LogError($"[Bootstrap] 패널 상태 오류: {error.Message}");
                },
                onCompleted: result =>
                {
                    // result.IsSuccess / result.IsFailure 로 완료 상태 구분
                    if (result.IsFailure == true)
                    {
                        Debug.LogError($"[Bootstrap] 스트림 비정상 종료: {result.Exception.Message}");
                    }
                }
            )
            .AddTo(ref _disposables);
    }

    bool CanPatch()
    {
        return false;
    }

    //패치 시작
    void StartPatch()
    {

    }
}
