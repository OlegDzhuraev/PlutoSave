using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PlutoSave
{
    [CustomEditor(typeof(AssetMap))]
    public class AssetMapEditor : Editor
    {
        GameObject selectedObject;
        bool foldout;

        AssetMap assetMap;
        readonly List<MappedPrefab> toRemove = new List<MappedPrefab>();
        
        public override void OnInspectorGUI()
        {
            assetMap = target as AssetMap;

            foldout = EditorGUILayout.Foldout(foldout, "Prefabs list");

            if (foldout)
            {
                for (var i = 0; i < assetMap.Map.Count; i++)
                    DrawMapped(assetMap.Map[i]);
                
                GUILayout.Space(10);
            }

            if (toRemove.Count > 0)
            {
                for (var i = 0; i < toRemove.Count; i++)
                    assetMap.Map.Remove(toRemove[i]);

                EditorUtility.SetDirty(target);
                
                toRemove.Clear();
            }

            if (GUILayout.Button("Gather saveables from prefabs"))
                GatherSaveables();
            
            if (GUILayout.Button("Update map prefab IDs"))
                assetMap.UpdateMap();
            
            GUILayout.Space(10);
            
            GUILayout.Label("Add new prefab maually", EditorStyles.boldLabel);
            
            GUILayout.BeginHorizontal();
            selectedObject = EditorGUILayout.ObjectField(selectedObject, typeof(GameObject), false) as GameObject;

            var prevGUIEnabled = GUI.enabled;
            GUI.enabled = selectedObject;
            
            if (GUILayout.Button("Add"))
                AddPrefab(assetMap, selectedObject);

            GUI.enabled = prevGUIEnabled;
            
            GUILayout.EndHorizontal();
        }

        void DrawMapped(MappedPrefab mappedPrefab)
        {
            var saveable = mappedPrefab.Prefab.GetComponent<Saveable>();

            var prevColor = GUI.color;
            
            if (!saveable)
                GUI.color = Color.red;
            else if (saveable.PrefabId != mappedPrefab.Id)
                GUI.color = Color.yellow;
            
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.BeginVertical();
                    
            GUILayout.Label(mappedPrefab.Prefab.name, EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Prefab id: ", GUILayout.MaxWidth(60));

            var prevEnabled = GUI.enabled;
            GUI.enabled = false;
            GUILayout.TextField(mappedPrefab.Id, GUILayout.MinWidth(120));
            GUI.enabled = prevEnabled;
            GUILayout.EndHorizontal();

            mappedPrefab.Prefab =
                (GameObject) EditorGUILayout.ObjectField(mappedPrefab.Prefab, typeof(GameObject), false);
                    
            GUILayout.EndVertical();
                    
            GUILayout.BeginVertical(GUILayout.Width(48));
            if (GUILayout.Button("Remove"))
                toRemove.Add(mappedPrefab);
            GUILayout.EndVertical();
                    
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUI.color = prevColor;
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

        void GatherSaveables()
        {
            var gos = AssetDatabase.FindAssets("t:GameObject");

            var gatheredCount = 0;
            
            for (var i = 0; i < gos.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(gos[i]);
                var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (go.GetComponent<Saveable>())
                {
                    if (assetMap.Map.Find(mp => mp.Prefab == go) != null)
                        continue;
                    
                    var mappedPrefab = new MappedPrefab();
                    mappedPrefab.Prefab = go;
                    mappedPrefab.UpdateId();
                    
                    assetMap.Map.Add(mappedPrefab);

                    gatheredCount++;
                }
            }
            
            Debug.Log("[Pluto Save] Gathered " + gatheredCount + " new saveable prefabs.");
            
            if (gatheredCount > 0)
                EditorUtility.SetDirty(target);
        }
    }
}