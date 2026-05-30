using System.IO;
using Cmune.DataCenter.Common.Entities;

namespace UberStrike.Core.Serialization
{
	public static class ItemTransactionViewProxy
	{
		public static void Serialize(Stream stream, ItemTransactionView instance)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Int32Proxy.Serialize(memoryStream, instance.Cmid);
				Int32Proxy.Serialize(memoryStream, instance.Credits);
				EnumProxy<BuyingDurationType>.Serialize(memoryStream, instance.Duration);
				BooleanProxy.Serialize(memoryStream, instance.IsAdminAction);
				Int32Proxy.Serialize(memoryStream, instance.ItemId);
				Int32Proxy.Serialize(memoryStream, instance.Points);
				DateTimeProxy.Serialize(memoryStream, instance.WithdrawalDate);
				Int32Proxy.Serialize(memoryStream, instance.WithdrawalId);
				memoryStream.WriteTo(stream);
			}
		}

		public static ItemTransactionView Deserialize(Stream bytes)
		{
			ItemTransactionView itemTransactionView = new ItemTransactionView();
			itemTransactionView.Cmid = Int32Proxy.Deserialize(bytes);
			itemTransactionView.Credits = Int32Proxy.Deserialize(bytes);
			itemTransactionView.Duration = EnumProxy<BuyingDurationType>.Deserialize(bytes);
			itemTransactionView.IsAdminAction = BooleanProxy.Deserialize(bytes);
			itemTransactionView.ItemId = Int32Proxy.Deserialize(bytes);
			itemTransactionView.Points = Int32Proxy.Deserialize(bytes);
			itemTransactionView.WithdrawalDate = DateTimeProxy.Deserialize(bytes);
			itemTransactionView.WithdrawalId = Int32Proxy.Deserialize(bytes);
			return itemTransactionView;
		}
	}
}
