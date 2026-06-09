using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SurvivorsLike
{
    //하나의 씬에서 여러 상태는 별도의 하위 스테이트를 enum으로 관리 할 것~
    //LobbyState => Stores, Equipment, Battle, Challenges, Evolution
    //InGameState => GamePlay, Result

    public enum GameState
    {
        None,
        Bootstrap,
        Patch,
        Title,
        Lobby,
        InGame
    }

    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        private GameSessionData _gameSessionData;
        public GameSessionData GameSessionData { get { return _gameSessionData; } }

        public GameState CurrentState { get; private set; }

        protected override void ChildAwake()
        {
            CurrentState = GameState.None;

            //UniTaskVoid을 사용하는 함수에서 호출되는 UniTask 비동기 함수들의 익셉션은 그냥 사라지므로
            //정확한 원인도 모르고 그냥 앱이 종료 됨~
            //따라서 UniTaskScheduler.UnobservedTaskException을 통해 그런 익셉션도
            //모두 받아서 로그를 출력하게 해야 한다.
            UniTaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }

        public void SetGameState(GameState state)
        {
            CurrentState = state;
        }

        public UniTask LoadSceneAsync(string sceneName)
        {
            CancellationToken ct = this.GetCancellationTokenOnDestroy();
            return SceneLoader.Instance.LoadSceneAsync(sceneName, ct: ct);
        }

        public void PauseGame() => Time.timeScale = 0f;
        public void ResumeGame() => Time.timeScale = 1f;

        public PatchCheckResult PatchCheckResultData { get; private set; }
        public void SetPatchResult(PatchCheckResult result)
        {
            PatchCheckResultData = result;
        }
        public void ClearPatchResult()
        {
            PatchCheckResultData = null;
        }

        public void CreateGameSessionData(ChapterDataSO chapterdata, PlayerData playerData)
        {
            ClearGameSessionData();
            _gameSessionData = new GameSessionData();
            _gameSessionData.Init(chapterdata, playerData);
        }
        public void ClearGameSessionData()
        {
            _gameSessionData?.Clear();
            _gameSessionData = null;
        }

        private void OnUnobservedTaskException(Exception ex)
        {
            Debug.LogException(ex);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UniTaskScheduler.UnobservedTaskException -= OnUnobservedTaskException;
        }
    }
}
