using System.IO;
using UberStrike.Core.Serialization;
using UnityEngine;

public class MouseInputChannel : IInputChannel
{
	private bool _wasDown;

	private bool _isDown;

	private int _button;

	public int Button
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
			return ConvertMouseButtonName(_button);
		}
	}

	public InputChannelType ChannelType
	{
		get
		{
			return InputChannelType.Mouse;
		}
	}

	public float Value
	{
		get
		{
			return _isDown ? 1 : 0;
		}
		set
		{
			_isDown = (_wasDown = ((value != 0f) ? true : false));
		}
	}

	public bool IsChanged
	{
		get
		{
			return _isDown != _wasDown;
		}
	}

	public MouseInputChannel(int button)
	{
		_button = button;
	}

	public void Listen()
	{
		_wasDown = _isDown;
		_isDown = Input.GetMouseButton(_button);
	}

	public float RawValue()
	{
		return Input.GetMouseButton(_button) ? 1 : 0;
	}

	public void Reset()
	{
		_wasDown = false;
		_isDown = false;
	}

	public override string ToString()
	{
		return string.Format("Mouse {0}", _button);
	}

	public override bool Equals(object obj)
	{
		if (obj is MouseInputChannel)
		{
			MouseInputChannel mouseInputChannel = obj as MouseInputChannel;
			if (mouseInputChannel.Button == Button)
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

	private string ConvertMouseButtonName(int _button)
	{
		switch (_button)
		{
		case 0:
			return "Left Mousebutton";
		case 1:
			return "Right Mousebutton";
		default:
			return string.Format("Mouse {0}", _button);
		}
	}

	public void Serialize(MemoryStream stream)
	{
		Int32Proxy.Serialize(stream, _button);
	}

	public static MouseInputChannel FromBytes(MemoryStream stream)
	{
		int button = Int32Proxy.Deserialize(stream);
		return new MouseInputChannel(button);
	}
}
