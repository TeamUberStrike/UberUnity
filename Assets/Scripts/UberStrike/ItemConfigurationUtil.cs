using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UberStrike.Core.Models.Views;
using UberStrike.Realtime.UnitySdk;

public static class ItemConfigurationUtil
{
	private static Dictionary<Type, List<FieldInfo>> fields = new Dictionary<Type, List<FieldInfo>>();

	private static List<FieldInfo> GetAllFields(Type type)
	{
		List<FieldInfo> value;
		if (!fields.TryGetValue(type, out value))
		{
			value = ReflectionHelper.GetAllFields(type, true);
			fields[type] = value;
		}
		return value;
	}

	public static void CopyProperties<T>(T config, BaseUberStrikeItemView item) where T : BaseUberStrikeItemView
	{
		CloneUtil.CopyAllFields(config, item);
		foreach (FieldInfo allField in GetAllFields(config.GetType()))
		{
			string customPropertyName = GetCustomPropertyName(allField);
			if (!string.IsNullOrEmpty(customPropertyName) && item.CustomProperties != null && item.CustomProperties.ContainsKey(customPropertyName))
			{
				allField.SetValue(config, Convert(item.CustomProperties[customPropertyName], allField.FieldType));
			}
		}
	}

	public static void CopyCustomProperties(BaseUberStrikeItemView src, object dst)
	{
		foreach (FieldInfo allField in GetAllFields(dst.GetType()))
		{
			string customPropertyName = GetCustomPropertyName(allField);
			if (!string.IsNullOrEmpty(customPropertyName) && src.CustomProperties != null && src.CustomProperties.ContainsKey(customPropertyName))
			{
				object value = Convert(src.CustomProperties[customPropertyName], allField.FieldType);
				allField.SetValue(dst, value);
			}
		}
	}

	private static string GetCustomPropertyName(FieldInfo info)
	{
		object[] customAttributes = info.GetCustomAttributes(typeof(CustomPropertyAttribute), true);
		return (customAttributes.Length <= 0) ? string.Empty : ((CustomPropertyAttribute)customAttributes[0]).Name;
	}

	private static object Convert(string value, Type type)
	{
		if (type == typeof(string))
		{
			return value;
		}
		if (type.IsEnum || type == typeof(int))
		{
			return int.Parse(value, CultureInfo.InvariantCulture);
		}
		if (type == typeof(float))
		{
			return float.Parse(value, CultureInfo.InvariantCulture);
		}
		if (type == typeof(bool))
		{
			return bool.Parse(value);
		}
		throw new NotSupportedException("ConfigurableItem has unsupported property of type: " + type);
	}
}
