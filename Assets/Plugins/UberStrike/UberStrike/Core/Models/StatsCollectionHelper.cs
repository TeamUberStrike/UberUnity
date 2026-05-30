using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace UberStrike.Core.Models
{
	public static class StatsCollectionHelper
	{
		private static List<PropertyInfo> properties;

		static StatsCollectionHelper()
		{
			properties = new List<PropertyInfo>();
			PropertyInfo[] array = typeof(StatsCollection).GetProperties(BindingFlags.Instance | BindingFlags.Public);
			PropertyInfo[] array2 = array;
			foreach (PropertyInfo propertyInfo in array2)
			{
				if (propertyInfo.PropertyType == typeof(int) && propertyInfo.CanRead && propertyInfo.CanWrite)
				{
					properties.Add(propertyInfo);
				}
			}
		}

		public static string ToString(StatsCollection instance)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (PropertyInfo property in properties)
			{
				stringBuilder.AppendFormat("{0}:{1}\n", property.Name, property.GetValue(instance, null));
			}
			return stringBuilder.ToString();
		}

		public static void Reset(StatsCollection instance)
		{
			foreach (PropertyInfo property in properties)
			{
				property.SetValue(instance, 0, null);
			}
		}

		public static void TakeBestValues(StatsCollection instance, StatsCollection that)
		{
			foreach (PropertyInfo property in properties)
			{
				int num = (int)property.GetValue(instance, null);
				int num2 = (int)property.GetValue(that, null);
				if (num < num2)
				{
					property.SetValue(instance, num2, null);
				}
			}
		}

		public static void AddAllValues(StatsCollection instance, StatsCollection that)
		{
			foreach (PropertyInfo property in properties)
			{
				int num = (int)property.GetValue(instance, null);
				int num2 = (int)property.GetValue(that, null);
				property.SetValue(instance, num + num2, null);
			}
		}
	}
}
