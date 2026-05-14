using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;


namespace SurvivorsLike
{
    public class TitleController : MonoBehaviour
    {
        private async UniTaskVoid Start()
        {
            GameManager.Instance.SetGameState(GameState.Title);

            CancellationToken ct = this.GetCancellationTokenOnDestroy();
            try
            {
                await DataManager.Instance.InitAsync(ct);
                await UniTask.Delay(TimeSpan.FromSeconds(2.0f), cancellationToken: ct);
                await GameManager.Instance.LoadSceneAsync("03_Lobby", ct);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("TitleController 작업 취소됨");
            }
        }
    }
}
