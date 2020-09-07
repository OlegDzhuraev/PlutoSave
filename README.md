# PlutoSave
Save system for **Pluto ECL** in Unity. Allows to save world state easily.

## Requirements
* [Pluto ECL](https://github.com/InsaneOneHub/PlutoECL). This is a base for PlutoSave save system. It saves only Pluto Entities objects with Saveable component added and Save attribute near Components fields which should be saved.
* [Newtonsoft JSON package](https://github.com/InsaneOneHub/PlutoECL). Used as Serializer to save world state into JSON. 

In future updates I want to add support of custom serialization, but now only JSON supported.

## What will be saved?
Saved only objects, which have Prefab. Also required PlutoECL.Entity and Saveable components.

This data will be saved for every Entity: 
* Position
* Rotation
* All Entity Tags
* Parts (Components) with fields, which have [Save] atrribute.
  * All fields with [Save] attribute, which can be serialized to Json. Do not use Save attribute for GameObject, Transform, etc.

## How to save
You need to place object with a **Saver** component added to it. Next, fill Asset Map field of **Saver** with your AssetMap. Finally, you can use its **SaveWorld** and **LoadWorld** methods.

In future updates I want to make it singleton or static, currently not decided what will be better.

## AssetMap
This is ScriptableObject, which contains info about all Prefabs which can be saved by PlutoSave. It generates unique IDs for them.

More info will be added soon.

## License
MIT License.
