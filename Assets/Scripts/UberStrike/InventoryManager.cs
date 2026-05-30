using System;
using System.Collections;
using System.Collections.Generic;
using Cmune.DataCenter.Common.Entities;
using UberStrike.Core.Models.Views;
using UberStrike.Core.Types;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
	private IDictionary<int, InventoryItem> _inventoryItems;

	public LoadoutSlotType CurrentWeaponSlot = LoadoutSlotType.WeaponPrimary;

	public LoadoutSlotType CurrentQuickItemSot = LoadoutSlotType.QuickUseItem1;

	public LoadoutSlotType CurrentFunctionalSlot = LoadoutSlotType.FunctionalItem1;

	private static readonly InventoryItem EmptyItem = new InventoryItem(null);

	public IEnumerable<InventoryItem> InventoryItems
	{
		get
		{
			return _inventoryItems.Values;
		}
	}

	public event Action OnInventoryUpdated;

	private InventoryManager()
	{
		_inventoryItems = new Dictionary<int, InventoryItem>();
		this.OnInventoryUpdated = (Action)Delegate.Combine(this.OnInventoryUpdated, (Action)delegate
		{
		});
	}

	public IEnumerator StartUpdateInventoryAndEquipNewItem(IUnityItem item, bool equipNow = false)
	{
		if (item != null)
		{
			IPopupDialog popupDialog = PopupSystem.ShowMessage(LocalizedStrings.UpdatingInventory, LocalizedStrings.WereUpdatingYourInventoryPleaseWait, PopupSystem.AlertType.None);
			yield return UnityRuntime.StartRoutine(Singleton<ItemManager>.Instance.StartGetInventory(false));
			PopupSystem.HideMessage(popupDialog);
			if (equipNow)
			{
				EquipItem(item);
			}
			else if (GameState.Current.HasJoinedGame && GameState.Current.IsInGame)
			{
				EquipItem(item);
			}
			else if (item.View.ItemProperties.ContainsKey(ItemPropertyType.PointsBoost) || item.View.ItemProperties.ContainsKey(ItemPropertyType.XpBoost))
			{
				InventoryItem invItem = GetItem(item.View.ID);
				PopupSystem.ShowItem(item, "\nYou just bought the boost item!\nThis item is activated and expires in " + invItem.DaysRemaining + " days");
			}
			else
			{
				PopupSystem.ShowItem(item, string.Empty);
			}
			yield return UnityRuntime.StartRoutine(Singleton<PlayerDataManager>.Instance.StartGetMember());
		}
	}

	public bool EquipItem(IUnityItem item)
	{
		LoadoutSlotType slotType = LoadoutSlotType.None;
		InventoryItem item2;
		if (TryGetInventoryItem(item.View.ID, out item2) && item2.IsValid && item2.Item.View.ItemType == UberstrikeItemType.Weapon && GameState.Current.Map != null)
		{
			slotType = FindBestSlotToEquipWeapon(item);
		}
		return EquipItemOnSlot(item.View.ID, slotType);
	}

	public static LoadoutSlotType FindBestSlotToEquipWeapon(IUnityItem weapon)
	{
		UberstrikeItemClass itemClass = weapon.View.ItemClass;
		if (itemClass == UberstrikeItemClass.WeaponMelee)
		{
			return LoadoutSlotType.WeaponMelee;
		}
		LoadoutSlotType itemClassSlotType = Singleton<LoadoutManager>.Instance.Loadout.GetItemClassSlotType(itemClass);
		if (itemClassSlotType != LoadoutSlotType.None)
		{
			return itemClassSlotType;
		}
		LoadoutSlotType firstEmptyWeaponSlot = Singleton<LoadoutManager>.Instance.Loadout.GetFirstEmptyWeaponSlot();
		if (firstEmptyWeaponSlot != LoadoutSlotType.None)
		{
			return firstEmptyWeaponSlot;
		}
		return LoadoutSlotType.WeaponPrimary;
	}

	public void UnequipWeaponSlot(LoadoutSlotType slotType)
	{
		Singleton<LoadoutManager>.Instance.ResetSlot(slotType);
		GameState.Current.Avatar.UnassignWeapon(slotType);
	}

	public bool EquipItemOnSlot(int itemId, LoadoutSlotType slotType)
	{
		InventoryItem item;
		if (TryGetInventoryItem(itemId, out item) && item.IsValid)
		{
			if (Singleton<LoadoutManager>.Instance.IsItemEquipped(itemId))
			{
				LoadoutSlotType slot;
				if (Singleton<LoadoutManager>.Instance.TryGetSlotForItem(item.Item, out slot))
				{
					EventHandler.Global.Fire(new ShopEvents.ShopHighlightSlot
					{
						SlotType = slot
					});
					Singleton<TemporaryLoadoutManager>.Instance.ResetLoadout(slot);
				}
			}
			else
			{
				HighlightItem(itemId, false);
				switch (item.Item.View.ItemType)
				{
				case UberstrikeItemType.Gear:
					slotType = ItemUtil.SlotFromItemClass(item.Item.View.ItemClass);
					Singleton<LoadoutManager>.Instance.SetLoadoutItem(slotType, item.Item);
					AutoMonoBehaviour<SfxManager>.Instance.PlayInGameAudioClip(GameAudio.EquipGear);
					if (GameState.Current.Avatar != null)
					{
						GameState.Current.Avatar.HideWeapons();
					}
					GameState.Current.Avatar.Decorator.AnimationController.TriggerGearAnimation(item.Item.View.ItemClass);
					break;
				case UberstrikeItemType.Weapon:
				{
					if (item.Item.View.ItemClass == UberstrikeItemClass.WeaponMelee)
					{
						slotType = LoadoutSlotType.WeaponMelee;
						Singleton<LoadoutManager>.Instance.RemoveDuplicateWeaponClass(item);
						Singleton<LoadoutManager>.Instance.SetLoadoutItem(slotType, item.Item);
						Singleton<LoadoutManager>.Instance.EquipWeapon(slotType, item.Item);
						AutoMonoBehaviour<SfxManager>.Instance.PlayInGameAudioClip(GameAudio.WeaponSwitch);
						break;
					}
					if (slotType == LoadoutSlotType.None || slotType == LoadoutSlotType.WeaponMelee)
					{
						slotType = GetNextFreeWeaponSlot();
					}
					LoadoutSlotType updatedSlot = slotType;
					if (Singleton<LoadoutManager>.Instance.RemoveDuplicateWeaponClass(item, ref updatedSlot) && slotType != updatedSlot)
					{
						Singleton<LoadoutManager>.Instance.SwapLoadoutItems(slotType, updatedSlot);
					}
					Singleton<LoadoutManager>.Instance.SetLoadoutItem(slotType, item.Item);
					Singleton<LoadoutManager>.Instance.EquipWeapon(slotType, item.Item);
					AutoMonoBehaviour<SfxManager>.Instance.PlayInGameAudioClip(GameAudio.EquipWeapon);
					break;
				}
				case UberstrikeItemType.QuickUse:
					EquipQuickItemOnSlot(item, slotType);
					break;
				case UberstrikeItemType.Functional:
					if (item.Item.Equippable)
					{
						if (slotType == LoadoutSlotType.None)
						{
							slotType = GetNextFreeFunctionalSlot();
						}
						LoadoutSlotType lastRemovedSlot = slotType;
						if (Singleton<LoadoutManager>.Instance.RemoveDuplicateFunctionalItemClass(item, ref lastRemovedSlot) && slotType != lastRemovedSlot)
						{
							Singleton<LoadoutManager>.Instance.SwapLoadoutItems(slotType, lastRemovedSlot);
						}
						Singleton<LoadoutManager>.Instance.SetLoadoutItem(slotType, item.Item);
						AutoMonoBehaviour<SfxManager>.Instance.PlayInGameAudioClip(GameAudio.EquipItem);
					}
					break;
				default:
					AutoMonoBehaviour<SfxManager>.Instance.PlayInGameAudioClip(GameAudio.EquipItem);
					Debug.LogError("Equip item of type: " + item.Item.View.ItemType);
					break;
				}
				Singleton<TemporaryLoadoutManager>.Instance.SetLoadoutItem(slotType, item.Item);
				EventHandler.Global.Fire(new ShopEvents.ShopHighlightSlot
				{
					SlotType = slotType
				});
			}
			return true;
		}
		return false;
	}

	private void EquipQuickItemOnSlot(InventoryItem item, LoadoutSlotType slotType)
	{
		if (slotType < LoadoutSlotType.QuickUseItem1 || slotType > LoadoutSlotType.QuickUseItem3)
		{
			slotType = GetNextFreeQuickItemSlot();
		}
		LoadoutSlotType lastRemovedSlot = slotType;
		if (slotType != lastRemovedSlot && Singleton<LoadoutManager>.Instance.RemoveDuplicateQuickItemClass(item.Item.View as UberStrikeItemQuickView, ref lastRemovedSlot))
		{
			Singleton<LoadoutManager>.Instance.SwapLoadoutItems(slotType, lastRemovedSlot);
		}
		AutoMonoBehaviour<SfxManager>.Instance.PlayInGameAudioClip(GameAudio.EquipItem);
		Singleton<LoadoutManager>.Instance.SetLoadoutItem(slotType, item.Item);
	}

	private LoadoutSlotType GetNextFreeWeaponSlot()
	{
		if (!Singleton<LoadoutManager>.Instance.HasLoadoutItem(LoadoutSlotType.WeaponPrimary))
		{
			return LoadoutSlotType.WeaponPrimary;
		}
		if (!Singleton<LoadoutManager>.Instance.HasLoadoutItem(LoadoutSlotType.WeaponSecondary))
		{
			return LoadoutSlotType.WeaponSecondary;
		}
		if (!Singleton<LoadoutManager>.Instance.HasLoadoutItem(LoadoutSlotType.WeaponTertiary))
		{
			return LoadoutSlotType.WeaponTertiary;
		}
		if (CurrentWeaponSlot == LoadoutSlotType.WeaponPrimary || CurrentWeaponSlot == LoadoutSlotType.WeaponSecondary || CurrentWeaponSlot == LoadoutSlotType.WeaponTertiary)
		{
			return CurrentWeaponSlot;
		}
		return LoadoutSlotType.WeaponPrimary;
	}

	private LoadoutSlotType GetNextFreeFunctionalSlot()
	{
		if (!Singleton<LoadoutManager>.Instance.HasLoadoutItem(LoadoutSlotType.FunctionalItem1))
		{
			return LoadoutSlotType.FunctionalItem1;
		}
		if (!Singleton<LoadoutManager>.Instance.HasLoadoutItem(LoadoutSlotType.FunctionalItem2))
		{
			return LoadoutSlotType.FunctionalItem2;
		}
		if (!Singleton<LoadoutManager>.Instance.HasLoadoutItem(LoadoutSlotType.FunctionalItem3))
		{
			return LoadoutSlotType.FunctionalItem3;
		}
		switch (CurrentFunctionalSlot)
		{
		case LoadoutSlotType.FunctionalItem1:
			return LoadoutSlotType.FunctionalItem2;
		case LoadoutSlotType.FunctionalItem2:
			return LoadoutSlotType.FunctionalItem3;
		case LoadoutSlotType.FunctionalItem3:
			return LoadoutSlotType.FunctionalItem1;
		default:
			return CurrentFunctionalSlot;
		}
	}

	private LoadoutSlotType GetNextFreeQuickItemSlot()
	{
		if (!Singleton<LoadoutManager>.Instance.HasLoadoutItem(LoadoutSlotType.QuickUseItem1))
		{
			return LoadoutSlotType.QuickUseItem1;
		}
		if (!Singleton<LoadoutManager>.Instance.HasLoadoutItem(LoadoutSlotType.QuickUseItem2))
		{
			return LoadoutSlotType.QuickUseItem2;
		}
		if (!Singleton<LoadoutManager>.Instance.HasLoadoutItem(LoadoutSlotType.QuickUseItem3))
		{
			return LoadoutSlotType.QuickUseItem3;
		}
		switch (CurrentQuickItemSot)
		{
		case LoadoutSlotType.QuickUseItem1:
			return LoadoutSlotType.QuickUseItem2;
		case LoadoutSlotType.QuickUseItem2:
			return LoadoutSlotType.QuickUseItem3;
		case LoadoutSlotType.QuickUseItem3:
			return LoadoutSlotType.QuickUseItem1;
		default:
			return CurrentQuickItemSot;
		}
	}

	public static LoadoutSlotType GetSlotTypeForGear(IUnityItem gearItem)
	{
		if (gearItem != null)
		{
			switch (gearItem.View.ItemClass)
			{
			case UberstrikeItemClass.GearHead:
				return LoadoutSlotType.GearHead;
			case UberstrikeItemClass.GearFace:
				return LoadoutSlotType.GearFace;
			case UberstrikeItemClass.GearUpperBody:
				return LoadoutSlotType.GearUpperBody;
			case UberstrikeItemClass.GearGloves:
				return LoadoutSlotType.GearGloves;
			case UberstrikeItemClass.GearLowerBody:
				return LoadoutSlotType.GearLowerBody;
			case UberstrikeItemClass.GearBoots:
				return LoadoutSlotType.GearBoots;
			case UberstrikeItemClass.GearHolo:
				return LoadoutSlotType.GearHolo;
			default:
				return LoadoutSlotType.None;
			}
		}
		return LoadoutSlotType.None;
	}

	public List<InventoryItem> GetAllItems(bool ignoreEquippedItems)
	{
		List<InventoryItem> list = new List<InventoryItem>();
		foreach (InventoryItem value in _inventoryItems.Values)
		{
			bool flag = value.DaysRemaining <= 0 && value.Item.View.Prices != null && value.Item.View.Prices.Count > 0;
			if (value.DaysRemaining <= 0 && !value.IsPermanent && !flag)
			{
				continue;
			}
			if (ignoreEquippedItems)
			{
				if (!Singleton<LoadoutManager>.Instance.IsItemEquipped(value.Item.View.ID))
				{
					list.Add(value);
				}
			}
			else
			{
				list.Add(value);
			}
		}
		return list;
	}

	public int GetGearItem(int itemID, UberstrikeItemClass itemClass)
	{
		InventoryItem value;
		if (_inventoryItems.TryGetValue(itemID, out value) && value != null && value.Item.View.ItemType == UberstrikeItemType.Gear)
		{
			return value.Item.View.ID;
		}
		IUnityItem item;
		if (Singleton<ItemManager>.Instance.TryGetDefaultItem(itemClass, out item))
		{
			return item.View.ID;
		}
		return 0;
	}

	public InventoryItem GetItem(int itemID)
	{
		InventoryItem value;
		if (_inventoryItems.TryGetValue(itemID, out value) && value != null)
		{
			return value;
		}
		return EmptyItem;
	}

	public InventoryItem GetWeaponItem(int itemId)
	{
		InventoryItem value;
		if (_inventoryItems.TryGetValue(itemId, out value) && value != null && value.Item.View.ItemType == UberstrikeItemType.Weapon)
		{
			return value;
		}
		return EmptyItem;
	}

	public bool TryGetInventoryItem(int itemID, out InventoryItem item)
	{
		return _inventoryItems.TryGetValue(itemID, out item) && item != null && item.Item != null;
	}

	public bool HasClanLicense()
	{
		return Contains(1234);
	}

	public bool IsItemValidForDays(InventoryItem item, int days)
	{
		return item != null && (item.DaysRemaining > days || item.IsPermanent);
	}

	public bool Contains(int itemId)
	{
		InventoryItem value;
		return _inventoryItems.TryGetValue(itemId, out value) && IsItemValidForDays(value, 0);
	}

	public void UpdateInventoryItems(List<ItemInventoryView> inventory)
	{
		if (Singleton<ItemManager>.Instance.ShopItemCount == 0)
		{
			Debug.LogWarning("Stopped updating inventory because shop is empty!");
			return;
		}
		HashSet<int> hashSet = new HashSet<int>(_inventoryItems.Keys);
		_inventoryItems.Clear();
		foreach (ItemInventoryView item in inventory)
		{
			IUnityItem itemInShop = Singleton<ItemManager>.Instance.GetItemInShop(item.ItemId);
			if (itemInShop != null && itemInShop.View.ID == item.ItemId)
			{
				IDictionary<int, InventoryItem> inventoryItems = _inventoryItems;
				int iD = itemInShop.View.ID;
				InventoryItem inventoryItem = new InventoryItem(itemInShop)
				{
					IsPermanent = !item.ExpirationDate.HasValue,
					AmountRemaining = item.AmountRemaining
				};
				DateTime? expirationDate = item.ExpirationDate;
				inventoryItem.ExpirationDate = ((!expirationDate.HasValue) ? DateTime.MinValue : expirationDate.Value);
				inventoryItem.IsHighlighted = hashSet.Count > 0 && !hashSet.Contains(itemInShop.View.ID);
				inventoryItems[iD] = inventoryItem;
			}
			else
			{
				Debug.LogWarning("Inventory Item not found: " + item.ItemId + " " + (itemInShop == null));
			}
		}
		this.OnInventoryUpdated();
	}

	internal void HighlightItem(int itemId, bool isHighlighted)
	{
		InventoryItem value;
		if (_inventoryItems.TryGetValue(itemId, out value) && value != null)
		{
			value.IsHighlighted = isHighlighted;
		}
	}

	public void EnableAllItems()
	{
		Debug.Log("PopulateCompleteInventory");
		_inventoryItems.Clear();
		foreach (IUnityItem shopItem in Singleton<ItemManager>.Instance.ShopItems)
		{
			_inventoryItems.Add(shopItem.View.ID, new InventoryItem(shopItem)
			{
				IsPermanent = true,
				AmountRemaining = 0,
				ExpirationDate = DateTime.MaxValue
			});
		}
	}
}
