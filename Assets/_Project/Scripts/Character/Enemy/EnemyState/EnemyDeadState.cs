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

            if(!DataManager.Instance.VfxDataDic.TryGetValue(_ctrl.EnemyData.DeathVfxId, out VfxData vfxData))
            {
                Debug.LogError($"{nameof(EnemyDeadState)}=> DeathVfxId does not exist. - DeathVfxId: {_ctrl.EnemyData.DeathVfxId}");
                return;
            }

            ParticleEffect particleObj = PoolManager.Instance.Get<ParticleEffect>(vfxData.PrefabKey);
            if(particleObj == null)
            {
                Debug.LogError($"{nameof(EnemyDeadState)}=> PrefabKey does not exist. - PrefabKey: {vfxData.PrefabKey}");
                return;
            }
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
