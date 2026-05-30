using System;
using UnityEngine;

[Serializable]
public class ArmorBuffConfiguration : QuickItemConfiguration
{
	private const int MaxArmor = 200;

	private const int StartArmor = 100;

	public IncreaseStyle ArmorIncrease;

	public int IncreaseFrequency;

	public int IncreaseTimes;

	[CustomProperty("ArmorPoints")]
	public int PointsGain;

	[CustomProperty("RobotDestruction")]
	public int RobotLifeTimeMilliSeconds;

	[CustomProperty("ScrapsDestruction")]
	public int ScrapsLifeTimeMilliSeconds;

	public bool IsNeedCharge
	{
		get
		{
			return base.WarmUpTime > 0;
		}
	}

	public bool IsOverTime
	{
		get
		{
			return IncreaseTimes > 0;
		}
	}

	public bool IsInstant
	{
		get
		{
			return !IsNeedCharge && !IsOverTime;
		}
	}

	public string GetArmorBonusDescription()
	{
		int num = ((IncreaseTimes == 0) ? 1 : IncreaseTimes);
		switch (ArmorIncrease)
		{
		case IncreaseStyle.Absolute:
			return (num * PointsGain).ToString();
		case IncreaseStyle.PercentFromMax:
			return Mathf.RoundToInt((float)(200 * num * PointsGain) / 100f) + "AP";
		case IncreaseStyle.PercentFromStart:
			return Mathf.RoundToInt((float)(100 * num * PointsGain) / 100f) + "AP";
		default:
			return "n/a";
		}
	}
}
