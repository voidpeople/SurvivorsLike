using UnityEngine;


namespace SurvivorsLike
{
    public class FixedJoystick : JoystickBase
    {
        // 배경이 고정이므로 OnFingerDown / OnFingerUp override 불필요
        // JoystickBase의 공통 Vector2 계산만 그대로 사용
    }
}
