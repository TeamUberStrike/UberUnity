using UnityEngine;

[RequireComponent(typeof(ParticleSystemRenderer))]
public class MuzzleSmoke : BaseWeaponEffect
{
	private ParticleSystem _particleEmitter;

	private void Awake()
	{
		_particleEmitter = GetComponentInChildren<ParticleSystem>();
	}

	public override void OnShoot()
	{
		if ((bool)_particleEmitter)
		{
			base.gameObject.SetActive(true);
			_particleEmitter.Emit(1);
		}
	}

	public override void OnPostShoot()
	{
	}

	public override void OnHits(RaycastHit[] hits)
	{
	}

	public override void Hide()
	{
	}
}
