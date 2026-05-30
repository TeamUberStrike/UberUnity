using System;
using UnityEngine;

[Serializable]
public class HealthBuffConfiguration : QuickItemConfiguration
{
	private const int MaxHealth = 200;

	private const int StartHealth = 100;

	[CustomProperty("IncreaseStyle")]
	public IncreaseStyle HealthIncrease;

	[CustomProperty("Frequency")]
	public int IncreaseFrequency;

	[CustomProperty("Times")]
	public int IncreaseTimes;

	[CustomProperty("HealthPoints")]
	public int PointsGain;

	[CustomProperty("RobotDestruction")]
	public int RobotLifeTimeMilliSeconds;

	[CustomProperty("ScrapsDestruction")]
	public int ScrapsLifeTimeMilliSeconds;

	public bool IsHealNeedCharge
	{
		get
		{
			return base.WarmUpTime > 0;
		}
	}

	public bool IsHealOverTime
	{
		get
		{
			return IncreaseTimes > 0;
		}
	}

	public bool IsHealInstant
	{
		get
		{
			return !IsHealNeedCharge && !IsHealOverTime;
		}
	}

	public string GetHealthBonusDescription()
	{
		int num = ((IncreaseTimes == 0) ? 1 : IncreaseTimes);
		switch (HealthIncrease)
		{
		case IncreaseStyle.Absolute:
			return num * PointsGain + "HP";
		case IncreaseStyle.PercentFromMax:
			return Mathf.RoundToInt((float)(200 * num * PointsGain) / 100f) + "HP";
		case IncreaseStyle.PercentFromStart:
			return Mathf.RoundToInt((float)(100 * num * PointsGain) / 100f) + "HP";
		default:
			return "n/a";
		}
	}
}
