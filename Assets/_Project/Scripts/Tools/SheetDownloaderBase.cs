using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace SurvivorsLike
{
    public abstract class SheetDownloaderBase : MonoBehaviour
    {
        [Header("Sheet 설정")]
        public string sheetName;        // 구글 시트 탭 이름

        //인스펙터 창에서 saveFolderPath에 폴더 경로를 열어주는 버튼을 추가해 준다.
        [FolderPath]
        public string saveFolderPath;   // SO 저장 경로

        // Manager가 호출 전에 주입
        [HideInInspector] public string spreadsheetId;

        public string BuildUrl() =>
            $"https://docs.google.com/spreadsheets/d/{spreadsheetId}/export?format=tsv&sheet={sheetName}";

        public abstract void StartDownload();
    }

    //구글 스프레드시트에서 데이터을 tsv 형식으로 다운 받아서 JSON으로 변환한 후
    //ScriptableObject에 저장하는 클래스
    public abstract class SheetDownloaderBase<T> : SheetDownloaderBase
        where T : ScriptableObject
    {
        //외부 호출 진입점
        public override void StartDownload()
        {
            StartCoroutine(Co_Download());
        }

        private IEnumerator Co_Download()
        {
            Debug.Log($"[{GetType().Name}] 다운로드 시작...");

            using var www = UnityWebRequest.Get(BuildUrl());
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[{GetType().Name}] 다운로드 실패: {www.error}");
                yield break;
            }

            ApplyTsvToSO(www.downloadHandler.text);
        }

        private void ApplyTsvToSO(string tsv)
        {
#if UNITY_EDITOR
            string[] lines = tsv.Split('\n');
            if (lines.Length < 2)
            {
                Debug.LogWarning($"[{GetType().Name}] 데이터가 없습니다.");
                return;
            }

            string[] headers = lines[0].Trim().Split('\t');

            ClearSaveFolder();

            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                string[] values = line.Split('\t');

                var row = new Dictionary<string, string>();
                for (int j = 0; j < headers.Length && j < values.Length; j++)
                    row[headers[j].Trim()] = values[j].Trim();

                //서브클래스가 SO 생성 및 필드 매핑을 담당
                T so = CreateSO(row);
                if (so == null) continue;

                string assetPath = $"{saveFolderPath}/{GetAssetFileName(so)}.asset";
                AssetDatabase.CreateAsset(so, assetPath);
                Debug.Log($"[{GetType().Name}] 생성 완료: {assetPath}");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[{GetType().Name}] 전체 저장 완료!");
#endif
        }

        protected abstract T CreateSO(Dictionary<string, string> row);

        //저장할 .asset 파일명 (확장자 제외)
        protected abstract string GetAssetFileName(T so);

        //Get 함수들~
        protected string Str(Dictionary<string, string> r, string k, string d = "") => r.TryGetValue(k, out var v) ? v : d;
        protected int Int(Dictionary<string, string> r, string k, int d = 0) => r.TryGetValue(k, out var v) && int.TryParse(v, out var n) ? n : d;
        protected float Float(Dictionary<string, string> r, string k, float d = 0f) => r.TryGetValue(k, out var v) && float.TryParse(v, out var n) ? n : d;
        protected bool Bool(Dictionary<string, string> r, string k, bool d = false) => r.TryGetValue(k, out var v) && bool.TryParse(v, out var n) ? n : d;

        private void ClearSaveFolder()
        {
#if UNITY_EDITOR
            if (!Directory.Exists(saveFolderPath)) return;
            foreach (var file in Directory.GetFiles(saveFolderPath, "*.asset"))
            {
                AssetDatabase.DeleteAsset(file);
                Debug.Log($"[{GetType().Name}] 삭제: {file}");
            }
#endif
        }
    }
}
