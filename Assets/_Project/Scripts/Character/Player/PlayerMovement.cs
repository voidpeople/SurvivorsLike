using Unity.Android.Gradle.Manifest;
using UnityEngine;


namespace SurvivorsLike
{
    public class PlayerMovement : MonoBehaviour
    {
        //_moveSpeedMultiplier값 곱셈 전 기본 이동속도 (단위: m/s)
        [SerializeField] private float _baseMoveSpeed = 5f;

        private CharacterController _characterCtrl;
        private Vector2 _moveDir;
        private Transform _camTrans;

        //프레임마다 누적되는 수직(Y축) 속도 — 중력·점프에 사용
        private float _verticalVelocity;
        //이동속도에 곱하는 배율 (1 = 기본, 0.5 = 절반, 2 = 두 배)
        //버프 용도임~
        private float _moveSpeedMultiplier = 1f;
        //이동 가능 여부
        private bool _canMove = false;

        // 현재 입력 크기 (0~1). 애니메이터의 Speed 파라미터로 전달하는 용도
        public float NormalizedSpeed { get; private set; }
        //작은 조이스틱 움직임에도 애니메이션 곧바로 플레이 되게 하기 위해 추가~
        public float AnimatorSpeed => IsMoving ? 1f : 0f;

        // 입력 벡터의 제곱 크기가 임계값 초과 시 true — 제곱 비교로 sqrt 연산 생략
        public bool IsMoving => _moveDir.sqrMagnitude > 0.001f;

        private void Awake()
        {
            // 같은 GameObject의 CharacterController를 1회 캐싱
            TryGetComponent(out _characterCtrl);

            _camTrans = Camera.main.transform;
        }

        //이동 활성화/ 비 활성화
        public void SetMove(bool canMove)
        {
            _canMove = canMove;
            if (canMove == false)
            {
                _moveDir = Vector2.zero;
                NormalizedSpeed = 0f;
            }
        }

        //조이스틱/키보드의 매 프레임 입력 방향 벡터 설정
        public void SetInputDirection(Vector2 input)
        {
            _moveDir = input;
        }


        //이동속도 배율을 설정 (버프·디버프 용으로 이용~)
        //Mathf.Max(0)으로 음수 배율(역방향 이동)을 원천 차단
        public void SetMoveSpeedMultiplier(float multiplier)
        {
            _moveSpeedMultiplier = Mathf.Max(0f, multiplier);
        }

        private void Update()
        {
            ApplyMovement();
            ApplyRotation();
        }

        // 수평 이동 벡터를 계산하고 GroundStickForce와 합산하여 CharacterController에 전달
        private void ApplyMovement()
        {
            Vector3 horizontalMotion = Vector3.zero;
            //방향 벡터의 크기인 _dir.magnitude 값을 0에서 1 사이의 값으로 잘라낸다.
            float inputMagnitude = Mathf.Clamp01(_moveDir.magnitude);

            if ((_canMove == true) && (inputMagnitude > 0.001f))
            {
                Vector2 normalizedInput = _moveDir / inputMagnitude;
                Vector3 moveDir = GetCameraRelativeMoveDir(normalizedInput);
                float speed = _baseMoveSpeed * _moveSpeedMultiplier;
                // 애니메이터에 전달할 속도 비율 (0~1)
                NormalizedSpeed = inputMagnitude;
                horizontalMotion = moveDir * (speed * Time.deltaTime);
            }
            else
            {
                NormalizedSpeed = 0f;
            }

            //중력은 _canMove와 무관하게 항상 적용 (사망 시 공중 부양 방지)
            horizontalMotion.y = _verticalVelocity * Time.deltaTime;

            //CharacterController에 최종 이동 벡터를 전달하는 유일한 지점
            _characterCtrl.Move(horizontalMotion);
        }

        private void ApplyRotation()
        {
            if (IsMoving == true)
            {
                //방향 전환은 즉시 전환
                float inputMagnitude = Mathf.Clamp01(_moveDir.magnitude);
                Vector2 normalizedInput = _moveDir / inputMagnitude;
                Vector3 moveDir = GetCameraRelativeMoveDir(normalizedInput);

                //이동 방향으로 즉시 회전 (Slerp 없이 LookRotation 직접 대입)
                transform.rotation = Quaternion.LookRotation(moveDir, Vector3.up);
            }
        }

        private Vector3 GetCameraRelativeMoveDir(Vector2 normalizedInput)
        {
            float yaw = _camTrans.eulerAngles.y;
            //x와 z축은 무시하고 y축으로만 회전~
            return Quaternion.Euler(0, yaw, 0) * new Vector3(normalizedInput.x, 0, normalizedInput.y);
        }
    }
}
