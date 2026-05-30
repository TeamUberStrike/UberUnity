using System;
using System.Collections.Generic;
using UberStrike.Core.Types;
using UnityEngine;

public class Avatar
{
	private bool _isLocal;

	private Dictionary<LoadoutSlotType, BaseWeaponDecorator> _weapons;

	public Loadout Loadout { get; private set; }

	public AvatarDecorator Decorator { get; private set; }

	public AvatarDecoratorConfig Ragdoll { get; private set; }

	public LoadoutSlotType CurrentWeaponSlot { get; private set; }

	public event Action OnDecoratorChanged = delegate
	{
	};

	public Avatar(Loadout loadout, bool local)
	{
		_isLocal = local;
		_weapons = new Dictionary<LoadoutSlotType, BaseWeaponDecorator>();
		SetLoadout(loadout);
	}

	public void SetDecorator(AvatarDecorator decorator)
	{
		Decorator = decorator;
		this.OnDecoratorChanged();
	}

	public void SetLoadout(Loadout loadout)
	{
		if (Loadout != null)
		{
			Loadout.ClearAllSlots();
			Loadout.OnGearChanged -= RebuildDecorator;
			Loadout.OnWeaponChanged -= UpdateWeapon;
		}
		Loadout = loadout;
		Loadout.OnGearChanged += RebuildDecorator;
		Loadout.OnWeaponChanged += UpdateWeapon;
		RebuildDecorator();
	}

	public void RebuildDecorator()
	{
		if ((bool)Decorator)
		{
			AvatarGearParts avatarGear = Loadout.GetAvatarGear();
			if (_isLocal)
			{
				AvatarBuilder.UpdateLocalAvatar(avatarGear);
			}
			else
			{
				SetDecorator(AvatarBuilder.UpdateRemoteAvatar(Decorator, avatarGear, Color.white));
			}
		}
	}

	public void CleanupRagdoll()
	{
		if ((bool)Ragdoll)
		{
			AvatarBuilder.Destroy(Ragdoll.gameObject);
			Ragdoll = null;
			if ((bool)Decorator && (bool)Decorator.gameObject)
			{
				Decorator.gameObject.SetActive(true);
			}
		}
	}

	public void SpawnRagdoll(DamageInfo damageInfo)
	{
		AvatarGearParts ragdollGear = Loadout.GetRagdollGear();
		Ragdoll = AvatarBuilder.InstantiateRagdoll(ragdollGear, Decorator.Configuration.GetSkinColor());
		try
		{
			ragdollGear.DestroyGearParts();
			Ragdoll.transform.position = Decorator.transform.position;
			Ragdoll.transform.rotation = Decorator.transform.rotation;
			AvatarDecoratorConfig.CopyBones(Decorator.Configuration, Ragdoll);
			ArrowProjectile[] componentsInChildren = Decorator.GetComponentsInChildren<ArrowProjectile>(true);
			foreach (ArrowProjectile arrowProjectile in componentsInChildren)
			{
				Vector3 localPosition = arrowProjectile.transform.localPosition;
				Quaternion localRotation = arrowProjectile.transform.localRotation;
				arrowProjectile.transform.parent = Ragdoll.GetBone(BoneIndex.Hips);
				arrowProjectile.transform.localPosition = localPosition;
				arrowProjectile.transform.localRotation = localRotation;
			}
			Rigidbody[] componentsInChildren2 = Ragdoll.GetComponentsInChildren<Rigidbody>(true);
			foreach (Rigidbody rigidbody in componentsInChildren2)
			{
				if (rigidbody.gameObject.GetComponent<RagdollBodyPart>() == null)
				{
					rigidbody.gameObject.AddComponent<RagdollBodyPart>();
				}
			}
			Ragdoll.ApplyDamageToRagdoll(damageInfo);
			Decorator.gameObject.SetActive(false);
		}
		catch (Exception message)
		{
			Debug.LogWarning(message);
		}
	}

	public void UpdateAllWeapons()
	{
		UpdateWeapon(LoadoutSlotType.WeaponMelee);
		UpdateWeapon(LoadoutSlotType.WeaponPrimary);
		UpdateWeapon(LoadoutSlotType.WeaponSecondary);
		UpdateWeapon(LoadoutSlotType.WeaponTertiary);
	}

	private void UpdateWeapon(LoadoutSlotType slot)
	{
		IUnityItem item;
		if (Loadout.TryGetItem(slot, out item) && (bool)Decorator && (bool)Decorator.WeaponAttachPoint)
		{
			GameObject gameObject = item.Create(Decorator.WeaponAttachPoint.position, Decorator.WeaponAttachPoint.rotation);
			if ((bool)gameObject)
			{
				AssignWeapon(slot, gameObject.GetComponent<BaseWeaponDecorator>(), item);
			}
		}
	}

	public void AssignWeapon(LoadoutSlotType slot, BaseWeaponDecorator weapon, IUnityItem item)
	{
		if ((bool)weapon)
		{
			BaseWeaponDecorator value;
			if (_weapons.TryGetValue(slot, out value) && (bool)value)
			{
				UnityEngine.Object.Destroy(value.gameObject);
			}
			_weapons[slot] = weapon;
			weapon.transform.parent = Decorator.WeaponAttachPoint;
			Transform[] componentsInChildren = weapon.gameObject.transform.GetComponentsInChildren<Transform>(true);
			foreach (Transform transform in componentsInChildren)
			{
				if (transform.gameObject.name == "Head")
				{
					transform.gameObject.name = "WeaponHead";
				}
			}
			LayerUtil.SetLayerRecursively(weapon.gameObject.transform, Decorator.gameObject.layer);
			weapon.transform.localPosition = Vector3.zero;
			weapon.transform.localRotation = Quaternion.identity;
			weapon.IsEnabled = slot == CurrentWeaponSlot;
			weapon.WeaponClass = item.View.ItemClass;
		}
		else
		{
			UnassignWeapon(slot);
		}
	}

	public void UnassignWeapon(LoadoutSlotType slot)
	{
		CurrentWeaponSlot = slot;
		BaseWeaponDecorator value;
		if (_weapons.TryGetValue(slot, out value) && (bool)value)
		{
			UnityEngine.Object.Destroy(value.gameObject);
		}
		_weapons.Remove(slot);
	}

	public void ShowWeapon(LoadoutSlotType slot)
	{
		CurrentWeaponSlot = slot;
		foreach (KeyValuePair<LoadoutSlotType, BaseWeaponDecorator> weapon in _weapons)
		{
			if ((bool)weapon.Value)
			{
				weapon.Value.IsEnabled = slot == weapon.Key;
				if (slot == weapon.Key)
				{
					Decorator.AnimationController.ChangeWeaponType(weapon.Value.WeaponClass);
				}
			}
		}
	}

	public void ShowFirstWeapon()
	{
		foreach (KeyValuePair<LoadoutSlotType, BaseWeaponDecorator> weapon in _weapons)
		{
			if ((bool)weapon.Value)
			{
				ShowWeapon(weapon.Key);
				break;
			}
		}
	}

	public void HideWeapons()
	{
		foreach (BaseWeaponDecorator value in _weapons.Values)
		{
			if ((bool)value)
			{
				value.IsEnabled = false;
			}
		}
		Decorator.AnimationController.ChangeWeaponType((UberstrikeItemClass)0);
	}
}
