using System.IO;
using UberStrike.DataCenter.Common.Entities;

namespace UberStrike.Core.Serialization
{
	public static class PlayerLevelCapViewProxy
	{
		public static void Serialize(Stream stream, PlayerLevelCapView instance)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Int32Proxy.Serialize(memoryStream, instance.Level);
				Int32Proxy.Serialize(memoryStream, instance.PlayerLevelCapId);
				Int32Proxy.Serialize(memoryStream, instance.XPRequired);
				memoryStream.WriteTo(stream);
			}
		}

		public static PlayerLevelCapView Deserialize(Stream bytes)
		{
			PlayerLevelCapView playerLevelCapView = new PlayerLevelCapView();
			playerLevelCapView.Level = Int32Proxy.Deserialize(bytes);
			playerLevelCapView.PlayerLevelCapId = Int32Proxy.Deserialize(bytes);
			playerLevelCapView.XPRequired = Int32Proxy.Deserialize(bytes);
			return playerLevelCapView;
		}
	}
}
