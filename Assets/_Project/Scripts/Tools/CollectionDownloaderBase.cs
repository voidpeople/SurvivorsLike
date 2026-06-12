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
    // Single/Multi 공통 로직: 다운로드, TSV 파싱, 폴더 정리, 에셋 저장, 헬퍼
    public abstract class CollectionDownloaderBase<TSO, TData> : SheetDownloaderBase
        where TSO : ScriptableObject
        where TData : class, new()
    {
        public override void StartDownload() => StartCoroutine(Co_Download());

        private IEnumerator Co_Download()
        {
            Debug.Log($"[{GetType().Name}] Download started...");

            using var www = UnityWebRequest.Get(BuildUrl());
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[{GetType().Name}] Download failed: {www.error}");
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
                Debug.LogWarning($"[{GetType().Name}] No data available.");
                return;
            }

            string[] headers = lines[0].Trim().Split('\t');
            var rows = new List<Dictionary<string, string>>();

            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                string[] values = line.Split('\t');
                var row = new Dictionary<string, string>();
                for (int j = 0; j < headers.Length && j < values.Length; j++)
                    row[headers[j].Trim()] = values[j].Trim();
                rows.Add(row);
            }

            ClearSaveFolder();
            ProcessRows(rows);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[{GetType().Name}] All assets saved!");
#endif
        }

        // Template Method: Single은 전체 rows → SO 1개, Multi는 GroupKey별 → SO N개
        protected abstract void ProcessRows(List<Dictionary<string, string>> rows);

        // 공통 추상 메서드 (Single/Multi 동일 시그니처)
        protected abstract TData  ParseRowData(Dictionary<string, string> row);
        protected abstract void   ApplyDataList(TSO so, List<TData> dataList);
        protected abstract string GetAssetFileName(TSO so);

        // 에셋 저장 헬퍼
        protected void SaveAsset(TSO so)
        {
#if UNITY_EDITOR
            string assetPath = $"{saveFolderPath}/{GetAssetFileName(so)}.asset";
            AssetDatabase.CreateAsset(so, assetPath);
            Debug.Log($"[{GetType().Name}] Created: {assetPath}");
#endif
        }

        private void ClearSaveFolder()
        {
#if UNITY_EDITOR
            if (!Directory.Exists(saveFolderPath)) return;
            foreach (var file in Directory.GetFiles(saveFolderPath, "*.asset"))
            {
                AssetDatabase.DeleteAsset(file);
                Debug.Log($"[{GetType().Name}] Deleted: {file}");
            }
#endif
        }

    }
}
