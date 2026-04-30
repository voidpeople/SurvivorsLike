using UnityEngine;

public class InGameController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.SetGameState(GaemState.InGame);
    }

}
