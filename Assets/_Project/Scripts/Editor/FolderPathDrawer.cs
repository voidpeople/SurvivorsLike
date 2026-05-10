#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

//PropertyDrawer는 SerializedProperty(직렬화된 필드)의
//Inspector 표시 방식을 커스터마이징하는 클래스
[CustomPropertyDrawer(typeof(FolderPathAttribute))]
public class FolderPathDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var textRect = new Rect(position.x, position.y, position.width - 34, position.height);
        var btnRect = new Rect(position.x + position.width - 32, position.y, 32, position.height);

        EditorGUI.BeginChangeCheck();
        string newValue = EditorGUI.TextField(textRect, label, property.stringValue);
        if (EditorGUI.EndChangeCheck())
        {
            property.stringValue = newValue;
            property.serializedObject.ApplyModifiedProperties();
        }

        if (GUI.Button(btnRect, "..."))
            EnterFolder(property.stringValue);
    }

    //인자로 주어진 경로의 폴더를 에디터이 Project 창에서 열어준다.
    private void EnterFolder(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("[FolderPath] 경로가 비어 있습니다.");
            return;
        }

        var folder = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
        if (folder == null)
        {
            Debug.LogWarning($"[FolderPath] 폴더를 찾을 수 없습니다: {path}");
            return;
        }

        //현재 선택 상태 저장
        var savedObjects = Selection.objects;
        var savedActiveObject = Selection.activeObject;

        Selection.activeObject = folder;
        EditorUtility.FocusProjectWindow();

        var projectBrowserType = typeof(Editor).Assembly
            .GetType("UnityEditor.ProjectBrowser");
        var window = EditorWindow.GetWindow(projectBrowserType);

        bool navigated = TrySetFolderSelection(window, projectBrowserType, folder);

        if (navigated)
        {
            EditorApplication.delayCall += () =>
            {
                Selection.objects = savedObjects;
                Selection.activeObject = savedActiveObject;
            };
        }
        else
        {
            EditorApplication.delayCall += () =>
            {
                EditorWindow.focusedWindow?.SendEvent(Event.KeyboardEvent("return"));
                EditorApplication.delayCall += () =>
                {
                    Selection.objects = savedObjects;
                    Selection.activeObject = savedActiveObject;
                };
            };
        }
    }

    private bool TrySetFolderSelection(
        EditorWindow window, Type projectBrowserType, UnityEngine.Object folder)
    {
        var method = projectBrowserType
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
            .FirstOrDefault(m => m.Name == "SetFolderSelection");

        if (method == null) return false;

        try
        {
            var firstParam = method.GetParameters()[0];
            var elementType = firstParam.ParameterType.GetElementType();

            object idArray;
            if (elementType == typeof(int))
            {
                idArray = new int[] { folder.GetInstanceID() };
            }
            else
            {
                var entityId = Activator.CreateInstance(
                    elementType,
                    BindingFlags.CreateInstance, null,
                    new object[] { folder.GetInstanceID() }, null);

                var arr = Array.CreateInstance(elementType, 1);
                arr.SetValue(entityId, 0);
                idArray = arr;
            }

            method.Invoke(window, new object[] { idArray, true });
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[FolderPath] SetFolderSelection 실패: {e.Message}");
            return false;
        }
    }
}
#endif
