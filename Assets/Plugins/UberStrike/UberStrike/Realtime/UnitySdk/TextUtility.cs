using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace UberStrike.Realtime.UnitySdk
{
	public static class TextUtility
	{
		public static string ConvertText(string textToSecure)
		{
			string text = HtmlEncode(textToSecure);
			text = text.Replace("`", "&#96;");
			text = text.Replace("\u00b4", "&acute");
			text = text.Replace("'", "&#39");
			text = text.Replace("-", "&#45;");
			text = text.Replace("!", "&#33;");
			return text.Replace("?", "&#63;");
		}

		public static string HtmlEncode(string text)
		{
			if (text == null)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder(text.Length);
			int length = text.Length;
			for (int i = 0; i < length; i++)
			{
				switch (text[i])
				{
				case '<':
					stringBuilder.Append("&lt;");
					continue;
				case '>':
					stringBuilder.Append("&gt;");
					continue;
				case '"':
					stringBuilder.Append("&quot;");
					continue;
				case '&':
					stringBuilder.Append("&amp;");
					continue;
				}
				if (text[i] > '\u009f')
				{
					stringBuilder.Append("&#");
					stringBuilder.Append(((int)text[i]).ToString(CultureInfo.InvariantCulture));
					stringBuilder.Append(";");
				}
				else
				{
					stringBuilder.Append(text[i]);
				}
			}
			return stringBuilder.ToString();
		}

		public static string ProtectSqlField(string textToSecure)
		{
			return textToSecure.Replace("'", "''");
		}

		public static string ConvertTextForJavaScript(string textToSecure)
		{
			string text = textToSecure.Replace("'", string.Empty);
			return text.Replace("|", string.Empty);
		}

		public static long InetAToN(string addressIP)
		{
			long result = 0L;
			if (addressIP.Equals("::1"))
			{
				addressIP = "127.0.0.1";
			}
			if (!IsNullOrEmpty(addressIP))
			{
				string[] array = addressIP.ToString().Split('.');
				if (array.Length == 4)
				{
					bool flag = true;
					bool flag2 = false;
					int result2 = 0;
					long num = 0L;
					for (int num2 = array.Length - 1; num2 >= 0; num2--)
					{
						if (int.TryParse(array[num2], out result2) && result2 >= 0 && result2 < 256)
						{
							num += result2 % 256 * (long)Math.Pow(256.0, 3 - num2);
						}
						else
						{
							flag = false;
						}
					}
					if (flag)
					{
						result = num;
					}
				}
			}
			return result;
		}

		public static string InetNToA(long networkAddress)
		{
			string result = string.Empty;
			long num = 0L;
			long num2 = 0L;
			long num3 = 0L;
			long num4 = 0L;
			if (networkAddress <= uint.MaxValue)
			{
				num = networkAddress / 16777216;
				if (num == 0L)
				{
					num = 255L;
					networkAddress += 16777216;
				}
				else if (num < 0)
				{
					if (networkAddress % 16777216 == 0L)
					{
						num += 256;
					}
					else
					{
						num += 255;
						networkAddress = ((num != 128) ? (networkAddress + 16777216 * (256 - num)) : (networkAddress + 2147483648u));
					}
				}
				else
				{
					networkAddress -= 16777216 * num;
				}
				networkAddress %= 16777216;
				num2 = networkAddress / 65536;
				networkAddress %= 65536;
				num3 = networkAddress / 256;
				networkAddress %= 256;
				num4 = networkAddress;
				result = num + "." + num2 + "." + num3 + "." + num4;
			}
			return result;
		}

		public static bool IsNumeric(string numericText)
		{
			bool flag = true;
			if (!IsNullOrEmpty(numericText))
			{
				if (numericText.StartsWith("-"))
				{
					numericText = numericText.Replace("-", string.Empty);
				}
				string text = numericText;
				foreach (char c in text)
				{
					flag = char.IsNumber(c);
					if (!flag)
					{
						return flag;
					}
				}
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public static string ShortenText(string input, int maxSize, bool addPoints)
		{
			string text = input;
			if (maxSize < input.Length && maxSize > 3)
			{
				text = text.Substring(0, maxSize - 3);
				if (addPoints)
				{
					text += "...";
				}
			}
			return text;
		}

		public static bool IsNullOrEmpty(string value)
		{
			bool result = true;
			if (!string.IsNullOrEmpty(value))
			{
				value = value.Trim();
				if (!string.IsNullOrEmpty(value))
				{
					result = false;
				}
			}
			return result;
		}

		public static List<int> IndexOfAll(string haystack, string needle)
		{
			int num = 0;
			int num2 = 0;
			List<int> list = new List<int>();
			if (!IsNullOrEmpty(haystack) && !IsNullOrEmpty(needle))
			{
				int length = needle.Length;
				do
				{
					num = haystack.IndexOf(needle);
					if (num > -1)
					{
						haystack = haystack.Substring(num + length);
						list.Add(num + num2);
						num2 += num + length;
					}
				}
				while (num > -1 && !IsNullOrEmpty(haystack));
			}
			return list;
		}

		public static string Base64Encode(string data)
		{
			string result = string.Empty;
			if (data != null)
			{
				byte[] array = new byte[data.Length];
				array = Encoding.UTF8.GetBytes(data);
				result = Convert.ToBase64String(array);
			}
			return result;
		}

		public static string Base64Decode(string data)
		{
			string result = string.Empty;
			if (data != null)
			{
				UTF8Encoding uTF8Encoding = new UTF8Encoding();
				Decoder decoder = uTF8Encoding.GetDecoder();
				byte[] array = Convert.FromBase64String(data);
				int charCount = decoder.GetCharCount(array, 0, array.Length);
				char[] array2 = new char[charCount];
				decoder.GetChars(array, 0, array.Length, array2, 0);
				result = new string(array2);
			}
			return result;
		}

		public static byte[] StringToByteArray(string inputString)
		{
			UTF8Encoding uTF8Encoding = new UTF8Encoding();
			return uTF8Encoding.GetBytes(inputString);
		}

		public static string CompleteTrim(string text)
		{
			if (text != null)
			{
				text = text.Trim();
				text = Regex.Replace(text, "\\s+", " ");
			}
			return text;
		}

		public static bool TryParseFacebookId(string handle, out long facebookId)
		{
			bool result = false;
			facebookId = 0L;
			if (long.TryParse(handle, out facebookId) && facebookId > 0)
			{
				result = true;
			}
			return result;
		}

		public static bool TryParseMySpaceId(string handle, out int mySpaceId)
		{
			bool result = false;
			mySpaceId = 0;
			if (int.TryParse(handle, out mySpaceId) && mySpaceId > 0)
			{
				result = true;
			}
			return result;
		}

		public static bool TryParseCyworldId(string handle, out int cyworldId)
		{
			bool result = false;
			cyworldId = 0;
			if (int.TryParse(handle, out cyworldId) && cyworldId > 0)
			{
				result = true;
			}
			return result;
		}

		public static string Join<T>(string separator, List<T> list)
		{
			string result = string.Empty;
			if (list != null && list.Count > 0)
			{
				string[] array = new string[list.Count];
				for (int i = 0; i < list.Count; i++)
				{
					array[i] = list[i].ToString();
				}
				result = string.Join(separator, array);
			}
			return result;
		}
	}
}
