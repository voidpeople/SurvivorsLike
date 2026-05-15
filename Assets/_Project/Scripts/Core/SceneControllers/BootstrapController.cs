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

            try
            {
                bool isAccountReady = await AccountAsync(ct);
                if (isAccountReady == false)
                    return;
            }
            catch (OperationCanceledException)
            {
                Debug.Log("AccountAsync: 작업이 취소되었습니다.");
                return;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return;
            }

            try
            {
                PatchCheckStatus resultStat = PatchCheckStatus.UpToDate;
                int numRetry = 3;

                do
                {
                    PatchCheckResult result = await PatchCheck.CheckPatchAsync(ct);
                    GameManager.Instance.SetPatchResult(result);

                    switch (result.Status)
                    { 
                        case PatchCheckStatus.UpToDate:
                            await GameManager.Instance.LoadSceneAsync("02_Title");
                            break;

                        case PatchCheckStatus.ForcePatch:
                        case PatchCheckStatus.NeedPatch:
                            await GameManager.Instance.LoadSceneAsync("01_Patch");
                            break;

                        case PatchCheckStatus.NetworkError:
                        {
                            var tcs = new UniTaskCompletionSource();
                            SystemUIManager.Instance.ShowNetworkErrorDialog(
                                onRetry: () => tcs.TrySetResult(),
                                onQuit:  () => Application.Quit()
                            );
                            await tcs.Task;
                            resultStat = PatchCheckStatus.NetworkError;
                            --numRetry;
                            break;
                        }
                        case PatchCheckStatus.ServerMaintenance:
                        case PatchCheckStatus.InvalidResponse:
                            break;
                    }

                } while ((numRetry > 0) && (resultStat == PatchCheckStatus.NetworkError));
            }
            catch (OperationCanceledException e)
            {
                // 오브젝트 파괴 등으로 작업이 취소된 경우 조용히 종료 [9, 10]
                Debug.Log("Bootstrap 작업이 취소되었습니다.");                

                Debug.Log($"취소된 토큰: {e.CancellationToken.GetHashCode()}");
                //Debug.LogException(e);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                SystemUIManager.Instance.ShowCriticalErrorDialog();
            }
        }

        private async UniTask<bool> AccountAsync(CancellationToken ct)
        {
            bool isAccountReady = false;
            do
            {
                isAccountReady = await AccountManager.Instance.SetupAsync(ct);
                //테스트 코드
                //isAccountReady = false;
                if (isAccountReady == false)
                {
                    var tcs = new UniTaskCompletionSource<bool>();
                    SystemUIManager.Instance.ShowAuthErrorDialog(
                        onRetry: () => tcs.TrySetResult(true),
                        onQuit:  () => tcs.TrySetResult(false)
                    );
                    bool isRetry = await tcs.Task;

                    ct.ThrowIfCancellationRequested();
                    if (isRetry == false)
                    {
                        Application.Quit();
                        return false;
                    }
                }
            } while (isAccountReady == false);

            isAccountReady = false;
            string userId = AccountManager.Instance.UserId;
            do
            {
                isAccountReady = await UserDataManager.Instance.LoadUserDataAsync(userId, ct);
                if (isAccountReady == false)
                {
                    var tcs = new UniTaskCompletionSource<bool>();
                    SystemUIManager.Instance.ShowAuthErrorDialog(
                        onRetry: () => tcs.TrySetResult(true),
                        onQuit:  () => tcs.TrySetResult(false)
                    );
                    bool isRetry = await tcs.Task;

                    ct.ThrowIfCancellationRequested();
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
