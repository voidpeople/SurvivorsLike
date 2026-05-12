using UnityEngine;

public class InGameController : MonoBehaviour
{
    void Start()
    {
        GameManager.Instance.SetGameState(GaemState.InGame);
    }

}
