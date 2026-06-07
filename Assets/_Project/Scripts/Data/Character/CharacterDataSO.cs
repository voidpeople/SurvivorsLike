using UnityEngine;


namespace SurvivorsLike
{
    public abstract class CharacterDataSO : ScriptableObject
    {
        public int Id;
        public string Name;
        public float Hp;
        public float MoveSpeed;
        public string IconKey; 
        public string PrefabKey;
    }
}
