using System;
using System.Collections.Generic;
using System.Linq;
using TriInspector;
using UnityEngine;
using static UnityEngine.Analytics.IAnalytic;


namespace SurvivorsLike
{
    [Serializable]
    public class LinearProjectileSkillData
    {
        public int SkillId;
        public string SkillName;
        public int Level;
        public float Damage;
        public float ProjectileSpeed;
        public float Cooldown;
        public int ProjectileCount;   //동시 발사 수
        public int PierceCount;       //관통 횟수
        public string PrefabKey;
    }

    //쿠나이, 드릴샷...
    [CreateAssetMenu(fileName = "LinearProjectileSkillDataSO", menuName = "SurvivorsLike/Data/LinearProjectileSkillDataSO")]
    public class LinearProjectileSkillDataSO : ScriptableObject
    {
        ////[TableList]은 Tri-Inspector의 명령어
        ////역할: List<T> 를 Unity Inspector에서 테이블 형태로 표시
        //[TableList]
        //public List<LinearProjectileSkillData> DataList;

        //<SkillId, List<LinearProjectileSkillData>>
        public Dictionary<int, List<LinearProjectileSkillData>> DataListDic;

        //특정 스킬 아이디의 레벨 데이터 리스트 반환
        public List<LinearProjectileSkillData> GetDataList(int skillId)
        {
            if(DataListDic.TryGetValue(skillId, out List<LinearProjectileSkillData> list) == true)
            {
                return list;
            }

            Debug.LogError($"GetDataList()::LinearProjectileSkillData 데이터를 찾을 수 없음~ : SkillId - {skillId}");
            return null;
        }

        public LinearProjectileSkillData GetLevelData(int skillId, int level)
        {
            if (DataListDic.TryGetValue(skillId, out List<LinearProjectileSkillData> dataList) == true)
            {
                LinearProjectileSkillData levelData = dataList.FirstOrDefault(d => d.Level == level);
                if (levelData != null)
                    return levelData;
                else
                {
                    Debug.LogError($"GetLevelData()::LinearProjectileSkillData 데이터를 찾을 수 없음~ : SkillId - {skillId}");
                    return null;
                }
            }

            Debug.LogError($"GetLevelData()::LinearProjectileSkillData 데이터를 찾을 수 없음~ : SkillId - {skillId}, Level - {level}");
            return null;
        }
    }
}
