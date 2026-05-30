using System.Collections.Generic;
using Cmune.DataCenter.Common.Entities;
using UberStrike.Core.Models.Views;

public static class ItemPackGuiUtil
{
	public const int Columns = 6;

	public const int Rows = 2;

	public static BuyingDurationType GetDuration(IUnityItem item)
	{
		BuyingDurationType result = BuyingDurationType.None;
		if (item != null && item.View != null && item.View.Prices != null)
		{
			IEnumerator<ItemPrice> enumerator = item.View.Prices.GetEnumerator();
			if (enumerator.MoveNext())
			{
				result = enumerator.Current.Duration;
			}
		}
		return result;
	}
}
