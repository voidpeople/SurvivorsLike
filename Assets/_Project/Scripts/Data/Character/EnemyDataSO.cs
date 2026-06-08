using System;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;


namespace SurvivorsLike
{
    [Serializable]
    public class EnemyData : CharacterData
    {
        public EnemyType Type;       //Normal/Elite/Boss
        public float DetectionRange;      //적(플레이어) 감지 거리
        public float Armor;               //기본 방어력
        public int ExpReward;             //죽으면 주는 경험치
        public int DropGold;              //죽을 떄 드롭하는 골드양
        public float KnockbackResistance; //넉백 효과 저항 (0 ~ 1.0)
    }

    [CreateAssetMenu(fileName = "EnemyDataSO", menuName = "Scriptable/Data/EnemyDataSO")]
    public class EnemyDataSO : ScriptableObject
    {
        //[TableList]은 Tri-Inspector의 명령어
        //역할: List<T> 를 Unity Inspector에서 테이블 형태로 표시
        [TableList]
        public List<EnemyData> EnemyDataList;
    }
}
