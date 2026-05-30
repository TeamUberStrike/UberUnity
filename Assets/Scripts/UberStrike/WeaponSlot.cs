using System;
using UberStrike.Core.Models.Views;
using UberStrike.Core.Types;
using UnityEngine;

public class WeaponSlot
{
	public LoadoutSlotType Slot { get; private set; }

	public BaseWeaponLogic Logic { get; private set; }

	public BaseWeaponDecorator Decorator { get; private set; }

	public IUnityItem UnityItem { get; private set; }

	public WeaponItem Item { get; private set; }

	public UberStrikeItemWeaponView View { get; private set; }

	public WeaponInputHandler InputHandler { get; private set; }

	public bool HasWeapon
	{
		get
		{
			return Item != null;
		}
	}

	public byte SlotId
	{
		get
		{
			switch (Slot)
			{
			case LoadoutSlotType.WeaponPrimary:
				return 1;
			case LoadoutSlotType.WeaponSecondary:
				return 2;
			case LoadoutSlotType.WeaponTertiary:
				return 3;
			default:
				return 0;
			}
		}
	}

	public WeaponSlot(LoadoutSlotType slot, IUnityItem item, Transform attachPoint, IWeaponController controller)
	{
		UnityItem = item;
		View = item.View as UberStrikeItemWeaponView;
		Slot = slot;
		Initialize(controller, attachPoint);
	}

	private void Initialize(IWeaponController controller, Transform attachPoint)
	{
		CreateWeaponLogic(View, controller);
		CreateWeaponInputHandler(Item, Logic, Decorator, controller.IsLocal);
		ConfigureWeaponDecorator(attachPoint);
		if (controller.IsLocal)
		{
			Decorator.EnableShootAnimation = true;
			Decorator.IronSightPosition = Item.Configuration.IronSightPosition;
		}
		else
		{
			Decorator.EnableShootAnimation = false;
			Decorator.DefaultPosition = Vector3.zero;
		}
	}

	private void CreateWeaponLogic(UberStrikeItemWeaponView view, IWeaponController controller)
	{
		switch (view.ItemClass)
		{
		case UberstrikeItemClass.WeaponCannon:
		case UberstrikeItemClass.WeaponSplattergun:
		case UberstrikeItemClass.WeaponLauncher:
		{
			ProjectileWeaponDecorator projectileWeaponDecorator = CreateProjectileWeaponDecorator(view.ID, view.MissileTimeToDetonate);
			Item = projectileWeaponDecorator.GetComponent<WeaponItem>();
			Logic = new ProjectileWeapon(Item, projectileWeaponDecorator, controller, view);
			Decorator = projectileWeaponDecorator;
			break;
		}
		case UberstrikeItemClass.WeaponMachinegun:
			Decorator = InstantiateWeaponDecorator(view.ID);
			Item = Decorator.GetComponent<WeaponItem>();
			if (view.ProjectilesPerShot > 1)
			{
				Logic = new InstantMultiHitWeapon(Item, Decorator, view.ProjectilesPerShot, controller, view);
			}
			else
			{
				Logic = new InstantHitWeapon(Item, Decorator, controller, view);
			}
			break;
		case UberstrikeItemClass.WeaponSniperRifle:
			Decorator = InstantiateWeaponDecorator(view.ID);
			Item = Decorator.GetComponent<WeaponItem>();
			Logic = new InstantHitWeapon(Item, Decorator, controller, view);
			break;
		case UberstrikeItemClass.WeaponMelee:
			Decorator = InstantiateWeaponDecorator(view.ID);
			Item = Decorator.GetComponent<WeaponItem>();
			Logic = new MeleeWeapon(Item, Decorator as MeleeWeaponDecorator, controller);
			break;
		case UberstrikeItemClass.WeaponShotgun:
			Decorator = InstantiateWeaponDecorator(view.ID);
			Item = Decorator.GetComponent<WeaponItem>();
			Logic = new InstantMultiHitWeapon(Item, Decorator, view.ProjectilesPerShot, controller, view);
			break;
		default:
			throw new Exception("Failed to create weapon logic!");
		}
	}

	private ProjectileWeaponDecorator CreateProjectileWeaponDecorator(int itemId, int missileTimeToDetonate)
	{
		IUnityItem itemInShop = Singleton<ItemManager>.Instance.GetItemInShop(itemId);
		GameObject gameObject = itemInShop.Create(Vector3.zero, Quaternion.identity);
		ProjectileWeaponDecorator component = gameObject.GetComponent<ProjectileWeaponDecorator>();
		if ((bool)component)
		{
			component.SetMissileTimeOut((float)missileTimeToDetonate / 1000f);
		}
		return component;
	}

	private BaseWeaponDecorator InstantiateWeaponDecorator(int itemId)
	{
		IUnityItem itemInShop = Singleton<ItemManager>.Instance.GetItemInShop(itemId);
		GameObject gameObject = itemInShop.Create(Vector3.zero, Quaternion.identity);
		return gameObject.GetComponent<BaseWeaponDecorator>();
	}

	private void ConfigureWeaponDecorator(Transform parent)
	{
		if ((bool)Decorator)
		{
			Decorator.IsEnabled = false;
			Decorator.transform.parent = parent;
			Decorator.DefaultPosition = Item.Configuration.Position;
			Decorator.DefaultAngles = Item.Configuration.Rotation;
			Decorator.CurrentPosition = Item.Configuration.Position;
			Decorator.gameObject.name = string.Concat(Slot, " ", View.ItemClass);
			Decorator.WeaponClass = View.ItemClass;
			Decorator.SetSurfaceEffect(Item.Configuration.ParticleEffect);
			LayerUtil.SetLayerRecursively(Decorator.transform, parent.gameObject.layer);
		}
		else
		{
			Debug.LogError("Failed to configure WeaponDecorator!");
		}
	}

	private void CreateWeaponInputHandler(WeaponItem item, IWeaponLogic logic, BaseWeaponDecorator decorator, bool isLocal)
	{
		switch ((WeaponSecondaryAction)View.WeaponSecondaryAction)
		{
		case WeaponSecondaryAction.ExplosionTrigger:
			InputHandler = new DefaultWeaponInputHandler(logic, isLocal, View, new GrenadeExplosionHander());
			break;
		case WeaponSecondaryAction.SniperRifle:
		{
			ZoomInfo zoomInfo2 = new ZoomInfo(View.DefaultZoomMultiplier, View.MinZoomMultiplier, View.MaxZoomMultiplier);
			InputHandler = new SniperRifleInputHandler(logic, isLocal, zoomInfo2, View);
			break;
		}
		case WeaponSecondaryAction.IronSight:
		{
			ZoomInfo zoomInfo = new ZoomInfo(View.DefaultZoomMultiplier, View.MinZoomMultiplier, View.MaxZoomMultiplier);
			InputHandler = new IronsightInputHandler(logic, isLocal, zoomInfo, View);
			break;
		}
		case WeaponSecondaryAction.Minigun:
			InputHandler = new MinigunInputHandler(logic, isLocal, decorator as MinigunWeaponDecorator, View);
			break;
		default:
			InputHandler = new DefaultWeaponInputHandler(logic, isLocal, View);
			break;
		}
	}
}
