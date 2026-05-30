using System.Collections.Generic;
using Cmune.DataCenter.Common.Entities;
using UnityEngine;

public class DebugConsoleManager : MonoBehaviour
{
	private Vector2 _scrollDebug;

	private List<string> _exceptions = new List<string>(10);

	private static IDebugPage[] _debugPages = new IDebugPage[0];

	private static string[] _debugPageDescriptors = new string[0];

	private static int _currentPageSelectedIdx = 0;

	private static IDebugPage _currentPageSelected;

	public static DebugConsoleManager Instance { get; private set; }

	public bool IsDebugConsoleEnabled { get; set; }

	private void Awake()
	{
		Instance = this;
		if (Application.isEditor)
		{
			UpdatePages(MemberAccessLevel.Admin);
			return;
		}
		EventHandler.Global.AddListener(delegate(GlobalEvents.Login ev)
		{
			UpdatePages(ev.AccessLevel);
		});
	}

	private void Update()
	{
		if (KeyInput.AltPressed && KeyInput.CtrlPressed && KeyInput.GetKeyDown(KeyCode.D))
		{
			IsDebugConsoleEnabled = !IsDebugConsoleEnabled;
		}
	}

	private void DrawDebugMenuGrid()
	{
		int num = GUILayout.SelectionGrid(_currentPageSelectedIdx, _debugPageDescriptors, _debugPageDescriptors.Length, BlueStonez.tab_medium);
		if (num != _currentPageSelectedIdx)
		{
			num = (_currentPageSelectedIdx = Mathf.Clamp(num, 0, _debugPages.Length - 1));
			_currentPageSelected = _debugPages[num];
		}
	}

	private void DrawDebugPage()
	{
		_scrollDebug = GUILayout.BeginScrollView(_scrollDebug);
		if (_currentPageSelected != null)
		{
			_currentPageSelected.Draw();
		}
		GUILayout.EndScrollView();
	}

	private void UpdatePages(MemberAccessLevel level)
	{
		if (level > MemberAccessLevel.Default)
		{
			_debugPages = new IDebugPage[4]
			{
				new DebugLogMessages(),
				new DebugApplication(),
				new DebugGameState(),
				new DebugServerState()
			};
		}
		else if (level >= MemberAccessLevel.SeniorModerator)
		{
			_debugPages = new IDebugPage[5]
			{
				new DebugLogMessages(),
				new DebugApplication(),
				new DebugGameState(),
				new DebugServerState(),
				new DebugGames()
			};
		}
		else
		{
			_debugPages = new IDebugPage[1]
			{
				new DebugLogMessages()
			};
		}
		_debugPageDescriptors = new string[_debugPages.Length];
		for (int i = 0; i < _debugPages.Length; i++)
		{
			_debugPageDescriptors[i] = _debugPages[i].Title;
		}
		_currentPageSelectedIdx = 0;
		_currentPageSelected = _debugPages[0];
	}
}
