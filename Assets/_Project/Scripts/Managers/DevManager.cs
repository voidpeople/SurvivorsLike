using Cysharp.Threading.Tasks;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.Universal;


namespace SurvivorsLike
{
    public class DevManager : MonoBehaviour
    {
        [SerializeField] private GameObject _managerPrefab;
        [SerializeField] private int _chapterId = 1;

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
                Instantiate(_managerPrefab);
            }
        }

        public async UniTask PrepareInGameAsync(CancellationToken ct)
        {
            await DataManager.Instance.InitAsync(ct);

            ChapterDataSO chapter = DataManager.Instance.ChapterDataSOList.FirstOrDefault<ChapterDataSO>(c => c.chapterId == _chapterId);
            if(chapter == null)
            {
                Debug.LogWarning($"선택한 챕터를 찾을 수 없습니다. : chapterId - {_chapterId}");
                chapter = DataManager.Instance.ChapterDataSOList.FirstOrDefault<ChapterDataSO>(c => c.chapterId == 1);
            }
            GameManager.Instance.SessionData.Clear();
            GameManager.Instance.SessionData.Init(chapter);
        }
#endif
    }
}
