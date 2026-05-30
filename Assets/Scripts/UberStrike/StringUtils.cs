using System;
using UberStrike.Realtime.UnitySdk;
using UnityEngine;

public static class StringUtils
{
	public static T ParseValue<T>(string value)
	{
		Type typeFromHandle = typeof(T);
		T result = default(T);
		try
		{
			if (typeFromHandle.IsEnum)
			{
				result = (T)Enum.Parse(typeFromHandle, value);
			}
			else if (typeFromHandle == typeof(int))
			{
				result = (T)(object)int.Parse(value);
			}
			else if (typeFromHandle == typeof(string))
			{
				result = (T)(object)value;
			}
			else if (typeFromHandle == typeof(DateTime))
			{
				result = (T)(object)DateTime.Parse(TextUtilities.Base64Decode(value));
			}
			else
			{
				Debug.LogError("ParseValue couldn't find a conversion of value '" + value + "' into type '" + typeFromHandle.Name + "'");
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("ParseValue failed converting value '" + value + "' into type '" + typeFromHandle.Name + "': " + ex.Message);
		}
		return result;
	}
}
