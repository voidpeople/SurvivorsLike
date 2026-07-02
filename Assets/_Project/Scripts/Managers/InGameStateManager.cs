using R3;
using UnityEngine;


namespace SurvivorsLike
{
    public enum InGameState
    {
        Ready,
        Playing,
        Paused,
        LevelingUp,
        PlayerDead,
        StageFail,
        StageClear
    }

    public class InGameStateManager : SingletonMonoBehaviour<InGameStateManager>
    {
        private readonly ReactiveProperty<InGameState> _state = new(InGameState.Ready);
        private readonly CompositeDisposable _disposables = new();

        public ReadOnlyReactiveProperty<InGameState> State => _state;

        //틱 매니저용 단축 프로퍼티
        public bool IsPlaying => _state.Value == InGameState.Playing;


        protected override bool UseDontDestroyOnLoad => false;

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
#if !UNITY_EDITOR
            if (pauseStatus && _state.Value == InGameState.Playing)
                PauseGame();

            if (!pauseStatus && _state.Value == InGameState.Paused)
                ResumeGame();
#endif
        }

        public void StartBattle() => ChangeState(from: InGameState.Ready, to: InGameState.Playing);
        public void PauseGame()
        {
            if (ChangeState(from: InGameState.Playing, to: InGameState.Paused))
                Time.timeScale = 0f;
        }
        
        public void ResumeGame()
        {
            if (ChangeState(from: InGameState.Paused, to: InGameState.Playing))
                Time.timeScale = 1f;
        }

        public void EnterLevelingUp()
        {
            if (ChangeState(from: InGameState.Playing, to: InGameState.LevelingUp))
                Time.timeScale = 0f;
        }

        public void ExitLevelingUp()
        {
            if (ChangeState(from: InGameState.LevelingUp, to: InGameState.Playing))
                Time.timeScale = 1f;
        }

        //플레이어 캐릭터 사망
        public void PlayerDead()
        {
            if (ChangeState(from: InGameState.Playing, to: InGameState.PlayerDead))
                Time.timeScale = 0f;
        }

        //플레이어 캐릭터 소생
        public void PlayerRevive()
        {
            if (ChangeState(from: InGameState.PlayerDead, to: InGameState.Playing))
                Time.timeScale = 1f;
        }

        public void ClearStage()
        {
            ChangeState(from: InGameState.Playing, to: InGameState.StageClear);
        }

        public void FailStage()
        {
            ChangeState(from: InGameState.Playing, to: InGameState.StageFail);
        }

        private bool ChangeState(InGameState from, InGameState to)
        {
            if (_state.Value != from)
            {
                Debug.LogWarning($"{nameof(InGameStateManager)}::ChangeState=> Invalid state transition ignored: {_state.Value} → {to} (required from state: {from})");
                  return false;
            }

            _state.Value = to;
            return true;
        }

    }
}
