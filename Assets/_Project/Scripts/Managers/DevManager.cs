using Cysharp.Threading.Tasks;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.Universal;


namespace SurvivorsLike
{
    //개발용 매니저~
    public class DevManager : MonoBehaviour
    {
        [Header("Lobby")]

        [Header("InGame")]
        [SerializeField] private GameObject _managerPrefab;
        [SerializeField] private int _loadChapterId = 8001;

#if UNITY_EDITOR
        private void Awake()
        {
            Init();
        }

        void Init()
        {
            //매니저들이 없다면 매니저 프리팹 로드
            if (Object.FindFirstObjectByType<GameManager>() == null)
            {
                //에디터에서 InGame씬 등에서 곧바로 플레이 할 경우 매니저들이 로드 안 되어 있어
                //오류가 발생하므로 그에 대한 처리~
                Instantiate(_managerPrefab);
            }
        }

        //Bootstrap 씬 부터 정상적인 절차로 게임을 진행하지 않고 에디터에서
        //InGame씬을 곧바로 플레이 할 경우 그와 관련된 매니저들의 사전 작업을 실행하는 함수
        public async UniTask PrepareInGameAsync(CancellationToken ct = default)
        {
            await DataManager.Instance.InitAsync(ct);

            ChapterDataSO chapter = DataManager.Instance.ChapterDataSOList.FirstOrDefault<ChapterDataSO>(c => c.Id == _loadChapterId);
            if(chapter == null)
            {
                Debug.LogWarning($"선택한 챕터를 찾을 수 없습니다. : chapterId - {_loadChapterId}");
                chapter = DataManager.Instance.ChapterDataSOList.FirstOrDefault<ChapterDataSO>(c => c.Id == 1);
            }
            GameManager.Instance.GameSessionData.Clear();
            DataManager.Instance.PlayerDataDic.TryGetValue(1001, out PlayerData playerData);
            GameManager.Instance.GameSessionData.Init(chapter, playerData);
        }
#endif
    }
}
