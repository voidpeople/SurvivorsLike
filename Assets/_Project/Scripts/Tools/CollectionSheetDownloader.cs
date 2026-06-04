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
    //List 멤버 변수를 가진 ScriptableObject 상속 클래스를 위한 다운로드~
    //스킬 레벨 데이터을 위해 구현~
    public abstract class CollectionSheetDownloader<TSO, TData> : SheetDownloaderBase
    where TSO : ScriptableObject
    where TData : class, new()
    {
        public override void StartDownload() => StartCoroutine(Co_Download());

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

            //전체 행을 groupKey로 그룹핑
            //skillId=1001 행 => KunaiSkillData 그룹
            //skillId=1002 행 => DrillShotSkillData 그룹
            var groups = new Dictionary<string, List<Dictionary<string, string>>>();
            var keyOrder = new List<string>(); // 시트 등장 순서 보존

            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                string[] values = line.Split('\t');
                var row = new Dictionary<string, string>();
                for (int j = 0; j < headers.Length && j < values.Length; j++)
                    row[headers[j].Trim()] = values[j].Trim();

                string key = GetGroupKey(row); // 서브클래스가 그룹핑 기준 결정
                if (string.IsNullOrEmpty(key)) continue;

                if (!groups.ContainsKey(key))
                {
                    groups[key] = new List<Dictionary<string, string>>();
                    keyOrder.Add(key);
                }
                groups[key].Add(row);
            }

            //기존 에셋 삭제
            ClearSaveFolder();

            //그룹별로 SO 에셋 1개씩 생성
            foreach (string key in keyOrder)
            {
                var groupRows = groups[key];
                if (groupRows.Count == 0) continue;

                //SO 생성 — groupKey + 그룹의 첫 번째 행에서 SkillBaseSO 공통 필드 채우기
                TSO so = CreateSO(key, groupRows[0]);
                if (so == null) continue;

                //그룹 전체 행 → LevelData 파싱 후 리스트 생성
                var dataList = new List<TData>();
                foreach (var row in groupRows)
                    dataList.Add(ParseRowData(row));

                //LevelData 리스트를 SO에 주입
                ApplyDataList(so, dataList);

                //에셋 저장
                string assetPath = $"{saveFolderPath}/{GetAssetFileName(so)}.asset";
                AssetDatabase.CreateAsset(so, assetPath);
                Debug.Log($"[{GetType().Name}] 생성 완료: {assetPath} (레벨 {dataList.Count}개)");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[{GetType().Name}] 전체 저장 완료! (SO 에셋 {keyOrder.Count}개)");
#endif
        }

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

        //행(raw row)에서 그룹핑 키를 반환.
        protected abstract string GetGroupKey(Dictionary<string, string> row);

        ///SO 인스턴스 생성 + SkillBaseSO 공통 필드 채우기.
        protected abstract TSO CreateSO(string groupKey, Dictionary<string, string> firstRow);

        //행 하나 → TData 하나 변환.
        protected abstract TData ParseRowData(Dictionary<string, string> row);

        //파싱된 TData 리스트를 SO 필드(levels 등)에 주입
        protected abstract void ApplyDataList(TSO so, List<TData> dataList);

        //.asset 파일명 (확장자 제외). so의 내용으로 생성
        protected abstract string GetAssetFileName(TSO so);

        //헬퍼 메서드 (SheetDownloaderBase<T>와 동일)
        protected string Str(Dictionary<string, string> r, string k, string d = "")
        {
            return r.TryGetValue(k, out var v) ? v : d;
        }

        protected int Int(Dictionary<string, string> r, string k, int d = 0)
        {
            return r.TryGetValue(k, out var v) && int.TryParse(v, out var n) ? n : d;
        }

        //InvariantCulture: 로케일 무관하게 소수점 파싱 (기존 SheetDownloaderBase<T> 버그 수정)
        protected float Float(Dictionary<string, string> r, string k, float d = 0f)
        {
            return r.TryGetValue(k, out var v) && float.TryParse(
                v,
                System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture,
                out var n) ? n : d;
        }

        protected bool Bool(Dictionary<string, string> r, string k, bool d = false)
        {
            return r.TryGetValue(k, out var v) && bool.TryParse(v, out var n) ? n : d;
        }
    }
}
