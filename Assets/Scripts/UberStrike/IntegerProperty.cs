using UnityEngine;

public class IntegerProperty : Property<int>
{
	public int Min { get; private set; }

	public int Max { get; private set; }

	public override int Value
	{
		get
		{
			return base.Value;
		}
		set
		{
			base.Value = Mathf.Clamp(value, Min, Max);
		}
	}

	public IntegerProperty(int value = 0, int min = int.MinValue, int max = int.MaxValue)
	{
		Min = min;
		Max = max;
		Value = value;
	}
}
