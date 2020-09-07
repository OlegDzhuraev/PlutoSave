using System.Collections.Generic;
using PlutoSave;

namespace PlutoSave
{
	public interface ISaveSerializer
	{
		string Serialize(List<Saveable> saveables);
		PackedEnt[] Deserialize(string saveString);
	}
}