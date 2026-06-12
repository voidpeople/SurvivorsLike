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

        public ReadOnlyReactiveProperty<InGameState> State => _state;

        //틱 매니저용 단축 프로퍼티
        public bool IsPlaying => _state.Value == InGameState.Playing;


        protected override bool UseDontDestroyOnLoad => false;

        // ─── Unity Lifecycle ─────────────────────────────────────────────────
        private void Start()
        {
        }

        protected override void OnDestroy()
        {
            Time.timeScale = 1f;   //Paused 중 씬 전환 대비 — 전역 값만은 수동 복구 필요
            _disposables.Dispose();
            _state.Dispose();
            base.OnDestroy();
        }

        //모바일 필수 — 백그라운드 전환 시 자동 일시정지~
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && _state.Value == InGameState.Playing)
                PauseGame();
        }


        public void StartGame() => ChangeState(InGameState.Playing, from: InGameState.Ready);

        public void PauseGame()
        {
            if (ChangeState(InGameState.Paused, from: InGameState.Playing))
                Time.timeScale = 0f;
        }

        public void ResumeGame()
        {
            if (ChangeState(InGameState.Playing, from: InGameState.Paused))
                Time.timeScale = 1f;
        }

        public void EndGame() => ChangeState(InGameState.GameOver, from: InGameState.Playing);
        public void ClearStage() => ChangeState(InGameState.StageClear, from: InGameState.Playing);

        private bool ChangeState(InGameState to, InGameState from)
        {
            if (_state.Value != from)
            {
                Debug.LogWarning($"[InGameStateManager] 잘못된 상태 전이 무시: {_state.Value} → {to} (요구 출발 상태: {from})");
                return false;
            }

            _state.Value = to;
            return true;
        }

    }
}
