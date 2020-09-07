using UnityEditor;
using UnityEngine;

namespace PlutoSave
{
    [CustomEditor(typeof(AssetMap))]
    public class AssetMapEditor : Editor
    {
        GameObject selectedObject;
        
        public override void OnInspectorGUI()
        {
            var assetMap = (target as AssetMap);

            DrawDefaultInspector();
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Update map"))
                (target as AssetMap).UpdateMap();
            
            GUILayout.Space(10);
            
            GUILayout.Label("Add new prefab", EditorStyles.boldLabel);
            
            GUILayout.BeginHorizontal();
            selectedObject = EditorGUILayout.ObjectField(selectedObject, typeof(GameObject), false) as GameObject;

            var prevGUIEnabled = GUI.enabled;
            GUI.enabled = selectedObject;

            
            if (GUILayout.Button("Add"))
                AddPrefab(assetMap, selectedObject);

            GUI.enabled = prevGUIEnabled;
            
            GUILayout.EndHorizontal();
        }

        void AddPrefab(AssetMap assetMap, GameObject prefab)
        {
            for (var i = 0; i < assetMap.Map.Count; i++)
                if (assetMap.Map[i].Prefab == prefab)
                {
                    Debug.LogWarning("Same prefab already added to the Map");
                    return;
                }
            
            var mappedPrefab = new MappedPrefab();
            mappedPrefab.Prefab = prefab;
            mappedPrefab.UpdateId();
            
            assetMap.Map.Add(mappedPrefab);
                
            EditorUtility.SetDirty(target);
        }
    }
}