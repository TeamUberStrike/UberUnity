using System.IO;
using UberStrike.DataCenter.Common.Entities;

namespace UberStrike.Core.Serialization
{
	public static class DailyPointsViewProxy
	{
		public static void Serialize(Stream stream, DailyPointsView instance)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Int32Proxy.Serialize(memoryStream, instance.Current);
				Int32Proxy.Serialize(memoryStream, instance.PointsMax);
				Int32Proxy.Serialize(memoryStream, instance.PointsTomorrow);
				memoryStream.WriteTo(stream);
			}
		}

		public static DailyPointsView Deserialize(Stream bytes)
		{
			DailyPointsView dailyPointsView = new DailyPointsView();
			dailyPointsView.Current = Int32Proxy.Deserialize(bytes);
			dailyPointsView.PointsMax = Int32Proxy.Deserialize(bytes);
			dailyPointsView.PointsTomorrow = Int32Proxy.Deserialize(bytes);
			return dailyPointsView;
		}
	}
}
