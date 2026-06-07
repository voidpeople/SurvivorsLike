using System.Collections.Generic;
using UnityEngine;


namespace SurvivorsLike
{
    public class ChapterSheetDownloader : SheetDownloaderBase<ChapterDataSO>
    {
        protected override ChapterDataSO CreateSO(Dictionary<string, string> row)
        {
            var so = ScriptableObject.CreateInstance<ChapterDataSO>();
            so.Id = Int(row, "Id");
            so.DisplayName = Str(row, "DisplayName");
            so.Difficulty = Str(row, "Difficulty");
            so.DisplaySpriteName = Str(row, "DisplaySpriteName");
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
