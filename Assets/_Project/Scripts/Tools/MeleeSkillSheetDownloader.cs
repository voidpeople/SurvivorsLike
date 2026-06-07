using System.Collections.Generic;
using UnityEngine;


namespace SurvivorsLike
{
    public class MeleeSkillSheetDownloader : SheetDownloaderBase<MeleeSkillDataSO>
    {
        protected override MeleeSkillDataSO CreateSO(Dictionary<string, string> row)
        {
            var so = ScriptableObject.CreateInstance<MeleeSkillDataSO>();
            so.Id = Int(row, "Id");
            so.Name = Str(row, "Name");
            so.AttackRange = Float(row, "AttackRange");
            so.AttackAngle = Float(row, "AttackAngle");
            so.PrefabKey = Str(row, "PrefabKey");
            return so;
        }

        protected override string GetAssetFileName(MeleeSkillDataSO so)
            => $"MeleeSkill_{so.Id:D2}";
    }
}
