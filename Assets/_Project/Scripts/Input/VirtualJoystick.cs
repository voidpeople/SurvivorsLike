using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;


namespace SurvivorsLike
{
    //추후에 여러가지 조이스틱 타입을 구현 할 경우 VirtualJoystick을 상속받아 구현할 것~
    //FloatingJoystick, FixedJoystick, ...

    public class VirtualJoystick : MonoBehaviour,
    IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private Canvas _canvas;
        //Floating Joystick 프리팹의 Background RectTransform
        [SerializeField] private RectTransform _backgroundRT;
        //Floating Joystick 프리팹의 Handle RectTransform
        [SerializeField] private RectTransform _handleRT;

        //_handleRT가 _backgroundRT 반지름 안에서 이동할 수 있는 비율 (1.0 = 100%)
        [SerializeField, Range(0.1f, 1f)] private float _handleRangeRatio = 1f;

        private Camera _canvasCamera;          //Overlay 모드면 null
        private Vector2 _defaultBackgroundPos; //손 가락을 떼었을 때 조이스틱이 돌아올 기본 위치
        private Vector2 _inputValue;           //최종 입력 값

        public Vector2 InputValue { get { return _inputValue; } }

        //현재 손가락이 눌렸는지 여부~
        public bool IsPressed { get; private set; }

        private void Awake()
        {
            //Screen Space - Overlay 모드는 카메라가 필요 없음
            if (_canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                _canvasCamera = null;
            else
                _canvasCamera = _canvas.worldCamera;

            _defaultBackgroundPos = _backgroundRT.anchoredPosition;
        }

        //손가락이 UI 영역을 처음 터치할 때 호출.
        public void OnPointerDown(PointerEventData eventData)
        {
            //Debug.Log($"OnPointerDown : {eventData.position}");

            _backgroundRT.anchoredPosition = eventData.position;
            IsPressed = true;

            //드래그 시작~
            OnDrag(eventData);
        }

        //손가락이 터치 영역 위에서 드래그될 때 매 프레임 호출.
        public void OnDrag(PointerEventData eventData)
        {
            //Debug.Log($"OnDrag : {eventData.position}");

            //현재 손가락 위치를 background 이미지의 RectTransform 좌표계로 변환
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _backgroundRT,
                    eventData.position,
                    _canvasCamera,
                    out Vector2 bgLocalPoint))
                return;

            //backgroundRT 반지름 계산 (sizeDelta.x가 지름이므로 / 2)
            float radius = _backgroundRT.sizeDelta.x * 0.5f * _handleRangeRatio;

            //handleRT가 계산된 반지름 밖으로 나가지 않도록 좌표 값을 클램프
            Vector2 clamped = Vector2.ClampMagnitude(bgLocalPoint, radius);
            _handleRT.anchoredPosition = clamped;

            //정규화: clamped / radius → 크기 0~1 범위의 방향 벡터
            _inputValue = clamped / radius;
        }

        //손가락이 떨어질 때 호출.
        public void OnPointerUp(PointerEventData eventData)
        {
            //Debug.Log($"OnPointerUp : {eventData.position}");

            //손가락이 떨어지면 모든 상태 초기화
            _inputValue = Vector2.zero;
            _handleRT.anchoredPosition = Vector2.zero;
            _backgroundRT.anchoredPosition = _defaultBackgroundPos;
            IsPressed = false;
        }
    }
}
