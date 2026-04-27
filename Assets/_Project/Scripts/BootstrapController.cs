using UnityEngine;
//using UniRx;
using Cysharp.Threading.Tasks;

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

    ////UniRx의 ReactiveProperty<T>는 “값 + 이벤트(변경 알림)”를 하나로 묶은 클래스
    ////즉, 단순 변수에서 끝나는 게 아니라 값이 바뀌는 순간 자동으로 구독자에게 통지해 준다.
    //private readonly ReactiveProperty<PanelType> _currentPanel
    //    = new ReactiveProperty<PanelType>(PanelType.None);

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
        ////Subscribe()은 _currentPanel 값이 변경 되면 자동 호출되는 로직
        //_currentPanel
        //    .Subscribe(panel =>
        //    {
        //        _patchPanel.SetActive(panel == PanelType.Patch);
        //        _patchPanel.SetActive(panel == PanelType.Title);
        //    })
        //    .AddTo(this); //BootstrapController 인스턴스가 소멸되면 Subscribe()의 델리게이트 자동 해제~
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
