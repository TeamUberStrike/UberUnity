using System;
using System.Collections;
using System.Collections.Generic;
using UberStrike.Core.Models.Views;
using UberStrike.Core.Types;
using UberStrike.Realtime.UnitySdk;
using UnityEngine;

public class WeaponController : Singleton<WeaponController>, IWeaponController
{
	private const float _upwardsForceMultiplier = 10f;

	public Property<LoadoutSlotType> SelectedLoadout = new Property<LoadoutSlotType>();

	private Dictionary<LoadoutSlotType, IUnityItem> _loadoutWeapons = new Dictionary<LoadoutSlotType, IUnityItem>();

	private WeaponSlot[] _weapons;

	private float _holsterTime;

	private CircularInteger _currentSlotID;

	private WeaponSlot _weapon_current;

	private int _projectileId;

	public Dictionary<LoadoutSlotType, IUnityItem> LoadoutWeapons
	{
		get
		{
			return _loadoutWeapons;
		}
	}

	private WeaponSlot _currentSlot
	{
		get
		{
			return _weapon_current;
		}
		set
		{
			_weapon_current = value;
			GameState.Current.PlayerData.ActiveWeapon.Value = _weapon_current;
		}
	}

	public byte PlayerNumber
	{
		get
		{
			return GameState.Current.PlayerData.Player.PlayerId;
		}
	}

	public int Cmid
	{
		get
		{
			return PlayerDataManager.Cmid;
		}
	}

	public bool IsLocal
	{
		get
		{
			return true;
		}
	}

	public bool HasAnyWeapon
	{
		get
		{
			WeaponSlot[] weapons = _weapons;
			foreach (WeaponSlot weaponSlot in weapons)
			{
				if (weaponSlot != null)
				{
					return true;
				}
			}
			return false;
		}
	}

	public BaseWeaponDecorator CurrentWeapon
	{
		get
		{
			if (IsWeaponValid)
			{
				return _currentSlot.Decorator;
			}
			return null;
		}
	}

	public bool IsWeaponValid
	{
		get
		{
			return _currentSlot != null && _currentSlot.Logic != null && _currentSlot.Decorator != null;
		}
	}

	public bool IsWeaponReady
	{
		get
		{
			return IsWeaponValid && _currentSlot.InputHandler.FireHandler.CanShoot && _currentSlot.Logic.IsWeaponActive;
		}
	}

	public bool IsSecondaryAction
	{
		get
		{
			return _currentSlot != null && !_currentSlot.InputHandler.CanChangeWeapon();
		}
	}

	public bool IsEnabled { get; set; }

	public LoadoutSlotType CurrentSlot
	{
		get
		{
			return (_currentSlot == null) ? LoadoutSlotType.None : _currentSlot.Slot;
		}
	}

	private WeaponController()
	{
		_weapons = new WeaponSlot[4];
		_currentSlotID = new CircularInteger(0, 3);
		IsEnabled = true;
		EventHandler.Global.AddListener<GlobalEvents.InputChanged>(OnInputChanged);
	}

	public void LateUpdate()
	{
		if (_holsterTime > 0f)
		{
			_holsterTime = Mathf.Max(_holsterTime - Time.deltaTime, 0f);
		}
		if (_currentSlot != _weapons[_currentSlotID.Current] && _holsterTime == 0f)
		{
			_currentSlot = _weapons[_currentSlotID.Current];
			PutdownCurrentWeapon();
			GameState.Current.PlayerData.SwitchWeaponSlot(_currentSlotID.Current);
			if (_currentSlot.Logic != null && _currentSlot.Decorator != null)
			{
				WeaponFeedbackManager.Instance.PickUp(_currentSlot);
				_currentSlot.Decorator.PlayEquipSound();
			}
		}
		if (CheckPerformShotConditions() && _currentSlot != null && _currentSlot.HasWeapon)
		{
			_currentSlot.InputHandler.Update();
		}
	}

	private void SetSlotWeapon(LoadoutSlotType slot, IUnityItem weapon)
	{
		if (weapon != null)
		{
			_loadoutWeapons[slot] = weapon;
		}
		else if (_loadoutWeapons.ContainsKey(slot))
		{
			_loadoutWeapons.Remove(slot);
		}
	}

	public void NextWeapon()
	{
		if (!HasAnyWeapon)
		{
			return;
		}
		int current = _currentSlotID.Current;
		int next = _currentSlotID.Next;
		while (_weapons[next] == null)
		{
			next = _currentSlotID.Next;
		}
		if (next != current)
		{
			if (_currentSlot != null && _currentSlot.InputHandler != null)
			{
				_currentSlot.InputHandler.Stop();
				_currentSlot = null;
			}
			GameState.Current.PlayerData.NextActiveWeapon.Value = _weapons[next];
		}
	}

	public void PrevWeapon()
	{
		if (!HasAnyWeapon)
		{
			return;
		}
		int current = _currentSlotID.Current;
		int prev = _currentSlotID.Prev;
		while (_weapons[prev] == null)
		{
			prev = _currentSlotID.Prev;
		}
		if (prev != current)
		{
			if (_currentSlot != null && _currentSlot.InputHandler != null)
			{
				_currentSlot.InputHandler.Stop();
				_currentSlot = null;
			}
			GameState.Current.PlayerData.NextActiveWeapon.Value = _weapons[prev];
		}
	}

	public void ShowFirstWeapon()
	{
		_currentSlotID.Reset();
		if (HasAnyWeapon)
		{
			if (_currentSlot != null && _currentSlot.InputHandler != null)
			{
				_currentSlot.InputHandler.Stop();
				_currentSlot = null;
			}
			int next = _currentSlotID.Next;
			while (_weapons[next] == null)
			{
				next = _currentSlotID.Next;
			}
		}
	}

	public bool CheckWeapons(List<int> weaponIds)
	{
		if (_weapons.Length != weaponIds.Count)
		{
			return false;
		}
		for (int i = 0; i < weaponIds.Count; i++)
		{
			if (_weapons[i] == null && weaponIds[i] != 0)
			{
				return false;
			}
			if (_weapons[i] != null && _weapons[i].View.ID != weaponIds[i])
			{
				return false;
			}
		}
		return true;
	}

	public void PutdownCurrentWeapon()
	{
		WeaponFeedbackManager.Instance.PutDown();
	}

	public void PickupCurrentWeapon()
	{
		if (_currentSlot != null)
		{
			WeaponFeedbackManager.Instance.PickUp(_currentSlot);
		}
	}

	public bool CheckAmmoCount()
	{
		return AmmoDepot.HasAmmoOfClass(_currentSlot.View.ItemClass);
	}

	public bool Shoot()
	{
		bool result = false;
		if (IsWeaponReady)
		{
			if (CheckAmmoCount())
			{
				_currentSlot.InputHandler.FireHandler.RegisterShot();
				_holsterTime = WeaponConfigurationHelper.GetRateOfFire(_currentSlot.View);
				Ray ray = new Ray(GameState.Current.PlayerData.ShootingPoint + GameState.Current.Player.EyePosition, GameState.Current.PlayerData.ShootingDirection);
				CmunePairList<BaseGameProp, ShotPoint> hits;
				_currentSlot.Logic.Shoot(ray, out hits);
				if (!_currentSlot.Decorator.HasShootAnimation)
				{
					WeaponFeedbackManager.Instance.Fire();
				}
				AmmoDepot.UseAmmoOfClass(_currentSlot.View.ItemClass, _currentSlot.Logic.AmmoCountPerShot);
				GameState.Current.PlayerData.WeaponFired.Value = _currentSlot;
				result = true;
			}
			else
			{
				_currentSlot.Decorator.PlayOutOfAmmoSound();
				GameData.Instance.OnNotificationFull.Fire(string.Empty, "Out of ammo!", 1f);
			}
		}
		return result;
	}

	public WeaponSlot GetPrimaryWeapon()
	{
		return _weapons[1];
	}

	public WeaponSlot GetCurrentWeapon()
	{
		return _currentSlot;
	}

	public void InitializeAllWeapons(Transform attachPoint)
	{
		for (int i = 0; i < _weapons.Length; i++)
		{
			if (_weapons[i] != null && _weapons[i].Decorator != null)
			{
				UnityEngine.Object.Destroy(_weapons[i].Decorator.gameObject);
			}
			_weapons[i] = null;
		}
		for (int j = 0; j < LoadoutManager.WeaponSlots.Length; j++)
		{
			LoadoutSlotType slot = LoadoutManager.WeaponSlots[j];
			InventoryItem item;
			if (Singleton<LoadoutManager>.Instance.TryGetItemInSlot(slot, out item))
			{
				WeaponSlot weaponSlot = new WeaponSlot(slot, item.Item, attachPoint, this);
				AddGameLogicToWeapon(weaponSlot);
				_weapons[j] = weaponSlot;
				AmmoDepot.SetMaxAmmoForType(item.Item.View.ItemClass, ((UberStrikeItemWeaponView)item.Item.View).MaxAmmo);
				AmmoDepot.SetStartAmmoForType(item.Item.View.ItemClass, ((UberStrikeItemWeaponView)item.Item.View).StartAmmo);
				SetSlotWeapon(slot, item.Item);
			}
			else
			{
				SetSlotWeapon(slot, null);
			}
		}
		GameState.Current.PlayerData.LoadoutWeapons.Value = LoadoutWeapons;
		Singleton<QuickItemController>.Instance.Initialize();
		Reset();
	}

	public void Reset()
	{
		AmmoDepot.Reset();
		_currentSlotID.SetRange(0, 3);
		_currentSlot = null;
		ShowFirstWeapon();
	}

	public void UpdateWeaponDecorator(IUnityItem item)
	{
	}

	public bool HasWeaponOfClass(UberstrikeItemClass itemClass)
	{
		for (int i = 0; i < 4; i++)
		{
			WeaponSlot weaponSlot = _weapons[i];
			if (weaponSlot != null && weaponSlot.HasWeapon && weaponSlot.View.ItemClass == itemClass)
			{
				return true;
			}
		}
		return false;
	}

	public void StopInputHandler()
	{
		if (_currentSlot != null)
		{
			_currentSlot.InputHandler.Stop();
		}
	}

	public int NextProjectileId()
	{
		return ProjectileManager.CreateGlobalProjectileID(PlayerNumber, ++_projectileId);
	}

	private int GetWeaponCount()
	{
		int num = 0;
		WeaponSlot[] weapons = _weapons;
		foreach (WeaponSlot weaponSlot in weapons)
		{
			if (weaponSlot != null)
			{
				num++;
			}
		}
		return num;
	}

	private void OnInputChanged(GlobalEvents.InputChanged ev)
	{
		if (AutoMonoBehaviour<InputManager>.Instance.IsInputEnabled && CheckPerformShotConditions())
		{
			switch (ev.Key)
			{
			case GameInputKey.WeaponMelee:
				OnSelectWeapon(ev, LoadoutSlotType.WeaponMelee);
				break;
			case GameInputKey.Weapon1:
				OnSelectWeapon(ev, LoadoutSlotType.WeaponPrimary);
				break;
			case GameInputKey.Weapon2:
				OnSelectWeapon(ev, LoadoutSlotType.WeaponSecondary);
				break;
			case GameInputKey.Weapon3:
				OnSelectWeapon(ev, LoadoutSlotType.WeaponTertiary);
				break;
			case GameInputKey.PrevWeapon:
				OnPrevWeapon(ev);
				break;
			case GameInputKey.NextWeapon:
				OnNextWeapon(ev);
				break;
			case GameInputKey.PrimaryFire:
				OnPrimaryFire(ev);
				break;
			case GameInputKey.SecondaryFire:
				OnSecondaryFire(ev);
				break;
			case (GameInputKey)14:
			case GameInputKey.QuickItem1:
			case GameInputKey.QuickItem2:
			case GameInputKey.QuickItem3:
				break;
			}
		}
	}

	private void OnSelectWeapon(GlobalEvents.InputChanged ev, LoadoutSlotType slotType)
	{
		if (!ev.IsDown || LevelCamera.IsZoomedIn || slotType == _weapons[_currentSlotID.Current].Slot || GetWeaponCount() <= 1)
		{
			return;
		}
		for (int i = 0; i < _weapons.Length; i++)
		{
			if (_weapons[i] != null && _weapons[i].Slot == slotType && _weapons[i] != _currentSlot)
			{
				if (_currentSlot != null)
				{
					_currentSlot.InputHandler.Stop();
					_currentSlot = null;
				}
				_currentSlotID.Current = i;
				GameState.Current.PlayerData.NextActiveWeapon.Value = _weapons[i];
			}
		}
	}

	private void OnPrevWeapon(GlobalEvents.InputChanged ev)
	{
		if ((_currentSlot == null || (ev.IsDown && _currentSlot.InputHandler.CanChangeWeapon())) && GUITools.SaveClickIn(0.2f))
		{
			GUITools.Clicked();
			NextWeapon();
		}
		else if (_currentSlot != null && ev.IsDown)
		{
			_currentSlot.InputHandler.OnPrevWeapon();
		}
	}

	private void OnNextWeapon(GlobalEvents.InputChanged ev)
	{
		if ((_currentSlot == null || (ev.IsDown && _currentSlot.InputHandler.CanChangeWeapon())) && GUITools.SaveClickIn(0.2f))
		{
			GUITools.Clicked();
			PrevWeapon();
		}
		else if (_currentSlot != null && ev.IsDown)
		{
			_currentSlot.InputHandler.OnNextWeapon();
		}
	}

	private void OnPrimaryFire(GlobalEvents.InputChanged ev)
	{
		if (ev.IsDown)
		{
			if (_currentSlot != null && _currentSlot.HasWeapon)
			{
				_currentSlot.InputHandler.OnPrimaryFire(true);
			}
		}
		else if (_currentSlot != null)
		{
			_currentSlot.InputHandler.OnPrimaryFire(false);
		}
	}

	private void OnSecondaryFire(GlobalEvents.InputChanged ev)
	{
		if (GameState.Current.PlayerData.IsAlive && IsEnabled && _currentSlot != null && _currentSlot.HasWeapon)
		{
			_currentSlot.InputHandler.OnSecondaryFire(ev.IsDown);
		}
	}

	private bool CheckPerformShotConditions()
	{
		return IsEnabled && GameState.Current.Player != null && GameState.Current.Player.EnableWeaponControl && !GameState.Current.IsPlayerPaused && !GameState.Current.IsPlayerDead;
	}

	private IEnumerator StartHidingWeapon(GameObject weapon, bool destroy)
	{
		for (float time = 0f; time < 2f; time += Time.deltaTime)
		{
			yield return new WaitForEndOfFrame();
		}
		if (destroy)
		{
			UnityEngine.Object.Destroy(weapon);
		}
	}

	private IEnumerator StartApplyDamage(WeaponSlot slot, float delay, CmunePairList<BaseGameProp, ShotPoint> hits)
	{
		yield return new WaitForSeconds(delay);
		ApplyDamage(slot, hits);
	}

	private void ApplyDamage(WeaponSlot slot, CmunePairList<BaseGameProp, ShotPoint> hits)
	{
		foreach (KeyValuePair<BaseGameProp, ShotPoint> hit in hits)
		{
			DamageInfo damageInfo = new DamageInfo(Convert.ToInt16(slot.View.DamagePerProjectile * hit.Value.Count));
			damageInfo.Bullets = (byte)hit.Value.Count;
			damageInfo.Force = GameState.Current.Player.WeaponCamera.transform.forward * (slot.View.DamagePerProjectile * hit.Value.Count);
			damageInfo.UpwardsForceMultiplier = 10f;
			damageInfo.Hitpoint = hit.Value.MidPoint;
			damageInfo.ProjectileID = hit.Value.ProjectileId;
			damageInfo.SlotId = slot.SlotId;
			damageInfo.WeaponID = slot.View.ID;
			damageInfo.WeaponClass = slot.View.ItemClass;
			damageInfo.CriticalStrikeBonus = WeaponConfigurationHelper.GetCriticalStrikeBonus(slot.View);
			DamageInfo damageInfo2 = damageInfo;
			if (hit.Key != null)
			{
				hit.Key.ApplyDamage(damageInfo2);
			}
		}
	}

	private void AddGameLogicToWeapon(WeaponSlot weapon)
	{
		float movement = WeaponConfigurationHelper.GetRecoilMovement(weapon.View);
		float kickback = WeaponConfigurationHelper.GetRecoilKickback(weapon.View);
		LoadoutSlotType slot = weapon.Slot;
		if (weapon.Logic is ProjectileWeapon)
		{
			ProjectileWeapon w = weapon.Logic as ProjectileWeapon;
			w.OnProjectileShoot += delegate(ProjectileInfo p)
			{
				ProjectileDetonator projectileDetonator = new ProjectileDetonator(WeaponConfigurationHelper.GetSplashRadius(weapon.View), weapon.View.DamagePerProjectile, weapon.View.DamageKnockback, p.Direction, weapon.SlotId, p.Id, weapon.View.ID, weapon.View.ItemClass, w.Config.DamageEffectFlag, w.Config.DamageEffectValue);
				if (p.Projectile != null)
				{
					p.Projectile.Detonator = projectileDetonator;
					if (weapon.View.ItemClass != UberstrikeItemClass.WeaponSplattergun)
					{
						GameState.Current.Actions.EmitProjectile(p.Position, p.Direction, slot, p.Id, false);
					}
				}
				else
				{
					projectileDetonator.Explode(p.Position);
					if (weapon.View.ItemClass != UberstrikeItemClass.WeaponSplattergun)
					{
						GameState.Current.Actions.EmitProjectile(p.Position, p.Direction, slot, p.Id, true);
					}
				}
				if (weapon.View.ItemClass != UberstrikeItemClass.WeaponSplattergun)
				{
					if (w.HasProjectileLimit)
					{
						Singleton<ProjectileManager>.Instance.AddLimitedProjectile(p.Projectile, p.Id, w.MaxConcurrentProjectiles);
					}
					else
					{
						Singleton<ProjectileManager>.Instance.AddProjectile(p.Projectile, p.Id);
					}
				}
				LevelCamera.DoFeedback(LevelCamera.FeedbackType.ShootWeapon, Vector3.back, 0f, movement / 8f, 0.1f, 0.3f, kickback / 3f, Vector3.left);
			};
			return;
		}
		if (weapon.Logic is MeleeWeapon)
		{
			float delay = weapon.Logic.HitDelay;
			weapon.Logic.OnTargetHit += delegate(CmunePairList<BaseGameProp, ShotPoint> h)
			{
				if (!weapon.View.HasAutomaticFire)
				{
					GameState.Current.Actions.SingleBulletFire();
				}
				if (h != null)
				{
					UnityRuntime.StartRoutine(StartApplyDamage(weapon, delay, h));
				}
				LevelCamera.DoFeedback(LevelCamera.FeedbackType.ShootWeapon, Vector3.back, 0f, movement / 8f, 0.1f, 0.3f, kickback / 3f, Vector3.left);
			};
			return;
		}
		weapon.Logic.OnTargetHit += delegate(CmunePairList<BaseGameProp, ShotPoint> h)
		{
			if (!weapon.View.HasAutomaticFire)
			{
				GameState.Current.Actions.SingleBulletFire();
			}
			if (h != null)
			{
				ApplyDamage(weapon, h);
			}
			LevelCamera.DoFeedback(LevelCamera.FeedbackType.ShootWeapon, Vector3.back, 0f, movement / 8f, 0.1f, 0.3f, kickback / 3f, Vector3.left);
		};
	}
}
