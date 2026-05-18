using System.Collections.Generic;
using UnityEngine;


namespace SurvivorsLike
{

    public class MapSheetDownloader : SheetDownloaderBase<MapDataSO>
    {
        protected override MapDataSO CreateSO(Dictionary<string, string> row)
        {
            var so = ScriptableObject.CreateInstance<MapDataSO>();
            so.mapId = Int(row, "mapId");               // 파일명 생성에 필수
            so.groundMaterialKey = Str(row, "groundMaterialKey");
            so.playAreaRadius = Float(row, "playAreaRadius", 60f);
            so.decorationGroupKey = Str(row, "decorationGroupKey");
            so.decorationCount = Int(row, "decorationCount", 40);
            so.decorationSeed = Int(row, "decorationSeed", 1);
            so.ambientLightColor = Color(row, "ambientLightColor", new UnityEngine.Color(0.4f, 0.4f, 0.5f, 1f));
            so.mainLightColor    = Color(row, "mainLightColor", UnityEngine.Color.white);
            so.bgmKey = Str(row, "bgmKey");
            return so;
        }

        protected override string GetAssetFileName(MapDataSO so)
            => $"Map_{so.mapId:D2}";
    }

}
