using UnityEngine;



namespace SurvivorsLike
{
    public abstract class StateBase
    {
        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual void Update() { }
    }
}
