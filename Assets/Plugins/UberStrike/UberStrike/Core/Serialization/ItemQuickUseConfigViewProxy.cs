using System.IO;
using UberStrike.Core.Types;
using UberStrike.DataCenter.Common.Entities;

namespace UberStrike.Core.Serialization
{
	public static class ItemQuickUseConfigViewProxy
	{
		public static void Serialize(Stream stream, ItemQuickUseConfigView instance)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				EnumProxy<QuickItemLogic>.Serialize(memoryStream, instance.BehaviourType);
				Int32Proxy.Serialize(memoryStream, instance.CoolDownTime);
				Int32Proxy.Serialize(memoryStream, instance.ItemId);
				Int32Proxy.Serialize(memoryStream, instance.LevelRequired);
				Int32Proxy.Serialize(memoryStream, instance.UsesPerGame);
				Int32Proxy.Serialize(memoryStream, instance.UsesPerLife);
				Int32Proxy.Serialize(memoryStream, instance.UsesPerRound);
				Int32Proxy.Serialize(memoryStream, instance.WarmUpTime);
				memoryStream.WriteTo(stream);
			}
		}

		public static ItemQuickUseConfigView Deserialize(Stream bytes)
		{
			ItemQuickUseConfigView itemQuickUseConfigView = new ItemQuickUseConfigView();
			itemQuickUseConfigView.BehaviourType = EnumProxy<QuickItemLogic>.Deserialize(bytes);
			itemQuickUseConfigView.CoolDownTime = Int32Proxy.Deserialize(bytes);
			itemQuickUseConfigView.ItemId = Int32Proxy.Deserialize(bytes);
			itemQuickUseConfigView.LevelRequired = Int32Proxy.Deserialize(bytes);
			itemQuickUseConfigView.UsesPerGame = Int32Proxy.Deserialize(bytes);
			itemQuickUseConfigView.UsesPerLife = Int32Proxy.Deserialize(bytes);
			itemQuickUseConfigView.UsesPerRound = Int32Proxy.Deserialize(bytes);
			itemQuickUseConfigView.WarmUpTime = Int32Proxy.Deserialize(bytes);
			return itemQuickUseConfigView;
		}
	}
}
