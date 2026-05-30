using System.IO;
using UberStrike.Core.Models.Views;

namespace UberStrike.Core.Serialization
{
	public static class MatchPointsViewProxy
	{
		public static void Serialize(Stream stream, MatchPointsView instance)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Int32Proxy.Serialize(memoryStream, instance.LoserPointsBase);
				Int32Proxy.Serialize(memoryStream, instance.LoserPointsPerMinute);
				Int32Proxy.Serialize(memoryStream, instance.MaxTimeInGame);
				Int32Proxy.Serialize(memoryStream, instance.WinnerPointsBase);
				Int32Proxy.Serialize(memoryStream, instance.WinnerPointsPerMinute);
				memoryStream.WriteTo(stream);
			}
		}

		public static MatchPointsView Deserialize(Stream bytes)
		{
			MatchPointsView matchPointsView = new MatchPointsView();
			matchPointsView.LoserPointsBase = Int32Proxy.Deserialize(bytes);
			matchPointsView.LoserPointsPerMinute = Int32Proxy.Deserialize(bytes);
			matchPointsView.MaxTimeInGame = Int32Proxy.Deserialize(bytes);
			matchPointsView.WinnerPointsBase = Int32Proxy.Deserialize(bytes);
			matchPointsView.WinnerPointsPerMinute = Int32Proxy.Deserialize(bytes);
			return matchPointsView;
		}
	}
}
