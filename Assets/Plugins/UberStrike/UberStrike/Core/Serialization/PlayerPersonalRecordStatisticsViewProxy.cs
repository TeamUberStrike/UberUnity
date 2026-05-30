using System.IO;
using UberStrike.DataCenter.Common.Entities;

namespace UberStrike.Core.Serialization
{
	public static class PlayerPersonalRecordStatisticsViewProxy
	{
		public static void Serialize(Stream stream, PlayerPersonalRecordStatisticsView instance)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Int32Proxy.Serialize(memoryStream, instance.MostArmorPickedUp);
				Int32Proxy.Serialize(memoryStream, instance.MostCannonSplats);
				Int32Proxy.Serialize(memoryStream, instance.MostConsecutiveSnipes);
				Int32Proxy.Serialize(memoryStream, instance.MostDamageDealt);
				Int32Proxy.Serialize(memoryStream, instance.MostDamageReceived);
				Int32Proxy.Serialize(memoryStream, instance.MostHeadshots);
				Int32Proxy.Serialize(memoryStream, instance.MostHealthPickedUp);
				Int32Proxy.Serialize(memoryStream, instance.MostLauncherSplats);
				Int32Proxy.Serialize(memoryStream, instance.MostMachinegunSplats);
				Int32Proxy.Serialize(memoryStream, instance.MostMeleeSplats);
				Int32Proxy.Serialize(memoryStream, instance.MostNutshots);
				Int32Proxy.Serialize(memoryStream, instance.MostShotgunSplats);
				Int32Proxy.Serialize(memoryStream, instance.MostSniperSplats);
				Int32Proxy.Serialize(memoryStream, instance.MostSplats);
				Int32Proxy.Serialize(memoryStream, instance.MostSplattergunSplats);
				Int32Proxy.Serialize(memoryStream, instance.MostXPEarned);
				memoryStream.WriteTo(stream);
			}
		}

		public static PlayerPersonalRecordStatisticsView Deserialize(Stream bytes)
		{
			PlayerPersonalRecordStatisticsView playerPersonalRecordStatisticsView = new PlayerPersonalRecordStatisticsView();
			playerPersonalRecordStatisticsView.MostArmorPickedUp = Int32Proxy.Deserialize(bytes);
			playerPersonalRecordStatisticsView.MostCannonSplats = Int32Proxy.Deserialize(bytes);
			playerPersonalRecordStatisticsView.MostConsecutiveSnipes = Int32Proxy.Deserialize(bytes);
			playerPersonalRecordStatisticsView.MostDamageDealt = Int32Proxy.Deserialize(bytes);
			playerPersonalRecordStatisticsView.MostDamageReceived = Int32Proxy.Deserialize(bytes);
			playerPersonalRecordStatisticsView.MostHeadshots = Int32Proxy.Deserialize(bytes);
			playerPersonalRecordStatisticsView.MostHealthPickedUp = Int32Proxy.Deserialize(bytes);
			playerPersonalRecordStatisticsView.MostLauncherSplats = Int32Proxy.Deserialize(bytes);
			playerPersonalRecordStatisticsView.MostMachinegunSplats = Int32Proxy.Deserialize(bytes);
			playerPersonalRecordStatisticsView.MostMeleeSplats = Int32Proxy.Deserialize(bytes);
			playerPersonalRecordStatisticsView.MostNutshots = Int32Proxy.Deserialize(bytes);
			playerPersonalRecordStatisticsView.MostShotgunSplats = Int32Proxy.Deserialize(bytes);
			playerPersonalRecordStatisticsView.MostSniperSplats = Int32Proxy.Deserialize(bytes);
			playerPersonalRecordStatisticsView.MostSplats = Int32Proxy.Deserialize(bytes);
			playerPersonalRecordStatisticsView.MostSplattergunSplats = Int32Proxy.Deserialize(bytes);
			playerPersonalRecordStatisticsView.MostXPEarned = Int32Proxy.Deserialize(bytes);
			return playerPersonalRecordStatisticsView;
		}
	}
}
