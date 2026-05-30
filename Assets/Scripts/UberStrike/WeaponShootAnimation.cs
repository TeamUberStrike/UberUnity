using UnityEngine;

public class WeaponShootAnimation : BaseWeaponEffect
{
	[SerializeField]
	private Animation _shootAnimation;

	private void Awake()
	{
		if ((bool)_shootAnimation)
		{
			_shootAnimation.playAutomatically = false;
		}
	}

	public override void OnShoot()
	{
		if ((bool)_shootAnimation)
		{
			_shootAnimation.Rewind();
			_shootAnimation.Play();
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
		if ((bool)_shootAnimation && (bool)_shootAnimation.clip)
		{
			_shootAnimation.clip.SampleAnimation(base.gameObject, 0f);
			_shootAnimation.Stop();
		}
	}
}
