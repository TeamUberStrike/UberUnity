using System;
using System.Collections.Generic;
using UberStrike.Core.Types;
using UnityEngine;

public static class AmmoDepot
{
	private static Dictionary<AmmoType, int> _currentAmmo;

	private static Dictionary<AmmoType, int> _startAmmo;

	private static Dictionary<AmmoType, int> _maxAmmo;

	static AmmoDepot()
	{
		_currentAmmo = new Dictionary<AmmoType, int>(7);
		_maxAmmo = new Dictionary<AmmoType, int>(7);
		_startAmmo = new Dictionary<AmmoType, int>(7);
		foreach (int value in Enum.GetValues(typeof(AmmoType)))
		{
			_startAmmo.Add((AmmoType)value, 100);
			_currentAmmo.Add((AmmoType)value, 0);
			_maxAmmo.Add((AmmoType)value, 200);
		}
	}

	public static void Reset()
	{
		_currentAmmo[AmmoType.Handgun] = _startAmmo[AmmoType.Handgun];
		_currentAmmo[AmmoType.Machinegun] = _startAmmo[AmmoType.Machinegun];
		_currentAmmo[AmmoType.Launcher] = _startAmmo[AmmoType.Launcher];
		_currentAmmo[AmmoType.Shotgun] = _startAmmo[AmmoType.Shotgun];
		_currentAmmo[AmmoType.Cannon] = _startAmmo[AmmoType.Cannon];
		_currentAmmo[AmmoType.Splattergun] = _startAmmo[AmmoType.Splattergun];
		_currentAmmo[AmmoType.Snipergun] = _startAmmo[AmmoType.Snipergun];
	}

	public static void SetMaxAmmoForType(UberstrikeItemClass weaponClass, int maxAmmoCount)
	{
		if (PlayerDataManager.IsPlayerLoggedIn)
		{
			switch (weaponClass)
			{
			case UberstrikeItemClass.WeaponCannon:
				_maxAmmo[AmmoType.Cannon] = maxAmmoCount;
				break;
			case UberstrikeItemClass.WeaponLauncher:
				_maxAmmo[AmmoType.Launcher] = maxAmmoCount;
				break;
			case UberstrikeItemClass.WeaponMachinegun:
				_maxAmmo[AmmoType.Machinegun] = maxAmmoCount;
				break;
			case UberstrikeItemClass.WeaponShotgun:
				_maxAmmo[AmmoType.Shotgun] = maxAmmoCount;
				break;
			case UberstrikeItemClass.WeaponSniperRifle:
				_maxAmmo[AmmoType.Snipergun] = maxAmmoCount;
				break;
			case UberstrikeItemClass.WeaponSplattergun:
				_maxAmmo[AmmoType.Splattergun] = maxAmmoCount;
				break;
			}
		}
	}

	public static void SetStartAmmoForType(UberstrikeItemClass weaponClass, int startAmmoCount)
	{
		if (PlayerDataManager.IsPlayerLoggedIn)
		{
			switch (weaponClass)
			{
			case UberstrikeItemClass.WeaponCannon:
				_startAmmo[AmmoType.Cannon] = startAmmoCount;
				break;
			case UberstrikeItemClass.WeaponLauncher:
				_startAmmo[AmmoType.Launcher] = startAmmoCount;
				break;
			case UberstrikeItemClass.WeaponMachinegun:
				_startAmmo[AmmoType.Machinegun] = startAmmoCount;
				break;
			case UberstrikeItemClass.WeaponShotgun:
				_startAmmo[AmmoType.Shotgun] = startAmmoCount;
				break;
			case UberstrikeItemClass.WeaponSniperRifle:
				_startAmmo[AmmoType.Snipergun] = startAmmoCount;
				break;
			case UberstrikeItemClass.WeaponSplattergun:
				_startAmmo[AmmoType.Splattergun] = startAmmoCount;
				break;
			}
		}
	}

	public static bool CanAddAmmo(AmmoType t)
	{
		UberstrikeItemClass itemClass;
		if (TryGetAmmoTypeFromItemClass(t, out itemClass) && Singleton<WeaponController>.Instance.HasWeaponOfClass(itemClass))
		{
			return _currentAmmo[t] < _maxAmmo[t];
		}
		return false;
	}

	public static void AddAmmoOfClass(UberstrikeItemClass c)
	{
		AmmoType t;
		if (TryGetAmmoType(c, out t))
		{
			AddDefaultAmmoOfType(t);
		}
	}

	public static void AddDefaultAmmoOfType(AmmoType t)
	{
		AddAmmoOfType(t, _startAmmo[t]);
	}

	public static void AddAmmoOfType(AmmoType t, int bullets)
	{
		_currentAmmo[t] = Mathf.Clamp(_currentAmmo[t] + bullets, 0, _maxAmmo[t]);
		GameState.Current.PlayerData.Ammo.Value = _currentAmmo[t];
	}

	public static void AddStartAmmoOfType(AmmoType t, float percentage = 1f)
	{
		int num = Mathf.CeilToInt((float)_startAmmo[t] * percentage);
		_currentAmmo[t] = Mathf.Clamp(_currentAmmo[t] + num, 0, _maxAmmo[t]);
		GameState.Current.PlayerData.Ammo.Value = _currentAmmo[t];
	}

	public static void AddMaxAmmoOfType(AmmoType t, float percentage = 1f)
	{
		int num = Mathf.CeilToInt((float)_maxAmmo[t] * percentage);
		_currentAmmo[t] = Mathf.Clamp(_currentAmmo[t] + num, 0, _maxAmmo[t]);
		GameState.Current.PlayerData.Ammo.Value = _currentAmmo[t];
	}

	public static bool HasAmmoOfType(AmmoType t)
	{
		return _currentAmmo[t] > 0;
	}

	public static bool HasAmmoOfClass(UberstrikeItemClass t)
	{
		AmmoType t2;
		return t == UberstrikeItemClass.WeaponMelee || (TryGetAmmoType(t, out t2) && HasAmmoOfType(t2));
	}

	public static int AmmoOfType(AmmoType t)
	{
		return _currentAmmo[t];
	}

	public static int AmmoOfClass(UberstrikeItemClass t)
	{
		AmmoType t2;
		if (TryGetAmmoType(t, out t2))
		{
			return AmmoOfType(t2);
		}
		return 0;
	}

	public static int MaxAmmoOfClass(UberstrikeItemClass t)
	{
		AmmoType t2;
		if (TryGetAmmoType(t, out t2))
		{
			return _maxAmmo[t2];
		}
		return 0;
	}

	public static bool TryGetAmmoType(UberstrikeItemClass item, out AmmoType t)
	{
		switch (item)
		{
		case UberstrikeItemClass.WeaponCannon:
			t = AmmoType.Cannon;
			return true;
		case UberstrikeItemClass.WeaponLauncher:
			t = AmmoType.Launcher;
			return true;
		case UberstrikeItemClass.WeaponMachinegun:
			t = AmmoType.Machinegun;
			return true;
		case UberstrikeItemClass.WeaponShotgun:
			t = AmmoType.Shotgun;
			return true;
		case UberstrikeItemClass.WeaponSniperRifle:
			t = AmmoType.Snipergun;
			return true;
		case UberstrikeItemClass.WeaponSplattergun:
			t = AmmoType.Splattergun;
			return true;
		default:
			t = AmmoType.Handgun;
			return false;
		}
	}

	public static bool TryGetAmmoTypeFromItemClass(AmmoType t, out UberstrikeItemClass itemClass)
	{
		switch (t)
		{
		case AmmoType.Cannon:
			itemClass = UberstrikeItemClass.WeaponCannon;
			return true;
		case AmmoType.Launcher:
			itemClass = UberstrikeItemClass.WeaponLauncher;
			return true;
		case AmmoType.Machinegun:
			itemClass = UberstrikeItemClass.WeaponMachinegun;
			return true;
		case AmmoType.Shotgun:
			itemClass = UberstrikeItemClass.WeaponShotgun;
			return true;
		case AmmoType.Snipergun:
			itemClass = UberstrikeItemClass.WeaponSniperRifle;
			return true;
		case AmmoType.Splattergun:
			itemClass = UberstrikeItemClass.WeaponSplattergun;
			return true;
		default:
			itemClass = UberstrikeItemClass.WeaponMachinegun;
			return false;
		}
	}

	public static bool UseAmmoOfType(AmmoType t, int count = 1)
	{
		if (_currentAmmo[t] > 0)
		{
			_currentAmmo[t] = Mathf.Max(_currentAmmo[t] - count, 0);
			GameState.Current.PlayerData.Ammo.Value = _currentAmmo[t];
			return true;
		}
		return false;
	}

	public static bool UseAmmoOfClass(UberstrikeItemClass t, int count = 1)
	{
		AmmoType t2;
		if (TryGetAmmoType(t, out t2))
		{
			return UseAmmoOfType(t2, count);
		}
		return false;
	}

	public static string ToString(AmmoType t)
	{
		return _currentAmmo[t].ToString();
	}

	public static void RemoveExtraAmmoOfType(UberstrikeItemClass t)
	{
		AmmoType t2;
		if (TryGetAmmoType(t, out t2) && _currentAmmo[t2] > _maxAmmo[t2])
		{
			_currentAmmo[t2] = _maxAmmo[t2];
		}
	}
}
