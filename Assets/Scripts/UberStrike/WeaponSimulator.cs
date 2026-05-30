using System.Collections.Generic;
using UberStrike.Core.Models;
using UberStrike.Core.Types;
using UberStrike.Realtime.UnitySdk;
using UnityEngine;

public class WeaponSimulator : IWeaponController
{
	private CharacterConfig character;

	private float _nextShootTime;

	private WeaponSlot _currentSlot;

	private WeaponSlot[] weaponSlots;

	private int _projectileId;

	public Vector3 StandingOffset = new Vector3(0f, 0.65f, 0f);

	public Vector3 CrouchingOffset = new Vector3(0f, 0.1f, 0f);

	public bool IsLocal
	{
		get
		{
			return false;
		}
	}

	public int CurrentSlotIndex { get; private set; }

	public int Cmid
	{
		get
		{
			return character.State.Player.Cmid;
		}
	}

	public byte PlayerNumber
	{
		get
		{
			return character.State.Player.PlayerId;
		}
	}

	public WeaponSimulator(CharacterConfig character)
	{
		this.character = character;
		weaponSlots = new WeaponSlot[4];
		CurrentSlotIndex = -1;
	}

	public Vector3 ShootingPoint(ICharacterState state)
	{
		Vector3 vector = (((character.State.MovementState & MoveStates.Ducked) != MoveStates.None) ? CrouchingOffset : StandingOffset);
		return state.Position + vector;
	}

	public void Update()
	{
		if (character.Avatar != null && character.State != null && character.State.Player.IsAlive && !character.IsLocal && character.State.Player.IsFiring)
		{
			Shoot(character.State);
		}
	}

	public void Shoot(ICharacterState state)
	{
		if (state != null && _nextShootTime < Time.time && _currentSlot != null && (!(_currentSlot.Logic is ProjectileWeapon) || _currentSlot.View.ItemClass == UberstrikeItemClass.WeaponSplattergun))
		{
			_nextShootTime = Time.time + WeaponConfigurationHelper.GetRateOfFire(_currentSlot.View);
			BeginShooting();
			CmunePairList<BaseGameProp, ShotPoint> hits;
			_currentSlot.Logic.Shoot(new Ray(ShootingPoint(state) + GameState.Current.Player.EyePosition, ShootingDirection(state)), out hits);
			EndShooting();
		}
	}

	public IProjectile EmitProjectile(int actorID, byte playerNumber, Vector3 origin, Vector3 direction, LoadoutSlotType slot, int projectileId, bool explode)
	{
		IProjectile result = null;
		if (character.Avatar.Decorator != null)
		{
			character.Avatar.Decorator.AnimationController.Shoot();
		}
		BeginShooting();
		switch (slot)
		{
		case LoadoutSlotType.WeaponPrimary:
			result = ShootProjectileFromSlot(1, origin, direction, projectileId, explode, actorID);
			break;
		case LoadoutSlotType.WeaponSecondary:
			result = ShootProjectileFromSlot(2, origin, direction, projectileId, explode, actorID);
			break;
		case LoadoutSlotType.WeaponTertiary:
			result = ShootProjectileFromSlot(3, origin, direction, projectileId, explode, actorID);
			break;
		}
		EndShooting();
		return result;
	}

	private void BeginShooting()
	{
		CharacterHitArea[] hitAreas = character.Avatar.Decorator.HitAreas;
		foreach (CharacterHitArea characterHitArea in hitAreas)
		{
			characterHitArea.gameObject.layer = 2;
		}
	}

	private void EndShooting()
	{
		CharacterHitArea[] hitAreas = character.Avatar.Decorator.HitAreas;
		foreach (CharacterHitArea characterHitArea in hitAreas)
		{
			characterHitArea.gameObject.layer = character.Avatar.Decorator.gameObject.layer;
		}
	}

	private IProjectile ShootProjectileFromSlot(int slot, Vector3 origin, Vector3 direction, int projectileID, bool explode, int actorID)
	{
		if (weaponSlots.Length > slot && weaponSlots[slot] != null)
		{
			ProjectileWeapon projectileWeapon = weaponSlots[slot].Logic as ProjectileWeapon;
			if (projectileWeapon != null)
			{
				projectileWeapon.Decorator.PlayShootSound();
				if (!explode)
				{
					return projectileWeapon.EmitProjectile(new Ray(origin, direction), projectileID, actorID);
				}
				projectileWeapon.ShowExplosionEffect(origin, Vector3.up, direction, projectileID);
			}
		}
		return null;
	}

	public void UpdateWeaponSlot(int slotIndex, bool showWeapon)
	{
		CurrentSlotIndex = slotIndex;
		switch (slotIndex)
		{
		case 0:
			_currentSlot = weaponSlots[0];
			if (showWeapon)
			{
				character.Avatar.ShowWeapon(LoadoutSlotType.WeaponMelee);
			}
			break;
		case 1:
			_currentSlot = weaponSlots[1];
			if (showWeapon)
			{
				character.Avatar.ShowWeapon(LoadoutSlotType.WeaponPrimary);
			}
			break;
		case 2:
			_currentSlot = weaponSlots[2];
			if (showWeapon)
			{
				character.Avatar.ShowWeapon(LoadoutSlotType.WeaponSecondary);
			}
			break;
		case 3:
			_currentSlot = weaponSlots[3];
			if (showWeapon)
			{
				character.Avatar.ShowWeapon(LoadoutSlotType.WeaponTertiary);
			}
			break;
		}
	}

	public void UpdateWeapons(int currentWeaponSlot, IList<int> weaponItemIds)
	{
		if (character.Avatar == null)
		{
			return;
		}
		IUnityItem[] array = new IUnityItem[4]
		{
			Singleton<ItemManager>.Instance.GetItemInShop((weaponItemIds != null && weaponItemIds.Count > 0) ? weaponItemIds[0] : 0),
			Singleton<ItemManager>.Instance.GetItemInShop((weaponItemIds != null && weaponItemIds.Count > 1) ? weaponItemIds[1] : 0),
			Singleton<ItemManager>.Instance.GetItemInShop((weaponItemIds != null && weaponItemIds.Count > 2) ? weaponItemIds[2] : 0),
			Singleton<ItemManager>.Instance.GetItemInShop((weaponItemIds != null && weaponItemIds.Count > 3) ? weaponItemIds[3] : 0)
		};
		LoadoutSlotType[] array2 = new LoadoutSlotType[4]
		{
			LoadoutSlotType.WeaponMelee,
			LoadoutSlotType.WeaponPrimary,
			LoadoutSlotType.WeaponSecondary,
			LoadoutSlotType.WeaponTertiary
		};
		int num = -1;
		for (int i = 0; i < weaponSlots.Length; i++)
		{
			if (weaponSlots[i] != null && weaponSlots[i].Decorator != null)
			{
				Object.Destroy(weaponSlots[i].Decorator.gameObject);
			}
			if (array[i] != null && (bool)character.Avatar.Decorator.WeaponAttachPoint)
			{
				WeaponSlot weaponSlot = new WeaponSlot(array2[i], array[i], character.Avatar.Decorator.WeaponAttachPoint, this);
				if ((bool)weaponSlot.Decorator)
				{
					if (num < 0)
					{
						num = i;
					}
					character.Avatar.AssignWeapon(array2[i], weaponSlot.Decorator, weaponSlot.UnityItem);
				}
				else
				{
					Debug.LogError("WeaponDecorator is NULL!");
				}
				weaponSlots[i] = weaponSlot;
			}
			else
			{
				weaponSlots[i] = null;
			}
		}
		if (CurrentSlotIndex >= 0 && weaponSlots[CurrentSlotIndex] != null && weaponSlots[CurrentSlotIndex].Decorator != null)
		{
			weaponSlots[CurrentSlotIndex].Decorator.IsEnabled = true;
		}
	}

	public void UpdateWeaponDecorator(IUnityItem item)
	{
		UpdateWeapons(character.State.Player.CurrentWeaponSlot, character.State.Player.Weapons);
	}

	public int NextProjectileId()
	{
		return ProjectileManager.CreateGlobalProjectileID(PlayerNumber, ++_projectileId);
	}

	public Vector3 ShootingDirection(ICharacterState state)
	{
		return Quaternion.Euler(state.VerticalRotation, state.HorizontalRotation.eulerAngles.y, 0f) * Vector3.forward;
	}
}
