using System;
using System.Collections.Generic;
using UnityEngine;



namespace SurvivorsLike
{
    public class ProjectileSheetDownloader
        : SingleCollectionSheetDownloader<ProjectileDataSO, ProjectileData>
    {
        protected override ProjectileDataSO CreateSO()
        {
            var so = ScriptableObject.CreateInstance<ProjectileDataSO>();
            return so;
        }

        protected override ProjectileData ParseRowData(
            Dictionary<string, string> row)
        {
            return new ProjectileData
            {
                Id = Int(row, "Id"),
                PrefabKey = Str(row, "PrefabKey"),
                PoolInitSize = Int(row, "PoolInitSize"),
                PoolMaxSize = Int(row, "PoolMaxSize"),
                MoveType = ProjectileMoveType(row, "MoveType"),
                ColliderRadius = Float(row, "ColliderRadius"),
                MaxRange = Float(row, "MaxRange"),
                LifeTime = Float(row, "LifeTime"),
                HitVfxId = Int(row, "HitVfxId"),
                HitSfxId = Int(row, "HitSfxId"),
                TrailVfxId = Int(row, "TrailVfxId"),
            };
        }

        //파싱된 LevelData 리스트를 SO의 LevelDataList 필드에 주입
        protected override void ApplyDataList(
            ProjectileDataSO so,
            List<ProjectileData> dataList)
            => so.ProjectileDataList = dataList;

        protected override string GetAssetFileName(ProjectileDataSO so)
            => $"ProjectileData";

        private ProjectileMoveType ProjectileMoveType(Dictionary<string, string> r, string k)
        {
            if (r.TryGetValue(k, out var v) == true)
            {
                if (Enum.TryParse<ProjectileMoveType>(v, true, out SurvivorsLike.ProjectileMoveType moveType) == true)
                    return moveType;
            }

            Debug.LogError($"{nameof(ProjectileSheetDownloader)}::ProjectileMoveType=> ProjectileMoveType is None.");
            return SurvivorsLike.ProjectileMoveType.None;
        }
    }
}
