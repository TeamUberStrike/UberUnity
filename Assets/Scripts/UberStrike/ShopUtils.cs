using System.Collections.Generic;
using Cmune.DataCenter.Common.Entities;
using UberStrike.Core.Models.Views;
using UberStrike.Core.Types;
using UnityEngine;

public static class ShopUtils
{
	public class PriceComparer<T> : IComparer<KeyValuePair<T, ItemPrice>>
	{
		public int Compare(KeyValuePair<T, ItemPrice> x, KeyValuePair<T, ItemPrice> y)
		{
			int value = x.Value.Price + ((x.Value.Currency == UberStrikeCurrencyType.Credits) ? 1000000 : 0);
			return (y.Value.Price + ((y.Value.Currency == UberStrikeCurrencyType.Credits) ? 1000000 : 0)).CompareTo(value);
		}
	}

	private class DescendedComparer : IComparer<int>
	{
		public int Compare(int x, int y)
		{
			return y - x;
		}
	}

	public static ItemPrice GetLowestPrice(IUnityItem item, UberStrikeCurrencyType currency = UberStrikeCurrencyType.None)
	{
		ItemPrice itemPrice = null;
		if (item != null && item.View != null && item.View.Prices != null)
		{
			foreach (ItemPrice price in item.View.Prices)
			{
				if ((currency == UberStrikeCurrencyType.None || price.Currency == currency) && (itemPrice == null || itemPrice.Price > price.Price))
				{
					itemPrice = price;
				}
			}
		}
		return itemPrice;
	}

	public static string PrintDuration(BuyingDurationType duration)
	{
		switch (duration)
		{
		case BuyingDurationType.Permanent:
			return " " + LocalizedStrings.Permanent;
		case BuyingDurationType.OneDay:
			return " 1 " + LocalizedStrings.Day;
		case BuyingDurationType.SevenDays:
			return " 1 " + LocalizedStrings.Week;
		case BuyingDurationType.ThirtyDays:
			return " 1 " + LocalizedStrings.Month;
		case BuyingDurationType.NinetyDays:
			return " " + LocalizedStrings.ThreeMonths;
		default:
			return string.Empty;
		}
	}

	public static Texture2D CurrencyIcon(UberStrikeCurrencyType currency)
	{
		switch (currency)
		{
		case UberStrikeCurrencyType.Credits:
			return ShopIcons.IconCredits20x20;
		case UberStrikeCurrencyType.Points:
			return ShopIcons.IconPoints20x20;
		default:
			return null;
		}
	}

	public static bool IsMeleeWeapon(IUnityItem view)
	{
		if (view != null && view.View != null)
		{
			return view.View.ItemClass == UberstrikeItemClass.WeaponMelee;
		}
		return false;
	}

	public static bool IsInstantHitWeapon(IUnityItem view)
	{
		if (view != null && view.View != null)
		{
			return view.View.ItemClass == UberstrikeItemClass.WeaponMachinegun || view.View.ItemClass == UberstrikeItemClass.WeaponShotgun || view.View.ItemClass == UberstrikeItemClass.WeaponSniperRifle;
		}
		return false;
	}

	public static bool IsProjectileWeapon(IUnityItem view)
	{
		if (view != null && view.View != null)
		{
			return view.View.ItemClass == UberstrikeItemClass.WeaponCannon || view.View.ItemClass == UberstrikeItemClass.WeaponLauncher || view.View.ItemClass == UberstrikeItemClass.WeaponSplattergun;
		}
		return false;
	}
}
