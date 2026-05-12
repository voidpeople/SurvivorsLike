using Cysharp.Threading.Tasks;
using SurvivorsLike;
using System;
using System.Threading;
using UnityEngine;

public class TitleController : MonoBehaviour
{
    private async UniTaskVoid Start()
    {
        GameManager.Instance.SetGameState(GaemState.Title);

        CancellationToken ct = this.GetCancellationTokenOnDestroy();
        await DataManager.Instance.InitAsync(ct);

        await UniTask.Delay(TimeSpan.FromSeconds(2.0f), cancellationToken: ct);
        await GameManager.Instance.LoadScene("03_Lobby");
    }
}
