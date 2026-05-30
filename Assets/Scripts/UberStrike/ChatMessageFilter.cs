using System;
using UberStrike.Realtime.UnitySdk;
using UnityEngine;

public static class ChatMessageFilter
{
	private class Message
	{
		public float Time;

		public string Text;

		public int Count;

		public Message(string text)
		{
			Text = text;
			Time = UnityEngine.Time.time;
		}
	}

	private static LimitedQueue<Message> _lastMessages = new LimitedQueue<Message>(5);

	public static bool IsSpamming(string message)
	{
		bool flag = false;
		bool flag2 = false;
		float num = 0f;
		float num2 = 0f;
		int num3 = 0;
		string value = string.Empty;
		foreach (Message lastMessage in _lastMessages)
		{
			if (lastMessage.Time + 5f > Time.time)
			{
				if (message.StartsWith(lastMessage.Text, StringComparison.InvariantCultureIgnoreCase))
				{
					lastMessage.Time = Time.time;
					lastMessage.Count++;
					flag = lastMessage.Count > 1;
					flag2 = true;
				}
				if (num2 != 0f)
				{
					num += Mathf.Clamp(1f - (lastMessage.Time - num2), 0f, 1f);
					num3++;
				}
			}
			num2 = lastMessage.Time;
			value = lastMessage.Text;
		}
		if (!flag2)
		{
			_lastMessages.Enqueue(new Message(message));
		}
		if (message.Equals(value, StringComparison.InvariantCultureIgnoreCase) && num2 + 10f > Time.time)
		{
			flag = true;
		}
		if (num3 > 0)
		{
			num /= (float)num3;
		}
		return flag || num > 0.3f;
	}

	public static string Cleanup(string msg)
	{
		return TextUtilities.ShortenText(TextUtilities.Trim(msg), 140, false);
	}
}
