using UnityEngine;
using R3;                              // UniRx 대신 R3
using Cysharp.Threading.Tasks;         // UniTask 2.5.10 그대로 유지
using System.Threading;

public class BootstrapController : MonoBehaviour
{
    void Start()
    {
        // DelayRun()은 비동기 함수(async)이기 때문에
        // 그냥 호출하면 실행은 되지만, 결과를 기다리지 않으면 경고가 발생할 수 있음

        // .Forget()은 "결과를 기다리지 않고 실행만 하겠다"는 의미
        // → fire-and-forget 패턴
        DelayRun().Forget();
    }

    // async: 비동기 함수라는 의미 (await 사용 가능)
    // UniTaskVoid: 반환값이 없는 UniTask (이벤트처럼 "실행만 하는 용도")
    async UniTaskVoid DelayRun()
    {
        // UniTask.Delay(3000)
        // → 3000 밀리초(= 3초) 동안 대기

        // cancellationToken:
        // → "이 작업을 취소할 수 있는 신호"
        // → 이 경우, 이 GameObject가 Destroy(삭제)되면 자동으로 취소됨

        // this.GetCancellationTokenOnDestroy()
        // → MonoBehaviour가 파괴될 때 자동으로 Cancel되는 토큰을 가져옴

        // await:
        // → Delay가 끝날 때까지 여기서 "비동기적으로 대기"
        // → Unity 메인 스레드를 멈추지 않음 (게임은 계속 돌아감)
        await UniTask.Delay(3000, cancellationToken: this.GetCancellationTokenOnDestroy());
        Debug.Log("3초 후 실행");

        await GameManager.Instance.LoadScene("01_Patch");
    }
}
