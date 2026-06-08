using System.Collections.Generic;
using UnityEngine;

namespace SurvivorsLike
{
    // 구글 시트 전체 행 → SO 1개의 List에 모두 저장
    public abstract class SingleCollectionSheetDownloader<TSO, TData>
        : CollectionDownloaderBase<TSO, TData>
        where TSO : ScriptableObject
        where TData : class, new()
    {
        protected override void ProcessRows(List<Dictionary<string, string>> rows)
        {
#if UNITY_EDITOR
            TSO so = CreateSO();
            if (so == null) return;

            var dataList = new List<TData>();
            foreach (var row in rows)
                dataList.Add(ParseRowData(row));

            ApplyDataList(so, dataList);
            SaveAsset(so);
            Debug.Log($"[{GetType().Name}] SO 1개 생성 완료 (총 {dataList.Count}행)");
#endif
        }

        // SO 인스턴스 생성
        // 전체 시트가 하나의 SO이므로 파라미터 없음
        protected abstract TSO CreateSO();
    }
}
