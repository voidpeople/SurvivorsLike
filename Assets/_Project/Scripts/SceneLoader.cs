using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;


namespace SurvivorsLike
{
    public class SceneLoader : SingletonMonoBehaviour<SceneLoader>
    {
        //씬의 중복 로드 방지를 위해~
        private bool _isLoading = false;

        //UniTask라는 이름은 기본적으로 유니티 환경에서 비동기적으로 수행되는 하나의 작업 단위나 연산을 나타내는 타입 이름 이면서
        //이 비동기 라이브러리의 이름으로 사용됨~
        public async UniTask LoadSceneAsync(string sceneName, float fadeOutDuration = 1f, float fadeInDuration = 1f, float holdTime = 0f, CancellationToken ct = default)
        {
            if (_isLoading)
                return;
            _isLoading = true;

            try
            {
                //페이드 아웃
                //AttachExternalCancellation()함수는 비동기 작업시 SceneLoader가 파괴되는 일이 발생하면 
                //해당 비동기 작업을 즉시 멈추도록 설정하는 함수이다.
                await FadeManager.Instance.FadeOutAsync(fadeOutDuration, ct);

                AsyncOperation op = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
                if(op == null)
                {
                    Debug.LogError($"[SceneLoader] 씬을 찾을 수 없음: {sceneName}");
                    _isLoading = false;
                    return;
                }    
                op.allowSceneActivation = false;

                while (op.progress < 0.9f)
                {
                    //PlayerLoopTiming은 UniTask 라이브러리의 열거형
                    //PlayerLoopTiming은 UniTask가 유니티의 메인 루프(PlayerLoop) 내에서 어느 시점에 작업을 재개할지 결정하는 열거형(enum)이다.
                    //따라서 PlayerLoopTiming.Update을 사용한다는 것은 일반적인 MonoBehaviour.Update나 uGUI 이벤트(버튼 클릭 등)가 처리되는
                    //ScriptRunBehaviourUpdate 단계 직전에 비동기 태스크가 호출되게 설정하는 것이다.
                    await UniTask.Yield(PlayerLoopTiming.Update, ct);
                }

                //씬 활성화 및 실제 완료 대기
                op.allowSceneActivation = true;

                //아래 로직은 op.isDone이 true가 될 때까지 기다리는 로직임~
                await op.WithCancellation(ct);

                //유지 시간 대기 (Delay 사용)
                if (holdTime > 0f)
                {
                    await UniTask.Delay(Mathf.RoundToInt(holdTime * 1000f), cancellationToken: ct);
                }

                //페이드 인
                await FadeManager.Instance.FadeInAsync(fadeInDuration, ct);
            }
            finally //언제나 거침~
            {
                _isLoading = false;
            }
        }
    }

}
