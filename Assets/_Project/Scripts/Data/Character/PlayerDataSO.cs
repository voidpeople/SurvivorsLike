using System;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;


namespace SurvivorsLike
{
    [Serializable]
    public class PlayerData : CharacterData
    {
        public float HpRegen;     //초당 기본 체력 회복량
        public float PickupRange; //경험치 젬 자동 픽업 반경
    }

    [CreateAssetMenu(fileName = "PlayerDataSO", menuName = "SurvivorsLike/Data/PlayerDataSO")]
    public class PlayerDataSO : ScriptableObject
    {
        //[TableList]은 Tri-Inspector의 명령어
        //역할: List<T> 를 Unity Inspector에서 테이블 형태로 표시
        [TableList]
        public List<PlayerData> PlayerDataList;
    }
}
