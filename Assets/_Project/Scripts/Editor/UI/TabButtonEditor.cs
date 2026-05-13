using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(TabButton))]
public class TabButtonEditor : ButtonEditor
{
    private SerializedProperty _normalSprite;
    private SerializedProperty _selectedSprite;

    protected override void OnEnable()
    {
        base.OnEnable();
        _normalSprite = serializedObject.FindProperty("_normalSprite");
        _selectedSprite = serializedObject.FindProperty("_selectedSprite");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); // Button 기본 Inspector 표시

        serializedObject.Update();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Tab Button", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_normalSprite);
        EditorGUILayout.PropertyField(_selectedSprite);
        serializedObject.ApplyModifiedProperties();
    }
}
