using System;
using UnityEngine;

[Serializable]
public class ExplosiveGrenadeConfiguration : QuickItemConfiguration
{
	[CustomProperty("Damage")]
	[SerializeField]
	private int _damage = 100;

	[CustomProperty("SplashRadius")]
	[SerializeField]
	private int _splash = 2;

	[SerializeField]
	[CustomProperty("LifeTime")]
	private int _lifeTime = 15;

	[SerializeField]
	[CustomProperty("Bounciness")]
	private int _bounciness = 3;

	[CustomProperty("Sticky")]
	[SerializeField]
	private bool _isSticky = true;

	[SerializeField]
	private int _speed = 15;

	public int Damage
	{
		get
		{
			return _damage;
		}
	}

	public int SplashRadius
	{
		get
		{
			return _splash;
		}
	}

	public int LifeTime
	{
		get
		{
			return _lifeTime;
		}
	}

	public float Bounciness
	{
		get
		{
			return (float)_bounciness * 0.1f;
		}
	}

	public bool IsSticky
	{
		get
		{
			return _isSticky;
		}
	}

	public int Speed
	{
		get
		{
			return _speed;
		}
	}
}
