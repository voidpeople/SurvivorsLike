using System.Collections.Generic;
using UnityEngine;


namespace SurvivorsLike
{

    public class MapSheetDownloader : SheetDownloaderBase<MapDataSO>
    {
        protected override MapDataSO CreateSO(Dictionary<string, string> row)
        {
            var so = ScriptableObject.CreateInstance<MapDataSO>();
            so.MapId = Int(row, "MapId");               // 파일명 생성에 필수
            so.GroundMaterialKey = Str(row, "GroundMaterialKey");
            so.PlayAreaRadius = Float(row, "PlayAreaRadius", 60f);
            so.DecorationGroupKey = Str(row, "DecorationGroupKey");
            so.DecorationCount = Int(row, "DecorationCount", 40);
            so.DecorationSeed = Int(row, "DecorationSeed", 1);
            so.AmbientLightColor = Color(row, "AmbientLightColor", new UnityEngine.Color(0.4f, 0.4f, 0.5f, 1f));
            so.MainLightColor    = Color(row, "MainLightColor", UnityEngine.Color.white);
            so.BgmKey = Str(row, "BgmKey");
            return so;
        }

        protected override string GetAssetFileName(MapDataSO so)
            => $"Map_{so.MapId:D2}";
    }

}
