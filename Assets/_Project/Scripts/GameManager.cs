using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;


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
    private GameSessionData _gameSessionData = new GameSessionData();
    public GameSessionData SessionData { get { return _gameSessionData; } }

    public GameState CurrentState { get; private set; }

    protected override void ChildAwake()
    {
        CurrentState = GameState.None;
    }

    public void SetGameState(GameState state)
    {
        CurrentState = state;
    }
  
    public async UniTask LoadScene(string sceneName)
    {
        await SceneLoader.Instance.LoadScene(sceneName);
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
}
