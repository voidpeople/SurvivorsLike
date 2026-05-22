using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace SurvivorsLike.UI
{
    public class FloatingJoystick : JoystickBase
    {
        private Vector2 _defaultBackgroundPos; //손 가락을 떼었을 때 조이스틱이 돌아올 기본 위치

        protected override void Awake()
        {
            base.Awake();
            _defaultBackgroundPos = _backgroundRT.anchoredPosition;
        }

        protected override void OnFingerDown(PointerEventData eventData)
        {
            _backgroundRT.anchoredPosition = eventData.position;
        }

        protected override void OnFingerUp(PointerEventData eventData)
        {
            _backgroundRT.anchoredPosition = _defaultBackgroundPos;
        }
    }
}
