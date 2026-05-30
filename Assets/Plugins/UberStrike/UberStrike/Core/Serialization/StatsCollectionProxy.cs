using System.IO;
using UberStrike.Core.Models;

namespace UberStrike.Core.Serialization
{
	public static class StatsCollectionProxy
	{
		public static void Serialize(Stream stream, StatsCollection instance)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Int32Proxy.Serialize(memoryStream, instance.ArmorPickedUp);
				Int32Proxy.Serialize(memoryStream, instance.CannonDamageDone);
				Int32Proxy.Serialize(memoryStream, instance.CannonKills);
				Int32Proxy.Serialize(memoryStream, instance.CannonShotsFired);
				Int32Proxy.Serialize(memoryStream, instance.CannonShotsHit);
				Int32Proxy.Serialize(memoryStream, instance.ConsecutiveSnipes);
				Int32Proxy.Serialize(memoryStream, instance.DamageReceived);
				Int32Proxy.Serialize(memoryStream, instance.Deaths);
				Int32Proxy.Serialize(memoryStream, instance.Headshots);
				Int32Proxy.Serialize(memoryStream, instance.HealthPickedUp);
				Int32Proxy.Serialize(memoryStream, instance.LauncherDamageDone);
				Int32Proxy.Serialize(memoryStream, instance.LauncherKills);
				Int32Proxy.Serialize(memoryStream, instance.LauncherShotsFired);
				Int32Proxy.Serialize(memoryStream, instance.LauncherShotsHit);
				Int32Proxy.Serialize(memoryStream, instance.MachineGunDamageDone);
				Int32Proxy.Serialize(memoryStream, instance.MachineGunKills);
				Int32Proxy.Serialize(memoryStream, instance.MachineGunShotsFired);
				Int32Proxy.Serialize(memoryStream, instance.MachineGunShotsHit);
				Int32Proxy.Serialize(memoryStream, instance.MeleeDamageDone);
				Int32Proxy.Serialize(memoryStream, instance.MeleeKills);
				Int32Proxy.Serialize(memoryStream, instance.MeleeShotsFired);
				Int32Proxy.Serialize(memoryStream, instance.MeleeShotsHit);
				Int32Proxy.Serialize(memoryStream, instance.Nutshots);
				Int32Proxy.Serialize(memoryStream, instance.Points);
				Int32Proxy.Serialize(memoryStream, instance.ShotgunDamageDone);
				Int32Proxy.Serialize(memoryStream, instance.ShotgunShotsFired);
				Int32Proxy.Serialize(memoryStream, instance.ShotgunShotsHit);
				Int32Proxy.Serialize(memoryStream, instance.ShotgunSplats);
				Int32Proxy.Serialize(memoryStream, instance.SniperDamageDone);
				Int32Proxy.Serialize(memoryStream, instance.SniperKills);
				Int32Proxy.Serialize(memoryStream, instance.SniperShotsFired);
				Int32Proxy.Serialize(memoryStream, instance.SniperShotsHit);
				Int32Proxy.Serialize(memoryStream, instance.SplattergunDamageDone);
				Int32Proxy.Serialize(memoryStream, instance.SplattergunKills);
				Int32Proxy.Serialize(memoryStream, instance.SplattergunShotsFired);
				Int32Proxy.Serialize(memoryStream, instance.SplattergunShotsHit);
				Int32Proxy.Serialize(memoryStream, instance.Suicides);
				Int32Proxy.Serialize(memoryStream, instance.Xp);
				memoryStream.WriteTo(stream);
			}
		}

		public static StatsCollection Deserialize(Stream bytes)
		{
			StatsCollection statsCollection = new StatsCollection();
			statsCollection.ArmorPickedUp = Int32Proxy.Deserialize(bytes);
			statsCollection.CannonDamageDone = Int32Proxy.Deserialize(bytes);
			statsCollection.CannonKills = Int32Proxy.Deserialize(bytes);
			statsCollection.CannonShotsFired = Int32Proxy.Deserialize(bytes);
			statsCollection.CannonShotsHit = Int32Proxy.Deserialize(bytes);
			statsCollection.ConsecutiveSnipes = Int32Proxy.Deserialize(bytes);
			statsCollection.DamageReceived = Int32Proxy.Deserialize(bytes);
			statsCollection.Deaths = Int32Proxy.Deserialize(bytes);
			statsCollection.Headshots = Int32Proxy.Deserialize(bytes);
			statsCollection.HealthPickedUp = Int32Proxy.Deserialize(bytes);
			statsCollection.LauncherDamageDone = Int32Proxy.Deserialize(bytes);
			statsCollection.LauncherKills = Int32Proxy.Deserialize(bytes);
			statsCollection.LauncherShotsFired = Int32Proxy.Deserialize(bytes);
			statsCollection.LauncherShotsHit = Int32Proxy.Deserialize(bytes);
			statsCollection.MachineGunDamageDone = Int32Proxy.Deserialize(bytes);
			statsCollection.MachineGunKills = Int32Proxy.Deserialize(bytes);
			statsCollection.MachineGunShotsFired = Int32Proxy.Deserialize(bytes);
			statsCollection.MachineGunShotsHit = Int32Proxy.Deserialize(bytes);
			statsCollection.MeleeDamageDone = Int32Proxy.Deserialize(bytes);
			statsCollection.MeleeKills = Int32Proxy.Deserialize(bytes);
			statsCollection.MeleeShotsFired = Int32Proxy.Deserialize(bytes);
			statsCollection.MeleeShotsHit = Int32Proxy.Deserialize(bytes);
			statsCollection.Nutshots = Int32Proxy.Deserialize(bytes);
			statsCollection.Points = Int32Proxy.Deserialize(bytes);
			statsCollection.ShotgunDamageDone = Int32Proxy.Deserialize(bytes);
			statsCollection.ShotgunShotsFired = Int32Proxy.Deserialize(bytes);
			statsCollection.ShotgunShotsHit = Int32Proxy.Deserialize(bytes);
			statsCollection.ShotgunSplats = Int32Proxy.Deserialize(bytes);
			statsCollection.SniperDamageDone = Int32Proxy.Deserialize(bytes);
			statsCollection.SniperKills = Int32Proxy.Deserialize(bytes);
			statsCollection.SniperShotsFired = Int32Proxy.Deserialize(bytes);
			statsCollection.SniperShotsHit = Int32Proxy.Deserialize(bytes);
			statsCollection.SplattergunDamageDone = Int32Proxy.Deserialize(bytes);
			statsCollection.SplattergunKills = Int32Proxy.Deserialize(bytes);
			statsCollection.SplattergunShotsFired = Int32Proxy.Deserialize(bytes);
			statsCollection.SplattergunShotsHit = Int32Proxy.Deserialize(bytes);
			statsCollection.Suicides = Int32Proxy.Deserialize(bytes);
			statsCollection.Xp = Int32Proxy.Deserialize(bytes);
			return statsCollection;
		}
	}
}
