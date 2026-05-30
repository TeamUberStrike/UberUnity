using System;
using UberStrike.Realtime.UnitySdk;
using UnityEngine;

public abstract class BaseWeaponLogic : IWeaponLogic
{
	public IWeaponController Controller { get; private set; }

	public WeaponItemConfiguration Config { get; private set; }

	public abstract BaseWeaponDecorator Decorator { get; }

	public virtual int AmmoCountPerShot
	{
		get
		{
			return 1;
		}
	}

	public virtual float HitDelay
	{
		get
		{
			return 0f;
		}
	}

	public bool IsWeaponReady { get; private set; }

	public bool IsWeaponActive { get; set; }

	public event Action<CmunePairList<BaseGameProp, ShotPoint>> OnTargetHit;

	protected BaseWeaponLogic(WeaponItem item, IWeaponController controller)
	{
		Controller = controller;
		Config = item.Configuration;
	}

	public abstract void Shoot(Ray ray, out CmunePairList<BaseGameProp, ShotPoint> hits);

	protected void OnHits(CmunePairList<BaseGameProp, ShotPoint> hits)
	{
		if (this.OnTargetHit != null)
		{
			this.OnTargetHit(hits);
		}
	}
}
