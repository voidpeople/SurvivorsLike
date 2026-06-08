using System;
using UnityEngine;


namespace SurvivorsLike
{
    [Serializable]
    public class PlayerData
    {
        public int Level;
        public float Damage;
        public float ProjectileSpeed;
        public float Cooldown;
        public int ProjectileCount;   //동시 발사 수
        public int PierceCount;       //관통 횟수
    }

    [CreateAssetMenu(fileName = "PlayerDataSO", menuName = "Scriptable/Data/PlayerDataSO")]
    public class PlayerDataSO : CharacterDataSO
    {
        public float HpRegen;     //초당 기본 체력 회복량
        public float PickupRange; //경험치 젬 자동 픽업 반경
    }
}
