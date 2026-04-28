using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum GaemState : byte
{
    None = 0,
    Bootstrap = 1,
    Patch = 2,
    Title = 3,
    Lobby = 4,
    InGame = 5,
    Result = 6,
}

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public GaemState CurrentState { get; private set; }

    protected override void ChildAwake()
    {
        CurrentState = GaemState.None;
    }
  
    public async UniTask LoadScene(string sceneName)
    {
        await SceneLoader.Instance.LoadScene(sceneName);
    }

    public void PauseGame() => Time.timeScale = 0f;
    public void ResumeGame() => Time.timeScale = 1f;
}
