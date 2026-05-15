using System.Collections.Generic;
using UnityEngine;


namespace SurvivorsLike
{
    public class ChapterSheetDownloader : SheetDownloaderBase<ChapterDataSO>
    {
        protected override ChapterDataSO CreateSO(Dictionary<string, string> row)
        {
            var so = ScriptableObject.CreateInstance<ChapterDataSO>();
            so.chapterId = Int(row, "chapterId");
            so.displayName = Str(row, "displayName");
            so.difficulty = Str(row, "difficulty");
            so.displaySpriteName = Str(row, "displaySpriteName");
            so.recommendedCP = Int(row, "recommendedCP");
            so.energyCost = Int(row, "energyCost", 5);
            so.durationSec = Float(row, "durationSec", 900f);
            so.rewardGold = Int(row, "rewardGold");
            so.rewardGem = Int(row, "rewardGem");
            return so;
        }

        protected override string GetAssetFileName(ChapterDataSO so)
            => $"Chapter_{so.chapterId:D2}";
    }
}
