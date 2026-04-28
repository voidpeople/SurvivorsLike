using UnityEngine;
using UnityEngine.SceneManagement;


public enum GaemState : byte
{
    None = 0,
    Patch = 1,
    Title = 2,
    Lobby = 3,
    InGame = 4,
    Result = 5,
}

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public GaemState CurrentState { get; private set; }

    protected override void ChildAwake()
    {
        CurrentState = GaemState.None;
    }
  

    public void LoadScene(string name) => SceneManager.LoadSceneAsync(name);
    public void PauseGame() => Time.timeScale = 0f;
    public void ResumeGame() => Time.timeScale = 1f;
}
