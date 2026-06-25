using System.Collections.Generic;
using UnityEngine;

namespace SurvivorsLike
{
    public class InGamePlayerLevelSheetDownloader
        : SingleCollectionSheetDownloader<InGamePlayerLevelDataSO, InGamePlayerLevelData>
    {
        protected override InGamePlayerLevelDataSO CreateSO()
        {
            var so = ScriptableObject.CreateInstance<InGamePlayerLevelDataSO>();
            return so;
        }

        protected override InGamePlayerLevelData ParseRowData(
            Dictionary<string, string> row)
        {
            return new InGamePlayerLevelData
            {
                Level = Int(row, "Level"),
                ExpRequiredPerLevel = Int(row, "ExpRequiredPerLevel"),
            };
        }

        //파싱된 LevelData 리스트를 SO의 LevelDataList 필드에 주입
        protected override void ApplyDataList(
            InGamePlayerLevelDataSO so,
            List<InGamePlayerLevelData> dataList)
            => so.LevelDataList = dataList;

        protected override string GetAssetFileName(InGamePlayerLevelDataSO so)
            => $"InGamePlayerLevelData";
    }
}
