using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UberStrike.Realtime.UnitySdk;
using UnityEngine;

public class CheatDetection : MonoBehaviour
{
	private static int _gameTime;

	private static DateTime _dateTime;

	private static List<float> _speedHack_table = new List<float>();

	private int GameTime
	{
		get
		{
			return SystemTime.Running - _gameTime;
		}
	}

	private int RealTime
	{
		get
		{
			return (int)(DateTime.Now - _dateTime).TotalMilliseconds;
		}
	}

	public static void SyncSystemTime()
	{
		_gameTime = SystemTime.Running;
		_dateTime = DateTime.Now;
	}

	private IEnumerator StartCheckSecureMemory()
	{
		while (true)
		{
			try
			{
				SecureMemoryMonitor.Instance.PerformCheck();
			}
			catch
			{
				AutoMonoBehaviour<CommConnectionManager>.Instance.DisableNetworkConnection("You have been disconnected. Please restart UberStrike.");
			}
			yield return new WaitForSeconds(10f);
		}
	}

	private IEnumerator StartNewSpeedhackDetection()
	{
		yield return new WaitForSeconds(5f);
		SyncSystemTime();
		LimitedQueue<float> timeDifference = new LimitedQueue<float>(5);
		while (true)
		{
			yield return new WaitForSeconds(5f);
			if (!GameState.Current.HasJoinedGame)
			{
				continue;
			}
			timeDifference.Enqueue((float)GameTime / (float)RealTime);
			SyncSystemTime();
			if (timeDifference.Count != 5)
			{
				continue;
			}
			float avg = averageSpeedHackResults(timeDifference);
			if (avg != -1f)
			{
				if ((double)avg >= 0.75)
				{
					break;
				}
				timeDifference.Clear();
			}
		}
		AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.Operations.SendSpeedhackDetectionNew(timeDifference.ToList());
	}

	private float averageSpeedHackResults(IEnumerable<float> list)
	{
		if (IsSpeedHacking(list))
		{
			_speedHack_table.Add(1f);
		}
		else
		{
			_speedHack_table.Add(0f);
		}
		if (_speedHack_table.Count == 10)
		{
			float num = 0f;
			foreach (float item in _speedHack_table)
			{
				float num2 = item;
				num += num2;
			}
			float result = num / (float)_speedHack_table.Count;
			_speedHack_table.Clear();
			return result;
		}
		return -1f;
	}

	private bool IsSpeedHacking(IEnumerable<float> list)
	{
		int num = 0;
		float num2 = 0f;
		foreach (float item in list)
		{
			float num3 = item;
			num2 += num3;
			num++;
		}
		num2 /= (float)num;
		float num4 = 0f;
		foreach (float item2 in list)
		{
			float num5 = item2;
			num4 += Mathf.Pow(num5 - num2, 2f);
		}
		num4 /= (float)(num - 1);
		if (num2 > 2f)
		{
			return true;
		}
		if (num2 > 1.1f && num4 < 0.02f)
		{
			return true;
		}
		return false;
	}
}
