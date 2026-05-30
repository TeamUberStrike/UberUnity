using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public static class EnumerationExtensions
{
	public static T[] ValueArray<S, T>(this Dictionary<S, T> dict)
	{
		T[] array = new T[dict.Count];
		dict.Values.CopyTo(array, 0);
		return array;
	}

	public static S[] KeyArray<S, T>(this Dictionary<S, T> dict)
	{
		S[] array = new S[dict.Count];
		dict.Keys.CopyTo(array, 0);
		return array;
	}

	public static KeyValuePair<S, T> First<S, T>(this Dictionary<S, T> dict)
	{
		Dictionary<S, T>.Enumerator enumerator = dict.GetEnumerator();
		enumerator.MoveNext();
		return enumerator.Current;
	}

	public static T Reduce<T, TList>(this IEnumerable<TList> list, Func<TList, T, T> func, T initialValue)
	{
		T val = initialValue;
		foreach (TList item in list)
		{
			val = func(item, val);
		}
		return val;
	}

	public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key, [Optional] TValue defaultValue)
	{
		TValue value = defaultValue;
		dic.TryGetValue(key, out value);
		return value;
	}
}
