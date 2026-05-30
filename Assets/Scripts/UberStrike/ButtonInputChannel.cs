using System.IO;
using UberStrike.Core.Serialization;
using UnityEngine;

public class ButtonInputChannel : IInputChannel
{
	private bool _isDown;

	private bool _wasDown;

	private string _button = string.Empty;

	public string Button
	{
		get
		{
			return _button;
		}
	}

	public string Name
	{
		get
		{
			return _button;
		}
	}

	public float Value
	{
		get
		{
			return _isDown ? 1 : 0;
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
			return _isDown;
		}
	}

	public bool IsChanged
	{
		get
		{
			return _wasDown != _isDown;
		}
	}

	public ButtonInputChannel(string button)
	{
		_button = button;
	}

	public void Listen()
	{
		_wasDown = _isDown;
		_isDown = Input.GetButton(_button) && !Input.GetMouseButton(0);
	}

	public void Reset()
	{
		_wasDown = false;
		_isDown = false;
	}

	public float RawValue()
	{
		return Input.GetButton(_button) ? 1 : 0;
	}

	public override string ToString()
	{
		return _button;
	}

	public override bool Equals(object obj)
	{
		if (obj is ButtonInputChannel)
		{
			ButtonInputChannel buttonInputChannel = obj as ButtonInputChannel;
			if (buttonInputChannel.Button == Button)
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
		StringProxy.Serialize(stream, _button);
	}

	public static ButtonInputChannel FromBytes(MemoryStream stream)
	{
		string button = StringProxy.Deserialize(stream);
		return new ButtonInputChannel(button);
	}
}
