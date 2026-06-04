using System.Collections.Generic;
using UnityEngine;


namespace SurvivorsLike
{
    public class LinearProjectileSkillSheetDownloader : SheetDownloaderBase<LinearProjectileSkillDataSO>
    {
        protected override LinearProjectileSkillDataSO CreateSO(Dictionary<string, string> row)
        {
            var so = ScriptableObject.CreateInstance<LinearProjectileSkillDataSO>();
            so.SkillId = Int(row, "SkillId");
            so.SkillName = Str(row, "SkillName");
            so.IconKey = Str(row, "IconKey");
            so.PrefabKey = Str(row, "PrefabKey");

            return so;
        }

        protected override string GetAssetFileName(LinearProjectileSkillDataSO so)
            => $"LinearProjectileSkill_{so.SkillId:D2}";
    }
}
