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
                Debug.Log("TitleController 작업 취소됨");
            }
            catch (Exception e)
            {
                Debug.LogException(e);

                await SystemUIManager.Instance.ShowAlertAsync(
                    "오류",
                    "데이터 로드에 실패했습니다.\n앱을 재시작해 주세요.",
                    DialogType.Alert,
                    "확인",
                    CancellationToken.None);
            }
        }
    }
}
