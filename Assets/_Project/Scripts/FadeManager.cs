using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading; // CancellationToken 사용을 위해 필요

namespace SurvivorsLike
{
    //씬와 씬 전환시 화면의 페이드 인 아웃을 담당하는 매니저
    public class FadeManager : SingletonMonoBehaviour<FadeManager>
    {
        [SerializeField] private Canvas _fadeCanvas;
        [SerializeField] private Image _fadeImage;

        private void Awake()
        {
            _fadeCanvas.gameObject.SetActive(false);
        }

        //화면을 어둡게~
        //async UniTask 구문은 FadeOut함수가 비동기 유니 태스크 함수임을 의미한다.
        public async UniTask FadeOut(float duration)
        {
            _fadeCanvas.gameObject.SetActive(true);

            //DOKill()함수 - 현재 _fadeImage에 걸려있는 모든 Tween(애니메이션)을 즉시 중지한다.
            _fadeImage.DOKill();
            // 1. ToUniTask()를 사용해 최적화
            // 2. GetCancellationTokenOnDestroy()를 전달하여 파괴 시 자동 취소 [1, 3]
            await _fadeImage.DOFade(1f, duration)
                .ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());

        }

        //화면을 밝게~
        public async UniTask FadeIn(float duration)
        {
            _fadeImage.DOKill();
            await _fadeImage.DOFade(0f, duration)
                .ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());

            _fadeCanvas.gameObject.SetActive(false);
        }
    }
}
