using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UberStrike.Core.Types;
using UnityEngine;

public class Loadout
{
	private Dictionary<LoadoutSlotType, IUnityItem> _items = new Dictionary<LoadoutSlotType, IUnityItem>();

	public int ItemCount
	{
		get
		{
			return _items.Count;
		}
	}

	public static Loadout Empty
	{
		get
		{
			return new Loadout(new Dictionary<LoadoutSlotType, IUnityItem>());
		}
	}

	public event Action OnGearChanged = delegate
	{
	};

	public event Action<LoadoutSlotType> OnWeaponChanged = delegate
	{
	};

	public Loadout(Loadout gearLoadout)
		: this(gearLoadout._items)
	{
	}

	public Loadout(Dictionary<LoadoutSlotType, IUnityItem> items)
	{
		foreach (KeyValuePair<LoadoutSlotType, IUnityItem> item in items)
		{
			SetSlot(item.Key, item.Value);
		}
	}

	public Loadout(List<int> gearItemIds, List<int> weaponItemIds)
	{
		if (gearItemIds.Count < 7 || weaponItemIds.Count < 4)
		{
			Debug.LogError("Invalid parameters: gear count = " + gearItemIds.Count + " weapon count = " + weaponItemIds.Count);
		}
		SetSlot(LoadoutSlotType.GearHead, gearItemIds[1]);
		SetSlot(LoadoutSlotType.GearFace, gearItemIds[2]);
		SetSlot(LoadoutSlotType.GearGloves, gearItemIds[3]);
		SetSlot(LoadoutSlotType.GearUpperBody, gearItemIds[4]);
		SetSlot(LoadoutSlotType.GearLowerBody, gearItemIds[5]);
		SetSlot(LoadoutSlotType.GearBoots, gearItemIds[6]);
		if (gearItemIds[0] > 0)
		{
			SetSlot(LoadoutSlotType.GearHolo, gearItemIds[0]);
		}
		SetSlot(LoadoutSlotType.WeaponMelee, weaponItemIds[0]);
		SetSlot(LoadoutSlotType.WeaponPrimary, weaponItemIds[1]);
		SetSlot(LoadoutSlotType.WeaponSecondary, weaponItemIds[2]);
		SetSlot(LoadoutSlotType.WeaponTertiary, weaponItemIds[3]);
	}

	public bool TryGetItem(LoadoutSlotType slot, out IUnityItem item)
	{
		return _items.TryGetValue(slot, out item);
	}

	public void SetSlot(LoadoutSlotType slot, int itemId)
	{
		SetSlot(slot, Singleton<ItemManager>.Instance.GetItemInShop(itemId));
	}

	public void SetSlot(LoadoutSlotType slot, IUnityItem item)
	{
		if (item != null && CanGoInSlot(slot, item.View.ItemType))
		{
			_items[slot] = item;
			switch (slot)
			{
			case LoadoutSlotType.GearHead:
			case LoadoutSlotType.GearFace:
			case LoadoutSlotType.GearGloves:
			case LoadoutSlotType.GearUpperBody:
			case LoadoutSlotType.GearLowerBody:
			case LoadoutSlotType.GearBoots:
			case LoadoutSlotType.GearHolo:
				this.OnGearChanged();
				break;
			case LoadoutSlotType.WeaponMelee:
			case LoadoutSlotType.WeaponPrimary:
			case LoadoutSlotType.WeaponSecondary:
			case LoadoutSlotType.WeaponTertiary:
				this.OnWeaponChanged(slot);
				break;
			}
		}
	}

	public bool CanGoInSlot(LoadoutSlotType slot, UberstrikeItemType type)
	{
		switch (type)
		{
		case UberstrikeItemType.Functional:
			return slot >= LoadoutSlotType.FunctionalItem1 && slot <= LoadoutSlotType.FunctionalItem3;
		case UberstrikeItemType.Gear:
			return slot >= LoadoutSlotType.GearHead && slot <= LoadoutSlotType.GearHolo;
		case UberstrikeItemType.QuickUse:
			return slot >= LoadoutSlotType.QuickUseItem1 && slot <= LoadoutSlotType.QuickUseItem3;
		case UberstrikeItemType.Weapon:
			return slot >= LoadoutSlotType.WeaponMelee && slot <= LoadoutSlotType.WeaponTertiary;
		default:
			Debug.LogError("Item attempted to be equipped into a slot that isn't supported.");
			return false;
		}
	}

	public void ClearSlot(LoadoutSlotType slot)
	{
		IUnityItem value;
		if (_items.TryGetValue(slot, out value))
		{
			_items.Remove(slot);
			this.OnGearChanged();
		}
	}

	public void ClearAllSlots()
	{
	}

	public bool Compare(Loadout a)
	{
		bool flag = ItemCount == a.ItemCount;
		if (flag)
		{
			foreach (KeyValuePair<LoadoutSlotType, IUnityItem> item2 in _items)
			{
				IUnityItem item;
				if (a.TryGetItem(item2.Key, out item))
				{
					if (item != item2.Value)
					{
						return false;
					}
					continue;
				}
				return false;
			}
		}
		return flag;
	}

	public LoadoutSlotType GetItemClassSlotType(UberstrikeItemClass itemClass)
	{
		foreach (KeyValuePair<LoadoutSlotType, IUnityItem> item in _items)
		{
			if (item.Value.View.ItemClass == itemClass)
			{
				return item.Key;
			}
		}
		return LoadoutSlotType.None;
	}

	public LoadoutSlotType GetFirstEmptyWeaponSlot()
	{
		if (!_items.ContainsKey(LoadoutSlotType.WeaponPrimary))
		{
			return LoadoutSlotType.WeaponPrimary;
		}
		if (!_items.ContainsKey(LoadoutSlotType.WeaponSecondary))
		{
			return LoadoutSlotType.WeaponSecondary;
		}
		if (!_items.ContainsKey(LoadoutSlotType.WeaponTertiary))
		{
			return LoadoutSlotType.WeaponTertiary;
		}
		return LoadoutSlotType.None;
	}

	public bool Contains(string prefabName)
	{
		bool result = false;
		foreach (IUnityItem value in _items.Values)
		{
			if (value.View.PrefabName.Equals(prefabName))
			{
				result = true;
				break;
			}
		}
		return result;
	}

	public bool Contains(int itemId)
	{
		bool result = false;
		foreach (IUnityItem value in _items.Values)
		{
			if (value.View.ID == itemId)
			{
				result = true;
				break;
			}
		}
		return result;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<LoadoutSlotType, IUnityItem> item in _items)
		{
			stringBuilder.AppendLine(string.Format("{0}: {1}", item.Key, item.Value.Name));
		}
		return stringBuilder.ToString();
	}

	public Dictionary<LoadoutSlotType, IUnityItem>.Enumerator GetEnumerator()
	{
		return _items.GetEnumerator();
	}

	private void OnItemPrefabUpdated(IUnityItem item)
	{
		KeyValuePair<LoadoutSlotType, IUnityItem> keyValuePair = _items.FirstOrDefault((KeyValuePair<LoadoutSlotType, IUnityItem> a) => a.Value.View.ID == item.View.ID);
		if (keyValuePair.Value != null)
		{
			LoadoutSlotType key = keyValuePair.Key;
			IUnityItem value;
			if (_items.TryGetValue(key, out value) && value == item)
			{
				switch (item.View.ItemType)
				{
				case UberstrikeItemType.Gear:
					CheckAllGear();
					break;
				case UberstrikeItemType.Weapon:
					this.OnWeaponChanged(key);
					break;
				case UberstrikeItemType.QuickUse:
					break;
				case UberstrikeItemType.WeaponMod:
					break;
				}
			}
		}
		else
		{
			Debug.LogError("OnItemPrefabUpdated failed because slot not found");
		}
	}

	private void CheckAllGear()
	{
		bool flag = false;
		IUnityItem value;
		if (_items.TryGetValue(LoadoutSlotType.GearHolo, out value))
		{
			flag = value.IsLoaded;
		}
		else
		{
			bool flag2 = true;
			LoadoutSlotType[] gearSlots = LoadoutManager.GearSlots;
			foreach (LoadoutSlotType key in gearSlots)
			{
				if (_items.TryGetValue(key, out value))
				{
					flag2 &= value.IsLoaded;
				}
			}
			flag = flag2;
		}
		if (flag)
		{
			this.OnGearChanged();
		}
	}

	public AvatarGearParts GetAvatarGear()
	{
		bool flag = false;
		AvatarGearParts avatarGearParts = new AvatarGearParts();
		IUnityItem value;
		if (_items.TryGetValue(LoadoutSlotType.GearHolo, out value))
		{
			avatarGearParts.Base = value.Create(Vector3.zero, Quaternion.identity);
		}
		if (!avatarGearParts.Base)
		{
			flag = true;
			avatarGearParts.Base = UnityEngine.Object.Instantiate(PrefabManager.Instance.DefaultAvatar.gameObject) as GameObject;
		}
		if (flag)
		{
			LoadoutSlotType[] gearSlots = LoadoutManager.GearSlots;
			foreach (LoadoutSlotType loadoutSlotType in gearSlots)
			{
				if (_items.TryGetValue(loadoutSlotType, out value))
				{
					GameObject gameObject = value.Create(Vector3.zero, Quaternion.identity);
					if ((bool)gameObject)
					{
						avatarGearParts.Parts.Add(gameObject);
					}
					continue;
				}
				GameObject defaultGearItem = Singleton<ItemManager>.Instance.GetDefaultGearItem(ItemUtil.ItemClassFromSlot(loadoutSlotType));
				if ((bool)defaultGearItem)
				{
					GameObject gameObject2 = UnityEngine.Object.Instantiate(defaultGearItem) as GameObject;
					if ((bool)gameObject2)
					{
						avatarGearParts.Parts.Add(gameObject2);
					}
				}
			}
		}
		return avatarGearParts;
	}

	public AvatarGearParts GetRagdollGear()
	{
		bool flag = false;
		AvatarGearParts avatarGearParts = new AvatarGearParts();
		try
		{
			IUnityItem value;
			if (_items.TryGetValue(LoadoutSlotType.GearHolo, out value))
			{
				flag = !value.IsLoaded;
				if ((bool)value.Prefab)
				{
					HoloGearItem component = value.Prefab.GetComponent<HoloGearItem>();
					if ((bool)component && (bool)component.Configuration.Ragdoll)
					{
						avatarGearParts.Base = UnityEngine.Object.Instantiate(component.Configuration.Ragdoll.gameObject) as GameObject;
					}
					else
					{
						avatarGearParts.Base = UnityEngine.Object.Instantiate(PrefabManager.Instance.DefaultRagdoll.gameObject) as GameObject;
					}
				}
				else
				{
					avatarGearParts.Base = UnityEngine.Object.Instantiate(PrefabManager.Instance.DefaultRagdoll.gameObject) as GameObject;
				}
			}
			else
			{
				flag = true;
				avatarGearParts.Base = UnityEngine.Object.Instantiate(PrefabManager.Instance.DefaultRagdoll.gameObject) as GameObject;
			}
			if (flag)
			{
				LoadoutSlotType[] gearSlots = LoadoutManager.GearSlots;
				foreach (LoadoutSlotType loadoutSlotType in gearSlots)
				{
					if (_items.TryGetValue(loadoutSlotType, out value))
					{
						GameObject gameObject = value.Create(Vector3.zero, Quaternion.identity);
						if ((bool)gameObject)
						{
							avatarGearParts.Parts.Add(gameObject);
						}
					}
					else if (Singleton<ItemManager>.Instance.TryGetDefaultItem(ItemUtil.ItemClassFromSlot(loadoutSlotType), out value))
					{
						GameObject gameObject2 = value.Create(Vector3.zero, Quaternion.identity);
						if ((bool)gameObject2)
						{
							avatarGearParts.Parts.Add(gameObject2);
						}
					}
				}
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		return avatarGearParts;
	}

	internal void UpdateWeaponSlots(List<int> weaponItemIds)
	{
		SetSlot(LoadoutSlotType.WeaponMelee, weaponItemIds[0]);
		SetSlot(LoadoutSlotType.WeaponPrimary, weaponItemIds[1]);
		SetSlot(LoadoutSlotType.WeaponSecondary, weaponItemIds[2]);
		SetSlot(LoadoutSlotType.WeaponTertiary, weaponItemIds[3]);
	}

	internal void UpdateGearSlots(List<int> gearItemIds)
	{
		SetSlot(LoadoutSlotType.GearHead, gearItemIds[1]);
		SetSlot(LoadoutSlotType.GearFace, gearItemIds[2]);
		SetSlot(LoadoutSlotType.GearGloves, gearItemIds[3]);
		SetSlot(LoadoutSlotType.GearUpperBody, gearItemIds[4]);
		SetSlot(LoadoutSlotType.GearLowerBody, gearItemIds[5]);
		SetSlot(LoadoutSlotType.GearBoots, gearItemIds[6]);
		if (gearItemIds[0] > 0)
		{
			SetSlot(LoadoutSlotType.GearHolo, gearItemIds[0]);
		}
	}
}
