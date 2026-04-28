using Cysharp.Threading.Tasks;
using UnityEngine;

public class PatchController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.SetGameState(GaemState.Patch);

        DelayRun().Forget();
    }

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
        await UniTask.Delay(1000, cancellationToken: this.GetCancellationTokenOnDestroy());
        Debug.Log("1초 후 실행");

        await GameManager.Instance.LoadScene("02_Title");
    }
}
