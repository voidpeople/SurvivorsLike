using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class TitleController : MonoBehaviour
{
    private async UniTaskVoid Start()
    {
        GameManager.Instance.SetGameState(GaemState.Title);
        await UniTask.Delay(TimeSpan.FromSeconds(2.0f));
        await GameManager.Instance.LoadScene("03_Lobby");
    }
}
