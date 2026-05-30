namespace Cmune.DataCenter.Common.Entities
{
	public enum BuyItemResult
	{
		OK = 0,
		DisableInShop = 1,
		DisableForRent = 3,
		DisableForPermanent = 4,
		DurationDisabled = 5,
		PackDisabled = 6,
		IsNotForSale = 7,
		NotEnoughCurrency = 8,
		InvalidMember = 9,
		InvalidExpirationDate = 10,
		AlreadyInInventory = 11,
		InvalidAmount = 12,
		NoStockRemaining = 13,
		InvalidData = 14,
		TooManyUsage = 15,
		InvalidLevel = 100,
		ItemNotFound = 404
	}
}
