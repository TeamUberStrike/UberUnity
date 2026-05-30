using System.IO;
using Cmune.DataCenter.Common.Entities;

namespace UberStrike.Core.Serialization
{
	public static class BundleItemViewProxy
	{
		public static void Serialize(Stream stream, BundleItemView instance)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Int32Proxy.Serialize(memoryStream, instance.Amount);
				Int32Proxy.Serialize(memoryStream, instance.BundleId);
				EnumProxy<BuyingDurationType>.Serialize(memoryStream, instance.Duration);
				Int32Proxy.Serialize(memoryStream, instance.ItemId);
				memoryStream.WriteTo(stream);
			}
		}

		public static BundleItemView Deserialize(Stream bytes)
		{
			BundleItemView bundleItemView = new BundleItemView();
			bundleItemView.Amount = Int32Proxy.Deserialize(bytes);
			bundleItemView.BundleId = Int32Proxy.Deserialize(bytes);
			bundleItemView.Duration = EnumProxy<BuyingDurationType>.Deserialize(bytes);
			bundleItemView.ItemId = Int32Proxy.Deserialize(bytes);
			return bundleItemView;
		}
	}
}
