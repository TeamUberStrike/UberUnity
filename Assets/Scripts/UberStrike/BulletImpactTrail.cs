using UnityEngine;

public class BulletImpactTrail : BaseWeaponEffect
{
	[SerializeField]
	private Transform _muzzle;

	[SerializeField]
	private MoveTrailrendererObject _trailRenderer;

	private void Awake()
	{
		_trailRenderer = GetComponent<MoveTrailrendererObject>();
	}

	public override void OnHits(RaycastHit[] hits)
	{
		if (!_trailRenderer)
		{
			return;
		}
		for (int i = 0; i < hits.Length; i++)
		{
			RaycastHit raycastHit = hits[i];
			MoveTrailrendererObject moveTrailrendererObject = Object.Instantiate(_trailRenderer, _muzzle.position, Quaternion.identity) as MoveTrailrendererObject;
			if ((bool)moveTrailrendererObject)
			{
				moveTrailrendererObject.MoveTrail(raycastHit.point, _muzzle.position, raycastHit.distance);
			}
		}
	}

	public override void OnShoot()
	{
		if (!_trailRenderer)
		{
		}
	}

	public override void OnPostShoot()
	{
	}

	public override void Hide()
	{
	}
}
