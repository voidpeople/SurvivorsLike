using UnityEngine;



namespace SurvivorsLike
{
    //스킬 테이블 정보
    [CreateAssetMenu(fileName = "ChapterDataSO", menuName = "SurvivorsLike/Data/SkillDataSO")]
    public abstract class SkillDataSO : ScriptableObject
    {
        public int skillId;
        public string skillName;
        public int level;
        public float damage;
        public float attackInterval;
        public string prefabKey;
    }
}
