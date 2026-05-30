using System.IO;
using Cmune.DataCenter.Common.Entities;

namespace UberStrike.Core.Serialization
{
	public static class PointDepositViewProxy
	{
		public static void Serialize(Stream stream, PointDepositView instance)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Int32Proxy.Serialize(memoryStream, instance.Cmid);
				DateTimeProxy.Serialize(memoryStream, instance.DepositDate);
				EnumProxy<PointsDepositType>.Serialize(memoryStream, instance.DepositType);
				BooleanProxy.Serialize(memoryStream, instance.IsAdminAction);
				Int32Proxy.Serialize(memoryStream, instance.PointDepositId);
				Int32Proxy.Serialize(memoryStream, instance.Points);
				memoryStream.WriteTo(stream);
			}
		}

		public static PointDepositView Deserialize(Stream bytes)
		{
			PointDepositView pointDepositView = new PointDepositView();
			pointDepositView.Cmid = Int32Proxy.Deserialize(bytes);
			pointDepositView.DepositDate = DateTimeProxy.Deserialize(bytes);
			pointDepositView.DepositType = EnumProxy<PointsDepositType>.Deserialize(bytes);
			pointDepositView.IsAdminAction = BooleanProxy.Deserialize(bytes);
			pointDepositView.PointDepositId = Int32Proxy.Deserialize(bytes);
			pointDepositView.Points = Int32Proxy.Deserialize(bytes);
			return pointDepositView;
		}
	}
}
