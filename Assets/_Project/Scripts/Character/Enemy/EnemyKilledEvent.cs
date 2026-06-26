using UnityEngine;


namespace SurvivorsLike
{
    public readonly struct EnemyKilledEvent
    {
        public readonly EnemyType Type;
        public readonly Vector3 Position;
        public readonly int ExpReward;

        public EnemyKilledEvent(EnemyType type, Vector3 pos, int exp)
        {
            Type = type;
            Position = pos;
            ExpReward = exp;
        }
    }
}
