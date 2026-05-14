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
                await GameManager.Instance.LoadSceneAsync("02_Title", ct);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("PatchController 작업 취소됨");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
