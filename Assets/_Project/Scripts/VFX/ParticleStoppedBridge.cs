using SurvivorsLike;
using UnityEngine;

public class ParticleStoppedBridge : MonoBehaviour
{
    private PoolableParticle _owner;

    public void Init(PoolableParticle owner)
    {
        _owner = owner;
    }

    private void OnParticleSystemStopped()
    {
        if(_owner != null)
            _owner.ReturnToPool();
    }
}
