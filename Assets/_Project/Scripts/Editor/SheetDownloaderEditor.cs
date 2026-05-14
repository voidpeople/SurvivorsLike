#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SurvivorsLike
{
    [CustomEditor(typeof(SheetDownloaderBase), true)]
    public class SheetDownloaderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Download This Sheet", GUILayout.Height(35)))
                ((SheetDownloaderBase)target).StartDownload();
        }
    }

    [CustomEditor(typeof(GameDataDownloadManager))]
    public class GameDataDownloadManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space(5);

            if (GUILayout.Button("Auto Collect Downloaders", GUILayout.Height(30)))
                ((GameDataDownloadManager)target).CollectDownloaders();

            EditorGUILayout.Space(3);

            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("⬇ Download ALL Sheets", GUILayout.Height(45)))
                ((GameDataDownloadManager)target).DownloadAll();
            GUI.backgroundColor = Color.white;
        }
    }
}

#endif
