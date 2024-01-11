using System;
using System.IO;

namespace InTheShadow
{
	public interface ISerializer
	{
		public void Serialize(Stream stream, object obj);

		public T Deserialize<T>(Stream stream);
	}
}