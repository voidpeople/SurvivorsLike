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

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GaemState CurrentState { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Object.Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Init();
    }

    private void Init()
    {
        CurrentState = GaemState.None;
    }

    public void LoadScene(string name) => SceneManager.LoadSceneAsync(name);
    public void PauseGame() => Time.timeScale = 0f;
    public void ResumeGame() => Time.timeScale = 1f;
}
