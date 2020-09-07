using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using PlutoECL;
using UnityEngine;

namespace PlutoSave
{
    public class Saver : MonoBehaviour
    {
        [SerializeField] AssetMap assetMap;

        ISaveSerializer saveSerialzier = new JsonSaveSerializer();
        
        string GetSavesDirectory() => Application.dataPath + "/Saves";

        public void SetSerializer(ISaveSerializer newSerializer) => saveSerialzier = newSerializer;
        
        public string SaveWorld(string fileName = "Save")
        {
            var saveables = FindObjectsOfType<Saveable>().ToList();
            saveables = saveables.FindAll(sv => sv.GetEntity() != null);

            var saveString = saveSerialzier.Serialize(saveables);
                
            var directoryPath = GetSavesDirectory();
            
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
      
            File.WriteAllText(directoryPath + "/" + fileName + ".sav", saveString);

            return saveString;
        }

        public void LoadWorld(string fileName)
        {
            string serializedSave;

            try
            {
                serializedSave = File.ReadAllText(GetSavesDirectory() + "/" + fileName + ".sav");
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
                return;
            }
            
            var packedEnts = saveSerialzier.Deserialize(serializedSave);

            var saveables = FindObjectsOfType<Saveable>().ToList();
            
            for (var i = 0; i < packedEnts.Length; i++)
                SpawnLoadedEntity(packedEnts[i], saveables);

            // removing all left saveables, which not exist in Save string and destroying it - it is not exist in saved world.
            for (var i = 0; i < saveables.Count; i++)
                saveables[i].GetEntity().Destroy();
        }
        
        Entity SpawnLoadedEntity(PackedEnt packedEnt, List<Saveable> saveablesOnScene)
        {
            Entity ent = null;
            
            for (var i = 0; i < saveablesOnScene.Count; i++)
                if (saveablesOnScene[i].SaveId == packedEnt.SaveId)
                {
                    ent = saveablesOnScene[i].GetEntity();
                    ent.transform.position = packedEnt.Position;
                    ent.transform.rotation = packedEnt.Rotation;
                    
                    saveablesOnScene.RemoveAt(i);
                    break;
                }

            if (ent == null)
            {
                var prefab = assetMap.GetById(packedEnt.PrefabId);
                
                ent = Entity.Spawn(prefab, packedEnt.Position, packedEnt.Rotation);
            }
            
            for (var i = 0; i < packedEnt.Tags.GetLength(0); i++)
            {
                var tag = packedEnt.Tags[i, 0];
                var tagCount = packedEnt.Tags[i, 1];

                var entTagCount = ent.Tags.Count(tag);
                for (int w = 0; w < entTagCount; w++) // better will be to correct tags count, not to make these for loops...
                    ent.Tags.Remove(tag);
                
                for (int w = 0; w < tagCount; w++)
                    ent.Tags.Add(tag); // Pluto ECL can't add several tag count per time, so adding it in for loop
            }

            for (var i = 0; i < packedEnt.Parts.Length; i++)
            {
                var packedPart = packedEnt.Parts[i];
                var type = Type.GetType(packedPart.PartTypeName);

                if (type == null)
                {
                    Debug.LogWarning("Saved type " + packedPart.PartTypeName + " is missing. Skipped from loading.");
                    continue;
                }

                var part = ent.Get(type);

                var packedFields = packedPart.Fields;

                for (var w = 0; w < packedFields.Length; w++)
                {
                    var packedField = packedFields[w];
                    var field = type.GetField(packedField.FieldName);

                    var realFieldType = field.FieldType;

                    // works, but looks dangerouns. If Convert.ChangeType fails, all load fails.
                    try
                    {
                        object unpackedValue;

                        if (packedField.FieldValue is JObject jObj)
                            unpackedValue = jObj.ToObject(realFieldType);
                        else if (packedField.FieldValue is JArray jArr)
                            unpackedValue = jArr.ToObject(realFieldType);
                        else
                            unpackedValue = Convert.ChangeType(packedField.FieldValue, realFieldType);

                        field.SetValue(part, unpackedValue);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex.ToString());
                    }
                }
            }

            return ent;
        }
    }
}