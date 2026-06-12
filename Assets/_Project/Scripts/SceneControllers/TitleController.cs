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
                await GameManager.Instance.LoadSceneAsync("03_Lobby");
            }
            catch (OperationCanceledException)
            {
                Debug.Log("TitleController operation cancelled");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                SystemUIManager.Instance.ShowCriticalErrorDialog();
            }
        }
    }
}
