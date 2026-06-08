using UnityEngine;


namespace SurvivorsLike
{
    [CreateAssetMenu(fileName = "EnemyDataSO", menuName = "Scriptable/Data/EnemyDataSO")]
    public class EnemyDataSO : CharacterDataSO
    {
        public EnemyType Type;       //Normal/Elite/Boss
        public float DetectionRange;      //적(플레이어) 감지 거리
        public float Armor;               //기본 방어력
        public int ExpReward;             //죽으면 주는 경험치
        public int DropGold;              //죽을 떄 드롭하는 골드양
        public float KnockbackResistance; //넉백 효과 저항 (0 ~ 1.0)
    }
}
