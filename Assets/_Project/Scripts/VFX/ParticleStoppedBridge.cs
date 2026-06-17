using SurvivorsLike;
using UnityEngine;

public class ParticleStoppedBridge : MonoBehaviour
{
    private ParticleEffect _owner;

    public void Init(ParticleEffect owner)
    {
        _owner = owner;
    }

    private void OnParticleSystemStopped()
    {
        if(_owner != null)
            _owner.ReturnToPool();
    }
}
