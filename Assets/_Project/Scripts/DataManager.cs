using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
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
        [Header("아틀라스")]
        [SerializeField] private SpriteAtlas _lobbyChapterAtlas;

        private const string ChapterDataLabel = "ChapterData";

        private AsyncOperationHandle<IList<ChapterDataSO>> _chapterDataSOListHandle;
        private readonly List<ChapterDataSO> _chapterDataSOList = new();

        //IReadOnlyList은 Add, Remove, Clear 등의 수정 메서드가 없는 인터페이스
        public IReadOnlyList<ChapterDataSO> ChapterDataSOList => _chapterDataSOList;

        public UniTask InitAsync(CancellationToken ct)
        {
            //챕터 데이터 파일 비동기 로드
            return LoadChapterDataAsync(ct);
        }

        //dㅇㅁㄴ
        private async UniTask LoadChapterDataAsync(CancellationToken ct)
        {
            ReleaseChapterDataSOListHandle();

            //어드레서블 어셋을 비동기 로드 시작
            _chapterDataSOListHandle = Addressables.LoadAssetsAsync<ChapterDataSO>(ChapterDataLabel, null);

            //.Net의 기본 Task을 성능 최적화를 위해 AsUniTask()함수를 이용해 UniTask로 변환하여 작업을 진행한다.
            //그리고 AttachExternalCancellation()함수를 통해 해당 비동기 작업이 취소 될 수 있도록
            //CancellationToken을 등록한다.
            await _chapterDataSOListHandle.Task.AsUniTask().AttachExternalCancellation(ct);
            ct.ThrowIfCancellationRequested();

            if (_chapterDataSOListHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"[DataManager] ChapterData 로드 실패: {_chapterDataSOListHandle.OperationException}");
                return;
            }

            _chapterDataSOList.Clear();
            _chapterDataSOList.AddRange(_chapterDataSOListHandle.Result);
            //챕터 아이디로 정렬~
            _chapterDataSOList.Sort((a, b) => a.chapterID.CompareTo(b.chapterID));

            BindChapterThumbnails();

            Debug.Log($"[DataManager] ChapterData {_chapterDataSOList.Count}개 로드 완료");
        }

        private void BindChapterThumbnails()
        {
            foreach(var charpterData in _chapterDataSOList)
            {
                Sprite s = _lobbyChapterAtlas.GetSprite(charpterData.displaySpriteName);
                charpterData.thumbnail = s;
            }
        }

        private void ReleaseChapterDataSOListHandle()
        {
            if(_chapterDataSOListHandle.IsValid() == true)
            {
                Addressables.Release(_chapterDataSOListHandle);
                _chapterDataSOList.Clear();
            }
        }

        protected override void OnDestroy()
        {
            ReleaseChapterDataSOListHandle();
            base.OnDestroy();
        }
    }
}
