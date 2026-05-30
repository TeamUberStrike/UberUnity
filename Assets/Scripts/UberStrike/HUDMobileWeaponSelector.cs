using System.Collections.Generic;
using UberStrike.Core.Types;
using UnityEngine;

public class HUDMobileWeaponSelector : MonoBehaviour
{
	[SerializeField]
	private NGUIScrollList scrollList;

	[SerializeField]
	private GameObject melee;

	[SerializeField]
	private GameObject machinegun;

	[SerializeField]
	private GameObject shotgun;

	[SerializeField]
	private GameObject sniper;

	[SerializeField]
	private GameObject cannon;

	[SerializeField]
	private GameObject splattergun;

	[SerializeField]
	private GameObject launcher;

	[SerializeField]
	private GameObject meleeSlot;

	[SerializeField]
	private GameObject primarySlot;

	[SerializeField]
	private GameObject secondarySlot;

	[SerializeField]
	private GameObject tertiarySlot;

	[SerializeField]
	private GameObject disableSlot;

	[SerializeField]
	private GameObject selectorBackground;

	private Dictionary<GameObject, LoadoutSlotType> slots = new Dictionary<GameObject, LoadoutSlotType>();

	private Dictionary<UberstrikeItemClass, GameObject> weapons = new Dictionary<UberstrikeItemClass, GameObject>();

	private void OnEnable()
	{
		GameState.Current.PlayerData.LoadoutWeapons.Fire();
		GameState.Current.PlayerData.ActiveWeapon.Fire();
	}

	private void Start()
	{
		AutoMonoBehaviour<TouchInput>.Instance.Shooter.IgnoreRect(new Rect((float)Screen.width - 240f, 0f, 240f, 200f));
		slots.Add(meleeSlot, LoadoutSlotType.WeaponMelee);
		slots.Add(primarySlot, LoadoutSlotType.WeaponPrimary);
		slots.Add(secondarySlot, LoadoutSlotType.WeaponSecondary);
		slots.Add(tertiarySlot, LoadoutSlotType.WeaponTertiary);
		weapons.Add(UberstrikeItemClass.WeaponMelee, melee);
		weapons.Add(UberstrikeItemClass.WeaponMachinegun, machinegun);
		weapons.Add(UberstrikeItemClass.WeaponShotgun, shotgun);
		weapons.Add(UberstrikeItemClass.WeaponSniperRifle, sniper);
		weapons.Add(UberstrikeItemClass.WeaponCannon, cannon);
		weapons.Add(UberstrikeItemClass.WeaponSplattergun, splattergun);
		weapons.Add(UberstrikeItemClass.WeaponLauncher, launcher);
		scrollList.SelectedElement.AddEvent(delegate(GameObject el)
		{
			if (el != null && slots.ContainsKey(el))
			{
				GameInputKey key;
				switch (slots[el])
				{
				case LoadoutSlotType.WeaponMelee:
					key = GameInputKey.WeaponMelee;
					break;
				case LoadoutSlotType.WeaponPrimary:
					key = GameInputKey.Weapon1;
					break;
				case LoadoutSlotType.WeaponSecondary:
					key = GameInputKey.Weapon2;
					break;
				case LoadoutSlotType.WeaponTertiary:
					key = GameInputKey.Weapon3;
					break;
				default:
					Debug.LogError("Cannot switch to unknown slot!");
					return;
				}
				EventHandler.Global.Fire(new GlobalEvents.InputChanged(key, 1f));
			}
		}, this);
		GameState.Current.PlayerData.LoadoutWeapons.AddEventAndFire(LoadWeaponList, this);
		GameState.Current.PlayerData.ActiveWeapon.AddEventAndFire(delegate(WeaponSlot el)
		{
			if (el != null)
			{
				scrollList.SpringToElement(ElementAtSlot(el.Slot));
			}
		}, this);
	}

	private void LoadWeaponList(Dictionary<LoadoutSlotType, IUnityItem> loadoutWeapons)
	{
		if (loadoutWeapons == null)
		{
			return;
		}
		foreach (GameObject value in weapons.Values)
		{
			UnloadWeapon(value);
		}
		List<GameObject> list = new List<GameObject>();
		foreach (KeyValuePair<LoadoutSlotType, IUnityItem> loadoutWeapon in loadoutWeapons)
		{
			GameObject gameObject = ElementAtSlot(loadoutWeapon.Key);
			LoadWeapon(loadoutWeapon.Value.View.ItemClass, gameObject);
			if (loadoutWeapon.Value != null && gameObject != null)
			{
				list.Add(gameObject);
			}
		}
		scrollList.SetActiveElements(list);
	}

	private void UnloadWeapon(GameObject weapon)
	{
		weapon.transform.parent = disableSlot.transform;
	}

	private void LoadWeapon(UberstrikeItemClass weaponClass, GameObject slot)
	{
		if (weapons.ContainsKey(weaponClass))
		{
			weapons[weaponClass].transform.parent = slot.transform;
			weapons[weaponClass].transform.localPosition = Vector3.zero;
		}
	}

	private GameObject ElementAtSlot(LoadoutSlotType slot)
	{
		switch (slot)
		{
		case LoadoutSlotType.WeaponMelee:
			return meleeSlot;
		case LoadoutSlotType.WeaponPrimary:
			return primarySlot;
		case LoadoutSlotType.WeaponSecondary:
			return secondarySlot;
		case LoadoutSlotType.WeaponTertiary:
			return tertiarySlot;
		default:
			return null;
		}
	}

	public void Show(bool show)
	{
		scrollList.Panel.panel.alpha = (show ? 1 : 0);
		selectorBackground.SetActive(show);
	}
}
