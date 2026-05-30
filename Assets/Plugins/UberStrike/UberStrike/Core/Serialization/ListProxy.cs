using System.Collections.Generic;
using System.IO;

namespace UberStrike.Core.Serialization
{
	public static class ListProxy<T>
	{
		public delegate void Serializer<U>(Stream stream, U instance);

		public delegate U Deserializer<U>(Stream stream);

		public static void Serialize(Stream bytes, ICollection<T> instance, Serializer<T> serialization)
		{
			UShortProxy.Serialize(bytes, (ushort)instance.Count);
			foreach (T item in instance)
			{
				serialization(bytes, item);
			}
		}

		public static List<T> Deserialize(Stream bytes, Deserializer<T> serialization)
		{
			ushort num = UShortProxy.Deserialize(bytes);
			List<T> list = new List<T>(num);
			for (int i = 0; i < num; i++)
			{
				list.Add(serialization(bytes));
			}
			return list;
		}
	}
}
