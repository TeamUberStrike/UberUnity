using System.Collections.Generic;
using UberStrike.Core.Models.Views;
using UberStrike.Realtime.UnitySdk;
using UnityEngine;

public class InstantMultiHitWeapon : BaseWeaponLogic
{
	private UberStrikeItemWeaponView _view;

	private int ShotgunGauge;

	private BaseWeaponDecorator _decorator;

	public override BaseWeaponDecorator Decorator
	{
		get
		{
			return _decorator;
		}
	}

	public InstantMultiHitWeapon(WeaponItem item, BaseWeaponDecorator decorator, int shotGauge, IWeaponController controller, UberStrikeItemWeaponView view)
		: base(item, controller)
	{
		ShotgunGauge = shotGauge;
		_view = view;
		_decorator = decorator;
	}

	public override void Shoot(Ray ray, out CmunePairList<BaseGameProp, ShotPoint> hits)
	{
		Dictionary<BaseGameProp, ShotPoint> dictionary = new Dictionary<BaseGameProp, ShotPoint>(ShotgunGauge);
		HitPoint hitPoint = null;
		RaycastHit[] array = new RaycastHit[ShotgunGauge];
		int projectileId = base.Controller.NextProjectileId();
		int num = 1000;
		for (int i = 0; i < ShotgunGauge; i++)
		{
			Vector3 direction = WeaponDataManager.ApplyDispersion(ray.direction, _view, false);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray.origin, direction, out hitInfo, num, (!base.Controller.IsLocal) ? UberstrikeLayerMasks.ShootMaskRemotePlayer : UberstrikeLayerMasks.ShootMask))
			{
				if (hitPoint == null)
				{
					hitPoint = new HitPoint(hitInfo.point, TagUtil.GetTag(hitInfo.collider));
				}
				BaseGameProp component = hitInfo.collider.GetComponent<BaseGameProp>();
				if ((bool)component)
				{
					ShotPoint value;
					if (dictionary.TryGetValue(component, out value))
					{
						value.AddPoint(hitInfo.point);
					}
					else
					{
						dictionary.Add(component, new ShotPoint(hitInfo.point, projectileId));
					}
				}
				array[i] = hitInfo;
			}
			else
			{
				array[i].point = ray.origin + ray.direction * 1000f;
				array[i].normal = hitInfo.normal;
			}
		}
		Decorator.PlayImpactSoundAt(hitPoint);
		hits = new CmunePairList<BaseGameProp, ShotPoint>(dictionary.Count);
		foreach (KeyValuePair<BaseGameProp, ShotPoint> item in dictionary)
		{
			hits.Add(item.Key, item.Value);
		}
		if ((bool)Decorator)
		{
			Decorator.ShowShootEffect(array);
		}
		OnHits(hits);
	}
}
