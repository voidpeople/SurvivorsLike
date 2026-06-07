using System;
using System.Collections.Generic;
using System.Linq;
using TriInspector;
using UnityEngine;


namespace SurvivorsLike
{
    [Serializable]
    public class LinearProjectileSkillLevelData
    {
        public int Level;
        public float Damage;
        public float ProjectileSpeed;
        public float Cooldown;
        public int ProjectileCount;   //동시 발사 수
        public int PierceCount;       //관통 횟수
    }

    //쿠나이, 드릴샷...
    [CreateAssetMenu(fileName = "LinearProjectileSkillDataSO", menuName = "SurvivorsLike/Data/LinearProjectileSkillDataSO")]
    public class LinearProjectileSkillDataSO : SkillDataSO
    {
        //[TableList]은 Tri-Inspector의 명령어
        //역할: List<T> 를 Unity Inspector에서 테이블 형태로 표시
        [TableList]
        public List<LinearProjectileSkillLevelData> LevelDataList;

        public LinearProjectileSkillLevelData GetLevelData(int level)
        {
            LinearProjectileSkillLevelData levelData = LevelDataList.FirstOrDefault(d => d.Level == level);
            if (levelData != null)
                return levelData;

            Debug.LogError($"LinearProjectileSkillLevelData 데이터를 찾을 수 없음~ : SkillId - {Id}, Level - {level}");
            return null;
        }

        public override float GetCooldown(int level)
        {
            LinearProjectileSkillLevelData data = GetLevelData(level);
            if (data != null)
                return data.Cooldown;

            return 0f;
        }
    }
}
