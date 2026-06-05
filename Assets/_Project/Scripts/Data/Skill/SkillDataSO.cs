using UnityEngine;



namespace SurvivorsLike
{
    //스킬 데이터 베이스 클래스
    public abstract class SkillDataSO : ScriptableObject
    {
        public int SkillId;
        public string SkillName;
        public string IconKey; //UI 상의 스킬 아이콘 스프라이트 이름
        public string PrefabKey;

        public abstract float GetCooldown(int level);
    }
}
