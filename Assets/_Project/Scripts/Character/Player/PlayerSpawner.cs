using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;


namespace SurvivorsLike
{
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] private string _playerBaseAddressKey = "character/player/player_base";
        [SerializeField] private JoystickBase _joystick;

        public PlayerController SpawnPlayerController { get; private set; }

        private AsyncOperationHandle<GameObject> _baseHandle;
        private AsyncOperationHandle<GameObject> _modelHandle;

        private void Awake()
        {
        }

        public async UniTask SpawnAsync(CancellationToken ct)
        {
            Vector3 spawnPos = GetSpawnPosition();
            SpawnPlayerController = await CreatePlayerAsync(spawnPos, ct);
        }

        private Vector3 GetSpawnPosition()
        {
            PlayerSpawnPoint point = FindFirstObjectByType<PlayerSpawnPoint>();
            return point != null ? point.transform.position : Vector3.zero;
        }

        private async UniTask<PlayerController> CreatePlayerAsync(Vector3 pos, CancellationToken ct)
        {
            GameSessionData sessionData = GameManager.Instance.SessionData;

            _baseHandle = Addressables.LoadAssetAsync<GameObject>(_playerBaseAddressKey);
            _modelHandle = Addressables.LoadAssetAsync<GameObject>(sessionData.PlayerData.PrefabKey);

            //UniTask.WhenAll()은 메인 스레드에서 여러 비동기 작업을 동시에 시작해서
            //I/O 대기 시간을 겹치게 하는 명령 이지만 멀티스레드 병렬 처리는 아님~
            //프리팹들을 병렬로 로드
            var (basePrefab, modelPrefab) = await UniTask.WhenAll(
                _baseHandle.ToUniTask(cancellationToken: ct),
                _modelHandle.ToUniTask(cancellationToken: ct));

            GameObject playerObj = Instantiate(basePrefab, pos, Quaternion.identity);
            PlayerController playerCtrl = playerObj.GetComponent<PlayerController>();

            GameObject modelObj = Instantiate(modelPrefab, playerCtrl.ModelRoot);
            modelObj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            modelObj.transform.localScale = Vector3.one;

            PlayerAnimationController aniCtrl= modelObj.GetComponent<PlayerAnimationController>();
            DataManager.Instance.PlayerDataDic.TryGetValue(1001, out PlayerData playerData);
            playerCtrl.Init(playerData, aniCtrl, _joystick);

            return playerCtrl;
        }
    }
}
