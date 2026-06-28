using System;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;


namespace SurvivorsLike
{
    [Serializable]
    public class EnemyData : CharacterData
    {
        public int PoolInitSize; //풀링 버퍼에 초기 생성한 마릿 수
        public int PoolMaxSize;  //풀링 버퍼에 최대 생성 가능한 마릿 수
        
        public EnemyType Type;            //Normal/Elite/Boss
        public float ContactDamage;       //플레이어와 접촉시 플레이어에게 줄 데미지 값
        public float Armor;               //기본 방어력
        public GemType DropGemType;       //사망시 드롭 젬 타입
        public int DropGold;              //죽을 떄 드롭하는 골드양
        public float KnockbackResistance; //넉백 효과 저항 (0 ~ 1.0)        
    }

    [CreateAssetMenu(fileName = "EnemyDataSO", menuName = "SurvivorsLike/Data/EnemyDataSO")]
    public class EnemyDataSO : ScriptableObject
    {
        //[TableList]은 Tri-Inspector의 명령어
        //역할: List<T> 를 Unity Inspector에서 테이블 형태로 표시
        [TableList]
        public List<EnemyData> EnemyDataList;
    }
}
