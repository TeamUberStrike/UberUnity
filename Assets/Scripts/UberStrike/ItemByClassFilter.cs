using UberStrike.Core.Types;

public class ItemByClassFilter : IShopItemFilter
{
	private UberstrikeItemType _itemType;

	private UberstrikeItemClass _itemClass;

	public ItemByClassFilter(UberstrikeItemType itemType, UberstrikeItemClass itemClass)
	{
		_itemType = itemType;
		_itemClass = itemClass;
	}

	public bool CanPass(IUnityItem item)
	{
		return item.View.ItemType == _itemType && item.View.ItemClass == _itemClass;
	}
}
