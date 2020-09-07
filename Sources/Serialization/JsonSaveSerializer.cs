using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using PlutoECL;

namespace PlutoSave
{
    public class JsonSaveSerializer : ISaveSerializer
    {
        JsonWriter writer;
        JsonReader reader;
        readonly JsonSerializer serializer;

        readonly Formatting formatting = Formatting.None;
        
        public JsonSaveSerializer()
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            
            serializer = JsonSerializer.Create(settings);
        }
        
        public string Serialize(List<Saveable> saveables)
        {
            var packedEnts = new PackedEnt[saveables.Count];
            
            for (int q = 0; q < saveables.Count; q++)
                WriteEntity(saveables[q], q, packedEnts);
            
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            
            using (writer = new JsonTextWriter(sw))
            {
                writer.Formatting = formatting;

                serializer.Serialize(writer, packedEnts);
            }

            return sw.ToString();
        }
        
        void WriteEntity(Saveable saveable, int id, PackedEnt[] packedEnts)
        {
            var packedEnt = packedEnts[id] = new PackedEnt();
            var entity = saveable.GetEntity();
            
            packedEnt.Position = entity.transform.position;
            packedEnt.Rotation = entity.transform.rotation;
            packedEnt.SaveId = saveable.SaveId;
            packedEnt.PrefabId = saveable.PrefabId;
            
            var tags = entity.Tags.GetTagsCopy();
            packedEnt.Tags = new int[tags.Count, 2];
            var tagIndex = 0;
            
            foreach (var tagCountPair in tags)
            {
                packedEnt.Tags[tagIndex, 0] = (int)tagCountPair.Key;
                packedEnt.Tags[tagIndex, 1] = tagCountPair.Value;
                
                tagIndex++;
            }

            var parts = entity.GetComponents<Part>();

            var packedParts = new List<PackedPart>();

            for (var w = 0; w < parts.Length; w++)
                WritePart(parts[w], packedParts);
                
            packedEnt.Parts = packedParts.ToArray();
        }
        
        void WritePart(Part part, List<PackedPart> packedParts)
        {
            var type = part.GetType();
            var fields = type.GetFields();
                    
            var needSave = false;
            
            for (var e = 0; e < fields.Length; e++)
                if (Attribute.IsDefined(fields[e], typeof(SaveAttribute)))
                {
                    needSave = true;
                    break;
                }

            if (!needSave)
                return;

            var packedPart = new PackedPart
            {
                PartTypeName = type.FullName
            };

            var packedFields = new List<PackedField>();
                    
            for (var e = 0; e < fields.Length; e++)
                WriteField(part, fields[e], packedFields);

            packedPart.Fields = packedFields.ToArray();

            packedParts.Add(packedPart);
        }
        
        void WriteField(Part part, FieldInfo field, List<PackedField> packedFields)
        {
            if (!Attribute.IsDefined(field, typeof(SaveAttribute)))
                return;

            var packedField = new PackedField
            {
                FieldName = field.Name, 
                FieldValue = field.GetValue(part)
            };
            
            packedFields.Add(packedField);
        }

        public PackedEnt[] Deserialize(string saveString)
        {
            reader = new JsonTextReader(new StringReader(saveString));
            
            return (PackedEnt[]) serializer.Deserialize(reader, typeof(PackedEnt[]));
        }
    }
}