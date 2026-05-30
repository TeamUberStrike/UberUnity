using System;
using System.Collections.Generic;
using UberStrike.Core.Models;
using UberStrike.Core.Types;
using UnityEngine;

public class TabScreenPanelGUI : MonoBehaviour
{
	private Rect _rect;

	private Vector2 _panelSize;

	private Vector2 _scrollPos;

	private Vector2 _redScrollPos;

	private Vector2 _blueScrollPos;

	private static string _gameName = string.Empty;

	private static string _serverName = string.Empty;

	private static List<GameActorInfo> _redTeam;

	private static List<GameActorInfo> _blueTeam;

	private static List<GameActorInfo> _allPlayers;

	private static bool _isEnabled = false;

	public static Action<IEnumerable<GameActorInfo>> SortPlayersByRank = delegate
	{
	};

	public static bool Enabled
	{
		get
		{
			return _isEnabled;
		}
		set
		{
			if (_isEnabled != value)
			{
				_isEnabled = value;
			}
		}
	}

	public static bool ForceShow { get; set; }

	private string MapName
	{
		get
		{
			return LocalizedStrings.Map + ": " + Singleton<MapManager>.Instance.GetMapName(GameState.Current.Map.SceneName);
		}
	}

	private string ModeName
	{
		get
		{
			return LocalizedStrings.Mode + ": " + GameStateHelper.GetModeName(GameState.Current.GameMode);
		}
	}

	public static void SetPlayerListAll(List<GameActorInfo> players)
	{
		_allPlayers = players;
	}

	public static void SetPlayerListRed(List<GameActorInfo> redPlayers)
	{
		_redTeam = redPlayers;
	}

	public static void SetPlayerListBlue(List<GameActorInfo> bluePlayers)
	{
		_blueTeam = bluePlayers;
	}

	public static void SetGameName(string name)
	{
		_gameName = name;
	}

	public static void SetServerName(string name)
	{
		_serverName = name;
	}

	private void Awake()
	{
		ForceShow = false;
		_rect = default(Rect);
		_panelSize.x = 700f;
		_panelSize.y = 400f;
		_allPlayers = new List<GameActorInfo>(0);
		_redTeam = new List<GameActorInfo>(0);
		_blueTeam = new List<GameActorInfo>(0);
	}

	private void Update()
	{
		_panelSize.x = Screen.width * 7 / 8;
		_panelSize.y = Screen.height * 8 / 9;
		_rect.x = ((float)Screen.width - _panelSize.x) * 0.5f;
		_rect.y = ((float)Screen.height - _panelSize.y) * 0.5f;
		_rect.width = _panelSize.x;
		_rect.height = _panelSize.y;
		bool flag = (AutoMonoBehaviour<InputManager>.Instance.IsDown(GameInputKey.Tabscreen) || ForceShow) && GameState.Current.IsMultiplayer;
		if (Enabled != flag)
		{
			Enabled = flag;
			if (flag)
			{
				SortPlayersByRank(GameState.Current.Players.Values);
			}
		}
	}

	private void OnGUI()
	{
		if (Enabled)
		{
			GUI.FocusControl(string.Empty);
			GUI.depth = 10;
			DrawTabScreen();
		}
	}

	private void DrawTabScreen()
	{
		GUI.skin = BlueStonez.Skin;
		GUI.BeginGroup(_rect, GUIContent.none, BlueStonez.label_interparkmed_11pt);
		DoTitle(new Rect(0f, 0f, _panelSize.x, 50f));
		int num = ((0 == 0) ? 60 : 174);
		GameModeType gameMode = GameState.Current.GameMode;
		if (gameMode == GameModeType.TeamDeathMatch || gameMode == GameModeType.EliminationMode)
		{
			DoTeamStats(new Rect(0f, 46f, _panelSize.x, _panelSize.y - (float)num));
		}
		else
		{
			_scrollPos = DoAllStats(new Rect(0f, 46f, _panelSize.x, _panelSize.y - (float)num), _scrollPos, _allPlayers);
		}
		GUI.EndGroup();
	}

	private void DoTitle(Rect position)
	{
		GUI.BeginGroup(position, GUIContent.none, BlueStonez.box_overlay);
		GUI.Label(new Rect(10f, 2f, position.width - 230f, 30f), LocalizedStrings.Game + ": ", BlueStonez.label_interparkbold_18pt_left);
		GUI.contentColor = ColorScheme.UberStrikeYellow;
		GUI.Label(new Rect(65f, 2f, position.width - 280f, 30f), _gameName, BlueStonez.label_interparkbold_18pt_left);
		GUI.contentColor = Color.white;
		GUI.Label(new Rect(10f, position.height - 32f, position.width - 230f, 30f), "Server: " + _serverName, BlueStonez.label_interparkbold_18pt_left);
		GUI.Label(new Rect(position.width - 230f, 2f, 230f, 30f), MapName, BlueStonez.label_interparkbold_18pt_left);
		GUI.Label(new Rect(position.width - 230f, position.height - 32f, 220f, 30f), ModeName, BlueStonez.label_interparkbold_18pt_left);
		GUI.EndGroup();
	}

	private void DoTeamStats(Rect position)
	{
		Rect position2 = new Rect(position.x, position.y, position.width * 0.5f, position.height);
		Rect position3 = new Rect(position.x + position2.width, position2.y, position2.width, position2.height);
		_blueScrollPos = DoTeam(position2, TeamID.BLUE, _blueScrollPos, _blueTeam);
		_redScrollPos = DoTeam(position3, TeamID.RED, _redScrollPos, _redTeam);
	}

	private Vector2 DoTeam(Rect position, TeamID teamID, Vector2 scroll, List<GameActorInfo> players)
	{
		GUI.BeginGroup(position);
		Color contentColor = GUI.contentColor;
		GUI.BeginGroup(new Rect(0f, 0f, position.width, 60f), GUIContent.none, BlueStonez.box_overlay);
		GUI.color = ((teamID != TeamID.BLUE) ? ColorScheme.HudTeamRed : ColorScheme.HudTeamBlue);
		if (teamID == TeamID.RED)
		{
			GUI.Label(new Rect(10f, 6f, 200f, 32f), teamID.ToString(), BlueStonez.label_interparkbold_32pt_left);
			GUI.Label(new Rect(10f, 34f, 200f, 18f), string.Format(LocalizedStrings.NPlayers, players.Count), BlueStonez.label_interparkbold_18pt_left);
			GUI.Label(new Rect(position.width - 215f, 8f, 200f, 48f), GameState.Current.ScoreRed.ToString(), BlueStonez.label_interparkbold_48pt_right);
		}
		else
		{
			GUI.Label(new Rect(15f, 8f, 200f, 48f), GameState.Current.ScoreBlue.ToString(), BlueStonez.label_interparkbold_48pt_left);
			GUI.Label(new Rect(position.width - 210f, 6f, 200f, 32f), teamID.ToString(), BlueStonez.label_interparkbold_32pt_right);
			GUI.Label(new Rect(position.width - 210f, 34f, 200f, 18f), string.Format(LocalizedStrings.NPlayers, players.Count), BlueStonez.label_interparkbold_18pt_right);
		}
		GUI.EndGroup();
		GUI.color = contentColor;
		scroll = DoAllStats(new Rect(0f, 56f, position.width, position.height - 56f), scroll, players);
		GUI.EndGroup();
		return scroll;
	}

	private Vector2 DoAllStats(Rect position, Vector2 scroll, List<GameActorInfo> players)
	{
		int num = 8;
		int num2 = 25;
		int num3 = 25;
		int num4 = 30;
		int num5 = 32;
		int num6 = ((position.width > 540f) ? 150 : 0);
		int num7 = ((position.width > 420f) ? 50 : 0);
		int num8 = ((position.width > 450f) ? 30 : 0);
		int num9 = ((position.width > 490f) ? 40 : 0);
		int num10 = 30;
		int num11 = 50;
		int num12 = Mathf.Clamp(Mathf.RoundToInt(position.width - 30f - (float)num - (float)num2 - (float)num3 - (float)num4 - (float)num5 - (float)num6 - (float)num10 - (float)num11 - (float)num7 - (float)num8 - (float)num9), 110, 300);
		GUI.BeginGroup(position, GUIContent.none, BlueStonez.box_overlay);
		int num13 = 10 + num + num2;
		GUI.Label(new Rect(num13, 10f, num12, 18f), LocalizedStrings.Name, BlueStonez.label_interparkmed_18pt_left);
		num13 += num12;
		GUI.Label(new Rect(num13, 15f, num4, 18f), LocalizedStrings.Kills, BlueStonez.label_interparkmed_11pt_left);
		num13 += num4;
		if (num7 > 0)
		{
			GUI.Label(new Rect(num13, 15f, num7, 18f), LocalizedStrings.Deaths, BlueStonez.label_interparkmed_11pt_left);
			num13 += num7;
		}
		if (num8 > 0)
		{
			GUI.Label(new Rect(num13, 15f, num8, 18f), LocalizedStrings.KDR, BlueStonez.label_interparkmed_11pt_left);
			num13 += num8;
		}
		GUI.Label(new Rect(num13, 10f, num10, 18f), GUIContent.none, BlueStonez.label_interparkbold_16pt_left);
		num13 += num10;
		GUI.Label(new Rect(num13, 15f, num3 + 10, 18f), LocalizedStrings.Level, BlueStonez.label_interparkmed_11pt_left);
		num13 += num3;
		GUI.Label(new Rect(num13, 10f, num5 + num6, 18f), GUIContent.none, BlueStonez.label_interparkbold_16pt_left);
		num13 += num5 + num6;
		GUI.Label(new Rect(position.width - (float)num11, 10f, num11, 18f), LocalizedStrings.Ping, BlueStonez.label_interparkmed_18pt_left);
		GUI.Label(new Rect(10f, 32f, position.width - 20f, 1f), GUIContent.none, BlueStonez.horizontal_line_grey95);
		scroll = GUITools.BeginScrollView(new Rect(10f, 36f, position.width - 20f, position.height - 45f), scroll, new Rect(0f, 0f, position.width - 40f, players.Count * 36));
		int num14 = 0;
		List<string> list = new List<string>();
		foreach (GameActorInfo player in players)
		{
			num13 = num;
			GUI.BeginGroup(new Rect(0f, num14 * 36, position.width, 36f));
			if (player.Cmid == PlayerDataManager.Cmid)
			{
				GUI.color = new Color(1f, 1f, 1f, 0.3f);
				GUI.Box(new Rect(0f, 0f, position.width - 21f, 36f), GUIContent.none, BlueStonez.box_white_rounded);
				GUI.color = Color.white;
			}
			GUI.DrawTexture(new Rect(num13, 10f, 16f, 16f), UberstrikeIconsHelper.GetIconForChannel(player.Channel));
			num13 += num2;
			Color contentColor = GUI.contentColor;
			GUI.color = Color.white;
			if (!GameState.Current.HasAvatarLoaded(player.Cmid))
			{
				GUI.color = Color.gray;
			}
			else if (player.TeamID == TeamID.BLUE)
			{
				GUI.color = ColorScheme.HudTeamBlue;
			}
			else if (player.TeamID == TeamID.RED)
			{
				GUI.color = ColorScheme.HudTeamRed;
			}
			string text = ((!string.IsNullOrEmpty(player.ClanTag)) ? ("[" + player.ClanTag + "] " + player.PlayerName) : player.PlayerName);
			GUI.Label(new Rect(num13, 0f, num12, 36f), text, BlueStonez.label_interparkbold_11pt_left_wrap);
			GUI.color = contentColor;
			num13 += num12;
			GUI.Label(new Rect(num13, 0f, num4, 36f), player.Kills.ToString(), BlueStonez.label_interparkbold_11pt_left);
			num13 += num4;
			if (num7 > 0)
			{
				GUI.Label(new Rect(num13, 0f, num7, 36f), player.Deaths.ToString("N0"), BlueStonez.label_interparkbold_11pt_left);
				num13 += num7;
			}
			if (num8 > 0)
			{
				GUI.Label(new Rect(num13, 0f, num8, 36f), GetKDR(player).ToString("N1"), BlueStonez.label_interparkbold_11pt_left);
				num13 += num8;
			}
			if (!player.IsAlive)
			{
				GUI.Label(new Rect(num13, 6f, 25f, 25f), CommunicatorIcons.SkullCrossbonesIcon, BlueStonez.label_interparkbold_11pt_right);
			}
			num13 += num10;
			GUI.Label(new Rect(num13 + 5, 0f, num3, 36f), player.Level.ToString(), BlueStonez.label_interparkbold_11pt_left);
			num13 += num3 + 5;
			IUnityItem itemInShop = Singleton<ItemManager>.Instance.GetItemInShop(player.CurrentWeaponID);
			if (itemInShop != null)
			{
				list.Add(itemInShop.View.PrefabName);
				itemInShop.DrawIcon(new Rect(num13, 2f, 32f, 32f));
				num13 += num5;
				if (num6 > 0)
				{
					GUI.Label(new Rect(num13 + 10, 0f, num6, 36f), itemInShop.Name, BlueStonez.label_interparkbold_11pt_left);
					num13 += num6;
				}
			}
			else
			{
				num13 += num5;
			}
			GUI.Label(new Rect(position.width - 40f - (float)num11, 0f, num11, 36f), player.Ping.ToString(), BlueStonez.label_interparkbold_11pt_right);
			GUI.EndGroup();
			num14++;
		}
		GUITools.EndScrollView();
		GUI.EndGroup();
		return scroll;
	}

	private float GetKDR(GameActorInfo player)
	{
		return ((player.Kills <= 0) ? 1f : ((float)player.Kills)) / ((player.Deaths <= 0) ? 1f : ((float)player.Deaths));
	}
}
