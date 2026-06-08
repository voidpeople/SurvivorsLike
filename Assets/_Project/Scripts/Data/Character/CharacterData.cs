using System;
using UnityEngine;


namespace SurvivorsLike
{
    [Serializable]
    public abstract class CharacterData
    {
        public int Id;
        public string Name;
        public float Hp;
        public float MoveSpeed;
        public int DefaultSkillId;
        public string IconKey; 
        public string PrefabKey;
    }
}
