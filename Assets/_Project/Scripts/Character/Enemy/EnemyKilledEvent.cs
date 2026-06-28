using UnityEngine;


namespace SurvivorsLike
{
    public readonly struct EnemyKilledEvent
    {
        public readonly EnemyType Type;
        public readonly Vector3 Position;
        public readonly GemType DropGemType;

        public EnemyKilledEvent(EnemyType type, Vector3 pos, GemType gemType)
        {
            Type = type;
            Position = pos;
            DropGemType = gemType;
        }
    }
}
