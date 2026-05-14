using Cysharp.Threading.Tasks;         // UniTask 2.5.10 그대로 유지
using R3;                              // UniRx 대신 R3
using System;
using System.Linq.Expressions;
using System.Threading;
using UnityEngine;


namespace SurvivorsLike
{
    public class BootstrapController : MonoBehaviour
    {
        private async UniTaskVoid Start()
        {
            GameManager.Instance.SetGameState(GameState.Bootstrap);

            //오브젝트 파괴 시 모든 비동기 로직을 멈추기 위한 토큰
            CancellationToken ct = this.GetCancellationTokenOnDestroy();

            bool isAccountReady = await AccountAsync();
            if (isAccountReady == false)
                return;

            try
            {
                PatchCheckStatus resultStat = PatchCheckStatus.UpToDate;
                int numRetry = 3;

                do
                {
                    PatchCheckResult result = await PatchCheck.CheckPatchAsync();
                    GameManager.Instance.SetPatchResult(result);

                    switch (result.Status)
                    {
                        case PatchCheckStatus.UpToDate:
                            await GameManager.Instance.LoadScene("02_Title");
                            break;

                        case PatchCheckStatus.ForcePatch:
                        case PatchCheckStatus.NeedPatch:
                            await GameManager.Instance.LoadScene("01_Patch");
                            break;

                        case PatchCheckStatus.NetworkError:
                            await SystemUIManager.Instance.ShowAlertAsync(
                                                                    "네트워크 오류",
                                                                    result.Message,
                                                                    DialogType.NetworkError,
                                                                    "재시도");
                            resultStat = PatchCheckStatus.NetworkError;
                            --numRetry;
                            break;
                        case PatchCheckStatus.ServerMaintenance:
                        case PatchCheckStatus.InvalidResponse:
                            break;
                    }

                } while ((numRetry > 0) && (resultStat == PatchCheckStatus.NetworkError));
            }
            catch (OperationCanceledException)
            {
                // 오브젝트 파괴 등으로 작업이 취소된 경우 조용히 종료 [9, 10]
                Debug.Log("Bootstrap 작업이 취소되었습니다.");
            }
            catch (Exception e)
            {
                // 그 외 예상치 못한 에러 처리 [1, 11]
                Debug.LogException(e);
            }
        }

        private async UniTask<bool> AccountAsync()
        {
            bool isAccountReady = false;
            do
            {
                isAccountReady = await AccountManager.Instance.SetupAsync();
                if (isAccountReady == false)
                {
                    bool isRetry = await SystemUIManager.Instance.ShowConfirmAsync(
                                                                            "오류",
                                                                            "인증 오류",
                                                                            DialogType.Confirm,
                                                                            "나가기",
                                                                            "재시도");
                    if (isRetry == false)
                    {
                        Application.Quit();
                        return false;
                    }
                }
            } while (isAccountReady == false);

            return true;
        }
    }
}
