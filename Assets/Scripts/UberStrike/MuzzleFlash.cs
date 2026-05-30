using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : BaseWeaponEffect
{
	private float _muzzleFlashEnd;

	private Animation _animation;

	private AnimationState _clip;

	private float _flashDuration = 0.1f;

	private List<Renderer> _renderers = new List<Renderer>();

	private void Awake()
	{
		_animation = GetComponentInChildren<Animation>();
		if ((bool)_animation)
		{
			_clip = _animation[_animation.clip.name];
			_clip.wrapMode = WrapMode.Once;
			_flashDuration = _clip.length;
			_animation.playAutomatically = false;
		}
		_muzzleFlashEnd = 0f;
		_renderers.AddRange(GetComponentsInChildren<Renderer>());
		foreach (Renderer renderer in _renderers)
		{
			renderer.enabled = false;
		}
	}

	public override void Hide()
	{
		_muzzleFlashEnd = 0f;
		if ((bool)_clip)
		{
			_clip.normalizedTime = 1f;
		}
		foreach (Renderer renderer in _renderers)
		{
			renderer.enabled = false;
		}
	}

	public override void OnShoot()
	{
		foreach (Renderer renderer in _renderers)
		{
			renderer.enabled = true;
		}
		base.transform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0, 360));
		if ((bool)_animation)
		{
			float speed = _clip.length / _flashDuration;
			_clip.speed = speed;
			_clip.time = 0f;
			_animation.Play();
		}
		_muzzleFlashEnd = Time.time + _flashDuration;
	}

	public override void OnPostShoot()
	{
	}

	private void Update()
	{
		if (!(_muzzleFlashEnd < Time.time))
		{
			return;
		}
		foreach (Renderer renderer in _renderers)
		{
			renderer.enabled = false;
		}
	}

	public override void OnHits(RaycastHit[] hits)
	{
	}
}
