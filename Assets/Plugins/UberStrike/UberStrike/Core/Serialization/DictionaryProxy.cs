using System.Collections.Generic;
using System.IO;

namespace UberStrike.Core.Serialization
{
	public static class DictionaryProxy<S, T>
	{
		public delegate void Serializer<U>(Stream stream, U instance);

		public delegate U Deserializer<U>(Stream stream);

		public static void Serialize(Stream bytes, Dictionary<S, T> instance, Serializer<S> keySerialization, Serializer<T> valueSerialization)
		{
			Int32Proxy.Serialize(bytes, instance.Count);
			foreach (KeyValuePair<S, T> item in instance)
			{
				keySerialization(bytes, item.Key);
				valueSerialization(bytes, item.Value);
			}
		}

		public static Dictionary<S, T> Deserialize(Stream bytes, Deserializer<S> keySerialization, Deserializer<T> valueSerialization)
		{
			int num = Int32Proxy.Deserialize(bytes);
			Dictionary<S, T> dictionary = new Dictionary<S, T>(num);
			for (int i = 0; i < num; i++)
			{
				dictionary.Add(keySerialization(bytes), valueSerialization(bytes));
			}
			return dictionary;
		}
	}
}
