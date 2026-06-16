using SurvivorsLike;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace SurvivorsLike
{
    public class WaveSheetDownloader
    : MultiCollectionSheetDownloader<WaveDataSO, WaveData>
    {
        protected override void ApplyDataList(
            WaveDataSO so,
            List<WaveData> dataList)
            => so.WaveDataList = dataList;


        protected override WaveDataSO CreateSO(
            string groupKey, Dictionary<string, string> firstRow)
        {
            var so = ScriptableObject.CreateInstance<WaveDataSO>();
            so.Id = Int(firstRow, "Id");
            return so;
        }

        protected override string GetAssetFileName(WaveDataSO so)
            => $"WaveData_{so.Id}";

        protected override string GetGroupKey(Dictionary<string, string> row)
            => Str(row, "Id");


        protected override WaveData ParseRowData(
            Dictionary<string, string> row)
        {
            return new WaveData
            {
                Type = WaveType(row, "Type"),
                EnemyId = Int(row, "EnemyId"),
                StartTime = Float(row, "StartTime"),
                EndTime = Float(row, "EndTime"),
                SpawnInterval = Float(row, "SpawnInterval"),
                SpawnCountPerTick = Int(row, "SpawnCountPerTick"),
            };
        }

        private WaveType WaveType(Dictionary<string, string> r, string k)
        {
            if (r.TryGetValue(k, out var v) == true)
            {
                if (Enum.TryParse<WaveType>(v, true, out SurvivorsLike.WaveType waveType) == true)
                    return waveType;
            }

            Debug.LogError($"{nameof(WaveSheetDownloader)}::WaveType=> WaveType is None.");
            return SurvivorsLike.WaveType.None;
        }
    }
}
