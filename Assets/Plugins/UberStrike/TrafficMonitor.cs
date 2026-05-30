using System;
using System.Collections.Generic;
using UnityEngine;

public class TrafficMonitor
{
	private string lastEvent;

	private Dictionary<int, string> peerOperationNames = new Dictionary<int, string>();

	private Dictionary<int, string> roomOperationNames = new Dictionary<int, string>();

	private Dictionary<int, string> eventNames = new Dictionary<int, string>();

	public LinkedList<string> AllEvents { get; private set; }

	public bool IsEnabled { get; internal set; }

	internal TrafficMonitor(bool enable = true)
	{
		AllEvents = new LinkedList<string>();
		IsEnabled = enable;
	}

	public void AddEvent(string ev)
	{
		if (lastEvent == ev)
		{
			AllEvents.Last.Value += ".";
		}
		else
		{
			AllEvents.AddLast(Time.frameCount + ": " + ev);
		}
		while (AllEvents.Count >= 200)
		{
			AllEvents.RemoveFirst();
		}
		lastEvent = ev;
	}

	internal bool SendOperation(byte operationCode, Dictionary<byte, object> customOpParameters, bool sendReliable, byte channelId, bool encrypted)
	{
		if (customOpParameters.ContainsKey(0))
		{
			AddEvent("Room Operation<" + operationCode + ">: " + ((!roomOperationNames.ContainsKey(operationCode)) ? operationCode.ToString() : roomOperationNames[operationCode]));
		}
		else if (customOpParameters.ContainsKey(1))
		{
			AddEvent("Peer Operation<" + operationCode + ">: " + ((!peerOperationNames.ContainsKey(operationCode)) ? operationCode.ToString() : peerOperationNames[operationCode]));
		}
		else
		{
			AddEvent("Operation<" + operationCode + ">");
		}
		return true;
	}

	internal void OnEvent(byte eventCode, byte[] data)
	{
		AddEvent("OnEvent<" + eventCode + ">: " + ((!eventNames.ContainsKey(eventCode)) ? eventCode.ToString() : eventNames[eventCode]));
	}

	public void AddNamesForPeerOperations(Type enumType)
	{
		if (!enumType.IsEnum)
		{
			throw new ArgumentException("AddNamesForPeerOperations failed because argument must be an enumerated type");
		}
		foreach (object value in Enum.GetValues(enumType))
		{
			peerOperationNames[(int)value] = value.ToString();
		}
	}

	public void AddNamesForRoomOperations(Type enumType)
	{
		if (!enumType.IsEnum)
		{
			throw new ArgumentException("AddNamesForPeerOperations failed because argument must be an enumerated type");
		}
		foreach (object value in Enum.GetValues(enumType))
		{
			roomOperationNames[(int)value] = value.ToString();
		}
	}

	public void AddNamesForEvents(Type enumType)
	{
		if (!enumType.IsEnum)
		{
			throw new ArgumentException("AddNamesForPeerOperations failed because argument must be an enumerated type");
		}
		foreach (object value in Enum.GetValues(enumType))
		{
			eventNames[(int)value] = value.ToString();
		}
	}
}
