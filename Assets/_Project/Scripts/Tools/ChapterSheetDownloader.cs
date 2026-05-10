
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using SurvivorsLike;
using UnityEditor;



public class ChapterSheetDownloader : MonoBehaviour
{
    // -------------------------------------------------------
    // Inspector 설정값
    // -------------------------------------------------------
    [Header("Google Sheet URL")]
    [Tooltip("/edit 이전 URL + export 파라미터")]
    //시트 URL에 마지막 "/edit?gid=0#gid=0"을 삭제하고 "/export?format=tsv&sheet=ChapterData"을 붙여야 함~
    public string sheetUrl = "https://docs.google.com/spreadsheets/d/1LHwKC_vGQ577d8Uy7hpc-UVRnHYrvAofJg3PDbFtXlE/export?format=tsv&sheet=ChapterData";

    [Header("SO 저장 경로")]
    public string saveFolderPath = "Assets/_Project/Data/Stage";


    //다운로드 코루틴 실행~
    public void StartDownload()
    {
        StartCoroutine(Co_Download());
    }

    private IEnumerator Co_Download()
    {
        Debug.Log("[ChapterSheet] 다운로드 시작...");

        using var www = UnityWebRequest.Get(sheetUrl);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"[ChapterSheet] 다운로드 실패: {www.error}");
            yield break;
        }

        ApplyTsvToSO(www.downloadHandler.text);
    }

    // TSV을 ChapterDataSO에 적용
    private void ApplyTsvToSO(string tsv)
    {
        string[] lines = tsv.Split('\n');
        if (lines.Length < 2)
        {
            Debug.LogWarning("[ChapterSheet] 데이터가 없습니다.");
            return;
        }

        // 헤더 파싱
        string[] headers = lines[0].Trim().Split('\t');

        // 기존 SO 전체 삭제
        ClearFolder();

        // 데이터 행 순회
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] values = line.Split('\t');

            // 헤더-값 딕셔너리 구성
            var row = new System.Collections.Generic.Dictionary<string, string>();
            for (int j = 0; j < headers.Length && j < values.Length; j++)
                row[headers[j].Trim()] = values[j].Trim();

            // SO 생성 및 값 적용
            var so = ScriptableObject.CreateInstance<ChapterDataSO>();
            so.chapterID = ParseInt(row, "chapterID");
            so.displayName = ParseStr(row, "displayName");
            so.difficulty = ParseStr(row, "difficulty");
            so.displaySpriteName = ParseStr(row, "displaySpriteName");
            so.recommendedCP = ParseInt(row, "recommendedCP");
            so.energyCost = ParseInt(row, "energyCost", 5);
            so.durationSec = ParseFloat(row, "durationSec", 900f);
            so.rewardGold = ParseInt(row, "rewardGold");
            so.rewardGem = ParseInt(row, "rewardGem");
            
            // 파일 저장
            string assetPath = $"{saveFolderPath}/Chapter_{so.chapterID:D2}.asset";
            AssetDatabase.CreateAsset(so, assetPath);
            Debug.Log($"[ChapterSheet] 생성 완료: {assetPath}");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[ChapterSheet] 전체 저장 완료!");
    }

    //파싱~
    private string ParseStr(System.Collections.Generic.Dictionary<string, string> row,
                            string key, string fallback = "")
        => row.TryGetValue(key, out var v) ? v : fallback;

    private int ParseInt(System.Collections.Generic.Dictionary<string, string> row,
                         string key, int fallback = 0)
        => row.TryGetValue(key, out var v) && int.TryParse(v, out var r) ? r : fallback;

    private float ParseFloat(System.Collections.Generic.Dictionary<string, string> row,
                              string key, float fallback = 0f)
        => row.TryGetValue(key, out var v) && float.TryParse(v, out var r) ? r : fallback;


    //기존의 스크립트블 오브젝트 삭제
    private void ClearFolder()
    {
#if UNITY_EDITOR
        if (!Directory.Exists(saveFolderPath)) return;

        foreach (var file in Directory.GetFiles(saveFolderPath, "*.asset"))
        {
            if(AssetDatabase.DeleteAsset(file) == true)
                Debug.Log($"[ChapterSheet] 삭제: {file}");
        }
#endif
    }
}
