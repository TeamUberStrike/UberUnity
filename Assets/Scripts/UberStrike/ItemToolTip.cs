using System;
using Cmune.DataCenter.Common.Entities;
using UberStrike.Core.Models.Views;
using UberStrike.Core.Types;
using UnityEngine;

public class ItemToolTip : AutoMonoBehaviour<ItemToolTip>
{
	private class FloatPropertyBar
	{
		private float _value;

		private float _lastValue;

		private float _max = 1f;

		private float _time;

		public string Title { get; private set; }

		public float SmoothValue
		{
			get
			{
				return Mathf.Lerp(_lastValue, Value, (Time.time - _time) * 5f);
			}
		}

		public float Value
		{
			get
			{
				return _value;
			}
			set
			{
				_lastValue = _value;
				_time = Time.time;
				_value = value;
			}
		}

		public float Percent
		{
			get
			{
				return SmoothValue / Max;
			}
		}

		public float Max
		{
			get
			{
				return _max;
			}
			set
			{
				_max = Mathf.Max(value, 1f);
			}
		}

		public FloatPropertyBar(string title)
		{
			Title = title;
		}
	}

	private const int TextWidth = 80;

	private readonly Vector2 Size = new Vector2(260f, 240f);

	private FloatPropertyBar _ammo = new FloatPropertyBar(LocalizedStrings.Ammo);

	private FloatPropertyBar _damage = new FloatPropertyBar(LocalizedStrings.Damage);

	private FloatPropertyBar _fireRate = new FloatPropertyBar(LocalizedStrings.RateOfFire);

	private FloatPropertyBar _accuracy = new FloatPropertyBar(LocalizedStrings.Accuracy);

	private FloatPropertyBar _velocity = new FloatPropertyBar(LocalizedStrings.Velocity);

	private FloatPropertyBar _damageRadius = new FloatPropertyBar(LocalizedStrings.Radius);

	private FloatPropertyBar _armorCarried = new FloatPropertyBar(LocalizedStrings.ArmorCarried);

	private Rect _finalRect = new Rect(0f, 0f, 260f, 230f);

	private Rect _rect = new Rect(0f, 0f, 260f, 230f);

	private int _level;

	private int _daysLeft;

	private int _criticalHit;

	private string _description;

	private IUnityItem _item;

	private Rect _cacheRect;

	private float _alpha;

	private BuyingDurationType _duration;

	private Action OnDrawItemDetails;

	private Action OnDrawTip;

	public bool IsEnabled { get; private set; }

	private float Alpha
	{
		get
		{
			return Mathf.Clamp01(_alpha - Time.time);
		}
	}

	private void OnGUI()
	{
		_rect = _rect.Lerp(_finalRect, Time.deltaTime * 5f);
		if (IsEnabled && !PanelManager.IsAnyPanelOpen)
		{
			GUI.color = new Color(1f, 1f, 1f, Alpha);
			GUI.BeginGroup(_rect, BlueStonez.box_grey_outlined);
			_item.DrawIcon(new Rect(20f, 10f, 48f, 48f));
			GUI.Label(new Rect(75f, 15f, 200f, 30f), _item.View.Name, BlueStonez.label_interparkbold_13pt_left);
			GUI.Label(new Rect(20f, 70f, 220f, 50f), _description, BlueStonez.label_interparkmed_11pt_left);
			if (_duration != BuyingDurationType.None)
			{
				GUIContent content = new GUIContent(ShopUtils.PrintDuration(_duration), ShopIcons.ItemexpirationIcon);
				GUI.Label(new Rect(75f, 40f, 200f, 20f), content, BlueStonez.label_interparkbold_11pt_left);
			}
			else if (_daysLeft == 0)
			{
				GUIContent content2 = new GUIContent(ShopUtils.PrintDuration(BuyingDurationType.Permanent), ShopIcons.ItemexpirationIcon);
				GUI.Label(new Rect(75f, 40f, 200f, 20f), content2, BlueStonez.label_interparkmed_11pt_left);
			}
			else if (_daysLeft > 0)
			{
				GUIContent content3 = new GUIContent(string.Format(LocalizedStrings.NDaysLeft, _daysLeft), ShopIcons.ItemexpirationIcon);
				GUI.Label(new Rect(75f, 40f, 200f, 20f), content3, BlueStonez.label_interparkbold_11pt_left);
			}
			if (OnDrawItemDetails != null)
			{
				OnDrawItemDetails();
			}
			GUI.Label(new Rect(20f, 200f, 210f, 20f), LocalizedStrings.LevelRequired + _level, BlueStonez.label_interparkbold_11pt_left);
			if (_criticalHit > 0)
			{
				GUI.Label(new Rect(20f, 218f, 210f, 20f), LocalizedStrings.CriticalHitBonus + _criticalHit + "%", BlueStonez.label_interparkmed_11pt_left);
			}
			GUI.EndGroup();
			OnDrawTip();
			GUI.color = Color.white;
			if (_alpha - Time.time < 0f)
			{
				IsEnabled = false;
			}
		}
	}

	public void SetItem(IUnityItem item, Rect bounds, PopupViewSide side, int daysLeft = -1, BuyingDurationType duration = BuyingDurationType.None)
	{
		if (Event.current.type != EventType.Repaint || item == null || Singleton<ItemManager>.Instance.IsDefaultGearItem(item.View.PrefabName) || (item.View.LevelLock > PlayerDataManager.PlayerLevel && !Singleton<InventoryManager>.Instance.Contains(item.View.ID)))
		{
			return;
		}
		bool flag = _alpha < Time.time + 0.1f;
		_alpha = Mathf.Lerp(_alpha, Time.time + 1.1f, Time.deltaTime * 12f);
		if (_item == item && !(_cacheRect != bounds) && IsEnabled)
		{
			return;
		}
		_cacheRect = bounds;
		bounds = GUITools.ToGlobal(bounds);
		IsEnabled = true;
		_item = item;
		_level = ((item.View != null) ? item.View.LevelLock : 0);
		_description = ((item.View == null) ? string.Empty : item.View.Description);
		_daysLeft = daysLeft;
		_criticalHit = 0;
		_duration = duration;
		switch (side)
		{
		case PopupViewSide.Left:
		{
			float tipPosition2 = bounds.y - 10f + bounds.height * 0.5f;
			float x3 = bounds.x;
			Vector2 size5 = Size;
			float left2 = x3 - size5.x - 9f;
			float y2 = bounds.y;
			Vector2 size6 = Size;
			float top2 = y2 - size6.y * 0.5f;
			Vector2 size7 = Size;
			float x4 = size7.x;
			Vector2 size8 = Size;
			Rect rect3 = new Rect(left2, top2, x4, size8.y);
			Rect rect4 = new Rect(rect3.xMax - 1f, tipPosition2, 12f, 21f);
			if (rect3.y <= (float)GlobalUIRibbon.Instance.Height())
			{
				rect3.y += (float)GlobalUIRibbon.Instance.Height() - rect3.y;
			}
			if (rect3.yMax >= (float)Screen.height)
			{
				rect3.y -= rect3.yMax - (float)Screen.height;
			}
			if (rect4.y < _finalRect.y || rect4.yMax > _finalRect.yMax || _finalRect.x != rect3.x)
			{
				_finalRect = rect3;
				if (flag)
				{
					_rect = rect3;
				}
			}
			OnDrawTip = delegate
			{
				GUI.DrawTexture(new Rect(_rect.xMax - 1f, tipPosition2, 12f, 21f), ConsumableHudTextures.TooltipRight);
			};
			break;
		}
		case PopupViewSide.Top:
		{
			float tipPosition = bounds.x - 10f + bounds.width * 0.5f;
			float x = bounds.x;
			float width = bounds.width;
			Vector2 size = Size;
			float left = x + (width - size.x) * 0.5f;
			float y = bounds.y;
			Vector2 size2 = Size;
			float top = y - size2.y - 9f;
			Vector2 size3 = Size;
			float x2 = size3.x;
			Vector2 size4 = Size;
			Rect rect = new Rect(left, top, x2, size4.y);
			Rect rect2 = new Rect(tipPosition, rect.yMax - 1f, 21f, 12f);
			if (rect.xMin <= 10f)
			{
				rect.x = 10f;
			}
			if (rect.xMax >= (float)(Screen.width - 10))
			{
				rect.x -= rect.xMax - (float)Screen.width + 10f;
			}
			if (rect2.x < _finalRect.x || rect2.xMax > _finalRect.xMax || _finalRect.y != rect.y)
			{
				_finalRect = rect;
				if (flag)
				{
					_rect = rect;
				}
			}
			OnDrawTip = delegate
			{
				GUI.DrawTexture(new Rect(tipPosition, _rect.yMax - 1f, 21f, 12f), ConsumableHudTextures.TooltipDown);
			};
			break;
		}
		}
		switch (item.View.ItemClass)
		{
		case UberstrikeItemClass.GearBoots:
		case UberstrikeItemClass.GearHead:
		case UberstrikeItemClass.GearFace:
		case UberstrikeItemClass.GearUpperBody:
		case UberstrikeItemClass.GearLowerBody:
		case UberstrikeItemClass.GearGloves:
		case UberstrikeItemClass.GearHolo:
			OnDrawItemDetails = DrawGear;
			_armorCarried.Value = ((UberStrikeItemGearView)item.View).ArmorPoints;
			_armorCarried.Max = 200f;
			break;
		case UberstrikeItemClass.WeaponMelee:
		{
			OnDrawItemDetails = DrawMeleeWeapon;
			UberStrikeItemWeaponView uberStrikeItemWeaponView2 = item.View as UberStrikeItemWeaponView;
			if (uberStrikeItemWeaponView2 != null)
			{
				_damage.Value = WeaponConfigurationHelper.GetDamage(uberStrikeItemWeaponView2);
				_damage.Max = WeaponConfigurationHelper.MaxDamage;
				_fireRate.Value = WeaponConfigurationHelper.GetRateOfFire(uberStrikeItemWeaponView2);
				_fireRate.Max = WeaponConfigurationHelper.MaxRateOfFire;
			}
			break;
		}
		case UberstrikeItemClass.WeaponMachinegun:
		case UberstrikeItemClass.WeaponShotgun:
		case UberstrikeItemClass.WeaponSniperRifle:
		{
			OnDrawItemDetails = DrawInstantHitWeapon;
			UberStrikeItemWeaponView uberStrikeItemWeaponView3 = item.View as UberStrikeItemWeaponView;
			if (uberStrikeItemWeaponView3 != null)
			{
				_ammo.Value = WeaponConfigurationHelper.GetAmmo(uberStrikeItemWeaponView3);
				_ammo.Max = WeaponConfigurationHelper.MaxAmmo;
				_damage.Value = WeaponConfigurationHelper.GetDamage(uberStrikeItemWeaponView3);
				_damage.Max = WeaponConfigurationHelper.MaxDamage;
				_fireRate.Value = WeaponConfigurationHelper.GetRateOfFire(uberStrikeItemWeaponView3);
				_fireRate.Max = WeaponConfigurationHelper.MaxRateOfFire;
				_accuracy.Value = WeaponConfigurationHelper.MaxAccuracySpread - WeaponConfigurationHelper.GetAccuracySpread(uberStrikeItemWeaponView3);
				_accuracy.Max = WeaponConfigurationHelper.MaxAccuracySpread;
				if (item.View.ItemProperties.ContainsKey(ItemPropertyType.CritDamageBonus))
				{
					_criticalHit = item.View.ItemProperties[ItemPropertyType.CritDamageBonus];
				}
				else
				{
					_criticalHit = 0;
				}
			}
			break;
		}
		case UberstrikeItemClass.WeaponCannon:
		case UberstrikeItemClass.WeaponSplattergun:
		case UberstrikeItemClass.WeaponLauncher:
		{
			OnDrawItemDetails = DrawProjectileWeapon;
			UberStrikeItemWeaponView uberStrikeItemWeaponView = item.View as UberStrikeItemWeaponView;
			if (uberStrikeItemWeaponView != null)
			{
				_ammo.Value = WeaponConfigurationHelper.GetAmmo(uberStrikeItemWeaponView);
				_ammo.Max = WeaponConfigurationHelper.MaxAmmo;
				_damage.Value = WeaponConfigurationHelper.GetDamage(uberStrikeItemWeaponView);
				_damage.Max = WeaponConfigurationHelper.MaxDamage;
				_fireRate.Value = WeaponConfigurationHelper.GetRateOfFire(uberStrikeItemWeaponView);
				_fireRate.Max = WeaponConfigurationHelper.MaxRateOfFire;
				_velocity.Value = WeaponConfigurationHelper.GetProjectileSpeed(uberStrikeItemWeaponView);
				_velocity.Max = WeaponConfigurationHelper.MaxProjectileSpeed;
				_damageRadius.Value = WeaponConfigurationHelper.GetSplashRadius(uberStrikeItemWeaponView);
				_damageRadius.Max = WeaponConfigurationHelper.MaxSplashRadius;
			}
			break;
		}
		case UberstrikeItemClass.QuickUseGeneral:
		case UberstrikeItemClass.QuickUseGrenade:
		case UberstrikeItemClass.QuickUseMine:
			OnDrawItemDetails = DrawQuickItem;
			break;
		default:
			OnDrawItemDetails = null;
			break;
		}
	}

	public void ComparisonOverlay(Rect position, float value, float otherValue)
	{
		float num = position.width - 80f - 50f;
		float num2 = (num - 4f) * Mathf.Clamp01(value);
		float num3 = (num - 4f) * Mathf.Clamp01(Mathf.Abs(value - otherValue));
		GUI.BeginGroup(position);
		if (value < otherValue)
		{
			GUI.color = Color.green.SetAlpha(Alpha * 0.9f);
			GUI.Label(new Rect(82f + num2, 3f, num3, 8f), string.Empty, BlueStonez.progressbar_thumb);
		}
		else
		{
			GUI.color = Color.red.SetAlpha(Alpha * 0.9f);
			GUI.Label(new Rect(82f + num2 - num3, 3f, num3, 8f), string.Empty, BlueStonez.progressbar_thumb);
		}
		GUI.color = new Color(1f, 1f, 1f, Alpha);
		GUI.EndGroup();
	}

	public void ProgressBar(Rect position, string text, float percentage, Color barColor, string value)
	{
		float num = position.width - 80f - 50f;
		GUI.BeginGroup(position);
		GUI.Label(new Rect(0f, 0f, 80f, 14f), text, BlueStonez.label_interparkbold_11pt_left);
		GUI.Label(new Rect(80f, 1f, num, 12f), GUIContent.none, BlueStonez.progressbar_background);
		GUI.color = barColor.SetAlpha(Alpha);
		GUI.Label(new Rect(82f, 3f, (num - 4f) * Mathf.Clamp01(percentage), 8f), string.Empty, BlueStonez.progressbar_thumb);
		GUI.color = new Color(1f, 1f, 1f, Alpha);
		if (!string.IsNullOrEmpty(value))
		{
			GUI.Label(new Rect(80f + num + 10f, 0f, 40f, 14f), value, BlueStonez.label_interparkmed_10pt_left);
		}
		GUI.EndGroup();
	}

	private void DrawGear()
	{
		ProgressBar(new Rect(20f, 120f, 200f, 12f), _armorCarried.Title, _armorCarried.Percent, ColorScheme.ProgressBar, string.Empty);
	}

	private void DrawProjectileWeapon()
	{
		bool flag = Singleton<DragAndDrop>.Instance.IsDragging && ShopUtils.IsProjectileWeapon(Singleton<DragAndDrop>.Instance.DraggedItem.Item) && Singleton<DragAndDrop>.Instance.DraggedItem.Item.View.ItemClass == _item.View.ItemClass;
		ProgressBar(new Rect(20f, 120f, 200f, 12f), _damage.Title, _damage.Percent, ColorScheme.ProgressBar, string.Empty);
		ProgressBar(new Rect(20f, 135f, 200f, 12f), _fireRate.Title, 1f - _fireRate.Percent, ColorScheme.ProgressBar, string.Empty);
		ProgressBar(new Rect(20f, 150f, 200f, 12f), _velocity.Title, _velocity.Percent, ColorScheme.ProgressBar, string.Empty);
		ProgressBar(new Rect(20f, 165f, 200f, 12f), _damageRadius.Title, _damageRadius.Percent, ColorScheme.ProgressBar, string.Empty);
		ProgressBar(new Rect(20f, 180f, 200f, 12f), _ammo.Title, _ammo.Percent, ColorScheme.ProgressBar, string.Empty);
		if (flag)
		{
			UberStrikeItemWeaponView view = Singleton<DragAndDrop>.Instance.DraggedItem.Item.View as UberStrikeItemWeaponView;
			ComparisonOverlay(new Rect(20f, 120f, 200f, 12f), _damage.Percent, WeaponConfigurationHelper.GetDamageNormalized(view));
			ComparisonOverlay(new Rect(20f, 135f, 200f, 12f), 1f - _fireRate.Percent, 1f - WeaponConfigurationHelper.GetRateOfFireNormalized(view));
			ComparisonOverlay(new Rect(20f, 150f, 200f, 12f), _velocity.Percent, WeaponConfigurationHelper.GetProjectileSpeedNormalized(view));
			ComparisonOverlay(new Rect(20f, 165f, 200f, 12f), _damageRadius.Percent, WeaponConfigurationHelper.GetSplashRadiusNormalized(view));
		}
	}

	private void DrawInstantHitWeapon()
	{
		bool flag = Singleton<DragAndDrop>.Instance.IsDragging && ShopUtils.IsInstantHitWeapon(Singleton<DragAndDrop>.Instance.DraggedItem.Item) && Singleton<DragAndDrop>.Instance.DraggedItem.Item.View.ItemClass == _item.View.ItemClass;
		ProgressBar(new Rect(20f, 120f, 200f, 12f), _damage.Title, _damage.Percent, ColorScheme.ProgressBar, string.Empty);
		ProgressBar(new Rect(20f, 135f, 200f, 12f), _fireRate.Title, 1f - _fireRate.Percent, ColorScheme.ProgressBar, string.Empty);
		ProgressBar(new Rect(20f, 150f, 200f, 12f), _accuracy.Title, _accuracy.Percent, ColorScheme.ProgressBar, string.Empty);
		ProgressBar(new Rect(20f, 165f, 200f, 12f), _ammo.Title, _ammo.Percent, ColorScheme.ProgressBar, string.Empty);
		if (flag)
		{
			UberStrikeItemWeaponView view = Singleton<DragAndDrop>.Instance.DraggedItem.Item.View as UberStrikeItemWeaponView;
			ComparisonOverlay(new Rect(20f, 120f, 200f, 12f), _damage.Percent, WeaponConfigurationHelper.GetDamageNormalized(view));
			ComparisonOverlay(new Rect(20f, 135f, 200f, 12f), 1f - _fireRate.Percent, 1f - WeaponConfigurationHelper.GetRateOfFireNormalized(view));
			ComparisonOverlay(new Rect(20f, 150f, 200f, 12f), _accuracy.Percent, 1f - WeaponConfigurationHelper.GetAccuracySpreadNormalized(view));
		}
	}

	private void DrawMeleeWeapon()
	{
		ProgressBar(new Rect(20f, 120f, 200f, 12f), _damage.Title, _damage.Percent, ColorScheme.ProgressBar, string.Empty);
		ProgressBar(new Rect(20f, 135f, 200f, 12f), _fireRate.Title, 1f - _fireRate.Percent, ColorScheme.ProgressBar, string.Empty);
		if (Singleton<DragAndDrop>.Instance.IsDragging && ShopUtils.IsMeleeWeapon(Singleton<DragAndDrop>.Instance.DraggedItem.Item))
		{
			UberStrikeItemWeaponView view = Singleton<DragAndDrop>.Instance.DraggedItem.Item.View as UberStrikeItemWeaponView;
			ComparisonOverlay(new Rect(20f, 120f, 200f, 12f), _damage.Percent, WeaponConfigurationHelper.GetDamageNormalized(view));
			ComparisonOverlay(new Rect(20f, 135f, 200f, 12f), 1f - _fireRate.Percent, 1f - WeaponConfigurationHelper.GetRateOfFireNormalized(view));
		}
	}

	private void DrawQuickItem()
	{
		if (_item != null)
		{
			QuickItemConfiguration quickItemConfiguration = _item.View as QuickItemConfiguration;
			if (_item.View is HealthBuffConfiguration)
			{
				HealthBuffConfiguration healthBuffConfiguration = _item.View as HealthBuffConfiguration;
				GUI.Label(new Rect(20f, 102f, 200f, 20f), LocalizedStrings.HealthColon + healthBuffConfiguration.GetHealthBonusDescription(), BlueStonez.label_interparkbold_11pt_left);
				GUI.Label(new Rect(20f, 117f, 200f, 20f), LocalizedStrings.TimeColon + ((healthBuffConfiguration.IncreaseTimes <= 0) ? LocalizedStrings.Instant : (((float)(healthBuffConfiguration.IncreaseFrequency * healthBuffConfiguration.IncreaseTimes) / 1000f).ToString("f1") + "s")), BlueStonez.label_interparkbold_11pt_left);
			}
			else if (_item.View is AmmoBuffConfiguration)
			{
				AmmoBuffConfiguration ammoBuffConfiguration = _item.View as AmmoBuffConfiguration;
				GUI.Label(new Rect(20f, 102f, 200f, 20f), LocalizedStrings.AmmoColon + ammoBuffConfiguration.GetAmmoBonusDescription(), BlueStonez.label_interparkbold_11pt_left);
				GUI.Label(new Rect(20f, 117f, 200f, 20f), LocalizedStrings.TimeColon + ((ammoBuffConfiguration.IncreaseTimes <= 0) ? LocalizedStrings.Instant : (((float)(ammoBuffConfiguration.IncreaseFrequency * ammoBuffConfiguration.IncreaseTimes) / 1000f).ToString("f1") + "s")), BlueStonez.label_interparkbold_11pt_left);
			}
			else if (_item.View is ArmorBuffConfiguration)
			{
				ArmorBuffConfiguration armorBuffConfiguration = _item.View as ArmorBuffConfiguration;
				GUI.Label(new Rect(20f, 102f, 200f, 20f), LocalizedStrings.ArmorColon + armorBuffConfiguration.GetArmorBonusDescription(), BlueStonez.label_interparkbold_11pt_left);
				GUI.Label(new Rect(20f, 117f, 200f, 20f), LocalizedStrings.TimeColon + ((armorBuffConfiguration.IncreaseTimes <= 0) ? LocalizedStrings.Instant : (((float)(armorBuffConfiguration.IncreaseFrequency * armorBuffConfiguration.IncreaseTimes) / 1000f).ToString("f1") + "s")), BlueStonez.label_interparkbold_11pt_left);
			}
			else if (_item.View is ExplosiveGrenadeConfiguration)
			{
				ExplosiveGrenadeConfiguration explosiveGrenadeConfiguration = _item.View as ExplosiveGrenadeConfiguration;
				GUI.Label(new Rect(20f, 102f, 200f, 20f), LocalizedStrings.DamageColon + explosiveGrenadeConfiguration.Damage + "HP", BlueStonez.label_interparkbold_11pt_left);
				GUI.Label(new Rect(20f, 117f, 200f, 20f), LocalizedStrings.RadiusColon + explosiveGrenadeConfiguration.SplashRadius + "m", BlueStonez.label_interparkbold_11pt_left);
			}
			else if (_item.View is SpringGrenadeConfiguration)
			{
				SpringGrenadeConfiguration springGrenadeConfiguration = _item.View as SpringGrenadeConfiguration;
				GUI.Label(new Rect(20f, 102f, 200f, 20f), LocalizedStrings.ForceColon + springGrenadeConfiguration.Force, BlueStonez.label_interparkbold_11pt_left);
				GUI.Label(new Rect(20f, 117f, 200f, 20f), LocalizedStrings.LifetimeColon + springGrenadeConfiguration.LifeTime + "s", BlueStonez.label_interparkbold_11pt_left);
			}
			if (quickItemConfiguration != null)
			{
				GUI.Label(new Rect(20f, 132f, 200f, 20f), LocalizedStrings.WarmupColon + ((quickItemConfiguration.WarmUpTime <= 0) ? LocalizedStrings.Instant : (((float)quickItemConfiguration.WarmUpTime / 1000f).ToString("f1") + "s")), BlueStonez.label_interparkbold_11pt_left);
				GUI.Label(new Rect(20f, 147f, 200f, 20f), LocalizedStrings.CooldownColon + ((float)quickItemConfiguration.CoolDownTime / 1000f).ToString("f1") + "s", BlueStonez.label_interparkbold_11pt_left);
				GUI.Label(new Rect(20f, 162f, 200f, 20f), LocalizedStrings.UsesPerLifeColon + ((quickItemConfiguration.UsesPerLife <= 0) ? LocalizedStrings.Unlimited : quickItemConfiguration.UsesPerLife.ToString()), BlueStonez.label_interparkbold_11pt_left);
				GUI.Label(new Rect(20f, 177f, 200f, 20f), LocalizedStrings.UsesPerGameColon + ((quickItemConfiguration.UsesPerGame <= 0) ? LocalizedStrings.Unlimited : quickItemConfiguration.UsesPerGame.ToString()), BlueStonez.label_interparkbold_11pt_left);
			}
		}
	}
}
