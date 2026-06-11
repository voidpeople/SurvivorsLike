using SurvivorsLike;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;


namespace SurvivorsLike
{

    //where TStateType : unmanaged, Enum은 TStateType의 타입으로 unmanaged나 Enum만 허용 한다는 의미
    //where TState : StateBase은 TState의 타입으로 StateBase만 허용 한다는 의미

    public class FSM<TStateType, TState>
    where TStateType : unmanaged, Enum
    where TState : StateBase
    {
        // ─── private 필드 ────────────────────────────────────────────────────
        private readonly TState[] _states;
        private TState _currentState;


        // ─── Properties ──────────────────────────────────────────────────────
        public TStateType CurrentType { get; private set; }


        // ─── 생성자 ───────────────────────────────────────────────────────────
        public FSM()
        {
            _states = new TState[Enum.GetValues(typeof(TStateType)).Length];
        }


        // ─── Public Methods ───────────────────────────────────────────────────
        public void RegisterState(TStateType type, TState state)
        {
            _states[ToIndex(type)] = state;
        }

        //PoolManager에서 하나의 오브젝트를 꺼낸 후에 가장 처음 Init함수를 실행한다.
        public void Init(TStateType type)
        {
            _currentState?.Exit();
            CurrentType = type;
            _currentState = _states[ToIndex(CurrentType)];
            _currentState.Enter();
        }

        public void ChangeState(TStateType type)
        {
            //== 비교 연산은 제네릭 타입의 비교에는 사용할 수 었다.
            //따라서 EqualityComparer을 이용하여 제네릭 타입을 비교하면 null 안전성을 얻을 수 있고 또한 boxing을 방지할 수 있다.
            //CurrentType와 type이 같으면 함수가 종료된다.
            if (EqualityComparer<TStateType>.Default.Equals(CurrentType, type))
                return;

            _currentState.Exit();
            CurrentType = type;
            _currentState = _states[ToIndex(CurrentType)];
            _currentState.Enter();
        }

        public void Update()
        {
            _currentState?.Update();
        }

        public void OnDestinationReached()
        {
            (_currentState as IMovementListener)?.OnDestinationReached();
        }

        //타겟이 죽은 경우~
        public void OnTargetDied()
        {
            (_currentState as ITargetListener)?.OnTargetDied();
        }


        // ─── Private Methods ──────────────────────────────────────────────────
        //[MethodImpl(MethodImplOptions.AggressiveInlining)] 명령은 되도록 해당 함수를 인라인으로 추가 하라는 명령~
        //강제성은 없는 명령임~
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int ToIndex(TStateType type)
        {
            //제네릭 로직에서는 int(enum)은 컴파일 오류가 발생한다.
            //UnsafeUtility.As은 Burst API이지만 제네릭 로직에서 GC와 Boxing 문제 없이 사용 가능하다.
            return UnsafeUtility.As<TStateType, int>(ref type);
        }
    }
}
