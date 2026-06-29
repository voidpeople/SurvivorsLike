using System;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;


namespace SurvivorsLike
{
    public enum ProjectileMoveType
    {
        None = 0,

        //직선 이동. 발사 시점의 방향(dir)으로 등속 직진한다.
        //예) 쿠나이, 기본 총알, 화살.
        Straight = 1,

        //포물선 이동. 중력의 영향을 받아 위로 솟았다가 떨어진다.
        //예) 수류탄, 박격포, 폭탄.
        Parabola = 2,

        //유도 이동. 지정된 타겟(target)을 향해 homingTurnRate만큼 선회하며 따라간다.
        //예) 유도 미사일, 추적탄.
        Homing = 3,

        //빔/레이저. 즉시 도달하는 직선형 지속 판정으로, Speed=0이며 lifetime 동안 라인 위의 대상에 틱 데미지를 준다.
        //예) 레이저, 화염방사.
        Beam = 4,

        //부메랑. 일정 거리/시간까지 전진한 뒤 발사 원점(오너)으로 되돌아오며, 왕복 중 모두 충돌 판정을 가진다.
        //예) 부메랑, 회전 차크람.
        Boomerang = 5,
    }

    [Serializable]
    public class ProjectileData
    {
        public int Id;
        public string PrefabKey;
        public int PoolInitSize;
        public int PoolMaxSize;
        public ProjectileMoveType MoveType;
        public float ColliderRadius; //충돌 체크 반지름
        public float MaxRange;
        public float LifeTime;
        public int HitVfxId;   //hit 이펙트 아이디
        public int HitSfxId;   //hit 사운드
        public int TrailVfxId; //퀘적 이펙트 아이디        
    }

    [CreateAssetMenu(fileName = "ProjectileDataSO", menuName = "SurvivorsLike/Data/ProjectileDataSO")]
    public class ProjectileDataSO : ScriptableObject
    {
        [TableList]
        public List<ProjectileData> ProjectileDataList;
    }
}
