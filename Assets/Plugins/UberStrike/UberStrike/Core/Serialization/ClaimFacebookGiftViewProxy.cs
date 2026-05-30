using System.IO;
using Cmune.DataCenter.Common.Entities;

namespace UberStrike.Core.Serialization
{
	public static class ClaimFacebookGiftViewProxy
	{
		public static void Serialize(Stream stream, ClaimFacebookGiftView instance)
		{
			int num = 0;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				EnumProxy<ClaimFacebookGiftResult>.Serialize(memoryStream, instance.ClaimResult);
				if (instance.ItemId.HasValue)
				{
					int? itemId = instance.ItemId;
					Int32Proxy.Serialize(memoryStream, itemId.HasValue ? itemId.Value : 0);
				}
				else
				{
					num |= 1;
				}
				Int32Proxy.Serialize(stream, ~num);
				memoryStream.WriteTo(stream);
			}
		}

		public static ClaimFacebookGiftView Deserialize(Stream bytes)
		{
			int num = Int32Proxy.Deserialize(bytes);
			ClaimFacebookGiftView claimFacebookGiftView = new ClaimFacebookGiftView();
			claimFacebookGiftView.ClaimResult = EnumProxy<ClaimFacebookGiftResult>.Deserialize(bytes);
			if ((num & 1) != 0)
			{
				claimFacebookGiftView.ItemId = Int32Proxy.Deserialize(bytes);
			}
			return claimFacebookGiftView;
		}
	}
}
