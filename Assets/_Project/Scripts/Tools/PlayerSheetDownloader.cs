using System.Collections.Generic;
using UnityEngine;


namespace SurvivorsLike
{
    public class PlayerSheetDownloader : SheetDownloaderBase<PlayerDataSO>
    {
        protected override PlayerDataSO CreateSO(Dictionary<string, string> row)
        {
            var so = ScriptableObject.CreateInstance<PlayerDataSO>();
            so.Id = Int(row, "Id");
            so.Name = Str(row, "Name");
            so.Hp = Int(row, "Hp");
            so.MoveSpeed = Float(row, "MoveSpeed");
            so.DefaultSkillId = Int(row, "DefaultSkillId");
            so.IconKey = Str(row, "IconKey");
            so.PrefabKey = Str(row, "PrefabKey");
            so.HpRegen = Float(row, "HpRegen");
            so.PickupRange = Float(row, "PickupRange");

            return so;
        }

        protected override string GetAssetFileName(PlayerDataSO so)
            => $"Player_{so.Id:D2}";
    }
}
