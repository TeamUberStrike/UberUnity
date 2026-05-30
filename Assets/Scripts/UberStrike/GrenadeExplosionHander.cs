public class GrenadeExplosionHander : IWeaponFireHandler
{
	public bool IsTriggerPulled { get; private set; }

	public bool CanShoot
	{
		get
		{
			return true;
		}
	}

	public GrenadeExplosionHander()
	{
		IsTriggerPulled = false;
	}

	public void OnTriggerPulled(bool pulled)
	{
		IsTriggerPulled = pulled;
		if (pulled)
		{
			Singleton<ProjectileManager>.Instance.RemoveAllLimitedProjectiles();
		}
	}

	public void Update()
	{
	}

	public void Stop()
	{
		IsTriggerPulled = false;
	}

	public void RegisterShot()
	{
	}
}
