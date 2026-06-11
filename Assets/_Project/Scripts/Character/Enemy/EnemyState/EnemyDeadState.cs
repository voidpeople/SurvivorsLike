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
            _ctrl.Movement.Stop();
            _ctrl.AnimCtrl.PlayIdle();

            PoolableParticle particleObj = PoolManager.Instance.Get<PoolableParticle>("vfx/explosion/explosion01");
            Vector3 pos = _ctrl.transform.position;
            pos.y = 1f;
            particleObj.Play(pos);

            ReturnToPoolAsync(_ctrl.CTS).Forget();
        }

        private async UniTaskVoid ReturnToPoolAsync(CancellationToken ct)
        {            
            await UniTask.Delay(500, cancellationToken: ct);

            //Pool매니저에 반환
            _ctrl.ReturnToPool();
        }
    }
}
