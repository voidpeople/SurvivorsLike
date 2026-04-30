using UnityEngine;

public class TitleController : MonoBehaviour
{
    void Start()
    {
        GameManager.Instance.SetGameState(GaemState.Title);
    }
}
