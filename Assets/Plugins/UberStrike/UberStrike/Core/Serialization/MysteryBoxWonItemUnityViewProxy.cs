using System.IO;
using Cmune.DataCenter.Common.Entities;

namespace UberStrike.Core.Serialization
{
	public static class MysteryBoxWonItemUnityViewProxy
	{
		public static void Serialize(Stream stream, MysteryBoxWonItemUnityView instance)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Int32Proxy.Serialize(memoryStream, instance.CreditWon);
				Int32Proxy.Serialize(memoryStream, instance.ItemIdWon);
				Int32Proxy.Serialize(memoryStream, instance.PointWon);
				memoryStream.WriteTo(stream);
			}
		}

		public static MysteryBoxWonItemUnityView Deserialize(Stream bytes)
		{
			MysteryBoxWonItemUnityView mysteryBoxWonItemUnityView = new MysteryBoxWonItemUnityView();
			mysteryBoxWonItemUnityView.CreditWon = Int32Proxy.Deserialize(bytes);
			mysteryBoxWonItemUnityView.ItemIdWon = Int32Proxy.Deserialize(bytes);
			mysteryBoxWonItemUnityView.PointWon = Int32Proxy.Deserialize(bytes);
			return mysteryBoxWonItemUnityView;
		}
	}
}
