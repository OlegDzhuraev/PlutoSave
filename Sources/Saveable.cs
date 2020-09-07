using System;
using System.Collections.Generic;
using UnityEngine;
using PlutoECL.Misc;

namespace PlutoSave
{
    /// <summary> This class should be added to the all prefabs, which should be allowed to save. </summary>
    public class Saveable : MonoBehaviour
    {
        static readonly Dictionary<string, GameObject> createdIds = new Dictionary<string, GameObject>();
        
        [ReadOnly] public string SaveId = String.Empty;
        [ReadOnly] public string PrefabId;

        void OnValidate()
        {
            if (SaveId == String.Empty)
            {
                GenerateId();
            }
            else
            {
                var exist = createdIds.TryGetValue(SaveId, out var savedGo);

                if (!exist)
                    createdIds.Add(SaveId, gameObject);
                else if (savedGo != gameObject)
                    GenerateId();
            }
        }

        void GenerateId()
        {
            SaveId = Guid.NewGuid().ToString();
            createdIds.Add(SaveId, gameObject);
        }
    }
}