using System.Collections.Generic;
using Cmune.DataCenter.Common.Entities;
using UberStrike.Core.Models.Views;
using UberStrike.Core.Types;
using UberStrike.DataCenter.Common.Entities;
using UnityEngine;

public class LoadoutManager : Singleton<LoadoutManager>
{
	public static readonly LoadoutSlotType[] QuickSlots = new LoadoutSlotType[3]
	{
		LoadoutSlotType.QuickUseItem1,
		LoadoutSlotType.QuickUseItem2,
		LoadoutSlotType.QuickUseItem3
	};

	public static readonly LoadoutSlotType[] WeaponSlots = new LoadoutSlotType[4]
	{
		LoadoutSlotType.WeaponMelee,
		LoadoutSlotType.WeaponPrimary,
		LoadoutSlotType.WeaponSecondary,
		LoadoutSlotType.WeaponTertiary
	};

	public static readonly LoadoutSlotType[] GearSlots = new LoadoutSlotType[6]
	{
		LoadoutSlotType.GearHead,
		LoadoutSlotType.GearFace,
		LoadoutSlotType.GearGloves,
		LoadoutSlotType.GearUpperBody,
		LoadoutSlotType.GearLowerBody,
		LoadoutSlotType.GearBoots
	};

	public static readonly UberstrikeItemClass[] GearSlotClasses = new UberstrikeItemClass[6]
	{
		UberstrikeItemClass.GearHead,
		UberstrikeItemClass.GearFace,
		UberstrikeItemClass.GearGloves,
		UberstrikeItemClass.GearUpperBody,
		UberstrikeItemClass.GearLowerBody,
		UberstrikeItemClass.GearBoots
	};

	public static readonly string[] GearSlotNames = new string[6]
	{
		LocalizedStrings.Head,
		LocalizedStrings.Face,
		LocalizedStrings.Gloves,
		LocalizedStrings.UpperBody,
		LocalizedStrings.LowerBody,
		LocalizedStrings.Boots
	};

	public Loadout Loadout { get; private set; }

	private LoadoutManager()
	{
		Dictionary<LoadoutSlotType, IUnityItem> dictionary = new Dictionary<LoadoutSlotType, IUnityItem>();
		LoadoutSlotType[] array = new LoadoutSlotType[5]
		{
			LoadoutSlotType.GearHead,
			LoadoutSlotType.GearGloves,
			LoadoutSlotType.GearUpperBody,
			LoadoutSlotType.GearLowerBody,
			LoadoutSlotType.GearBoots
		};
		int[] array2 = new int[5] { 1084, 1086, 1087, 1088, 1089 };
		for (int i = 0; i < array.Length; i++)
		{
			IUnityItem itemInShop = Singleton<ItemManager>.Instance.GetItemInShop(array2[i]);
			if (itemInShop != null)
			{
				dictionary.Add(array[i], itemInShop);
			}
		}
		Loadout = new Loadout(dictionary);
	}

	public void EquipWeapon(LoadoutSlotType weaponSlot, IUnityItem itemWeapon)
	{
		if (itemWeapon != null)
		{
			GameObject gameObject = itemWeapon.Create(Vector3.zero, Quaternion.identity);
			if ((bool)gameObject)
			{
				BaseWeaponDecorator component = gameObject.GetComponent<BaseWeaponDecorator>();
				component.EnableShootAnimation = false;
				GameState.Current.Avatar.AssignWeapon(weaponSlot, component, itemWeapon);
				GameState.Current.Avatar.ShowWeapon(weaponSlot);
			}
			else
			{
				GameState.Current.Avatar.UnassignWeapon(weaponSlot);
			}
		}
		else
		{
			GameState.Current.Avatar.UnassignWeapon(weaponSlot);
		}
	}

	public void UpdateLoadout(LoadoutView view)
	{
		if (view.Head == 0)
		{
			Loadout.SetSlot(LoadoutSlotType.GearHead, Singleton<ItemManager>.Instance.GetItemInShop(1084));
		}
		else
		{
			Loadout.SetSlot(LoadoutSlotType.GearHead, Singleton<ItemManager>.Instance.GetItemInShop(view.Head));
		}
		if (view.Gloves == 0)
		{
			Loadout.SetSlot(LoadoutSlotType.GearGloves, Singleton<ItemManager>.Instance.GetItemInShop(1086));
		}
		else
		{
			Loadout.SetSlot(LoadoutSlotType.GearGloves, Singleton<ItemManager>.Instance.GetItemInShop(view.Gloves));
		}
		if (view.UpperBody == 0)
		{
			Loadout.SetSlot(LoadoutSlotType.GearUpperBody, Singleton<ItemManager>.Instance.GetItemInShop(1087));
		}
		else
		{
			Loadout.SetSlot(LoadoutSlotType.GearUpperBody, Singleton<ItemManager>.Instance.GetItemInShop(view.UpperBody));
		}
		if (view.LowerBody == 0)
		{
			Loadout.SetSlot(LoadoutSlotType.GearLowerBody, Singleton<ItemManager>.Instance.GetItemInShop(1088));
		}
		else
		{
			Loadout.SetSlot(LoadoutSlotType.GearLowerBody, Singleton<ItemManager>.Instance.GetItemInShop(view.LowerBody));
		}
		if (view.Boots == 0)
		{
			Loadout.SetSlot(LoadoutSlotType.GearBoots, Singleton<ItemManager>.Instance.GetItemInShop(1089));
		}
		else
		{
			Loadout.SetSlot(LoadoutSlotType.GearBoots, Singleton<ItemManager>.Instance.GetItemInShop(view.Boots));
		}
		Loadout.SetSlot(LoadoutSlotType.GearFace, Singleton<ItemManager>.Instance.GetItemInShop(view.Face));
		Loadout.SetSlot(LoadoutSlotType.GearHolo, Singleton<ItemManager>.Instance.GetItemInShop(view.Webbing));
		Loadout.SetSlot(LoadoutSlotType.WeaponMelee, Singleton<ItemManager>.Instance.GetItemInShop(view.MeleeWeapon));
		Loadout.SetSlot(LoadoutSlotType.WeaponPrimary, Singleton<ItemManager>.Instance.GetItemInShop(view.Weapon1));
		Loadout.SetSlot(LoadoutSlotType.WeaponSecondary, Singleton<ItemManager>.Instance.GetItemInShop(view.Weapon2));
		Loadout.SetSlot(LoadoutSlotType.WeaponTertiary, Singleton<ItemManager>.Instance.GetItemInShop(view.Weapon3));
		Loadout.SetSlot(LoadoutSlotType.QuickUseItem1, Singleton<ItemManager>.Instance.GetItemInShop(view.QuickItem1));
		Loadout.SetSlot(LoadoutSlotType.QuickUseItem2, Singleton<ItemManager>.Instance.GetItemInShop(view.QuickItem2));
		Loadout.SetSlot(LoadoutSlotType.QuickUseItem3, Singleton<ItemManager>.Instance.GetItemInShop(view.QuickItem3));
		Loadout.SetSlot(LoadoutSlotType.FunctionalItem1, Singleton<ItemManager>.Instance.GetItemInShop(view.FunctionalItem1));
		Loadout.SetSlot(LoadoutSlotType.FunctionalItem2, Singleton<ItemManager>.Instance.GetItemInShop(view.FunctionalItem2));
		Loadout.SetSlot(LoadoutSlotType.FunctionalItem3, Singleton<ItemManager>.Instance.GetItemInShop(view.FunctionalItem3));
		UpdateArmor();
	}

	public bool RemoveDuplicateWeaponClass(InventoryItem baseItem)
	{
		LoadoutSlotType updatedSlot = LoadoutSlotType.None;
		return RemoveDuplicateWeaponClass(baseItem, ref updatedSlot);
	}

	public bool RemoveDuplicateWeaponClass(InventoryItem baseItem, ref LoadoutSlotType updatedSlot)
	{
		bool result = false;
		if (baseItem != null && baseItem.Item.View.ItemType == UberstrikeItemType.Weapon)
		{
			LoadoutSlotType[] weaponSlots = WeaponSlots;
			foreach (LoadoutSlotType loadoutSlotType in weaponSlots)
			{
				InventoryItem item;
				if (TryGetItemInSlot(loadoutSlotType, out item) && item.Item.View.ItemClass == baseItem.Item.View.ItemClass && item.Item.View.ID != baseItem.Item.View.ID)
				{
					GameState.Current.Avatar.UnassignWeapon(loadoutSlotType);
					ResetSlot(loadoutSlotType);
					updatedSlot = loadoutSlotType;
					result = true;
					break;
				}
			}
		}
		return result;
	}

	public bool RemoveDuplicateQuickItemClass(UberStrikeItemQuickView view, ref LoadoutSlotType lastRemovedSlot)
	{
		bool result = false;
		if (view.ItemType == UberstrikeItemType.QuickUse)
		{
			LoadoutSlotType[] array = new LoadoutSlotType[3]
			{
				LoadoutSlotType.QuickUseItem1,
				LoadoutSlotType.QuickUseItem2,
				LoadoutSlotType.QuickUseItem3
			};
			LoadoutSlotType[] array2 = array;
			foreach (LoadoutSlotType loadoutSlotType in array2)
			{
				InventoryItem item;
				if (TryGetItemInSlot(loadoutSlotType, out item))
				{
					UberStrikeItemQuickView uberStrikeItemQuickView = item.Item as UberStrikeItemQuickView;
					if (item.Item.View.ItemType == UberstrikeItemType.QuickUse && uberStrikeItemQuickView.BehaviourType == view.BehaviourType)
					{
						ResetSlot(loadoutSlotType);
						result = true;
						lastRemovedSlot = loadoutSlotType;
					}
				}
			}
		}
		return result;
	}

	public bool RemoveDuplicateFunctionalItemClass(InventoryItem inventoryItem, ref LoadoutSlotType lastRemovedSlot)
	{
		bool result = false;
		if (inventoryItem != null && inventoryItem.Item.View.ItemType == UberstrikeItemType.Functional)
		{
			LoadoutSlotType[] array = new LoadoutSlotType[3]
			{
				LoadoutSlotType.FunctionalItem1,
				LoadoutSlotType.FunctionalItem2,
				LoadoutSlotType.FunctionalItem3
			};
			LoadoutSlotType[] array2 = array;
			foreach (LoadoutSlotType loadoutSlotType in array2)
			{
				if (HasLoadoutItem(loadoutSlotType) && GetItemOnSlot(loadoutSlotType).View.ItemClass == inventoryItem.Item.View.ItemClass)
				{
					ResetSlot(loadoutSlotType);
					result = true;
					lastRemovedSlot = loadoutSlotType;
				}
			}
		}
		return result;
	}

	public void SwitchItemInSlot(LoadoutSlotType slot1, LoadoutSlotType slot2)
	{
		IUnityItem item;
		bool flag = Loadout.TryGetItem(slot1, out item);
		IUnityItem item2;
		bool flag2 = Loadout.TryGetItem(slot2, out item2);
		if (flag)
		{
			if (flag2)
			{
				Loadout.SetSlot(slot1, item2);
				Loadout.SetSlot(slot2, item);
			}
			else
			{
				Loadout.SetSlot(slot2, item);
				Loadout.ClearSlot(slot1);
			}
		}
		else if (flag2)
		{
			Loadout.SetSlot(slot1, item2);
			Loadout.ClearSlot(slot2);
		}
	}

	public bool IsWeaponSlotType(LoadoutSlotType slot)
	{
		return slot >= LoadoutSlotType.WeaponMelee && slot <= LoadoutSlotType.WeaponTertiary;
	}

	public bool IsQuickItemSlotType(LoadoutSlotType slot)
	{
		return slot >= LoadoutSlotType.QuickUseItem1 && slot <= LoadoutSlotType.QuickUseItem3;
	}

	public bool IsFunctionalItemSlotType(LoadoutSlotType slot)
	{
		return slot >= LoadoutSlotType.FunctionalItem1 && slot <= LoadoutSlotType.FunctionalItem3;
	}

	public bool SwapLoadoutItems(LoadoutSlotType slotA, LoadoutSlotType slotB)
	{
		bool result = false;
		if (slotA != slotB)
		{
			if (IsWeaponSlotType(slotA) && IsWeaponSlotType(slotB))
			{
				InventoryItem item = null;
				InventoryItem item2 = null;
				TryGetItemInSlot(slotA, out item);
				TryGetItemInSlot(slotB, out item2);
				if (item != null || item2 != null)
				{
					object item4;
					if (item2 != null)
					{
						IUnityItem item3 = item2.Item;
						item4 = item3;
					}
					else
					{
						item4 = null;
					}
					SetLoadoutItem(slotA, (IUnityItem)item4);
					object item5;
					if (item != null)
					{
						IUnityItem item3 = item.Item;
						item5 = item3;
					}
					else
					{
						item5 = null;
					}
					SetLoadoutItem(slotB, (IUnityItem)item5);
					object itemWeapon;
					if (item2 != null)
					{
						IUnityItem item3 = item2.Item;
						itemWeapon = item3;
					}
					else
					{
						itemWeapon = null;
					}
					EquipWeapon(slotA, (IUnityItem)itemWeapon);
					object itemWeapon2;
					if (item != null)
					{
						IUnityItem item3 = item.Item;
						itemWeapon2 = item3;
					}
					else
					{
						itemWeapon2 = null;
					}
					EquipWeapon(slotB, (IUnityItem)itemWeapon2);
					result = true;
				}
			}
			else if ((IsQuickItemSlotType(slotA) && IsQuickItemSlotType(slotB)) || (IsFunctionalItemSlotType(slotA) && IsFunctionalItemSlotType(slotB)))
			{
				InventoryItem item6 = null;
				InventoryItem item7 = null;
				TryGetItemInSlot(slotA, out item6);
				TryGetItemInSlot(slotB, out item7);
				if (item6 != null || item7 != null)
				{
					object item8;
					if (item7 != null)
					{
						IUnityItem item3 = item7.Item;
						item8 = item3;
					}
					else
					{
						item8 = null;
					}
					SetLoadoutItem(slotA, (IUnityItem)item8);
					object item9;
					if (item6 != null)
					{
						IUnityItem item3 = item6.Item;
						item9 = item3;
					}
					else
					{
						item9 = null;
					}
					SetLoadoutItem(slotB, (IUnityItem)item9);
					result = true;
				}
			}
		}
		return result;
	}

	public void SetLoadoutItem(LoadoutSlotType loadoutSlotType, IUnityItem item)
	{
		if (item == null)
		{
			ResetSlot(loadoutSlotType);
			return;
		}
		InventoryItem item2;
		if (Singleton<InventoryManager>.Instance.TryGetInventoryItem(item.View.ID, out item2) && item2.IsValid)
		{
			if (item.View.ItemType == UberstrikeItemType.Weapon)
			{
				RemoveDuplicateWeaponClass(item2);
			}
			Loadout.SetSlot(loadoutSlotType, item);
		}
		else if (item.View != null)
		{
			BuyPanelGUI buyPanelGUI = PanelManager.Instance.OpenPanel(PanelType.BuyItem) as BuyPanelGUI;
			if ((bool)buyPanelGUI)
			{
				buyPanelGUI.SetItem(item, BuyingLocationType.Shop, BuyingRecommendationType.None);
			}
		}
		UnityRuntime.StartRoutine(Singleton<PlayerDataManager>.Instance.StartSetLoadout());
		UpdateArmor();
	}

	public void ResetSlot(LoadoutSlotType loadoutSlotType)
	{
		Loadout.ClearSlot(loadoutSlotType);
		UnityRuntime.StartRoutine(Singleton<PlayerDataManager>.Instance.StartSetLoadout());
		UpdateArmor();
	}

	public void GetArmorValues(out int armorPoints)
	{
		armorPoints = 0;
		InventoryItem item;
		if (TryGetItemInSlot(LoadoutSlotType.GearLowerBody, out item) && item.Item.View.ItemType == UberstrikeItemType.Gear)
		{
			UberStrikeItemGearView uberStrikeItemGearView = item.Item.View as UberStrikeItemGearView;
			armorPoints += uberStrikeItemGearView.ArmorPoints;
		}
		if (TryGetItemInSlot(LoadoutSlotType.GearUpperBody, out item) && item.Item.View.ItemType == UberstrikeItemType.Gear)
		{
			UberStrikeItemGearView uberStrikeItemGearView2 = item.Item.View as UberStrikeItemGearView;
			armorPoints += uberStrikeItemGearView2.ArmorPoints;
		}
		if (TryGetItemInSlot(LoadoutSlotType.GearHolo, out item) && item.Item.View.ItemType == UberstrikeItemType.Gear)
		{
			UberStrikeItemGearView uberStrikeItemGearView3 = item.Item.View as UberStrikeItemGearView;
			armorPoints += uberStrikeItemGearView3.ArmorPoints;
		}
	}

	public bool HasLoadoutItem(LoadoutSlotType loadoutSlotType)
	{
		IUnityItem item;
		return Loadout.TryGetItem(loadoutSlotType, out item);
	}

	public int GetItemIdOnSlot(LoadoutSlotType loadoutSlotType)
	{
		int result = 0;
		IUnityItem item;
		if (Loadout.TryGetItem(loadoutSlotType, out item))
		{
			result = item.View.ID;
		}
		return result;
	}

	public IUnityItem GetItemOnSlot(LoadoutSlotType loadoutSlotType)
	{
		IUnityItem item = null;
		Loadout.TryGetItem(loadoutSlotType, out item);
		return item;
	}

	public bool IsItemEquipped(int itemId)
	{
		return Loadout.Contains(itemId);
	}

	public bool HasItemInSlot(LoadoutSlotType slot)
	{
		InventoryItem item;
		return TryGetItemInSlot(slot, out item);
	}

	public bool TryGetItemInSlot(LoadoutSlotType slot, out InventoryItem item)
	{
		item = null;
		IUnityItem item2;
		return Loadout.TryGetItem(slot, out item2) && Singleton<InventoryManager>.Instance.TryGetInventoryItem(item2.View.ID, out item);
	}

	public bool TryGetSlotForItem(IUnityItem item, out LoadoutSlotType slot)
	{
		slot = LoadoutSlotType.None;
		Dictionary<LoadoutSlotType, IUnityItem>.Enumerator enumerator = Loadout.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (enumerator.Current.Value == item)
			{
				slot = enumerator.Current.Key;
				return true;
			}
		}
		return false;
	}

	public bool ValidateLoadout()
	{
		return Loadout.ItemCount > 0;
	}

	public void UpdateArmor()
	{
		int armorPoints;
		GetArmorValues(out armorPoints);
		GameState.Current.PlayerData.ArmorCarried.Value = armorPoints;
	}

	public List<int> GetWeapons()
	{
		List<int> list = new List<int>();
		list.Add(GetItemIdOnSlot(LoadoutSlotType.WeaponMelee));
		list.Add(GetItemIdOnSlot(LoadoutSlotType.WeaponPrimary));
		list.Add(GetItemIdOnSlot(LoadoutSlotType.WeaponSecondary));
		list.Add(GetItemIdOnSlot(LoadoutSlotType.WeaponTertiary));
		return list;
	}

	public List<int> GetGear()
	{
		List<int> list = new List<int>();
		list.Add(GetItemIdOnSlot(LoadoutSlotType.GearHead));
		list.Add(GetItemIdOnSlot(LoadoutSlotType.GearFace));
		list.Add(GetItemIdOnSlot(LoadoutSlotType.GearGloves));
		list.Add(GetItemIdOnSlot(LoadoutSlotType.GearUpperBody));
		list.Add(GetItemIdOnSlot(LoadoutSlotType.GearLowerBody));
		list.Add(GetItemIdOnSlot(LoadoutSlotType.GearBoots));
		list.Add(GetItemIdOnSlot(LoadoutSlotType.GearHolo));
		return list;
	}

	public List<int> GetQuickItems()
	{
		List<int> list = new List<int>();
		list.Add(GetItemIdOnSlot(LoadoutSlotType.QuickUseItem1));
		list.Add(GetItemIdOnSlot(LoadoutSlotType.QuickUseItem2));
		list.Add(GetItemIdOnSlot(LoadoutSlotType.QuickUseItem3));
		return list;
	}
}
