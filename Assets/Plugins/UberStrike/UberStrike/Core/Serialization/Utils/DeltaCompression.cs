using System.IO;

namespace UberStrike.Core.Serialization.Utils
{
	public class DeltaCompression
	{
		public static byte[] Deflate(byte[] baseData, byte[] newData)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				byte b = 0;
				for (int i = 0; i < newData.Length; i++)
				{
					if (i < baseData.Length)
					{
						if (baseData[i] == newData[i])
						{
							b++;
							continue;
						}
						memoryStream.WriteByte(b);
						memoryStream.WriteByte(newData[i]);
						b = 0;
					}
					else
					{
						memoryStream.WriteByte(newData[i]);
					}
				}
				return memoryStream.ToArray();
			}
		}

		public static byte[] Inflate(byte[] baseData, byte[] delta)
		{
			if (delta.Length == 0)
			{
				return baseData;
			}
			using (MemoryStream memoryStream = new MemoryStream())
			{
				int num = 0;
				int num2 = 0;
				while (num2 < delta.Length)
				{
					if (num < baseData.Length)
					{
						int num3 = 0;
						while (num3 < delta[num2])
						{
							memoryStream.WriteByte(baseData[num]);
							num3++;
							num++;
						}
						memoryStream.WriteByte(delta[num2 + 1]);
						num++;
						num2 += 2;
					}
					else
					{
						memoryStream.WriteByte(delta[num2]);
						num2++;
					}
				}
				return memoryStream.ToArray();
			}
		}
	}
}
