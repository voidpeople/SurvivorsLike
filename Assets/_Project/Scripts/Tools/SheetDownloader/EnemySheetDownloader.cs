using System;
using System.Collections.Generic;
using UnityEngine;

namespace SurvivorsLike
{
    public class EnemySheetDownloader
        : SingleCollectionSheetDownloader<EnemyDataSO, EnemyData>
    {
        protected override EnemyDataSO CreateSO()
        {
            var so = ScriptableObject.CreateInstance<EnemyDataSO>();
            return so;
        }

        protected override EnemyData ParseRowData(
            Dictionary<string, string> row)
        {
            return new EnemyData
            {
                Id = Int(row, "Id"),
                Name = Str(row, "Name"),
                Hp = Int(row, "Hp"),
                MoveSpeed = Float(row, "MoveSpeed"),
                DefaultSkillId = Int(row, "DefaultSkillId"),
                IconKey = Str(row, "IconKey"),
                PrefabKey = Str(row, "PrefabKey"),
                DeathVfxId = Int(row, "DeathVfxId"),

                PoolInitSize = Int(row, "PoolInitSize"),
                PoolMaxSize = Int(row, "PoolMaxSize"),
                Type = EnemyType(row, "Type"),
                ContactDamage = Float(row, "ContactDamage"),
                Armor = Float(row, "Armor"),
                DropGemType = GemType(row, "DropGemType"),
                DropGold = Int(row, "DropGold"),
                KnockbackResistance = Float(row, "KnockbackResistance"),                
            };
        }

        //파싱된 LevelData 리스트를 SO의 LevelDataList 필드에 주입
        protected override void ApplyDataList(
            EnemyDataSO so,
            List<EnemyData> dataList)
            => so.EnemyDataList = dataList;

        protected override string GetAssetFileName(EnemyDataSO so)
            => $"EnemyData";

        private EnemyType EnemyType(Dictionary<string, string> r, string k)
        {
            if (r.TryGetValue(k, out var v) == true)
            {
                if (Enum.TryParse<EnemyType>(v, true, out SurvivorsLike.EnemyType enemyType) == true)
                    return enemyType;
            }

            Debug.LogError($"{nameof(EnemySheetDownloader)}::EnemyType=> EnemyType is None.");
            return SurvivorsLike.EnemyType.None;
        }

        private GemType GemType(Dictionary<string, string> r, string k)
        {
            if (r.TryGetValue(k, out var v) == true)
            {
                if (Enum.TryParse<GemType>(v, true, out SurvivorsLike.GemType gemType) == true)
                    return gemType;
            }

            Debug.LogError($"{nameof(EnemySheetDownloader)}::GemType=> GemType is None.");
            return SurvivorsLike.GemType.None;
        }
    }
}
