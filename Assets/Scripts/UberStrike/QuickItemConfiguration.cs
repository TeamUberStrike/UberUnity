using UberStrike.Core.Models.Views;

public class QuickItemConfiguration : UberStrikeItemQuickView
{
	[CustomProperty("Amount")]
	private int _totalAmount;

	[CustomProperty("RechargeTime")]
	private int _rechargeTime;

	[CustomProperty("SlowdownOnCharge")]
	private float _slowdownOnCharge = 2f;

	public int AmountRemaining
	{
		get
		{
			return _totalAmount;
		}
		set
		{
			_totalAmount = value;
		}
	}

	public int RechargeTime
	{
		get
		{
			return _rechargeTime;
		}
	}

	public float SlowdownOnCharge
	{
		get
		{
			return _slowdownOnCharge;
		}
	}
}
