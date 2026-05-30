using UberStrike.Core.Models.Views;

public class DefaultWeaponInputHandler : WeaponInputHandler
{
	private IWeaponFireHandler _secondaryFireHandler;

	public DefaultWeaponInputHandler(IWeaponLogic logic, bool isLocal, UberStrikeItemWeaponView view, IWeaponFireHandler secondaryFireHandler = null)
		: base(logic, isLocal)
	{
		if (view.HasAutomaticFire)
		{
			base.FireHandler = new FullAutoFireHandler(logic.Decorator, WeaponConfigurationHelper.GetRateOfFire(view));
		}
		else
		{
			base.FireHandler = new SemiAutoFireHandler(logic.Decorator, WeaponConfigurationHelper.GetRateOfFire(view));
		}
		_secondaryFireHandler = secondaryFireHandler;
	}

	public override void OnPrimaryFire(bool pressed)
	{
		base.FireHandler.OnTriggerPulled(pressed);
	}

	public override void OnSecondaryFire(bool pressed)
	{
		if (_secondaryFireHandler != null)
		{
			_secondaryFireHandler.OnTriggerPulled(pressed);
		}
	}

	public override void OnPrevWeapon()
	{
	}

	public override void OnNextWeapon()
	{
	}

	public override void Update()
	{
		base.FireHandler.Update();
	}

	public override bool CanChangeWeapon()
	{
		return true;
	}

	public override void Stop()
	{
		base.FireHandler.Stop();
	}
}
