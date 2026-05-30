using System.IO;
using Cmune.DataCenter.Common.Entities;
using UberStrike.Core.ViewModel;

namespace UberStrike.Core.Serialization
{
	public static class PointDepositsViewModelProxy
	{
		public static void Serialize(Stream stream, PointDepositsViewModel instance)
		{
			int num = 0;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				if (instance.PointDeposits != null)
				{
					ListProxy<PointDepositView>.Serialize(memoryStream, instance.PointDeposits, PointDepositViewProxy.Serialize);
				}
				else
				{
					num |= 1;
				}
				Int32Proxy.Serialize(memoryStream, instance.TotalCount);
				Int32Proxy.Serialize(stream, ~num);
				memoryStream.WriteTo(stream);
			}
		}

		public static PointDepositsViewModel Deserialize(Stream bytes)
		{
			int num = Int32Proxy.Deserialize(bytes);
			PointDepositsViewModel pointDepositsViewModel = new PointDepositsViewModel();
			if ((num & 1) != 0)
			{
				pointDepositsViewModel.PointDeposits = ListProxy<PointDepositView>.Deserialize(bytes, PointDepositViewProxy.Deserialize);
			}
			pointDepositsViewModel.TotalCount = Int32Proxy.Deserialize(bytes);
			return pointDepositsViewModel;
		}
	}
}
