using System;
using System.Collections.Generic;
using System.Linq;
using TriInspector;
using UnityEngine;


namespace SurvivorsLike
{
    [Serializable]
    public class InGamePlayerLevelData
    {
        public int Level;
        public int ExpRequiredPerLevel;  //레벨 당 필요 경험치
    }

    //쿠나이, 드릴샷...
    [CreateAssetMenu(fileName = "InGamePlayerLevelDataSO", menuName = "SurvivorsLike/Data/InGamePlayerLevelDataSO")]
    public class InGamePlayerLevelDataSO : ScriptableObject
    {
        //[TableList]은 Tri-Inspector의 명령어
        //역할: List<T> 를 Unity Inspector에서 테이블 형태로 표시
        [TableList]
        public List<InGamePlayerLevelData> LevelDataList;

        public int GetRequiredExp(int level)
        {
            InGamePlayerLevelData levelData = LevelDataList.FirstOrDefault(d => d.Level == level);
            if (levelData != null)
                return levelData.ExpRequiredPerLevel;

            Debug.LogError($"{nameof(InGamePlayerLevelData)}::GetRequiredExp => InGamePlayerLevelData not found. - Level: {level}");
            return 0;
        }

        public int MaxLevel
        {
            get
            {
                return LevelDataList.Count > 0 ? LevelDataList[LevelDataList.Count - 1].Level : 1;
            }
        }
    }
}
