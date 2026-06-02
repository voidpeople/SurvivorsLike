using UnityEngine;



namespace SurvivorsLike
{
    //스킬 테이블 정보
    [CreateAssetMenu(fileName = "ChapterDataSO", menuName = "SurvivorsLike/Data/SkillDataSO")]
    public abstract class SkillDataSO : ScriptableObject
    {
        public int SkillId;
        public string SkillName;
        public int Level;
        public float Damage;
        public float AttackInterval;
        public string PrefabKey;
    }
}
