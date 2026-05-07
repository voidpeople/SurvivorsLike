using UnityEngine;
using R3;

//MVP 패턴 - Model

public class LobbyTabModel
{
    public ReactiveProperty<LobbyTabType> CurrentTab { get; }
    = new ReactiveProperty<LobbyTabType>(LobbyTabType.Battle);

    public void SelectTab(LobbyTabType tabType)
        => CurrentTab.Value = tabType;
}
