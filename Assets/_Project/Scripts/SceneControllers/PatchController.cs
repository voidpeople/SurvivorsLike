using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;


namespace SurvivorsLike
{
    public class PatchController : MonoBehaviour
    {
        private async UniTaskVoid Start()
        {
            GameManager.Instance.SetGameState(GameState.Patch);

            CancellationToken ct = this.GetCancellationTokenOnDestroy();
            try
            {
                await UniTask.Delay(1000, cancellationToken: ct);
                await GameManager.Instance.LoadSceneAsync("02_Title");
            }
            catch (OperationCanceledException)
            {
                Debug.Log("PatchController operation cancelled");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                SystemUIManager.Instance.ShowCriticalErrorDialog();
            }
        }
    }
}
