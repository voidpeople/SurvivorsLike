using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(ChapterSheetDownloader))]
public class ChapterSheetDownloaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Download Chapter Data", GUILayout.Height(40)))
        {
            ((ChapterSheetDownloader)target).StartDownload();
        }
    }
}
#endif

