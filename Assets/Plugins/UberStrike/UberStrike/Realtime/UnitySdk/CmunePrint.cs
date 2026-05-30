using System;
using System.Collections;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace UberStrike.Realtime.UnitySdk
{
	public static class CmunePrint
	{
		private static readonly byte _byteBitCountConstant = 7;

		private static readonly byte _byteBitMaskConstant = 128;

		public static string Properties(object instance, bool publicOnly = true)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (instance == null)
			{
				stringBuilder.Append("[Class=null]");
			}
			else
			{
				stringBuilder.AppendFormat("[Class={0}] ", instance.GetType().Name);
				PropertyInfo[] properties = instance.GetType().GetProperties((!publicOnly) ? (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) : (BindingFlags.Instance | BindingFlags.Public));
				foreach (PropertyInfo propertyInfo in properties)
				{
					stringBuilder.AppendFormat("[{0}={1}],", propertyInfo.Name, Object(propertyInfo.GetValue(instance, null)));
				}
			}
			return stringBuilder.ToString();
		}

		public static string Object(object value)
		{
			if (value == null)
			{
				return "null";
			}
			if (value is string)
			{
				return value as string;
			}
			if (value.GetType().IsValueType)
			{
				return value.ToString();
			}
			if (value is ICollection)
			{
				return Values(value);
			}
			return value.ToString();
		}

		public static int GetHashCode(object obj)
		{
			if (obj == null)
			{
				return 0;
			}
			if (obj is ICollection)
			{
				int num = 0;
				{
					foreach (object item in obj as ICollection)
					{
						num += item.GetHashCode();
					}
					return num;
				}
			}
			return obj.GetHashCode();
		}

		public static string Percent(float f)
		{
			return string.Format("{0:N0}%", Math.Round(f * 100f));
		}

		public static string Order(int time)
		{
			if (time > 0)
			{
				switch (time)
				{
				case 1:
					return "1st";
				case 2:
					return "2nd";
				case 3:
					return "3rd";
				default:
					return time + "th";
				}
			}
			return time.ToString();
		}

		public static string Time(DateTime time)
		{
			return time.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss.fffffffK");
		}

		public static string Time(TimeSpan s)
		{
			if (s.Days > 0)
			{
				return string.Format("{0:D1}d, {1:D2}:{2:D2}h", s.Days, s.Hours, s.Minutes);
			}
			if (s.Hours > 0)
			{
				return string.Format("{0:D2}:{1:D2}:{2:D2}", s.Hours, s.Minutes, s.Seconds);
			}
			if (s.Minutes > 0)
			{
				return string.Format("{0:D2}:{1:D2}", s.Minutes, s.Seconds);
			}
			if (s.Seconds > 10)
			{
				return string.Format("{0:D2}", s.Seconds);
			}
			return string.Format("{0:D1}", s.Seconds);
		}

		public static string Time(int seconds)
		{
			return Time(TimeSpan.FromSeconds(Math.Max(seconds, 0)));
		}

		public static string Flag(sbyte flag)
		{
			return Flag((uint)flag, 7);
		}

		public static string Flag(byte flag)
		{
			return Flag(flag, 7);
		}

		public static string Flag(ushort flag)
		{
			return Flag(flag, 15);
		}

		public static string Flag(short flag)
		{
			return Flag((uint)flag, 15);
		}

		public static string Flag(int flag)
		{
			return Flag((uint)flag, 31);
		}

		public static string Flag(uint flag)
		{
			return Flag(flag, 31);
		}

		public static string Flag<T>(T flag) where T : IConvertible
		{
			if (typeof(T).IsEnum)
			{
				return Flag(Convert.ToUInt32(flag), typeof(T));
			}
			return Flag(Convert.ToUInt32(flag), 31);
		}

		private static string Flag(uint flag, int bytes)
		{
			int num = 1 << bytes;
			StringBuilder stringBuilder = new StringBuilder();
			for (int num2 = bytes; num2 >= 0; num2--)
			{
				stringBuilder.Append(((flag & num) != 0L) ? '1' : '0');
				if (num2 % 8 == 0)
				{
					stringBuilder.Append(' ');
				}
				flag <<= 1;
			}
			return stringBuilder.ToString();
		}

		private static string Flag(uint flag, Type type)
		{
			Type underlyingType = Enum.GetUnderlyingType(type);
			try
			{
				int num = 31;
				if (underlyingType == typeof(byte) || underlyingType == typeof(sbyte))
				{
					num = 7;
				}
				else if (underlyingType == typeof(short) || underlyingType == typeof(ushort))
				{
					num = 15;
				}
				int num2 = 1 << num;
				StringBuilder stringBuilder = new StringBuilder();
				for (int num3 = num; num3 >= 0; num3--)
				{
					if (underlyingType == typeof(byte))
					{
						if ((flag & num2) != 0L && Enum.IsDefined(type, (byte)(1 << num3)))
						{
							stringBuilder.Append(Enum.GetName(type, 1 << num3) + " ");
						}
					}
					else if (underlyingType == typeof(ushort))
					{
						if ((flag & num2) != 0L && Enum.IsDefined(type, (ushort)(1 << num3)))
						{
							stringBuilder.Append(Enum.GetName(type, 1 << num3) + " ");
						}
					}
					else if ((flag & num2) != 0L && Enum.IsDefined(type, 1 << num3))
					{
						stringBuilder.Append(Enum.GetName(type, 1 << num3) + " ");
					}
					flag <<= 1;
				}
				return stringBuilder.ToString();
			}
			catch
			{
				return type.Name + " unsupported: " + underlyingType;
			}
		}

		public static string Values(params object[] args)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (args != null)
			{
				if (args.Length == 0)
				{
					stringBuilder.Append("EMPTY");
				}
				else
				{
					for (int i = 0; i < args.Length; i++)
					{
						object obj = args[i];
						if (obj != null)
						{
							if (obj is IEnumerable)
							{
								IEnumerable enumerable = obj as IEnumerable;
								stringBuilder.Append("|");
								IEnumerator enumerator = enumerable.GetEnumerator();
								int num = 0;
								while (enumerator.MoveNext() && num < 50)
								{
									if (enumerator.Current != null)
									{
										stringBuilder.AppendFormat("{0}|", enumerator.Current);
									}
									else
									{
										stringBuilder.Append("null|");
									}
									num++;
								}
								switch (num)
								{
								case 0:
									stringBuilder.Append("empty|");
									break;
								case 50:
									stringBuilder.Append("...");
									break;
								}
							}
							else
							{
								stringBuilder.AppendFormat("{0}", obj);
							}
						}
						else
						{
							stringBuilder.AppendFormat("null");
						}
						if (i < args.Length - 1)
						{
							stringBuilder.AppendFormat(", ");
						}
					}
				}
			}
			else
			{
				stringBuilder.Append("NULL");
			}
			return stringBuilder.ToString();
		}

		public static string Types(params object[] args)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (args != null)
			{
				if (args.Length == 0)
				{
					stringBuilder.Append("EMPTY");
				}
				else
				{
					for (int i = 0; i < args.Length; i++)
					{
						object obj = args[i];
						if (obj != null)
						{
							if (obj is ICollection)
							{
								ICollection collection = obj as ICollection;
								stringBuilder.AppendFormat("{0}({1})", collection.GetType().Name, collection.Count);
							}
							else
							{
								stringBuilder.AppendFormat("{0}", obj.GetType().Name);
							}
						}
						else
						{
							stringBuilder.AppendFormat("null");
						}
						if (i < args.Length - 1)
						{
							stringBuilder.AppendFormat(", ");
						}
					}
				}
			}
			else
			{
				stringBuilder.Append("NULL");
			}
			return stringBuilder.ToString();
		}

		public static string Dictionary(IDictionary t)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (DictionaryEntry item in t)
			{
				stringBuilder.AppendFormat("{0}: {1}\n", item.Key, item.Value);
			}
			return stringBuilder.ToString();
		}

		public static void DebugBitString(byte[] x)
		{
			Debug.Log(BitString(x));
		}

		public static void DebugBitString(int x)
		{
			Debug.Log(BitString(x));
		}

		public static void DebugBitString(string x)
		{
			Debug.Log(BitString(x));
		}

		public static void DebugBitString(byte x)
		{
			Debug.Log(BitString(x));
		}

		public static string BitString(byte x)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i <= _byteBitCountConstant; i++)
			{
				stringBuilder.Append(((x & _byteBitMaskConstant) != 0) ? '1' : '0');
				x <<= 1;
			}
			return stringBuilder.ToString();
		}

		public static string BitString(int x)
		{
			return BitString(BitConverter.GetBytes(x));
		}

		public static string BitString(string x)
		{
			return BitString(Encoding.Unicode.GetBytes(x));
		}

		public static string BitString(byte[] bytes)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int num = bytes.Length - 1; num >= 0; num--)
			{
				stringBuilder.Append(BitString(bytes[num])).Append(' ');
			}
			return stringBuilder.ToString();
		}
	}
}
