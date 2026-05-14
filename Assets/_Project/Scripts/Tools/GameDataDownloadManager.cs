using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace SurvivorsLike
{
    //하이라키 상의 SheetDownloaderBase 상속 클래스 객체들을 찾아서
    //관련 구글 시트들을 다운로드 테이블 데이터를 스크립트블 오브젝트에 저장한다.
    public class GameDataDownloadManager : MonoBehaviour
    {
        [Header("구글 시트의 URL에서 ID 가져와 설정")]
        [SerializeField] private string _spreadsheetId;

        [Header("Sheet Downloaders")]
        [SerializeField] private SheetDownloaderBase[] _downloaders;

        public void DownloadAll()
        {
            foreach (var downloader in _downloaders)
            {
                if (downloader == null) continue;
                downloader.spreadsheetId = _spreadsheetId; // 구글 시트 ID
                downloader.StartDownload();
            }
        }

        public void CollectDownloaders()
        {
            _downloaders = GetComponents<SheetDownloaderBase>();
            Debug.Log($"[GameDataDownloadManager] {_downloaders.Length}개 수집 완료");
        }
    }
}

