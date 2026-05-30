using System;
using System.IO;
using Cmune.DataCenter.Common.Entities;

namespace UberStrike.Core.Serialization
{
	public static class ItemInventoryViewProxy
	{
		public static void Serialize(Stream stream, ItemInventoryView instance)
		{
			int num = 0;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Int32Proxy.Serialize(memoryStream, instance.AmountRemaining);
				Int32Proxy.Serialize(memoryStream, instance.Cmid);
				if (instance.ExpirationDate.HasValue)
				{
					DateTime? expirationDate = instance.ExpirationDate;
					DateTimeProxy.Serialize(memoryStream, (!expirationDate.HasValue) ? default(DateTime) : expirationDate.Value);
				}
				else
				{
					num |= 1;
				}
				Int32Proxy.Serialize(memoryStream, instance.ItemId);
				Int32Proxy.Serialize(stream, ~num);
				memoryStream.WriteTo(stream);
			}
		}

		public static ItemInventoryView Deserialize(Stream bytes)
		{
			int num = Int32Proxy.Deserialize(bytes);
			ItemInventoryView itemInventoryView = new ItemInventoryView();
			itemInventoryView.AmountRemaining = Int32Proxy.Deserialize(bytes);
			itemInventoryView.Cmid = Int32Proxy.Deserialize(bytes);
			if ((num & 1) != 0)
			{
				itemInventoryView.ExpirationDate = DateTimeProxy.Deserialize(bytes);
			}
			itemInventoryView.ItemId = Int32Proxy.Deserialize(bytes);
			return itemInventoryView;
		}
	}
}
