using System.IO;
using UberStrike.Core.Serialization;
using UnityEngine;

public class AxisInputChannel : IInputChannel
{
	public enum AxisReadingMethod
	{
		All = 0,
		PositiveOnly = 1,
		NegativeOnly = 2
	}

	private string _axis = string.Empty;

	private string _axisName = string.Empty;

	private float _value;

	private float _lastValue;

	private float _deadRange = 0.1f;

	private AxisReadingMethod _axisReading;

	public string Axis
	{
		get
		{
			return _axis;
		}
	}

	public string Name
	{
		get
		{
			return _axisName;
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
			_value = (_lastValue = value);
		}
	}

	public InputChannelType ChannelType
	{
		get
		{
			return InputChannelType.Axis;
		}
	}

	public bool IsPressed
	{
		get
		{
			return _value != 0f;
		}
	}

	public bool IsChanged
	{
		get
		{
			return _lastValue != _value;
		}
	}

	public AxisInputChannel(string axis)
	{
		_axis = axis;
		_axisName = _axis;
	}

	public AxisInputChannel(string axis, float deadRange)
		: this(axis)
	{
		_deadRange = deadRange;
	}

	public AxisInputChannel(string axis, float deadRange, AxisReadingMethod method)
		: this(axis, deadRange)
	{
		_axisReading = method;
		switch (method)
		{
		case AxisReadingMethod.NegativeOnly:
			_axisName += " Up";
			break;
		case AxisReadingMethod.PositiveOnly:
			_axisName += " Down";
			break;
		}
	}

	public void Listen()
	{
		_lastValue = _value;
		_value = RawValue();
		switch (_axisReading)
		{
		case AxisReadingMethod.NegativeOnly:
			if (_value > 0f)
			{
				_value = 0f;
			}
			break;
		case AxisReadingMethod.PositiveOnly:
			if (_value < 0f)
			{
				_value = 0f;
			}
			break;
		}
		if (Mathf.Abs(_value) < _deadRange)
		{
			_value = 0f;
		}
	}

	public void Reset()
	{
		_value = 0f;
		_lastValue = 0f;
	}

	public float RawValue()
	{
		return Input.GetAxis(_axis);
	}

	public override string ToString()
	{
		return _axis;
	}

	public override bool Equals(object obj)
	{
		if (obj is AxisInputChannel)
		{
			AxisInputChannel axisInputChannel = obj as AxisInputChannel;
			if (axisInputChannel.Axis == Axis)
			{
				return true;
			}
		}
		return false;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public void Serialize(MemoryStream stream)
	{
		StringProxy.Serialize(stream, _axis);
		SingleProxy.Serialize(stream, _deadRange);
		EnumProxy<AxisReadingMethod>.Serialize(stream, _axisReading);
	}

	public static AxisInputChannel FromBytes(MemoryStream stream)
	{
		string axis = StringProxy.Deserialize(stream);
		float deadRange = SingleProxy.Deserialize(stream);
		AxisReadingMethod method = EnumProxy<AxisReadingMethod>.Deserialize(stream);
		return new AxisInputChannel(axis, deadRange, method);
	}
}
