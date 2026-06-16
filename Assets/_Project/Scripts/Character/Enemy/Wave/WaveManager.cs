using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEditor.Overlays;
using UnityEngine;


namespace SurvivorsLike
{
    public class WaveManager : MonoBehaviour
    {
        private WaveDataSO _waveData;
        private EnemySpawner _spawner;

        private float _currentTime;
        private float[] _nextSpawnTimes;
        private bool[] _isOneShotSpawn;
        private bool _isRunning;

        public void Init(WaveDataSO data, EnemySpawner spawn)
        {
            _waveData = data;
            _spawner = spawn;

            int count = data.WaveDataList.Count;
            _nextSpawnTimes = new float[count];
            _isOneShotSpawn = new bool[count];

            for(int ii = 0; ii < count; ++ii)
            {
                WaveData wave = data.WaveDataList[ii];
                _nextSpawnTimes[ii] = wave.StartTime;

                if (wave.Type == WaveType.Repeat && wave.SpawnInterval <= 0f)
                {
                    Debug.LogError($"{nameof(WaveManager)}=> SpawnInterval is an error value. " +
                        $"- WaveDataSO.Id: {data.Id}, List Index: {ii}, SpawnInterval: {wave.SpawnInterval}");
                }
            }
        }

        public void StartWave() => _isRunning = true;
        public void StopWave() => _isRunning = false;

        private void Update()
        {
            if (!_isRunning)
                return;

            _currentTime += Time.deltaTime;
            for (int ii = 0; ii < _waveData.WaveDataList.Count; ++ii)
            {
                WaveData wave = _waveData.WaveDataList[ii];

                switch (wave.Type)
                {
                    case WaveType.Repeat:
                        UpdateRepeatWave(wave, ii);
                        break;
                    case WaveType.OneShot:
                        UpdateOneShotWave(wave, ii);
                        break;
                }
            }
        }

        //일반몹 반복 스폰
        private void UpdateRepeatWave(WaveData data, int index)
        {
            if (_currentTime < data.StartTime || _currentTime > data.EndTime)
                return;

            if (_currentTime < _nextSpawnTimes[index])
                return;

            SpawnEnemy(data);
            _nextSpawnTimes[index] = _currentTime + data.SpawnInterval;
        }

        //보스, 엘리트 몹을 특정 시간대에 한번만 스폰~
        private void UpdateOneShotWave(WaveData data, int index)
        {
            if (_isOneShotSpawn[index] || _currentTime < data.StartTime)
                return;
                        
            SpawnEnemy(data);
            _isOneShotSpawn[index] = true;
        }

        private void SpawnEnemy(WaveData data)
        {
            if (!DataManager.Instance.EnemyDataDic.TryGetValue(data.EnemyId, out EnemyData enemyData))
            {
                Debug.LogError($"{nameof(WaveManager)}=> EnemyId does not exist. - EnemyId: {data.EnemyId}");
                return;
            }

            _spawner.Spawn(enemyData, data.SpawnCountPerTick);
        }
    }
}
