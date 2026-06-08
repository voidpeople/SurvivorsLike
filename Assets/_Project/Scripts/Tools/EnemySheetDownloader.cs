using System;
using System.Collections.Generic;
using UnityEngine;


namespace SurvivorsLike
{
    public class EnemySheetDownloader : SheetDownloaderBase<EnemyDataSO>
    {
        protected override EnemyDataSO CreateSO(Dictionary<string, string> row)
        {
            var so = ScriptableObject.CreateInstance<EnemyDataSO>();
            so.Id = Int(row, "Id");
            so.Name = Str(row, "Name");
            so.Hp = Int(row, "Hp");
            so.MoveSpeed = Float(row, "MoveSpeed");
            so.DefaultSkillId = Int(row, "DefaultSkillId");
            so.IconKey = Str(row, "IconKey");
            so.PrefabKey = Str(row, "PrefabKey");

            so.Type = EnemyType(row, "Type");
            if (so.Type == SurvivorsLike.EnemyType.None)
                Debug.LogError($"EnemySheetDownloader::CreateSO - EnemyType is None.");

            so.DetectionRange = Float(row, "DetectionRange");
            so.Armor = Float(row, "Armor");
            so.ExpReward = Int(row, "ExpReward");
            so.DropGold = Int(row, "DropGold");
            so.KnockbackResistance = Float(row, "KnockbackResistance");
            return so;
        }

        protected override string GetAssetFileName(EnemyDataSO so)
            => $"Enemy_{so.Id:D2}";
    }
}
