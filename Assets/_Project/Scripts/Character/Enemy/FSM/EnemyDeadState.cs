using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;


namespace SurvivorsLike
{
    public class EnemyDeadState : EnemyStateBase
    {
        public EnemyDeadState(EnemyController controller, EnemyFSM fsm)
            : base(controller, fsm)
        {
        }

        public override void Enter()
        {
            _controller.Movement.Stop();
            _controller.AnimCtrl.PlayIdle();

            ReturnToPoolAsync(_controller.CTS).Forget();
        }

        public override void Exit()
        {
        }

        public override void Update()
        {
        }

        private async UniTaskVoid ReturnToPoolAsync(CancellationToken ct)
        {
            await UniTask.Delay(1000, cancellationToken: ct);

            //Pool매니저에 반환
            _controller.TryGetComponent<PoolableObject>(out PoolableObject poolableObj);
            poolableObj?.Return();
        }
    }
}
