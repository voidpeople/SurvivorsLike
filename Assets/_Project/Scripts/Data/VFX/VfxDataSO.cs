using System;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace SurvivorsLike
{
    public enum VfxType
    {
        None,
        Muzzle,     //발사 머즐
        Hit,        //적중 임팩트
        Explosion,  //폭발 / 범위 장판
        Buff,       //버프·오라 (루프, 캐릭터 부착)
        Death,      //적/보스 사망 연출
        Pickup,     //경험치·골드·아이템 획득
        LevelUp,    //레벨업 연출
        Environment //환경 / 앰비언트
    }

    [Serializable]
    public class VfxData
    {
        public int Id;
        public VfxType Type;
        public string PrefabKey;
        public float LifeTime;
        public float Scale;
        public int PoolInitSize;
        public int PoolMaxSize;
        public int SfxId;          //사운드 아이디        
    }

    [CreateAssetMenu(fileName = "VfxDataSO", menuName = "SurvivorsLike/Data/VfxDataSO")]
    public class VfxDataSO : ScriptableObject
    {
        [TableList]
        public List<VfxData> VfxDataList;
    }
}
