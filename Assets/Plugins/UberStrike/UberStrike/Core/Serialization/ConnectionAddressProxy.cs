using System.IO;
using UberStrike.Core.Models;

namespace UberStrike.Core.Serialization
{
	public static class ConnectionAddressProxy
	{
		public static void Serialize(Stream stream, ConnectionAddress instance)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Int32Proxy.Serialize(memoryStream, instance.Ipv4);
				UInt16Proxy.Serialize(memoryStream, instance.Port);
				memoryStream.WriteTo(stream);
			}
		}

		public static ConnectionAddress Deserialize(Stream bytes)
		{
			ConnectionAddress connectionAddress = new ConnectionAddress();
			connectionAddress.Ipv4 = Int32Proxy.Deserialize(bytes);
			connectionAddress.Port = UInt16Proxy.Deserialize(bytes);
			return connectionAddress;
		}
	}
}
