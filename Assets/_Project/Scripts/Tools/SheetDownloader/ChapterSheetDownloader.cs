using System.Collections.Generic;
using UnityEngine;


namespace SurvivorsLike
{
    //TODO: 추후에 ChapterData도 List로 저장할 것~
    public class ChapterSheetDownloader : SheetDownloaderBase<ChapterDataSO>
    {
        protected override ChapterDataSO CreateSO(Dictionary<string, string> row)
        {
            var so = ScriptableObject.CreateInstance<ChapterDataSO>();
            so.Id = Int(row, "Id");
            so.DisplayName = Str(row, "DisplayName");
            so.DisplaySpriteName = Str(row, "DisplaySpriteName");
            so.MapId = Int(row, "MapId");
            so.WaveId = Int(row, "WaveId");
            so.RecommendedCP = Int(row, "RecommendedCP");
            so.EnergyCost = Int(row, "EnergyCost", 5);
            so.DurationSec = Float(row, "DurationSec", 900f);
            so.RewardGold = Int(row, "RewardGold");
            so.RewardGem = Int(row, "RewardGem");
            return so;
        }

        protected override string GetAssetFileName(ChapterDataSO so)
            => $"Chapter_{so.Id:D2}";
    }
}
