using R3;
using SurvivorsLike;
using UnityEngine;


namespace SurvivorsLike
{
    public enum InGameState
    {
        Ready,
        Playing,
        Paused,
        GameOver,
        StageClear
    }

    public class InGameStateManager : SingletonMonoBehaviour<GameManager>
    {
        private readonly ReactiveProperty<InGameState> _state = new(InGameState.Ready);
        private readonly CompositeDisposable _disposables = new();


        protected override bool UseDontDestroyOnLoad => false;

    }
}
