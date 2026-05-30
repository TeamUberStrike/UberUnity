using System;
using System.IO;

namespace UberStrike.Core.Serialization
{
	public static class ArrayProxy<T>
	{
		public delegate void Serializer<U>(Stream stream, U instance);

		public delegate U Deserializer<U>(Stream stream);

		public static void Serialize(Stream bytes, T[] instance, Action<Stream, T> serialization)
		{
			UShortProxy.Serialize(bytes, (ushort)instance.Length);
			foreach (T arg in instance)
			{
				serialization(bytes, arg);
			}
		}

		public static T[] Deserialize(Stream bytes, Deserializer<T> serialization)
		{
			ushort num = UShortProxy.Deserialize(bytes);
			T[] array = new T[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = serialization(bytes);
			}
			return array;
		}
	}
}
