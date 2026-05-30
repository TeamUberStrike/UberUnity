using UberStrike.Core.Types;

public class ItemUtil
{
	public static UberstrikeItemClass ItemClassFromSlot(LoadoutSlotType slot)
	{
		UberstrikeItemClass result = (UberstrikeItemClass)0;
		switch (slot)
		{
		case LoadoutSlotType.GearBoots:
			result = UberstrikeItemClass.GearBoots;
			break;
		case LoadoutSlotType.GearFace:
			result = UberstrikeItemClass.GearFace;
			break;
		case LoadoutSlotType.GearGloves:
			result = UberstrikeItemClass.GearGloves;
			break;
		case LoadoutSlotType.GearHead:
			result = UberstrikeItemClass.GearHead;
			break;
		case LoadoutSlotType.GearHolo:
			result = UberstrikeItemClass.GearHolo;
			break;
		case LoadoutSlotType.GearLowerBody:
			result = UberstrikeItemClass.GearLowerBody;
			break;
		case LoadoutSlotType.GearUpperBody:
			result = UberstrikeItemClass.GearUpperBody;
			break;
		}
		return result;
	}

	public static LoadoutSlotType SlotFromItemClass(UberstrikeItemClass itemClass)
	{
		LoadoutSlotType result = LoadoutSlotType.None;
		switch (itemClass)
		{
		case UberstrikeItemClass.GearHead:
			result = LoadoutSlotType.GearHead;
			break;
		case UberstrikeItemClass.GearFace:
			result = LoadoutSlotType.GearFace;
			break;
		case UberstrikeItemClass.GearGloves:
			result = LoadoutSlotType.GearGloves;
			break;
		case UberstrikeItemClass.GearUpperBody:
			result = LoadoutSlotType.GearUpperBody;
			break;
		case UberstrikeItemClass.GearLowerBody:
			result = LoadoutSlotType.GearLowerBody;
			break;
		case UberstrikeItemClass.GearBoots:
			result = LoadoutSlotType.GearBoots;
			break;
		case UberstrikeItemClass.GearHolo:
			result = LoadoutSlotType.GearHolo;
			break;
		}
		return result;
	}
}
