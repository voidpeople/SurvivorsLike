using R3;
using System;
using UnityEngine;


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
    public class LobbyTabModel : IDisposable
    {
        public ReactiveProperty<LobbyTabType> CurrentTab { get; }
        = new ReactiveProperty<LobbyTabType>(LobbyTabType.Battle);

        public void SelectTab(LobbyTabType tabType)
            => CurrentTab.Value = tabType;

        public void Dispose()
        {
            CurrentTab?.Dispose();
        }
    }
}
