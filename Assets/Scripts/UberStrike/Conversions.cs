using Cmune.DataCenter.Common.Entities;
using UberStrike.Core.Models.Views;
using UberStrike.Core.Types;
using UnityEngine;

public static class Conversions
{
	public static GameModeType GetGameModeType(this GameMode mode)
	{
		switch (mode)
		{
		case GameMode.DeathMatch:
			return GameModeType.DeathMatch;
		case GameMode.TeamDeathMatch:
			return GameModeType.TeamDeathMatch;
		case GameMode.TeamElimination:
			return GameModeType.EliminationMode;
		default:
			return GameModeType.None;
		}
	}

	public static GUIContent PriceTag(this ItemPrice price, bool printCurrency = false, string tooltip = "")
	{
		switch (price.Currency)
		{
		case UberStrikeCurrencyType.Points:
			return new GUIContent(price.Price.ToString("N0") + ((!printCurrency) ? string.Empty : "Points"), ShopIcons.IconPoints20x20, tooltip);
		case UberStrikeCurrencyType.Credits:
			return new GUIContent(price.Price.ToString("N0") + ((!printCurrency) ? string.Empty : "Credits"), ShopIcons.IconCredits20x20, tooltip);
		default:
			return new GUIContent("N/A");
		}
	}
}
