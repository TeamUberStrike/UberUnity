using System.IO;
using Cmune.DataCenter.Common.Entities;
using UberStrike.Core.Models.Views;

namespace UberStrike.Core.Serialization
{
	public static class ItemPriceProxy
	{
		public static void Serialize(Stream stream, ItemPrice instance)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Int32Proxy.Serialize(memoryStream, instance.Amount);
				EnumProxy<UberStrikeCurrencyType>.Serialize(memoryStream, instance.Currency);
				Int32Proxy.Serialize(memoryStream, instance.Discount);
				EnumProxy<BuyingDurationType>.Serialize(memoryStream, instance.Duration);
				EnumProxy<PackType>.Serialize(memoryStream, instance.PackType);
				Int32Proxy.Serialize(memoryStream, instance.Price);
				memoryStream.WriteTo(stream);
			}
		}

		public static ItemPrice Deserialize(Stream bytes)
		{
			ItemPrice itemPrice = new ItemPrice();
			itemPrice.Amount = Int32Proxy.Deserialize(bytes);
			itemPrice.Currency = EnumProxy<UberStrikeCurrencyType>.Deserialize(bytes);
			itemPrice.Discount = Int32Proxy.Deserialize(bytes);
			itemPrice.Duration = EnumProxy<BuyingDurationType>.Deserialize(bytes);
			itemPrice.PackType = EnumProxy<PackType>.Deserialize(bytes);
			itemPrice.Price = Int32Proxy.Deserialize(bytes);
			return itemPrice;
		}
	}
}
