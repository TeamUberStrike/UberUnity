using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class Heartbeat : MonoBehaviour
{
	public static Heartbeat Instance;

	private static string m_AuthToken;

	private static string m_HeartbeatHash;

	private bool m_MonoInitialized;

	private static List<string> m_Keys;

	[DllImport("uberheartbeat", CallingConvention = CallingConvention.Cdecl)]
	public static extern void I();

	[DllImport("uberheartbeat", CallingConvention = CallingConvention.Cdecl)]
	public static extern int S();

	[DllImport("uberheartbeat", CallingConvention = CallingConvention.Cdecl)]
	public static extern int D();

	[DllImport("uberheartbeat", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	public static extern IntPtr C(StringBuilder input);

	private IEnumerator Start()
	{
		m_MonoInitialized = false;
		I();
		yield return new WaitForSeconds(1f);
		int monoRet = S();
		m_MonoInitialized = monoRet == 0;
	}

	private void Awake()
	{
		Instance = this;
	}

	private void OnApplicationQuit()
	{
		if (m_MonoInitialized)
		{
			D();
		}
	}

	public string CheckHeartbeat(string input)
	{
		if (m_MonoInitialized)
		{
			StringBuilder input2 = new StringBuilder(input);
			IntPtr ptr = C(input2);
			return Marshal.PtrToStringAnsi(ptr);
		}
		Debug.Log("Plugin failed to init somewhere!");
		return "Failed to init";
	}
}
