using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;


//하나의 씬에서 여러 상태는 별도의 하위 스테이트를 enum으로 관리 할 것~
//LobbyState => Stores, Equipment, Battle, Challenges, Evolution
//InGameState => GamePlay, Result

public enum GaemState : byte
{
    None = 0,
    Bootstrap = 1,
    Patch = 2,
    Title = 3,
    Lobby = 4,
    InGame = 5,
}

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public GaemState CurrentState { get; private set; }

    protected override void ChildAwake()
    {
        CurrentState = GaemState.None;
    }

    public void SetGameState(GaemState state)
    {
        CurrentState = state;
    }
  
    public async UniTask LoadScene(string sceneName)
    {
        await SceneLoader.Instance.LoadScene(sceneName);
    }

    public void PauseGame() => Time.timeScale = 0f;
    public void ResumeGame() => Time.timeScale = 1f;
}
