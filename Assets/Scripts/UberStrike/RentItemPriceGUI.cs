using System;
using System.Collections.Generic;
using Cmune.DataCenter.Common.Entities;
using UberStrike.Core.Models.Views;
using UnityEngine;

public class RentItemPriceGUI : ItemPriceGUI
{
	private List<ItemPrice> _prices;

	public RentItemPriceGUI(IUnityItem item, Action<ItemPrice> onPriceSelected)
		: base(item.View.LevelLock, onPriceSelected)
	{
		_prices = new List<ItemPrice>(item.View.Prices);
		if (_prices.Count > 0)
		{
			_onPriceSelected(_prices[_prices.Count - 1]);
		}
	}

	public override void Draw(Rect rect)
	{
		GUI.BeginGroup(rect);
		int num = 30;
		if (_prices.Exists((ItemPrice p) => p.Duration != BuyingDurationType.Permanent))
		{
			GUI.Label(new Rect(0f, 0f, rect.width, 16f), "Limited Use", BlueStonez.label_interparkbold_16pt_left);
			foreach (ItemPrice price in _prices)
			{
				if (price.Duration != BuyingDurationType.Permanent)
				{
					GUIContent gUIContent = new GUIContent(ShopUtils.PrintDuration(price.Duration));
					if (price.Currency == UberStrikeCurrencyType.Points && _levelLocked)
					{
						GUI.enabled = false;
						gUIContent.tooltip = _tooltip;
					}
					if (GUI.Toggle(new Rect(0f, num, rect.width, 20f), _selectedPrice == price, gUIContent, BlueStonez.toggle) && price != _selectedPrice)
					{
						_onPriceSelected(price);
					}
					num = DrawPrice(price, rect.width * 0.5f, num);
					GUI.enabled = true;
				}
			}
			num += 20;
		}
		if (_prices.Exists((ItemPrice p) => p.Duration == BuyingDurationType.Permanent))
		{
			GUI.Label(new Rect(0f, num, rect.width, 16f), "Unlimited Use", BlueStonez.label_interparkbold_16pt_left);
			num += 30;
			foreach (ItemPrice price2 in _prices)
			{
				if (price2.Duration == BuyingDurationType.Permanent)
				{
					string empty = string.Empty;
					if (GUI.Toggle(new Rect(0f, num, rect.width, 20f), _selectedPrice == price2, new GUIContent(LocalizedStrings.Permanent, empty), BlueStonez.toggle) && price2 != _selectedPrice)
					{
						_onPriceSelected(price2);
					}
					num = DrawPrice(price2, rect.width * 0.5f, num);
				}
			}
		}
		base.Height = num;
		GUI.EndGroup();
	}

	private string GetRentDuration(BuyingDurationType duration)
	{
		string result = string.Empty;
		switch (duration)
		{
		case BuyingDurationType.OneDay:
			result = LocalizedStrings.OneDay;
			break;
		case BuyingDurationType.SevenDays:
			result = LocalizedStrings.SevenDays;
			break;
		}
		return result;
	}
}
