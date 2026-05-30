using UnityEngine;

public class ProjectileWeaponDecorator : BaseWeaponDecorator
{
	[SerializeField]
	private Projectile _missle;

	[SerializeField]
	private AudioClip _missleExplosionSound;

	private float _missileTimeOut;

	public Projectile Missle
	{
		get
		{
			return _missle;
		}
	}

	public float MissileTimeOut
	{
		get
		{
			return _missileTimeOut;
		}
	}

	public AudioClip ExplosionSound
	{
		get
		{
			return _missleExplosionSound;
		}
	}

	public void ShowExplosionEffect(Vector3 position, Vector3 normal, ParticleConfigurationType explosionEffect)
	{
		ShowShootEffect(new RaycastHit[0]);
		Singleton<ExplosionManager>.Instance.ShowExplosionEffect(position, normal, base.tag, explosionEffect);
	}

	public void SetMissileTimeOut(float timeOut)
	{
		_missileTimeOut = timeOut;
	}
}
