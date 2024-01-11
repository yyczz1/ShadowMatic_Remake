using System.IO;
using System.Text;
using UnityEngine;
using Object = System.Object;

namespace InTheShadow
{
	public class BinaryJsonSerializer : ISerializer
	{
		public void Serialize(Stream stream, object obj)
		{
			using BinaryWriter binaryWriter = new BinaryWriter(stream);
			string jsonString = JsonUtility.ToJson(obj);
			binaryWriter.Write(jsonString);
		}

		public T Deserialize<T>(Stream stream)
		{
			using BinaryReader binaryReader = new BinaryReader(stream);
			string jsonString = binaryReader.ReadString();
			T result = JsonUtility.FromJson<T>(jsonString);
			return result;
		}
	}
}