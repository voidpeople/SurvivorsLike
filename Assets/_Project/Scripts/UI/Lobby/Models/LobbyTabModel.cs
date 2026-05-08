using UnityEngine;
using R3;


namespace SurvivorsLike
{
    public enum LobbyTabType
    {
        Stores = 0,
        Equipment,
        Battle,
        Challenge,
        Evolution
    }

    //MVP 패턴 - Model
    public class LobbyTabModel
    {
        public ReactiveProperty<LobbyTabType> CurrentTab { get; }
        = new ReactiveProperty<LobbyTabType>(LobbyTabType.Battle);

        public void SelectTab(LobbyTabType tabType)
            => CurrentTab.Value = tabType;
    }
}
