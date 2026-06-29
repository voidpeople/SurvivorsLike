using System.Collections.Generic;
using UnityEngine;


namespace SurvivorsLike
{
    public class LinearProjectileSkillSheetDownloader
        : MultiCollectionSheetDownloader<LinearProjectileSkillDataSO, LinearProjectileSkillLevelData>
    {
        //행의 SkillId를 그룹핑 키로 반환
        //"1001" => Kunai 그룹 => KunaiSkillData.asset
        //"1002" => DrillShot 그룹 => DrillShotSkillData.asset
        protected override string GetGroupKey(Dictionary<string, string> row)
            => Str(row, "Id");

        protected override LinearProjectileSkillDataSO CreateSO(
            string groupKey, Dictionary<string, string> firstRow)
        {
            var so = ScriptableObject.CreateInstance<LinearProjectileSkillDataSO>();
            so.Id = Int(firstRow, "Id");    
            so.Name = Str(firstRow, "Name");
            so.DisplayName = Str(firstRow, "DisplayName");
            so.Description = Str(firstRow, "Description");
            so.IconKey = Str(firstRow, "IconKey");
            so.RequiresTarget = Bool(firstRow, "RequiresTarget");

            so.ProjectileId = Int(firstRow, "ProjectileId");
            return so;
        }

        protected override LinearProjectileSkillLevelData ParseRowData(
            Dictionary<string, string> row)
        {
            return new LinearProjectileSkillLevelData
            {
                Level = Int(row, "Level"),
                Damage = Float(row, "Damage"),                
                Cooldown = Float(row, "Cooldown"),
                ProjectileSpeed = Float(row, "ProjectileSpeed"),
                ProjectileCount = Int(row, "ProjectileCount"),
                PierceCount = Int(row, "PierceCount"),
            };
        }

        //파싱된 LevelData 리스트를 SO의 LevelDataList 필드에 주입
        protected override void ApplyDataList(
            LinearProjectileSkillDataSO so,
            List<LinearProjectileSkillLevelData> dataList)
            => so.LevelDataList = dataList;

        protected override string GetAssetFileName(LinearProjectileSkillDataSO so)
            => $"LinearProjectileSkill_{so.Name}";
    }
}
