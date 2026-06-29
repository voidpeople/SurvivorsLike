#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.Shapes;

namespace SurvivorsLike.EditorTools
{
    public static class DrillPrefabCreator
    {
        [MenuItem("SurvivorsLike/Create Drill Prefab")]
        public static void CreateDrillPrefab()
        {
            var root = new GameObject("Drill");
            root.layer = 8;

            var proj = root.AddComponent<Projectile>();
            var so = new SerializedObject(proj);
            so.FindProperty("_maxRange").floatValue = 20f;
            so.FindProperty("_lifetime").floatValue = 8f;
            so.FindProperty("_targetLayer").FindPropertyRelative("m_Bits").intValue = 128;
            so.ApplyModifiedProperties();

            // 드릴 몸통 (Cylinder)
            ProBuilderMesh body = ShapeFactory.Instantiate<Cylinder>();
            body.name = "Drill_Body";
            body.transform.SetParent(root.transform);
            body.transform.localPosition = new Vector3(0f, 0f, 0.15f);
            body.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            body.transform.localScale = new Vector3(0.12f, 0.3f, 0.12f);
            body.gameObject.layer = 8;

            // 드릴 끝 (Cone)
            ProBuilderMesh tip = ShapeFactory.Instantiate<Cone>();
            tip.name = "Drill_Tip";
            tip.transform.SetParent(root.transform);
            tip.transform.localPosition = new Vector3(0f, 0f, -0.15f);
            tip.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            tip.transform.localScale = new Vector3(0.12f, 0.2f, 0.12f);
            tip.gameObject.layer = 8;

            const string prefabPath = "Assets/_Project/Projectiles/Prefabs/Drill.prefab";
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            Object.DestroyImmediate(root);
            AssetDatabase.Refresh();

            Debug.Log("[DrillPrefabCreator] Drill prefab saved → " + prefabPath);
            EditorUtility.RevealInFinder(prefabPath);
        }
    }
}
#endif
