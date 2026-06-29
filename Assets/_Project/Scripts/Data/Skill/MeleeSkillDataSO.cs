using System;
using System.Collections.Generic;
using System.Linq;
using TriInspector;
using UnityEngine;

namespace SurvivorsLike
{

    [Serializable]
    public class MeleeSkillData
    {
        public int Level;
        public float Damage;
        public float Cooldown;
        public float AttackRange; //공격 거리
        public float AttackAngle; //캐릭터 정면을 기준으로 어느 정도 앵글 범위로 스킬을 사용할 것인가? 만약 360이면 모든 범위 공격~
    }

    //스킬 테이블 정보
    [CreateAssetMenu(fileName = "ChapterDataSO", menuName = "SurvivorsLike/Data/MeleeSkillDataSO")]
    public class MeleeSkillDataSO : SkillDataSO
    {
        //[TableList]은 Tri-Inspector의 명령어
        //역할: List<T> 를 Unity Inspector에서 테이블 형태로 표시
        [TableList]
        public List<MeleeSkillData> LevelDataList;

        public MeleeSkillData GetLevelData(int level)
        {
            MeleeSkillData levelData = LevelDataList.FirstOrDefault(d => d.Level == level);
            if (levelData != null)
                return levelData;

            Debug.LogError($"{nameof(MeleeSkillDataSO)}::GetLevelData=> MeleeSkillData not found: SkillId - {Id}, Level - {level}");
            return null;
        }

        public override float GetCooldown(int level)
        {
            MeleeSkillData data = GetLevelData(level);
            if (data != null)
                return data.Cooldown;

            return 0f;
        }

        public override void CollectPoolAssetRef(List<PoolAssetRef> list)
        {

        }
    }
}
