using System;
using System.Collections.Generic;
using UnityEngine;



namespace SurvivorsLike
{
    public class VfxSheetDownloader
        : SingleCollectionSheetDownloader<VfxDataSO, VfxData>
    {
        protected override VfxDataSO CreateSO()
        {
            var so = ScriptableObject.CreateInstance<VfxDataSO>();
            return so;
        }

        protected override VfxData ParseRowData(
            Dictionary<string, string> row)
        {
            return new VfxData
            {
                Id = Int(row, "Id"),
                Type = VfxType(row, "Type"),
                PrefabKey = Str(row, "PrefabKey"),
                LifeTime = Float(row, "LifeTime"),
                Scale = Float(row, "Scale"),
                PoolInitSize = Int(row, "PoolInitSize"),
                PoolMaxSize = Int(row, "PoolMaxSize"),
                SfxId = Int(row, "SfxId"),              
            };
        }

        //파싱된 LevelData 리스트를 SO의 LevelDataList 필드에 주입
        protected override void ApplyDataList(
            VfxDataSO so,
            List<VfxData> dataList)
            => so.VfxDataList = dataList;

        protected override string GetAssetFileName(VfxDataSO so)
            => $"VfxData";

        private VfxType VfxType(Dictionary<string, string> r, string k)
        {
            if (r.TryGetValue(k, out var v) == true)
            {
                if (Enum.TryParse<VfxType>(v, true, out SurvivorsLike.VfxType vfxType) == true)
                    return vfxType;
            }

            Debug.LogError($"{nameof(VfxSheetDownloader)}::VfxType=> VfxType is None.");
            return SurvivorsLike.VfxType.None;
        }
    }
}
