using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PlutoSave
{
    [CreateAssetMenu]
    public class AssetMap : ScriptableObject
    {
        public List<MappedPrefab> Map;

        public GameObject GetById(string id)
        {
            for (var i = 0; i < Map.Count; i++)
                if (Map[i].Id == id)
                    return Map[i].Prefab;

            throw new ArgumentOutOfRangeException("No prefab with this id registered");
        }

        public void UpdateMap()
        {
            for (var i = 0; i < Map.Count; i++)
                Map[i].UpdateId();
            
            Debug.Log("[Pluto Save] Saveable prefabs IDs was updated.");

        }
    }

    [System.Serializable]
    public class MappedPrefab
    {
        [PlutoECL.Misc.ReadOnly] public string Id = String.Empty;
        public GameObject Prefab;

        public void UpdateId()
        {
            if (Id == String.Empty)
                Id = Guid.NewGuid().ToString();

            var saveable = Prefab.GetComponent<Saveable>();

            if (!saveable)
                saveable = Prefab.AddComponent<Saveable>();

            saveable.PrefabId = Id;
            
            #if UNITY_EDITOR
            EditorUtility.SetDirty(Prefab);
            #endif
        }
    }
}