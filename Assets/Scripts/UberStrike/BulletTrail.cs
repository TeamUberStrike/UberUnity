using System.Collections;
using UnityEngine;

public class BulletTrail : BaseWeaponEffect
{
	private Animation _animation;

	private AnimationState _clip;

	private float _trailDuration = 0.1f;

	private Renderer[] _renderers = new Renderer[0];

	private void Awake()
	{
		_animation = GetComponentInChildren<Animation>();
		if ((bool)_animation)
		{
			_clip = _animation[_animation.clip.name];
			_clip.wrapMode = WrapMode.Once;
			_trailDuration = _clip.length;
			_animation.playAutomatically = false;
		}
		_renderers = GetComponentsInChildren<Renderer>();
		Renderer[] renderers = _renderers;
		foreach (Renderer renderer in renderers)
		{
			renderer.enabled = false;
		}
	}

	public override void OnShoot()
	{
		Renderer[] renderers = _renderers;
		foreach (Renderer renderer in renderers)
		{
			renderer.enabled = true;
		}
		if ((bool)_animation)
		{
			float speed = _trailDuration / _clip.length;
			_clip.speed = speed;
			_animation.Play();
		}
		StartCoroutine(StartTrailEffect(_trailDuration));
	}

	public override void OnPostShoot()
	{
	}

	public override void Hide()
	{
	}

	public override void OnHits(RaycastHit[] hits)
	{
	}

	private IEnumerator StartTrailEffect(float time)
	{
		yield return new WaitForSeconds(time);
		Renderer[] renderers = _renderers;
		foreach (Renderer r in renderers)
		{
			r.enabled = false;
		}
	}
}
