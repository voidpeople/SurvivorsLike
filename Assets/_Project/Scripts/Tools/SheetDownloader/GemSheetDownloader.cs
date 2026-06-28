using System;
using System.Collections.Generic;
using UnityEngine;

namespace SurvivorsLike
{
    // 구글 시트 GemData 탭 전체 행(Green/Blue/Yellow) → GemDataSO 1개의 DataList에 저장
    // SingleCollectionSheetDownloader: 전체 행을 하나의 SO에 모으는 패턴
    public class GemSheetDownloader
        : SingleCollectionSheetDownloader<GemDataSO, GemData>
    {
        protected override GemDataSO CreateSO()
        {
            return ScriptableObject.CreateInstance<GemDataSO>();
        }

        protected override GemData ParseRowData(Dictionary<string, string> row)
        {
            return new GemData
            {
                GemType      = GemType(row, "GemType"),
                ExpReward    = Int(row, "ExpReward"),
                PrefabKey    = Str(row, "PrefabKey"),
                PoolInitSize = Int(row, "PoolInitSize"),
                PoolMaxSize  = Int(row, "PoolMaxSize"),
            };
        }

        protected override void ApplyDataList(GemDataSO so, List<GemData> dataList)
            => so.DataList = dataList;

        protected override string GetAssetFileName(GemDataSO so)
            => "GemDataSO";

        private GemType GemType(Dictionary<string, string> r, string k)
        {
            if (r.TryGetValue(k, out var v) == true)
            {
                if (Enum.TryParse<GemType>(v, true, out SurvivorsLike.GemType gemType) == true)
                    return gemType;
            }

            Debug.LogError($"{nameof(GemSheetDownloader)}::GemType=> GemType is None.");
            return SurvivorsLike.GemType.None;
        }
    }
}
