using UnityEngine;
using UnityEngine.UI;

public enum LobbyTabType
{
    Stores = 0,
    Equipment,
    Battle,
    Challenge,
    Evolution
}

public class LobbyController : MonoBehaviour
{
    [SerializeField] private ToggleGroup _lobbyStateTabBtnGroup;
    [SerializeField] private Toggle[] _lobbyStateTabBtns;

    private LobbyTabType _currentLobbyTabType = LobbyTabType.Battle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.SetGameState(GaemState.Lobby);

        SetBattleState();
    }

    void SetBattleState()
    {
        _currentLobbyTabType = LobbyTabType.Battle;
        _lobbyStateTabBtns[(int)_currentLobbyTabType].isOn = true;
    }
}
