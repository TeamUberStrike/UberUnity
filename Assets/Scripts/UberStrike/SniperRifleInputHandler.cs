using UberStrike.Core.Models.Views;

public class SniperRifleInputHandler : WeaponInputHandler
{
	protected const float ZOOM = 4f;

	protected bool _scopeOpen;

	protected float _zoom;

	public SniperRifleInputHandler(IWeaponLogic logic, bool isLocal, ZoomInfo zoomInfo, UberStrikeItemWeaponView view)
		: base(logic, isLocal)
	{
		_zoomInfo = zoomInfo;
		if (view.HasAutomaticFire)
		{
			base.FireHandler = new FullAutoFireHandler(logic.Decorator, WeaponConfigurationHelper.GetRateOfFire(view));
		}
		else
		{
			base.FireHandler = new SemiAutoFireHandler(logic.Decorator, WeaponConfigurationHelper.GetRateOfFire(view));
		}
	}

	public override void OnSecondaryFire(bool pressed)
	{
		_scopeOpen = pressed;
		Update();
	}

	public override void OnPrevWeapon()
	{
		_zoom = -4f;
	}

	public override void OnNextWeapon()
	{
		_zoom = 4f;
	}

	public override void Update()
	{
		base.FireHandler.Update();
		if (_scopeOpen)
		{
			if (!LevelCamera.IsZoomedIn || _zoom != 0f)
			{
				WeaponInputHandler.ZoomIn(_zoomInfo, _weaponLogic.Decorator, _zoom, true);
				_zoom = 0f;
				EventHandler.Global.Fire(new GameEvents.PlayerZoomIn());
				GameState.Current.PlayerData.IsZoomedIn.Value = true;
			}
		}
		else if (LevelCamera.IsZoomedIn)
		{
			WeaponInputHandler.ZoomOut(_zoomInfo, _weaponLogic.Decorator);
			GameState.Current.PlayerData.IsZoomedIn.Value = false;
		}
	}

	public override bool CanChangeWeapon()
	{
		return !_scopeOpen;
	}

	public override void Stop()
	{
		base.FireHandler.Stop();
		if (_scopeOpen)
		{
			_scopeOpen = false;
			if (_isLocal)
			{
				LevelCamera.ResetZoom();
			}
		}
	}

	public override void OnPrimaryFire(bool pressed)
	{
		base.FireHandler.OnTriggerPulled(pressed);
	}
}
