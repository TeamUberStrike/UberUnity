using System;
using UnityEngine;

[Serializable]
public class WeaponItemConfiguration
{
	[CustomProperty("SwitchDelay")]
	private int _switchDelay = 500;

	[CustomProperty("MaxConcurrentProjectiles")]
	private int _maxConcurrentProjectiles;

	[CustomProperty("Sticky")]
	private int _sticky;

	private int _criticalStrikeBonus;

	[SerializeField]
	private DamageEffectType _damageEffectFlag;

	[SerializeField]
	private float _damageEffectValue;

	[SerializeField]
	private Vector3 _ironSightPosition;

	[SerializeField]
	private bool _showReticleForPrimaryAction;

	[SerializeField]
	private int _minProjectileDistance = 2;

	[SerializeField]
	private Vector3 _position;

	[SerializeField]
	private Vector3 _rotation;

	[SerializeField]
	private ParticleConfigurationType _impactEffect;

	public bool Sticky
	{
		get
		{
			return _sticky != 0;
		}
	}

	public int SwitchDelayMilliSeconds
	{
		get
		{
			return _switchDelay;
		}
		set
		{
			_switchDelay = value;
		}
	}

	public int MaxConcurrentProjectiles
	{
		get
		{
			return _maxConcurrentProjectiles;
		}
	}

	public int MinProjectileDistance
	{
		get
		{
			return _minProjectileDistance;
		}
		set
		{
			_minProjectileDistance = value;
		}
	}

	public Vector3 Position
	{
		get
		{
			return _position;
		}
		set
		{
			_position = value;
		}
	}

	public Vector3 Rotation
	{
		get
		{
			return _rotation;
		}
		set
		{
			_rotation = value;
		}
	}

	public bool ShowReticleForPrimaryAction
	{
		get
		{
			return _showReticleForPrimaryAction;
		}
		set
		{
			_showReticleForPrimaryAction = value;
		}
	}

	public Vector3 IronSightPosition
	{
		get
		{
			return _ironSightPosition;
		}
		set
		{
			_ironSightPosition = value;
		}
	}

	public DamageEffectType DamageEffectFlag
	{
		get
		{
			return _damageEffectFlag;
		}
		set
		{
			_damageEffectFlag = value;
		}
	}

	public float DamageEffectValue
	{
		get
		{
			return _damageEffectValue;
		}
		set
		{
			_damageEffectValue = value;
		}
	}

	public ParticleConfigurationType ParticleEffect
	{
		get
		{
			return _impactEffect;
		}
		set
		{
			_impactEffect = value;
		}
	}

	public int CriticalStrikeBonus
	{
		get
		{
			return _criticalStrikeBonus;
		}
		set
		{
			_criticalStrikeBonus = value;
		}
	}
}
