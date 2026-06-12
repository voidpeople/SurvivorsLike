using System.Collections.Generic;
using UnityEngine;


namespace SurvivorsLike
{
    // 구글 시트 전체 행 → GroupKey 기준으로 분류 → SO N개 생성
    public abstract class MultiCollectionSheetDownloader<TSO, TData>
        : CollectionDownloaderBase<TSO, TData>
        where TSO : ScriptableObject
        where TData : class, new()
    {
        protected override void ProcessRows(List<Dictionary<string, string>> rows)
        {
#if UNITY_EDITOR
            var groups   = new Dictionary<string, List<Dictionary<string, string>>>();
            var keyOrder = new List<string>();

            foreach (var row in rows)
            {
                string key = GetGroupKey(row);
                if (string.IsNullOrEmpty(key)) continue;

                if (!groups.ContainsKey(key))
                {
                    groups[key] = new List<Dictionary<string, string>>();
                    keyOrder.Add(key);
                }
                groups[key].Add(row);
            }

            foreach (string key in keyOrder)
            {
                var groupRows = groups[key];
                if (groupRows.Count == 0) continue;

                TSO so = CreateSO(key, groupRows[0]);
                if (so == null) continue;

                var dataList = new List<TData>();
                foreach (var row in groupRows)
                    dataList.Add(ParseRowData(row));

                ApplyDataList(so, dataList);
                SaveAsset(so);
            }

            Debug.Log($"[{GetType().Name}] {keyOrder.Count} SOs created");
#endif
        }

        // 행에서 그룹핑 키 반환 (예: skillId 필드값)
        protected abstract string GetGroupKey(Dictionary<string, string> row);

        // SO 인스턴스 생성 + 공통 헤더 필드 채우기
        // firstRow: 그룹의 첫 번째 행 (SO 공통 필드 초기화에 사용)
        protected abstract TSO CreateSO(string groupKey, Dictionary<string, string> firstRow);
    }
}
