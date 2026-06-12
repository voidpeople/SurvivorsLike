using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;


namespace SurvivorsLike
{

    public class DataManager : SingletonMonoBehaviour<DataManager>
    {
        #region ChapterData
        [Header("아틀라스")]
        [SerializeField] private SpriteAtlas _lobbyChapterAtlas;

        private const string ChapterDataLabel = "ChapterData";
        private const string MapDataLabel = "MapData";
        private const string PlayerDataLabel = "PlayerData";
        private const string EnemyDataLabel = "EnemyData";
        private const string SkillDataLabel = "SkillData";


        private AsyncOperationHandle<IList<ChapterDataSO>> _chapterDataSOListHandle;
        private readonly List<ChapterDataSO> _chapterDataSOList = new();

        //IReadOnlyList은 Add, Remove, Clear 등의 수정 메서드가 없는 인터페이스
        public IReadOnlyList<ChapterDataSO> ChapterDataSOList => _chapterDataSOList;
        #endregion

        #region MapData
        private AsyncOperationHandle<IList<MapDataSO>> _mapDataSOListHandle;
        //Dictionary<맵 아이디, MapDataSO>
        private readonly Dictionary<int, MapDataSO> _mapDataSODic = new();

        public IReadOnlyDictionary<int, MapDataSO> MapDataSODic => _mapDataSODic;
        #endregion

        #region PlayerData
        private AsyncOperationHandle<IList<PlayerDataSO>> _playerDataSOListHandle;
        //Dictionary<플레이어 캐릭터 아이디, PlayerData>
        private readonly Dictionary<int, PlayerData> _playerDataDic = new();
        public IReadOnlyDictionary<int, PlayerData> PlayerDataDic => _playerDataDic;
        #endregion

        #region EnemyData
        private AsyncOperationHandle<IList<EnemyDataSO>> _enemyDataSOListHandle;
        //Dictionary<적 캐릭터 아이디, EnemyData>
        private readonly Dictionary<int, EnemyData> _enemyDataDic = new();
        public IReadOnlyDictionary<int, EnemyData> EnemyDataDic => _enemyDataDic;
        #endregion

        #region SkillData
        private AsyncOperationHandle<IList<SkillDataSO>> _skillDataSOListHandle;
        //Dictionary<스킬 아이디, SkillDataSO>
        private readonly Dictionary<int, SkillDataSO> _skillDataSODic = new();
        public IReadOnlyDictionary<int, SkillDataSO> SkillDataSODic => _skillDataSODic;
        #endregion


        public async UniTask InitAsync(CancellationToken ct)
        {
            ReleaseDataSOListHandle();
            await LoadMapDataAsync(ct);
            await LoadChapterDataAsync(ct);
            await LoadPlayerDataAsync(ct);
            await LoadEnemyDataAsync(ct);
            await LoadSkillDataAsync(ct);
        }

        private async UniTask LoadChapterDataAsync(CancellationToken ct)
        {
            //어드레서블 어셋을 비동기 로드 시작
            _chapterDataSOListHandle = Addressables.LoadAssetsAsync<ChapterDataSO>(ChapterDataLabel, null);

            //.Net의 기본 Task을 성능 최적화를 위해 AsUniTask()함수를 이용해 UniTask로 변환하여 작업을 진행한다.
            //그리고 AttachExternalCancellation()함수를 통해 해당 비동기 작업이 취소 될 수 있도록
            //CancellationToken을 등록한다.
            await _chapterDataSOListHandle.Task.AsUniTask().AttachExternalCancellation(ct);
            ct.ThrowIfCancellationRequested();

            if (_chapterDataSOListHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"[DataManager] ChapterData load failed: {_chapterDataSOListHandle.OperationException}");
                return;
            }

            _chapterDataSOList.Clear();
            _chapterDataSOList.AddRange(_chapterDataSOListHandle.Result);
            //챕터 아이디로 정렬~
            _chapterDataSOList.Sort((a, b) => a.Id.CompareTo(b.Id));

            LinkChapterMapData();
            LinkChapterThumbnails();

            Debug.Log($"[DataManager] ChapterData loaded: {_chapterDataSOList.Count} items");
        }

        private void LinkChapterThumbnails()
        {
            foreach(var charpterData in _chapterDataSOList)
            {
                Sprite s = _lobbyChapterAtlas.GetSprite(charpterData.DisplaySpriteName);
                charpterData.ThumbnailSprite = s;
            }
        }

        private void LinkChapterMapData()
        {
            foreach (var charpterData in _chapterDataSOList)
            {
                MapDataSO data = null;
                if (MapDataSODic.TryGetValue(charpterData.MapId, out data) == true)
                    charpterData.MapData = data;
                else
                    Debug.LogError($"Map data not found: MapId - {charpterData.MapId}");
            }
        }

        private async UniTask LoadMapDataAsync(CancellationToken ct)
        {
            //어드레서블 어셋을 비동기 로드 시작
            _mapDataSOListHandle = Addressables.LoadAssetsAsync<MapDataSO>(MapDataLabel, null);

            //.Net의 기본 Task을 성능 최적화를 위해 AsUniTask()함수를 이용해 UniTask로 변환하여 작업을 진행한다.
            //그리고 AttachExternalCancellation()함수를 통해 해당 비동기 작업이 취소 될 수 있도록
            //CancellationToken을 등록한다.
            await _mapDataSOListHandle.Task.AsUniTask().AttachExternalCancellation(ct);
            ct.ThrowIfCancellationRequested();

            if (_mapDataSOListHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"[DataManager] MapData load failed: {_mapDataSOListHandle.OperationException}");
                return;
            }

            _mapDataSODic.Clear();
            foreach (var mapData in _mapDataSOListHandle.Result)
            {
                _mapDataSODic.Add(mapData.Id, mapData);
            }

            Debug.Log($"[DataManager] MapData loaded: {_mapDataSODic.Count} items");
        }

        private async UniTask LoadPlayerDataAsync(CancellationToken ct)
        {
            //어드레서블 어셋을 비동기 로드 시작
            _playerDataSOListHandle = Addressables.LoadAssetsAsync<PlayerDataSO>(PlayerDataLabel, true);

            //.Net의 기본 Task을 성능 최적화를 위해 AsUniTask()함수를 이용해 UniTask로 변환하여 작업을 진행한다.
            //그리고 AttachExternalCancellation()함수를 통해 해당 비동기 작업이 취소 될 수 있도록
            //CancellationToken을 등록한다.
            await _playerDataSOListHandle.Task.AsUniTask().AttachExternalCancellation(ct);
            ct.ThrowIfCancellationRequested();

            if (_playerDataSOListHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"[DataManager] PlayerData load failed: {_playerDataSOListHandle.OperationException}");
                return;
            }

            _playerDataDic.Clear();
            foreach (var playerDataSOList in _playerDataSOListHandle.Result)
            {
                List<PlayerData> playerDataList = playerDataSOList.PlayerDataList;
                foreach (var playerData in playerDataList)
                {
                    _playerDataDic.Add(playerData.Id, playerData);
                }
            }

            Debug.Log($"[DataManager] PlayerData loaded: {_playerDataDic.Count} items");
        }

        private async UniTask LoadEnemyDataAsync(CancellationToken ct)
        {
            //어드레서블 어셋을 비동기 로드 시작
            _enemyDataSOListHandle = Addressables.LoadAssetsAsync<EnemyDataSO>(EnemyDataLabel, true);

            //.Net의 기본 Task을 성능 최적화를 위해 AsUniTask()함수를 이용해 UniTask로 변환하여 작업을 진행한다.
            //그리고 AttachExternalCancellation()함수를 통해 해당 비동기 작업이 취소 될 수 있도록
            //CancellationToken을 등록한다.
            await _enemyDataSOListHandle.Task.AsUniTask().AttachExternalCancellation(ct);
            ct.ThrowIfCancellationRequested();

            if (_enemyDataSOListHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"[DataManager] EnemyData load failed: {_enemyDataSOListHandle.OperationException}");
                return;
            }

            _enemyDataDic.Clear();
            foreach (var enemyDataSOList in _enemyDataSOListHandle.Result)
            {
                List<EnemyData> enemyDataList = enemyDataSOList.EnemyDataList;
                foreach (var enemyData in enemyDataList)
                {
                    _enemyDataDic.Add(enemyData.Id, enemyData);
                }
            }

            Debug.Log($"[DataManager] EnemyData loaded: {_enemyDataDic.Count} items");
        }


        private async UniTask LoadSkillDataAsync(CancellationToken ct)
        {
            //어드레서블 어셋을 비동기 로드 시작
            _skillDataSOListHandle = Addressables.LoadAssetsAsync<SkillDataSO>(SkillDataLabel, null);

            await _skillDataSOListHandle.Task.AsUniTask().AttachExternalCancellation(ct);
            ct.ThrowIfCancellationRequested();

            if (_skillDataSOListHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"[DataManager] SkillData load failed: {_skillDataSOListHandle.OperationException}");
                return;
            }

            _skillDataSODic.Clear();
            foreach (var skillData in _skillDataSOListHandle.Result)
            {
                _skillDataSODic.Add(skillData.Id, skillData);
            }

            Debug.Log($"[DataManager] SkillData loaded: {_skillDataSODic.Count} items");
        }


        private void ReleaseDataSOListHandle()
        {
            if(_chapterDataSOListHandle.IsValid() == true)
            {
                Addressables.Release(_chapterDataSOListHandle);
                _chapterDataSOList.Clear();
            }

            if (_mapDataSOListHandle.IsValid() == true)
            {
                Addressables.Release(_mapDataSOListHandle);
                _mapDataSODic.Clear();
            }

            if (_playerDataSOListHandle.IsValid() == true)
            {
                Addressables.Release(_playerDataSOListHandle);
                _playerDataDic.Clear();
            }

            if (_enemyDataSOListHandle.IsValid() == true)
            {
                Addressables.Release(_enemyDataSOListHandle);
                _enemyDataDic.Clear();
            }

            if (_skillDataSOListHandle.IsValid() == true)
            {
                Addressables.Release(_skillDataSOListHandle);
                _skillDataSODic.Clear();
            }
        }

        protected override void OnDestroy()
        {
            ReleaseDataSOListHandle();
            base.OnDestroy();
        }
    }
}
