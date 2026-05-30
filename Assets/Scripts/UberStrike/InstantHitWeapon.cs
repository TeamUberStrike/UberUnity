using UberStrike.Core.Models.Views;
using UberStrike.Realtime.UnitySdk;
using UnityEngine;

public class InstantHitWeapon : BaseWeaponLogic
{
	private UberStrikeItemWeaponView _view;

	private BaseWeaponDecorator _decorator;

	private bool _supportIronSight;

	public override BaseWeaponDecorator Decorator
	{
		get
		{
			return _decorator;
		}
	}

	public InstantHitWeapon(WeaponItem item, BaseWeaponDecorator decorator, IWeaponController controller, UberStrikeItemWeaponView view)
		: base(item, controller)
	{
		_view = view;
		_decorator = decorator;
		_supportIronSight = view.WeaponSecondaryAction == 2;
	}

	public override void Shoot(Ray ray, out CmunePairList<BaseGameProp, ShotPoint> hits)
	{
		hits = null;
		Vector3 direction = WeaponDataManager.ApplyDispersion(ray.direction, _view, _supportIronSight);
		int projectileId = base.Controller.NextProjectileId();
		RaycastHit hitInfo;
		if (Physics.Raycast(ray.origin, direction, out hitInfo, 1000f, (!base.Controller.IsLocal) ? UberstrikeLayerMasks.ShootMaskRemotePlayer : UberstrikeLayerMasks.ShootMask))
		{
			HitPoint point = new HitPoint(hitInfo.point, TagUtil.GetTag(hitInfo.collider));
			BaseGameProp component = hitInfo.collider.GetComponent<BaseGameProp>();
			if ((bool)component)
			{
				hits = new CmunePairList<BaseGameProp, ShotPoint>(1);
				hits.Add(component, new ShotPoint(hitInfo.point, projectileId));
			}
			Decorator.PlayImpactSoundAt(point);
		}
		else
		{
			hitInfo.point = ray.origin + ray.direction * 1000f;
		}
		if ((bool)Decorator)
		{
			Decorator.ShowShootEffect(new RaycastHit[1] { hitInfo });
		}
		OnHits(hits);
	}
}
