using System.IO;
using System.Text;
using UberStrike.Core.Serialization;
using UnityEngine;

public class UserInputMap
{
	private KeyCode _prefix;

	public GameInputKey Slot { get; private set; }

	public string Description { get; private set; }

	public string Assignment
	{
		get
		{
			if (Channel == null)
			{
				return "None";
			}
			return (_prefix == KeyCode.None) ? Channel.Name : string.Format("{0} + {1}", PrintKeyCode(_prefix), Channel.Name);
		}
	}

	public IInputChannel Channel { get; set; }

	public bool IsConfigurable { get; set; }

	public float Value
	{
		get
		{
			if (Channel != null)
			{
				return (_prefix != KeyCode.None && !Input.GetKey(_prefix)) ? 0f : Channel.Value;
			}
			return 0f;
		}
	}

	public bool IsEventSender { get; private set; }

	public UserInputMap(string description, GameInputKey s, IInputChannel channel = null, bool isConfigurable = true, bool isEventSender = true, KeyCode prefix = KeyCode.None)
	{
		_prefix = prefix;
		IsConfigurable = isConfigurable;
		IsEventSender = isEventSender;
		Channel = channel;
		Slot = s;
		Description = description;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder(Description);
		stringBuilder.AppendFormat(": {0}", Channel);
		return stringBuilder.ToString();
	}

	public string GetPlayerPrefs()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			if (Channel is KeyInputChannel)
			{
				ByteProxy.Serialize(memoryStream, 0);
				Channel.Serialize(memoryStream);
			}
			else if (Channel is MouseInputChannel)
			{
				ByteProxy.Serialize(memoryStream, 1);
				Channel.Serialize(memoryStream);
			}
			else if (Channel is AxisInputChannel)
			{
				ByteProxy.Serialize(memoryStream, 2);
				Channel.Serialize(memoryStream);
			}
			else if (Channel is ButtonInputChannel)
			{
				ByteProxy.Serialize(memoryStream, 3);
				Channel.Serialize(memoryStream);
			}
			else
			{
				ByteProxy.Serialize(memoryStream, byte.MaxValue);
			}
			return WWW.EscapeURL(Encoding.ASCII.GetString(memoryStream.ToArray()), Encoding.ASCII);
		}
	}

	public void ReadPlayerPrefs(string pref)
	{
		byte[] bytes = Encoding.ASCII.GetBytes(WWW.UnEscapeURL(pref, Encoding.ASCII));
		using (MemoryStream memoryStream = new MemoryStream(bytes))
		{
			switch (ByteProxy.Deserialize(memoryStream))
			{
			case 0:
				Channel = KeyInputChannel.FromBytes(memoryStream);
				break;
			case 1:
				Channel = MouseInputChannel.FromBytes(memoryStream);
				break;
			case 2:
				Channel = AxisInputChannel.FromBytes(memoryStream);
				break;
			case 3:
				Channel = ButtonInputChannel.FromBytes(memoryStream);
				break;
			}
		}
	}

	private string PrintKeyCode(KeyCode keyCode)
	{
		switch (keyCode)
		{
		case KeyCode.LeftAlt:
			return "Left Alt";
		case KeyCode.LeftControl:
			return "Left Ctrl";
		case KeyCode.LeftShift:
			return "Left Shift";
		case KeyCode.RightAlt:
			return "Right Alt";
		case KeyCode.RightControl:
			return "Right Ctrl";
		case KeyCode.RightShift:
			return "Right Shift";
		default:
			return keyCode.ToString();
		}
	}

	public float RawValue()
	{
		if (Channel != null && (_prefix == KeyCode.None || Input.GetKey(_prefix)))
		{
			return Channel.RawValue();
		}
		return 0f;
	}
}
