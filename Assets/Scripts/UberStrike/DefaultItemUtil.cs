using UberStrike.Core.Models.Views;
using UberStrike.Core.Types;

public static class DefaultItemUtil
{
	public const string HeadName = "LutzDefaultGearHead";

	public const string GlovesName = "LutzDefaultGearGloves";

	public const string UpperbodyName = "LutzDefaultGearUpperBody";

	public const string LowerbodyName = "LutzDefaultGearLowerBody";

	public const string BootsName = "LutzDefaultGearBoots";

	public const string FaceName = "LutzDefaultGearFace";

	public const string MeleeName = "TheSplatbat";

	public const string HandGunName = "HandGun";

	public const string MachineGunName = "MachineGun";

	public const string SplatterGunName = "SplatterGun";

	public const string CannonName = "Cannon";

	public const string SniperRifleName = "SniperRifle";

	public const string LauncherName = "Launcher";

	public const string ShotGunName = "ShotGun";

	public const int HeadId = 1084;

	public const int GlovesId = 1086;

	public const int UpperbodyId = 1087;

	public const int LowerbodyId = 1088;

	public const int BootsId = 1089;

	public const int MeleeId = 1000;

	public const int HandgunId = 1001;

	public const int MachineGunId = 1002;

	public const int ShotGunId = 1003;

	public const int SniperRifleId = 1004;

	public const int CannonId = 1005;

	public const int SplatterGunId = 1006;

	public const int LauncherId = 1007;

	public static void ConfigureDefaultGearAndWeapons()
	{
		Singleton<ItemManager>.Instance.AddDefaultItem(new UberStrikeItemGearView
		{
			ID = 1084,
			PrefabName = "LutzDefaultGearHead"
		});
		Singleton<ItemManager>.Instance.AddDefaultItem(new UberStrikeItemGearView
		{
			ID = 1086,
			PrefabName = "LutzDefaultGearGloves"
		});
		Singleton<ItemManager>.Instance.AddDefaultItem(new UberStrikeItemGearView
		{
			ID = 1087,
			PrefabName = "LutzDefaultGearUpperBody"
		});
		Singleton<ItemManager>.Instance.AddDefaultItem(new UberStrikeItemGearView
		{
			ID = 1088,
			PrefabName = "LutzDefaultGearLowerBody"
		});
		Singleton<ItemManager>.Instance.AddDefaultItem(new UberStrikeItemGearView
		{
			ID = 1089,
			PrefabName = "LutzDefaultGearBoots"
		});
		for (UberstrikeItemClass uberstrikeItemClass = UberstrikeItemClass.WeaponMelee; uberstrikeItemClass <= UberstrikeItemClass.WeaponLauncher; uberstrikeItemClass++)
		{
			Singleton<ItemManager>.Instance.AddDefaultItem(GetDefaultWeaponView(uberstrikeItemClass));
		}
	}

	public static UberStrikeItemWeaponView GetDefaultWeaponView(UberstrikeItemClass itemClass)
	{
		switch (itemClass)
		{
		case UberstrikeItemClass.WeaponCannon:
		{
			UberStrikeItemWeaponView uberStrikeItemWeaponView = new UberStrikeItemWeaponView();
			uberStrikeItemWeaponView.ID = 1005;
			uberStrikeItemWeaponView.ItemClass = UberstrikeItemClass.WeaponCannon;
			uberStrikeItemWeaponView.PrefabName = "Cannon";
			uberStrikeItemWeaponView.DamageKnockback = 600;
			uberStrikeItemWeaponView.DamagePerProjectile = 65;
			uberStrikeItemWeaponView.AccuracySpread = 0;
			uberStrikeItemWeaponView.RecoilKickback = 12;
			uberStrikeItemWeaponView.StartAmmo = 10;
			uberStrikeItemWeaponView.MaxAmmo = 25;
			uberStrikeItemWeaponView.MissileTimeToDetonate = 5000;
			uberStrikeItemWeaponView.MissileForceImpulse = 0;
			uberStrikeItemWeaponView.MissileBounciness = 0;
			uberStrikeItemWeaponView.RateOfFire = 1000;
			uberStrikeItemWeaponView.SplashRadius = 250;
			uberStrikeItemWeaponView.ProjectilesPerShot = 1;
			uberStrikeItemWeaponView.ProjectileSpeed = 50;
			uberStrikeItemWeaponView.RecoilMovement = 32;
			uberStrikeItemWeaponView.DefaultZoomMultiplier = 1;
			uberStrikeItemWeaponView.MinZoomMultiplier = 1;
			uberStrikeItemWeaponView.MaxZoomMultiplier = 1;
			return uberStrikeItemWeaponView;
		}
		case UberstrikeItemClass.WeaponLauncher:
		{
			UberStrikeItemWeaponView uberStrikeItemWeaponView = new UberStrikeItemWeaponView();
			uberStrikeItemWeaponView.ID = 1007;
			uberStrikeItemWeaponView.ItemClass = UberstrikeItemClass.WeaponLauncher;
			uberStrikeItemWeaponView.PrefabName = "Launcher";
			uberStrikeItemWeaponView.DamageKnockback = 450;
			uberStrikeItemWeaponView.DamagePerProjectile = 70;
			uberStrikeItemWeaponView.AccuracySpread = 0;
			uberStrikeItemWeaponView.RecoilKickback = 15;
			uberStrikeItemWeaponView.StartAmmo = 15;
			uberStrikeItemWeaponView.MaxAmmo = 30;
			uberStrikeItemWeaponView.MissileTimeToDetonate = 1250;
			uberStrikeItemWeaponView.MissileForceImpulse = 0;
			uberStrikeItemWeaponView.MissileBounciness = 0;
			uberStrikeItemWeaponView.RateOfFire = 1000;
			uberStrikeItemWeaponView.SplashRadius = 400;
			uberStrikeItemWeaponView.ProjectilesPerShot = 1;
			uberStrikeItemWeaponView.ProjectileSpeed = 20;
			uberStrikeItemWeaponView.RecoilMovement = 9;
			uberStrikeItemWeaponView.DefaultZoomMultiplier = 1;
			uberStrikeItemWeaponView.MinZoomMultiplier = 1;
			uberStrikeItemWeaponView.MaxZoomMultiplier = 1;
			return uberStrikeItemWeaponView;
		}
		case UberstrikeItemClass.WeaponMachinegun:
		{
			UberStrikeItemWeaponView uberStrikeItemWeaponView = new UberStrikeItemWeaponView();
			uberStrikeItemWeaponView.ID = 1002;
			uberStrikeItemWeaponView.ItemClass = UberstrikeItemClass.WeaponMachinegun;
			uberStrikeItemWeaponView.PrefabName = "MachineGun";
			uberStrikeItemWeaponView.DamageKnockback = 50;
			uberStrikeItemWeaponView.DamagePerProjectile = 13;
			uberStrikeItemWeaponView.AccuracySpread = 3;
			uberStrikeItemWeaponView.RecoilKickback = 4;
			uberStrikeItemWeaponView.StartAmmo = 100;
			uberStrikeItemWeaponView.MaxAmmo = 300;
			uberStrikeItemWeaponView.MissileTimeToDetonate = 0;
			uberStrikeItemWeaponView.MissileForceImpulse = 0;
			uberStrikeItemWeaponView.MissileBounciness = 0;
			uberStrikeItemWeaponView.RateOfFire = 125;
			uberStrikeItemWeaponView.SplashRadius = 100;
			uberStrikeItemWeaponView.ProjectilesPerShot = 1;
			uberStrikeItemWeaponView.ProjectileSpeed = 0;
			uberStrikeItemWeaponView.RecoilMovement = 5;
			uberStrikeItemWeaponView.WeaponSecondaryAction = 2;
			uberStrikeItemWeaponView.HasAutomaticFire = true;
			uberStrikeItemWeaponView.DefaultZoomMultiplier = 2;
			uberStrikeItemWeaponView.MinZoomMultiplier = 2;
			uberStrikeItemWeaponView.MaxZoomMultiplier = 2;
			return uberStrikeItemWeaponView;
		}
		case UberstrikeItemClass.WeaponMelee:
		{
			UberStrikeItemWeaponView uberStrikeItemWeaponView = new UberStrikeItemWeaponView();
			uberStrikeItemWeaponView.ID = 1000;
			uberStrikeItemWeaponView.ItemClass = UberstrikeItemClass.WeaponMelee;
			uberStrikeItemWeaponView.PrefabName = "TheSplatbat";
			uberStrikeItemWeaponView.DamageKnockback = 1000;
			uberStrikeItemWeaponView.DamagePerProjectile = 99;
			uberStrikeItemWeaponView.AccuracySpread = 0;
			uberStrikeItemWeaponView.RecoilKickback = 0;
			uberStrikeItemWeaponView.StartAmmo = 0;
			uberStrikeItemWeaponView.MaxAmmo = 0;
			uberStrikeItemWeaponView.MissileTimeToDetonate = 0;
			uberStrikeItemWeaponView.MissileForceImpulse = 0;
			uberStrikeItemWeaponView.MissileBounciness = 0;
			uberStrikeItemWeaponView.RateOfFire = 500;
			uberStrikeItemWeaponView.SplashRadius = 100;
			uberStrikeItemWeaponView.ProjectilesPerShot = 1;
			uberStrikeItemWeaponView.ProjectileSpeed = 0;
			uberStrikeItemWeaponView.RecoilMovement = 0;
			uberStrikeItemWeaponView.HasAutomaticFire = true;
			uberStrikeItemWeaponView.DefaultZoomMultiplier = 1;
			uberStrikeItemWeaponView.MinZoomMultiplier = 1;
			uberStrikeItemWeaponView.MaxZoomMultiplier = 1;
			return uberStrikeItemWeaponView;
		}
		case UberstrikeItemClass.WeaponShotgun:
		{
			UberStrikeItemWeaponView uberStrikeItemWeaponView = new UberStrikeItemWeaponView();
			uberStrikeItemWeaponView.ID = 1003;
			uberStrikeItemWeaponView.ItemClass = UberstrikeItemClass.WeaponShotgun;
			uberStrikeItemWeaponView.PrefabName = "ShotGun";
			uberStrikeItemWeaponView.DamageKnockback = 160;
			uberStrikeItemWeaponView.DamagePerProjectile = 9;
			uberStrikeItemWeaponView.AccuracySpread = 8;
			uberStrikeItemWeaponView.RecoilKickback = 15;
			uberStrikeItemWeaponView.StartAmmo = 20;
			uberStrikeItemWeaponView.MaxAmmo = 50;
			uberStrikeItemWeaponView.MissileTimeToDetonate = 0;
			uberStrikeItemWeaponView.MissileForceImpulse = 0;
			uberStrikeItemWeaponView.MissileBounciness = 0;
			uberStrikeItemWeaponView.RateOfFire = 1000;
			uberStrikeItemWeaponView.SplashRadius = 100;
			uberStrikeItemWeaponView.ProjectilesPerShot = 11;
			uberStrikeItemWeaponView.ProjectileSpeed = 0;
			uberStrikeItemWeaponView.RecoilMovement = 10;
			uberStrikeItemWeaponView.DefaultZoomMultiplier = 1;
			uberStrikeItemWeaponView.MinZoomMultiplier = 1;
			uberStrikeItemWeaponView.MaxZoomMultiplier = 1;
			return uberStrikeItemWeaponView;
		}
		case UberstrikeItemClass.WeaponSniperRifle:
		{
			UberStrikeItemWeaponView uberStrikeItemWeaponView = new UberStrikeItemWeaponView();
			uberStrikeItemWeaponView.ID = 1004;
			uberStrikeItemWeaponView.ItemClass = UberstrikeItemClass.WeaponSniperRifle;
			uberStrikeItemWeaponView.PrefabName = "SniperRifle";
			uberStrikeItemWeaponView.DamageKnockback = 150;
			uberStrikeItemWeaponView.DamagePerProjectile = 70;
			uberStrikeItemWeaponView.AccuracySpread = 0;
			uberStrikeItemWeaponView.RecoilKickback = 12;
			uberStrikeItemWeaponView.StartAmmo = 20;
			uberStrikeItemWeaponView.MaxAmmo = 50;
			uberStrikeItemWeaponView.MissileTimeToDetonate = 0;
			uberStrikeItemWeaponView.MissileForceImpulse = 0;
			uberStrikeItemWeaponView.MissileBounciness = 0;
			uberStrikeItemWeaponView.RateOfFire = 1500;
			uberStrikeItemWeaponView.SplashRadius = 100;
			uberStrikeItemWeaponView.ProjectilesPerShot = 1;
			uberStrikeItemWeaponView.ProjectileSpeed = 0;
			uberStrikeItemWeaponView.RecoilMovement = 15;
			uberStrikeItemWeaponView.WeaponSecondaryAction = 1;
			uberStrikeItemWeaponView.DefaultZoomMultiplier = 2;
			uberStrikeItemWeaponView.MinZoomMultiplier = 2;
			uberStrikeItemWeaponView.MaxZoomMultiplier = 4;
			return uberStrikeItemWeaponView;
		}
		case UberstrikeItemClass.WeaponSplattergun:
		{
			UberStrikeItemWeaponView uberStrikeItemWeaponView = new UberStrikeItemWeaponView();
			uberStrikeItemWeaponView.ID = 1006;
			uberStrikeItemWeaponView.ItemClass = UberstrikeItemClass.WeaponSplattergun;
			uberStrikeItemWeaponView.PrefabName = "SplatterGun";
			uberStrikeItemWeaponView.DamageKnockback = 150;
			uberStrikeItemWeaponView.DamagePerProjectile = 16;
			uberStrikeItemWeaponView.AccuracySpread = 0;
			uberStrikeItemWeaponView.RecoilKickback = 0;
			uberStrikeItemWeaponView.StartAmmo = 60;
			uberStrikeItemWeaponView.MaxAmmo = 200;
			uberStrikeItemWeaponView.MissileTimeToDetonate = 5000;
			uberStrikeItemWeaponView.MissileForceImpulse = 0;
			uberStrikeItemWeaponView.MissileBounciness = 80;
			uberStrikeItemWeaponView.RateOfFire = 90;
			uberStrikeItemWeaponView.SplashRadius = 80;
			uberStrikeItemWeaponView.ProjectilesPerShot = 1;
			uberStrikeItemWeaponView.ProjectileSpeed = 70;
			uberStrikeItemWeaponView.RecoilMovement = 0;
			uberStrikeItemWeaponView.DefaultZoomMultiplier = 1;
			uberStrikeItemWeaponView.MinZoomMultiplier = 1;
			uberStrikeItemWeaponView.MaxZoomMultiplier = 1;
			return uberStrikeItemWeaponView;
		}
		default:
			return null;
		}
	}
}
