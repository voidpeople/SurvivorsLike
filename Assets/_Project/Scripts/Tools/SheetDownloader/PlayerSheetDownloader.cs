using System.Collections.Generic;
using UnityEngine;

namespace SurvivorsLike
{
    public class PlayerSheetDownloader
        : SingleCollectionSheetDownloader<PlayerDataSO, PlayerData>
    {
        protected override PlayerDataSO CreateSO()
        {
            var so = ScriptableObject.CreateInstance<PlayerDataSO>();
            return so;
        }

        protected override PlayerData ParseRowData(
            Dictionary<string, string> row)
        {
            return new PlayerData
            {
                Id = Int(row, "Id"),
                Name = Str(row, "Name"),
                Hp = Int(row, "Hp"),
                MoveSpeed = Float(row, "MoveSpeed"),
                DefaultSkillId = Int(row, "DefaultSkillId"),
                IconKey = Str(row, "IconKey"),
                PrefabKey = Str(row, "PrefabKey"),
                HpRegen = Float(row, "HpRegen"),
                PickupRange = Float(row, "PickupRange"),
            };
        }

        //파싱된 LevelData 리스트를 SO의 LevelDataList 필드에 주입
        protected override void ApplyDataList(
            PlayerDataSO so,
            List<PlayerData> dataList)
            => so.PlayerDataList = dataList;

        protected override string GetAssetFileName(PlayerDataSO so)
            => $"PlayerData";
    }
}
