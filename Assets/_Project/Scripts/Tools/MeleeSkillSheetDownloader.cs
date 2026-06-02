using System.Collections.Generic;
using UnityEngine;


namespace SurvivorsLike
{
    public class MeleeSkillSheetDownloader : SheetDownloaderBase<MeleeSkillDataSO>
    {
        protected override MeleeSkillDataSO CreateSO(Dictionary<string, string> row)
        {
            var so = ScriptableObject.CreateInstance<MeleeSkillDataSO>();
            so.SkillId = Int(row, "SkillId");
            so.SkillName = Str(row, "SkillName");
            so.Level = Int(row, "Level");
            so.Damage = Int(row, "Damage");
            so.AttackInterval = Int(row, "AttackInterval");
            so.AttackRange = Float(row, "AttackRange");
            so.AttackAngle = Float(row, "AttackAngle");
            so.PrefabKey = Str(row, "PrefabKey");
            return so;
        }

        protected override string GetAssetFileName(MeleeSkillDataSO so)
            => $"MeleeSkill_{so.SkillId:D2}";
    }
}
