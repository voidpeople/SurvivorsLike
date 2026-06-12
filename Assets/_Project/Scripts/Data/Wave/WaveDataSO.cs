using System;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace SurvivorsLike
{
    public enum WaveType
    {
        None,
        Repeat,     //구간 동안 일정 간격으로 반복 스폰 (일반 몹)
        OneShot     //StartTime에 1회만 스폰 (보스, 포위 이벤트)
    }

    [Serializable]
    public class WaveData
    {     
        public WaveType Type;
        public int EnemyId;            //EnemyDataSO의 적 ID. 프리팹 직접 참조 금지
        public float StartTime;        //웨이브 시작 게임 시간(초) 
        public float EndTime;          //종료 시간(초). OneShot은 무시 (값이 0인 부분은 그 웨이브 시간대에 적 캐릭터를 StartTime에 한번만 스폰하기 때문~)
        public float SpawnInterval;    //반복 스폰 간격(초) (값이 0인 부분은 그 웨이브 시간대에 적 캐릭터를 StartTime에 한번만 스폰하기 때문~)
        public int SpawnCountPerTick;  //한 번에 스폰할 마리 수
    }

    [CreateAssetMenu(fileName = "WaveDataSO", menuName = "Scriptable/Data/WaveDataSO")]
    public class WaveDataSO : ScriptableObject
    {
        public int Id;                 //웨이브 아이디
        public int ChapterId;          //챕터 아이디

        [TableList]
        public List<WaveData> WaveDataList;
    }
}

