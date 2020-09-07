namespace PlutoSave
{
	[System.Serializable]
	public class PackedEnt
	{
		public string SaveId;
		public string PrefabId;
		
		public SerializableVector3 Position;
		public SerializableQuaternion Rotation;
		
		public int[,] Tags;
		public PackedPart[] Parts;
	}
	
	[System.Serializable]
	public class PackedPart
	{
		public string PartTypeName;
		public PackedField[] Fields;
	}

	[System.Serializable]
	public class PackedField
	{
		public string FieldName;
		public object FieldValue;
	}
}