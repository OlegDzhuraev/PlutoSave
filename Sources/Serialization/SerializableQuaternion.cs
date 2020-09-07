 using UnityEngine;
 using System;

 namespace PlutoSave
 {
     /// <summary> Since unity doesn't flag the Quaternion as serializable, we
     /// need to create our own version. This one will automatically convert
     /// between Quaternion and SerializableQuaternion </summary>
     [System.Serializable]
     public struct SerializableQuaternion
     {
         public float x, y, z, w;

         public SerializableQuaternion(float rX, float rY, float rZ, float rW)
         {
             x = rX;
             y = rY;
             z = rZ;
             w = rW;
         }

         /// <summary> Returns a string representation of the object </summary>
         public override string ToString() => String.Format("[{0}, {1}, {2}, {3}]", x, y, z, w);

         /// <summary> Automatic conversion from SerializableQuaternion to Quaternion </summary>
         public static implicit operator Quaternion(SerializableQuaternion rValue)
         {
             return new Quaternion(rValue.x, rValue.y, rValue.z, rValue.w);
         }

         /// <summary> Automatic conversion from Quaternion to SerializableQuaternion </summary>
         public static implicit operator SerializableQuaternion(Quaternion rValue)
         {
             return new SerializableQuaternion(rValue.x, rValue.y, rValue.z, rValue.w);
         }
     }
 }