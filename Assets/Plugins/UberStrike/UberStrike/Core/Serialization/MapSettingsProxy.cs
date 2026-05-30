using System.IO;
using UberStrike.Core.Models.Views;

namespace UberStrike.Core.Serialization
{
	public static class MapSettingsProxy
	{
		public static void Serialize(Stream stream, MapSettings instance)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Int32Proxy.Serialize(memoryStream, instance.KillsCurrent);
				Int32Proxy.Serialize(memoryStream, instance.KillsMax);
				Int32Proxy.Serialize(memoryStream, instance.KillsMin);
				Int32Proxy.Serialize(memoryStream, instance.PlayersCurrent);
				Int32Proxy.Serialize(memoryStream, instance.PlayersMax);
				Int32Proxy.Serialize(memoryStream, instance.PlayersMin);
				Int32Proxy.Serialize(memoryStream, instance.TimeCurrent);
				Int32Proxy.Serialize(memoryStream, instance.TimeMax);
				Int32Proxy.Serialize(memoryStream, instance.TimeMin);
				memoryStream.WriteTo(stream);
			}
		}

		public static MapSettings Deserialize(Stream bytes)
		{
			MapSettings mapSettings = new MapSettings();
			mapSettings.KillsCurrent = Int32Proxy.Deserialize(bytes);
			mapSettings.KillsMax = Int32Proxy.Deserialize(bytes);
			mapSettings.KillsMin = Int32Proxy.Deserialize(bytes);
			mapSettings.PlayersCurrent = Int32Proxy.Deserialize(bytes);
			mapSettings.PlayersMax = Int32Proxy.Deserialize(bytes);
			mapSettings.PlayersMin = Int32Proxy.Deserialize(bytes);
			mapSettings.TimeCurrent = Int32Proxy.Deserialize(bytes);
			mapSettings.TimeMax = Int32Proxy.Deserialize(bytes);
			mapSettings.TimeMin = Int32Proxy.Deserialize(bytes);
			return mapSettings;
		}
	}
}
