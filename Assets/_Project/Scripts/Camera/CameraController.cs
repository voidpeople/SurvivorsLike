using UnityEngine;
using DG.Tweening;

namespace SurvivorsLike
{
    public class CameraController : MonoBehaviour
    {
        // 자식 오브젝트에 붙어 있는 카메라 컴포넌트
        [SerializeField] private Camera _camera;

        [Header("Follow")]
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _offset = new Vector3(0f, 14f, -12f);
        [SerializeField] private float _smoothTime = 0.15f;

        [Header("Angle")]
        [SerializeField] private Vector3 _eulerAngles = new Vector3(-40f, 45f, 0f);

        [Header("Zoom")]
        [SerializeField] private float _defaultFOV = 65f;

        private Vector3 _velocity;

        private void Awake()
        {
            ApplySettings();
        }

        //Inspector 값 변경 시 씬 뷰에 즉시 반영
        private void OnValidate()
        {
            if (_camera == null) return;
            ApplySettings();
        }

        //카메라가 따라가고자 하는 타겟 오브젝트 설정(플레이어 캐릭터~)
        public void SetTarget(Transform target)
        {
            _target = target;
        }

        //카메라 흔들기~
        public void Shake(float duration = 0.3f, float strength = 0.3f)
        {
            _camera.transform.DOPunchPosition(new Vector3(strength, strength * 0.5f, 0f), duration, 10, 0.5f);
        }

        public void ZoomTo(float fov, float duration)
        {
            DOTween.To(
                () => { return _camera.fieldOfView; },
                (x) => { _camera.fieldOfView = x; },
                fov,
                duration)
                .SetEase(Ease.OutCubic);
        }

        public void ResetZoom(float duration = 0.5f)
        {
            ZoomTo(_defaultFOV, duration);
        }

        private void ApplySettings()
        {
            _camera.transform.localPosition = Vector3.zero;
            _camera.transform.localRotation = Quaternion.Euler(_eulerAngles);
            _camera.fieldOfView = _defaultFOV;

            //에디터 씬 뷰 미리보기 전용 (런타임은 LateUpdate가 제어)
            if (!Application.isPlaying)
                transform.position = _offset;
        }

        private void LateUpdate()
        {
            if (_target == null)
                return;

            Vector3 desired = _target.position + _offset;
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref _velocity, _smoothTime);
        }
    }
}
