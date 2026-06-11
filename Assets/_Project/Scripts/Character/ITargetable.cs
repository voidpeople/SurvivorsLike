using UnityEngine;


namespace SurvivorsLike
{
    public interface ITargetable
    {
        Transform Transform { get; }
        Vector3 AimPoint { get; }   //조준점    
    }
}
