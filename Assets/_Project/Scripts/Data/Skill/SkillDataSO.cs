using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;



namespace SurvivorsLike
{
    public readonly struct PoolAssetRef
    {
        public readonly PoolAssetType Type;
        public readonly int Id;
        public PoolAssetRef(PoolAssetType type, int id)
        {
            Type = type;
            Id = id;
        }
    }

    //스킬 데이터 베이스 클래스
    public abstract class SkillDataSO : ScriptableObject
    {
        public int Id;
        public string Name;
        public bool RequiresTarget; //타겟이 필요한 스킬이면 TRUE
        public string IconKey;      //UI 상의 스킬 아이콘 스프라이트 이름

        public abstract float GetCooldown(int level);
        public abstract void CollectPoolAssetRef(List<PoolAssetRef> list);
    }
}
