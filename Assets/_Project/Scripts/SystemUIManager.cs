using UnityEditor.PackageManager;
using UnityEngine;

public enum SystemUIType : byte
{
    None,
    Loading,
    Error,
    NetworkError,
    Maintenance,
    ForceUpdate,
    Warning
}

public enum SystemUIActionType : byte
{
    None,
    Retry,
    GoTotitle,
    OpenStore   //새로운 버전을 받아야 하는 경우~
}

public class SystemUIData
{
    public string title;
    public string message;
    public SystemUIActionType actionType;
}


//로딩(대기) UI, 오류 팝업창, 경고 팝업창 담당 클래스~
//SystemUIManager의 모든 UI은 기본적으로 백그라운드에 투명한 이미지를 배치하여 레이캐스팅을 막는다.
//한번에 하나의 UI만 출력된다.
public class SystemUIManager : SingletonMonoBehaviour<SystemUIManager>
{
    private SystemUIType _currentUIType;

    private void Show(SystemUIType type)
    {
        _currentUIType = type;
    }

    //현재 띄워진 창 숨기기
    public void HideCurrentUI()
    {
    }

    public void ShowLoading()
    {
    }

    public void ShowNetworkError(SystemUIData data, System.Action onRetry = null)
    {
    }

    //서버 점검창 띄우기
    public void ShowMaintenance(SystemUIData data)
    {
    }

    public void ShowForceUpdate(SystemUIData data, System.Action onStore = null)
    {
    }

    //경고창 띄우기~
    public void ShowWarning(SystemUIData data)
    {
    }
}
