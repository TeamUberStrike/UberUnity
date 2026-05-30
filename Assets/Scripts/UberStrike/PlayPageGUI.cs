using System;
using System.Collections.Generic;
using Cmune.Core.Models;
using Cmune.DataCenter.Common.Entities;
using UberStrike.Core.Models;
using UberStrike.Realtime.UnitySdk;
using UnityEngine;

public class PlayPageGUI : MonoBehaviour
{
	private enum ServerLatency
	{
		Fast = 100,
		Med = 300
	}

	private enum GameListColumns
	{
		None = 0,
		Lock = 1,
		Star = 2,
		GameName = 3,
		GameMap = 4,
		GameMode = 5,
		PlayerCount = 6,
		GameServerPing = 7,
		GameTime = 8
	}

	private enum ServerListColumns
	{
		None = 0,
		ServerName = 1,
		ServerCapacity = 2,
		ServerSpeed = 3
	}

	private class FilterSavedData
	{
		public bool UseFilter;

		public string MapName = string.Empty;

		public string GameMode = string.Empty;

		public bool NoFriendlyFire;

		public bool ISGameNotFull;

		public bool NoPasswordProtection;
	}

	private const float JoinServerButtonWidth = 130f;

	private const int GameRowHeight = 70;

	private const float _doubleClickFrame = 0.5f;

	private const int _dropDownListMap = 1;

	private const int _dropDownListGameMode = 2;

	private const int _dropDownListSingleWeapon = 4;

	public const int MAX_PLAYERS_ON_MOBILE = 6;

	private const int _mapImageWidth = 110;

	private const int _gameTimeWidth = 80;

	private const int _playerCountWidth = 80;

	private const int _joinGameWidth = 110;

	private const float _serverSpeedColumnWidth = 110f;

	private const float _serverPlayerCountColumnWidth = 130f;

	private const int ServerCheckDelay = 5;

	private Dictionary<int, DynamicTexture> _mapImages = new Dictionary<int, DynamicTexture>();

	[SerializeField]
	private Texture2D _level1GameIcon;

	[SerializeField]
	private Texture2D _level2GameIcon;

	[SerializeField]
	private Texture2D _level3GameIcon;

	[SerializeField]
	private Texture2D _level5GameIcon;

	[SerializeField]
	private Texture2D _level10GameIcon;

	[SerializeField]
	private Texture2D _level20GameIcon;

	[SerializeField]
	private Texture2D _privateGameIcon;

	[SerializeField]
	private Texture2D _sortUpArrow;

	[SerializeField]
	private Texture2D _sortDownArrow;

	private PhotonServer _currentSelectedServer;

	private string[] _mapsFilter;

	private string[] _modesFilter;

	private float _gameJoinDoubleClick;

	private float _serverJoinDoubleClick;

	private bool _refreshGameListOnFilterChange;

	private bool _refreshGameListOnSortChange;

	private Vector2 _serverScroll;

	private Vector2 _filterScroll;

	private bool _gameNotFull;

	private bool _noPrivateGames;

	private bool _instasplat;

	private bool _lowGravity;

	private bool _justForFun;

	private bool _singleWeapon;

	private bool _noFriendFire;

	private bool _showFilters;

	private string[] _weaponClassTexts;

	private int _dropDownList;

	private Rect _dropDownRect;

	private int _currentMap;

	private int _currentMode;

	private int _currentWeapon;

	private GameRoomData _selectedGame;

	private int _mapNameWidth = 135;

	private int _gameModeWidth = 170;

	private int _gameNameWidth = 200;

	private float _serverNameColumnWidth;

	private bool _unFocus;

	private bool _sortServerAscending;

	private bool _sortGamesAscending;

	private int _filteredActiveRoomCount;

	private GameListColumns _lastSortedColumn;

	private FilterSavedData _filterSavedData;

	private List<GameRoomData> _cachedGameList;

	private IComparer<GameRoomData> _gameSortingMethod;

	private Vector2 _serverSelectionHelpScrollBar;

	private Vector2 _serverSelectionScrollBar;

	private ServerListColumns _lastSortedServerColumn;

	private float _nextServerCheckTime;

	private SearchBarGUI _searchBar;

	public static PlayPageGUI Instance { get; private set; }

	private bool IsAnyFilterOn
	{
		get
		{
			if (_currentMap != 0)
			{
				return true;
			}
			if (_currentMode != 0)
			{
				return true;
			}
			if (_currentWeapon != 0)
			{
				return true;
			}
			if (_noFriendFire)
			{
				return true;
			}
			if (_gameNotFull)
			{
				return true;
			}
			if (_noPrivateGames)
			{
				return true;
			}
			if (_instasplat)
			{
				return true;
			}
			if (_lowGravity)
			{
				return true;
			}
			if (_justForFun)
			{
				return true;
			}
			if (_singleWeapon)
			{
				return true;
			}
			return false;
		}
	}

	private void Awake()
	{
		Instance = this;
		_filterSavedData = new FilterSavedData();
		_cachedGameList = new List<GameRoomData>();
		_sortGamesAscending = false;
		_gameSortingMethod = new GameDataPlayerComparer();
		_lastSortedColumn = GameListColumns.PlayerCount;
		_searchBar = new SearchBarGUI("SearchGame");
		if (_privateGameIcon == null)
		{
			throw new Exception("_privateGameIcon not assigned");
		}
	}

	private void Start()
	{
		_weaponClassTexts = new string[4]
		{
			LocalizedStrings.Machineguns,
			LocalizedStrings.SniperRifles,
			LocalizedStrings.Shotguns,
			LocalizedStrings.Launchers
		};
		_modesFilter = new string[4]
		{
			LocalizedStrings.All + " Modes",
			LocalizedStrings.DeathMatch,
			LocalizedStrings.TeamDeathMatch,
			LocalizedStrings.TeamElimination
		};
		List<string> list = new List<string>();
		list.Add(LocalizedStrings.All + " Maps");
		foreach (UberstrikeMap allMap in Singleton<MapManager>.Instance.AllMaps)
		{
			if (allMap.Id != 0)
			{
				list.Add(allMap.Name);
			}
		}
		list.RemoveAll((string s) => string.IsNullOrEmpty(s));
		_mapsFilter = list.ToArray();
	}

	private void OnEnable()
	{
		_showFilters = false;
		ResetFilters();
		_unFocus = true;
		_currentSelectedServer = Singleton<GameServerController>.Instance.SelectedServer;
	}

	private void OnGUI()
	{
		GUI.depth = 11;
		GUI.skin = BlueStonez.Skin;
		if (_unFocus)
		{
			if (GUIUtility.keyboardControl != 0)
			{
				GUIUtility.keyboardControl = 0;
			}
			_unFocus = false;
		}
		Rect rect = new Rect(0f, GlobalUIRibbon.Instance.Height(), Screen.width, Screen.height - GlobalUIRibbon.Instance.Height());
		GUI.Box(rect, string.Empty, BlueStonez.box_grey31);
		if (Singleton<GameServerController>.Instance.SelectedServer != null)
		{
			DoGamePage(rect);
		}
		else
		{
			DoServerPage(rect);
		}
		GuiManager.DrawTooltip();
	}

	private void ResetFilters()
	{
		_currentMap = 0;
		_currentMode = 0;
		_currentWeapon = 0;
		_noFriendFire = false;
		_gameNotFull = false;
		_noPrivateGames = false;
		_instasplat = false;
		_lowGravity = false;
		_justForFun = false;
		_singleWeapon = false;
		_searchBar.ClearFilter();
	}

	private void DoServerPage(Rect rect)
	{
		float num = 200f;
		GUI.BeginGroup(rect);
		GUI.Label(new Rect(0f, 0f, rect.width, 56f), LocalizedStrings.ChooseYourRegionCaps, BlueStonez.tab_strip);
		GUI.Box(new Rect(0f, 55f, rect.width, rect.height - 57f), string.Empty, BlueStonez.window_standard_grey38);
		GUI.color = new Color(1f, 1f, 1f, 0.5f);
		GUI.Label(new Rect(0f, 28f, rect.width - 5f, 28f), string.Format("{0} {1}, {2} {3} ", Singleton<GameServerManager>.Instance.AllPlayersCount, LocalizedStrings.PlayersOnline, Singleton<GameServerManager>.Instance.AllGamesCount, LocalizedStrings.Games), BlueStonez.label_interparkbold_18pt_right);
		GUI.color = Color.white;
		bool flag = GUI.enabled;
		GUI.enabled = flag && Time.time > _nextServerCheckTime;
		if (GUITools.Button(new Rect(rect.width - 150f, 6f, 140f, 23f), new GUIContent(LocalizedStrings.Refresh), BlueStonez.buttondark_medium))
		{
			RefreshServerLoad();
		}
		GUI.enabled = flag;
		DoServerList(new Rect(10f, 55f, rect.width - num - 10f, rect.height - 49f));
		DoServerHelpText(new Rect(rect.width - num, 55f, num - 10f, rect.height - 49f));
		if (GUITools.Button(new Rect(rect.width - 180f, rect.height - 42f, 160f, 32f), new GUIContent(LocalizedStrings.ExploreMaps), BlueStonez.button_white))
		{
			MenuPageManager.Instance.LoadPage(PageType.Training);
		}
		GUI.EndGroup();
	}

	private void DoServerHelpText(Rect position)
	{
		GUI.BeginGroup(position);
		GUI.Box(new Rect(0f, 0f, position.width, 32f), LocalizedStrings.HelpCaps, BlueStonez.box_grey50);
		GUI.Box(new Rect(0f, 31f, position.width, position.height - 31f - 55f), string.Empty, BlueStonez.box_grey50);
		_serverSelectionHelpScrollBar = GUITools.BeginScrollView(new Rect(0f, 33f, position.width, position.height - 31f - 60f), _serverSelectionHelpScrollBar, new Rect(0f, 0f, position.width - 20f, 400f));
		DrawGroupLabel(new Rect(5f, 5f, position.width - 25f, 100f), "1. " + LocalizedStrings.ServerName, LocalizedStrings.ServerNameDesc);
		DrawGroupLabel(new Rect(5f, 105f, position.width - 25f, 70f), "2. " + LocalizedStrings.Capacity, LocalizedStrings.CapacityDesc);
		DrawGroupLabel(new Rect(5f, 180f, position.width - 25f, 180f), "3. " + LocalizedStrings.Speed, LocalizedStrings.SpeedDesc);
		GUITools.EndScrollView();
		GUI.EndGroup();
	}

	private void DoServerList(Rect position)
	{
		_serverNameColumnWidth = position.width - 130f - 110f - 130f + 1f - 20f;
		GUI.BeginGroup(position);
		GUI.Box(new Rect(0f, 0f, _serverNameColumnWidth + 1f, 32f), string.Empty, BlueStonez.box_grey50);
		GUI.Box(new Rect(_serverNameColumnWidth, 0f, 131f, 32f), string.Empty, BlueStonez.box_grey50);
		GUI.Box(new Rect(_serverNameColumnWidth + 130f, 0f, 111f, 32f), string.Empty, BlueStonez.box_grey50);
		GUI.Box(new Rect(_serverNameColumnWidth + 130f + 130f - 20f, 0f, 150f, 32f), string.Empty, BlueStonez.box_grey50);
		GUI.Box(new Rect(0f, 31f, position.width + 1f, position.height - 31f - 55f), string.Empty, BlueStonez.box_grey50);
		if (_lastSortedServerColumn == ServerListColumns.ServerName)
		{
			if (_sortServerAscending)
			{
				GUI.Label(new Rect(5f, 0f, _serverNameColumnWidth + 1f - 5f, 32f), new GUIContent(LocalizedStrings.ServerName, _sortUpArrow), BlueStonez.label_interparkbold_18pt_left);
			}
			else
			{
				GUI.Label(new Rect(5f, 0f, _serverNameColumnWidth + 1f - 5f, 32f), new GUIContent(LocalizedStrings.ServerName, _sortDownArrow), BlueStonez.label_interparkbold_18pt_left);
			}
		}
		else
		{
			GUI.Label(new Rect(12f, 0f, _serverNameColumnWidth + 1f - 5f, 32f), LocalizedStrings.ServerName, BlueStonez.label_interparkbold_18pt_left);
		}
		if (GUI.Button(new Rect(0f, 0f, _serverNameColumnWidth + 1f - 5f, 32f), GUIContent.none, BlueStonez.label_interparkbold_11pt_left))
		{
			SortServerList(ServerListColumns.ServerName);
		}
		if (_lastSortedServerColumn == ServerListColumns.ServerCapacity)
		{
			if (_sortServerAscending)
			{
				GUI.Label(new Rect(5f + _serverNameColumnWidth, 0f, 126f, 32f), new GUIContent(LocalizedStrings.Capacity, _sortUpArrow), BlueStonez.label_interparkbold_18pt_left);
			}
			else
			{
				GUI.Label(new Rect(5f + _serverNameColumnWidth, 0f, 126f, 32f), new GUIContent(LocalizedStrings.Capacity, _sortDownArrow), BlueStonez.label_interparkbold_18pt_left);
			}
		}
		else
		{
			GUI.Label(new Rect(_serverNameColumnWidth + 12f, 0f, 126f, 32f), LocalizedStrings.Capacity, BlueStonez.label_interparkbold_18pt_left);
		}
		if (GUI.Button(new Rect(_serverNameColumnWidth, 0f, 126f, 32f), GUIContent.none, BlueStonez.label_interparkbold_11pt_left))
		{
			SortServerList(ServerListColumns.ServerCapacity);
		}
		if (_lastSortedServerColumn == ServerListColumns.ServerSpeed)
		{
			if (_sortServerAscending)
			{
				GUI.Label(new Rect(5f + _serverNameColumnWidth + 130f, 0f, 105f, 32f), new GUIContent(LocalizedStrings.Speed, _sortUpArrow), BlueStonez.label_interparkbold_18pt_left);
			}
			else
			{
				GUI.Label(new Rect(5f + _serverNameColumnWidth + 130f, 0f, 105f, 32f), new GUIContent(LocalizedStrings.Speed, _sortDownArrow), BlueStonez.label_interparkbold_18pt_left);
			}
		}
		else
		{
			GUI.Label(new Rect(_serverNameColumnWidth + 130f + 12f, 0f, 105f, 32f), LocalizedStrings.Speed, BlueStonez.label_interparkbold_18pt_left);
		}
		if (GUI.Button(new Rect(_serverNameColumnWidth + 130f, 0f, 105f, 32f), GUIContent.none, BlueStonez.label_interparkbold_11pt_left))
		{
			SortServerList(ServerListColumns.ServerSpeed);
		}
		DrawAllServers(position);
		GUI.EndGroup();
	}

	private void DrawProgressBarLarge(Rect position, float amount)
	{
		amount = Mathf.Clamp01(amount);
		GUI.Box(new Rect(position.x, position.y, position.width, 23f), GUIContent.none, BlueStonez.progressbar_large_background);
		GUI.color = ColorScheme.ProgressBar;
		GUI.Box(new Rect(position.x + 2f, position.y + 2f, Mathf.RoundToInt((position.width - 4f) * amount), 19f), GUIContent.none, BlueStonez.progressbar_large_thumb);
		GUI.color = Color.white;
	}

	private void DrawAllServers(Rect pos)
	{
		int num = Singleton<GameServerManager>.Instance.PhotonServerCount * 48;
		GUI.color = Color.white;
		_serverSelectionScrollBar = GUITools.BeginScrollView(new Rect(0f, 31f, pos.width + 1f, pos.height - 31f - 55f), _serverSelectionScrollBar, new Rect(0f, 0f, pos.width - 20f, num));
		List<string> list = new List<string>();
		int num2 = 0;
		string empty = string.Empty;
		foreach (PhotonServer photonServer in Singleton<GameServerManager>.Instance.PhotonServerList)
		{
			GUI.BeginGroup(new Rect(0f, num2 * 48, pos.width + 2f, 49f), BlueStonez.box_grey50);
			if (photonServer == _currentSelectedServer)
			{
				GUI.color = new Color(1f, 1f, 1f, 0.03f);
				GUI.DrawTexture(new Rect(1f, 0f, pos.width + 1f, 49f), UberstrikeIconsHelper.White);
				GUI.color = Color.white;
			}
			photonServer.Flag.Draw(new Rect(5f, 8f, 32f, 32f));
			list.Add(photonServer.Flag.Url);
			GUI.Label(new Rect(42f, 1f, _serverNameColumnWidth + 1f - 42f, 48f), photonServer.Name, BlueStonez.label_interparkbold_16pt_left);
			if (photonServer.Data.State == PhotonServerLoad.Status.Alive)
			{
				GUI.BeginGroup(new Rect(5f + _serverNameColumnWidth, 0f, 126f, 48f));
				int num3 = 0;
				num3 = ((PlayerDataManager.AccessLevel != MemberAccessLevel.Admin) ? Mathf.Clamp(photonServer.Data.PlayersConnected, 0, (int)photonServer.Data.MaxPlayerCount) : photonServer.Data.PlayersConnected);
				float num4 = 0f;
				DrawProgressBarLarge(amount: (!((float)num3 >= photonServer.Data.MaxPlayerCount)) ? ((float)num3 / photonServer.Data.MaxPlayerCount) : 1f, position: new Rect(2f, 12f, 58f, 20f));
				empty = string.Format("{0}/{1}", num3, photonServer.Data.MaxPlayerCount);
				GUI.Label(new Rect(64f, 14f, 60f, 20f), empty, BlueStonez.label_interparkmed_10pt_left);
				GUI.EndGroup();
				GUI.BeginGroup(new Rect(5f + _serverNameColumnWidth + 130f, 0f, 105f - (float)(((float)num > pos.height - 31f) ? 21 : 0), 48f));
				int latency = photonServer.Latency;
				empty = string.Empty;
				if (latency < 100)
				{
					GUI.color = ColorConverter.RgbToColor(80f, 99f, 42f);
					empty = LocalizedStrings.FastCaps;
				}
				else if (latency < 300)
				{
					GUI.color = ColorConverter.RgbToColor(234f, 112f, 13f);
					empty = LocalizedStrings.MedCaps;
				}
				else
				{
					GUI.color = ColorConverter.RgbToColor(192f, 80f, 70f);
					empty = LocalizedStrings.SlowCaps;
				}
				GUI.DrawTexture(new Rect(0f, 14f, 45f, 20f), UberstrikeIconsHelper.White);
				GUI.color = Color.white;
				GUI.Label(new Rect(2f, 14f, 40f, 20f), empty, BlueStonez.label_interparkbold_16pt);
				GUI.Label(new Rect(48f, 4f, 40f, 40f), string.Format("{0}ms", latency), BlueStonez.label_interparkmed_10pt_left);
				GUI.EndGroup();
				GUI.BeginGroup(new Rect(5f + _serverNameColumnWidth + 130f + 130f - 5f, 0f, 125f - (float)(((float)num > pos.height - 31f) ? 21 : 0), 48f));
				int height = BlueStonez.button.normal.background.height;
				if (GUI.Button(new Rect(2f, 24 - height / 2, 100f, height), LocalizedStrings.JoinCaps, BlueStonez.button_white))
				{
					Singleton<GameServerController>.Instance.SelectedServer = photonServer;
					SelectedServerUpdated(photonServer);
					AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.JoinServer, 0uL, 1f, 1f);
				}
				GUI.EndGroup();
			}
			else if (photonServer.Data.State == PhotonServerLoad.Status.None)
			{
				Rect position = new Rect(5f + _serverNameColumnWidth, 0f, 236f - (float)(((float)num > pos.height - 31f) ? 21 : 0), 48f);
				GUI.BeginGroup(position);
				GUI.Label(new Rect(0f, 0f, position.width, 48f), LocalizedStrings.RefreshingServer, BlueStonez.label_interparkbold_16pt);
				GUI.EndGroup();
			}
			else if (photonServer.Data.State == PhotonServerLoad.Status.NotReachable)
			{
				Rect position2 = new Rect(5f + _serverNameColumnWidth, 0f, 236f - (float)(((float)num > pos.height - 31f) ? 21 : 0), 48f);
				GUI.BeginGroup(position2);
				GUI.Label(new Rect(0f, 0f, position2.width, 48f), LocalizedStrings.ServerIsNotReachable, BlueStonez.label_interparkbold_16pt);
				GUI.EndGroup();
			}
			if (GUI.Button(new Rect(0f, 0f, pos.width + 1f, 49f), GUIContent.none, GUIStyle.none) && photonServer.Data.State != PhotonServerLoad.Status.NotReachable)
			{
				if (_currentSelectedServer == photonServer && _serverJoinDoubleClick > Time.time)
				{
					_serverJoinDoubleClick = 0f;
					SelectedServerUpdated(photonServer);
					AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.JoinServer, 0uL, 1f, 1f);
				}
				else
				{
					_serverJoinDoubleClick = Time.time + 0.5f;
				}
				_currentSelectedServer = photonServer;
			}
			GUI.EndGroup();
			num2++;
		}
		GUITools.EndScrollView();
	}

	public void SelectedServerUpdated(PhotonServer view)
	{
		if (view != null && view.Data.State == PhotonServerLoad.Status.Alive)
		{
			if (PlayerDataManager.AccessLevel >= MemberAccessLevel.Moderator)
			{
				ShowGameSelection(view);
			}
			else if ((float)view.Data.PlayersConnected >= view.Data.MaxPlayerCount)
			{
				PopupSystem.ShowMessage(LocalizedStrings.ServerFull, LocalizedStrings.ServerFullMsg);
			}
			else if (!view.CheckLatency())
			{
				PopupSystem.ShowMessage(LocalizedStrings.Warning, "Your connection to this server is too slow.", PopupSystem.AlertType.OK, null);
			}
			else if (view.Latency >= 300)
			{
				PopupSystem.ShowMessage(LocalizedStrings.Warning, LocalizedStrings.ConnectionSlowMsg, PopupSystem.AlertType.OKCancel, delegate
				{
					ShowGameSelection(view);
				}, LocalizedStrings.OkCaps, null, LocalizedStrings.CancelCaps);
			}
			else
			{
				ShowGameSelection(view);
			}
		}
		else
		{
			Debug.LogError("Couldn't connect to server!");
		}
	}

	private void DrawGroupLabel(Rect position, string header, string text)
	{
		GUI.color = Color.white;
		GUI.Label(new Rect(position.x, position.y, position.width, 16f), header, BlueStonez.label_interparkbold_13pt);
		GUI.color = new Color(1f, 1f, 1f, 0.8f);
		GUI.Label(new Rect(position.x, position.y + 16f, position.width, position.height - 16f), text, BlueStonez.label_interparkbold_11pt_left_wrap);
		GUI.color = Color.white;
	}

	private void DoGamePage(Rect rect)
	{
		GUI.BeginGroup(rect);
		GUI.Label(new Rect(0f, 0f, rect.width, 56f), LocalizedStrings.ChooseAGameCaps, BlueStonez.tab_strip);
		GUI.color = new Color(1f, 1f, 1f, 0.5f);
		GUI.Label(new Rect(10f, 28f, rect.width - 37f, 28f), string.Format("{0} ({1}ms)", Singleton<GameServerController>.Instance.SelectedServer.Name, Singleton<GameServerController>.Instance.SelectedServer.Latency.ToString()), BlueStonez.label_interparkbold_18pt_left);
		GUI.Label(new Rect(0f, 28f, rect.width - 5f, 28f), string.Format("{0} {1}, {2} {3} ", Singleton<GameListManager>.Instance.PlayersCount, LocalizedStrings.PlayersOnline, _filteredActiveRoomCount, LocalizedStrings.Games), BlueStonez.label_interparkbold_18pt_right);
		GUI.color = Color.white;
		GUI.Box(new Rect(0f, 55f, rect.width, rect.height - 57f), string.Empty, BlueStonez.window_standard_grey38);
		DrawQuickSearch(new Rect(rect.width - 150f, 8f, 142f, 20f));
		bool flag = GUI.enabled;
		GUI.enabled &= _dropDownList == 0;
		DoGameList(rect);
		DoBottomArea(rect);
		GUI.enabled = flag;
		if (_showFilters)
		{
			DoFilterArea(rect);
		}
		GUI.EndGroup();
	}

	private void DoGameList(Rect rect)
	{
		UpdateColumnWidth();
		int num = Mathf.RoundToInt(rect.height) - 73 - 104;
		if (!_showFilters)
		{
			num += 73;
		}
		Rect rect2 = new Rect(10f, 55f, rect.width - 20f, num);
		GUI.Box(rect2, string.Empty, BlueStonez.box_grey50);
		GUI.BeginGroup(rect2);
		if (Singleton<GameStateController>.Instance.Client.IsConnected)
		{
			if (!Singleton<GameStateController>.Instance.Client.IsConnectedToLobby)
			{
				GUI.Label(new Rect(0f, rect2.height * 0.5f, rect2.width, 23f), LocalizedStrings.PressRefreshToSeeCurrentGames, BlueStonez.label_interparkmed_11pt);
				if (GUITools.Button(new Rect(rect2.width * 0.5f - 70f, rect2.height * 0.5f - 30f, 140f, 23f), new GUIContent(LocalizedStrings.Refresh), BlueStonez.buttondark_medium))
				{
					Singleton<GameStateController>.Instance.Client.RefreshGameLobby();
					RefreshGameList();
				}
			}
			else if (_cachedGameList.Count == 0)
			{
				GUI.Label(new Rect(0f, 0f, rect2.width, rect2.height - 1f), "No games found.", BlueStonez.label_interparkmed_11pt);
			}
		}
		else
		{
			GUI.Label(new Rect(0f, 0f, rect2.width, rect2.height - 1f), "Lost connection to server.", BlueStonez.label_interparkmed_11pt);
		}
		num = ((Singleton<GameServerController>.Instance.SelectedServer != null) ? (70 * ((_filteredActiveRoomCount < 0 || _filteredActiveRoomCount > _cachedGameList.Count) ? _cachedGameList.Count : _filteredActiveRoomCount) + 5) : 0);
		int num2 = 0;
		int num3 = 0;
		Texture2D texture2D = null;
		texture2D = ((_lastSortedColumn != GameListColumns.GameMap) ? null : ((!_sortGamesAscending) ? _sortDownArrow : _sortUpArrow));
		num3 = ((_lastSortedColumn != GameListColumns.GameMap) ? 12 : 5);
		GUI.Box(new Rect(num2, 0f, _mapNameWidth, 25f), string.Empty, BlueStonez.box_grey50);
		GUI.Label(new Rect(num2 + num3, 0f, _mapNameWidth, 25f), new GUIContent(LocalizedStrings.Map, texture2D), BlueStonez.label_interparkbold_16pt_left);
		if (GUI.Button(new Rect(num2, 0f, _mapNameWidth, 25f), string.Empty, BlueStonez.label_interparkbold_11pt_left))
		{
			SortGameList(GameListColumns.GameMap);
		}
		num2 = 108;
		texture2D = ((_lastSortedColumn != GameListColumns.GameName) ? null : ((!_sortGamesAscending) ? _sortDownArrow : _sortUpArrow));
		num3 = ((_lastSortedColumn != GameListColumns.GameName) ? 12 : 5);
		GUI.Box(new Rect(num2, 0f, _gameNameWidth, 25f), string.Empty, BlueStonez.box_grey50);
		GUI.Label(new Rect(num2 + num3, 0f, _gameNameWidth, 25f), new GUIContent(LocalizedStrings.Name, texture2D), BlueStonez.label_interparkbold_16pt_left);
		if (GUI.Button(new Rect(num2, 0f, _gameNameWidth, 25f), GUIContent.none, BlueStonez.label_interparkbold_11pt_left))
		{
			SortGameList(GameListColumns.GameName);
		}
		num2 = 110 + _gameNameWidth - 3;
		texture2D = ((_lastSortedColumn != GameListColumns.GameMode) ? null : ((!_sortGamesAscending) ? _sortDownArrow : _sortUpArrow));
		num3 = ((_lastSortedColumn != GameListColumns.GameMode) ? 12 : 5);
		GUI.Box(new Rect(num2, 0f, _gameModeWidth, 25f), string.Empty, BlueStonez.box_grey50);
		GUI.Label(new Rect(num2 + num3, 0f, _gameModeWidth, 25f), new GUIContent(LocalizedStrings.Mode, texture2D), BlueStonez.label_interparkbold_16pt_left);
		if (GUI.Button(new Rect(num2, 0f, _gameModeWidth, 25f), string.Empty, BlueStonez.label_interparkbold_11pt_left))
		{
			SortGameList(GameListColumns.GameMode);
		}
		GUI.Box(new Rect(num2 + _gameModeWidth - 1, 0f, rect2.width - (float)(num2 + _gameModeWidth - 1), 25f), string.Empty, BlueStonez.box_grey50);
		if (Singleton<GameStateController>.Instance.Client.IsConnected)
		{
			_serverScroll = GUITools.BeginScrollView(new Rect(0f, 25f, rect2.width, rect2.height - 1f - 25f), _serverScroll, new Rect(0f, 0f, rect2.width - 60f, num), BlueStonez.horizontalScrollbar, BlueStonez.verticalScrollbar);
			_filteredActiveRoomCount = DrawAllGames(rect2, rect2.height <= (float)num);
			GUITools.EndScrollView();
		}
		GUI.EndGroup();
	}

	private void SortServerList(ServerListColumns sortedColumn, bool changeDirection = true)
	{
		if (changeDirection && sortedColumn == _lastSortedServerColumn)
		{
			_sortServerAscending = !_sortServerAscending;
		}
		_lastSortedServerColumn = sortedColumn;
		switch (sortedColumn)
		{
		case ServerListColumns.ServerName:
			Singleton<GameServerManager>.Instance.SortServers(new GameServerNameComparer(), _sortServerAscending);
			break;
		case ServerListColumns.ServerCapacity:
			Singleton<GameServerManager>.Instance.SortServers(new GameServerPlayerCountComparer(), _sortServerAscending);
			break;
		case ServerListColumns.ServerSpeed:
			Singleton<GameServerManager>.Instance.SortServers(new GameServerLatencyComparer(), _sortServerAscending);
			break;
		default:
			Singleton<GameServerManager>.Instance.SortServers(new GameServerLatencyComparer(), _sortServerAscending);
			break;
		}
	}

	private void SortGameList(GameListColumns sortedColumn)
	{
		if (sortedColumn == _lastSortedColumn)
		{
			_sortGamesAscending = !_sortGamesAscending;
		}
		_lastSortedColumn = sortedColumn;
		switch (sortedColumn)
		{
		case GameListColumns.Lock:
			SortGameList(new GameDataAccessComparer());
			break;
		case GameListColumns.GameMap:
			SortGameList(new GameDataMapComparer());
			break;
		case GameListColumns.GameMode:
			SortGameList(new GameDataRuleComparer());
			break;
		case GameListColumns.PlayerCount:
			SortGameList(new GameDataPlayerComparer());
			break;
		case GameListColumns.GameTime:
			SortGameList(new GameDataTimeComparer());
			break;
		default:
			SortGameList(new GameDataNameComparer());
			break;
		}
	}

	private void SortGameList(IComparer<GameRoomData> method)
	{
		_gameSortingMethod = method;
		RefreshGameList();
	}

	public void RefreshGameList()
	{
		bool flag = false;
		_cachedGameList.Clear();
		if (Singleton<GameListManager>.Instance.GamesCount > 0)
		{
			foreach (GameRoomData game in Singleton<GameListManager>.Instance.GameList)
			{
				_cachedGameList.Add(game);
				if (_selectedGame != null && game.Number == _selectedGame.Number)
				{
					flag = true;
				}
			}
			GameDataComparer.SortAscending = _sortGamesAscending;
			_cachedGameList.Sort(_gameSortingMethod);
		}
		if (!flag)
		{
			_selectedGame = null;
		}
	}

	private void DoFilterArea(Rect rect)
	{
		bool flag = GUI.enabled;
		Rect position = new Rect(10f, rect.height - 73f - 50f, rect.width - 20f, 74f);
		GUI.Box(position, string.Empty, BlueStonez.box_grey50);
		GUI.BeginGroup(new Rect(position.x, position.y, position.width, position.width + 60f));
		GUI.enabled = flag && (_dropDownList == 0 || _dropDownList == 1);
		GUI.Label(new Rect(10f, 10f, 115f, 21f), _mapsFilter[_currentMap], BlueStonez.label_dropdown);
		if (GUI.Button(new Rect(123f, 9f, 21f, 21f), GUIContent.none, BlueStonez.dropdown_button))
		{
			_dropDownList = ((_dropDownList == 0) ? 1 : 0);
			_dropDownRect = new Rect(10f, 31f, 133f, 80f);
		}
		GUI.enabled = flag && (_dropDownList == 0 || _dropDownList == 2);
		GUI.Label(new Rect(10f, 42f, 115f, 21f), _modesFilter[_currentMode], BlueStonez.label_dropdown);
		if (GUI.Button(new Rect(123f, 41f, 21f, 21f), GUIContent.none, BlueStonez.dropdown_button))
		{
			_dropDownList = ((_dropDownList == 0) ? 2 : 0);
			_dropDownRect = new Rect(10f, 63f, 133f, 60f);
		}
		GUI.enabled = flag && _dropDownList == 0;
		_gameNotFull = GUI.Toggle(new Rect(165f, 7f, 170f, 16f), _gameNotFull, LocalizedStrings.GameNotFull, BlueStonez.toggle);
		_noPrivateGames = GUI.Toggle(new Rect(165f, 28f, 170f, 16f), _noPrivateGames, LocalizedStrings.NotPasswordProtected, BlueStonez.toggle);
		GUI.enabled = false;
		if (CheckChangesInFilter())
		{
			_serverScroll = new Vector2(0f, 0f);
			if (!CanPassFilter(_selectedGame))
			{
				_selectedGame = null;
			}
			RefreshGameList();
		}
		GUI.enabled = flag;
		if (_dropDownList != 0)
		{
			DoDropDownList();
		}
		GUI.EndGroup();
	}

	private bool CheckChangesInFilter()
	{
		bool result = false;
		if (_filterSavedData.UseFilter != _showFilters)
		{
			_filterSavedData.UseFilter = _showFilters;
			result = true;
		}
		if (_filterSavedData.MapName != _mapsFilter[_currentMap])
		{
			_filterSavedData.MapName = _mapsFilter[_currentMap];
			result = true;
		}
		if (_filterSavedData.GameMode != _modesFilter[_currentMode])
		{
			_filterSavedData.GameMode = _modesFilter[_currentMode];
			result = true;
		}
		if (_filterSavedData.NoFriendlyFire != _noFriendFire)
		{
			_filterSavedData.NoFriendlyFire = _noFriendFire;
			result = true;
		}
		if (_filterSavedData.ISGameNotFull != _gameNotFull)
		{
			_filterSavedData.ISGameNotFull = _gameNotFull;
			result = true;
		}
		if (_filterSavedData.NoPasswordProtection != _noPrivateGames)
		{
			_filterSavedData.NoPasswordProtection = _noPrivateGames;
			result = true;
		}
		return result;
	}

	private void DoBottomArea(Rect rect)
	{
		GUITools.PushGUIState();
		GUI.enabled = _dropDownList == 0;
		bool showFilters = _showFilters;
		_showFilters = GUI.Toggle(new Rect(22f, rect.height - 42f, 120f, 32f), _showFilters, LocalizedStrings.FiltersCaps, BlueStonez.button);
		if (showFilters != _showFilters)
		{
			Singleton<GameStateController>.Instance.Client.RefreshGameLobby();
		}
		if (_showFilters && IsAnyFilterOn && GUITools.Button(new Rect(153f, rect.height - 42f, 145f, 32f), new GUIContent(LocalizedStrings.ResetFiltersCaps), BlueStonez.button))
		{
			Singleton<GameStateController>.Instance.Client.RefreshGameLobby();
			ResetFilters();
		}
		if (!_showFilters && _filterSavedData.UseFilter)
		{
			_filterSavedData.UseFilter = false;
		}
		if (!_refreshGameListOnFilterChange && IsAnyFilterOn)
		{
			RefreshGameList();
			_refreshGameListOnFilterChange = true;
		}
		if (_refreshGameListOnFilterChange && !IsAnyFilterOn)
		{
			RefreshGameList();
			_refreshGameListOnFilterChange = false;
		}
		GUI.enabled = true;
		if (GUITools.Button(new Rect(rect.width - 160f, rect.height - 42f, 140f, 32f), new GUIContent(LocalizedStrings.CreateGameCaps), BlueStonez.button))
		{
			PanelManager.Instance.OpenPanel(PanelType.CreateGame);
		}
		GUI.enabled = Singleton<GameStateController>.Instance.Client.IsConnected && _selectedGame != null && Singleton<GameServerController>.Instance.SelectedServer != null && Singleton<GameServerController>.Instance.SelectedServer.Data.RoomsCreated != 0 && !PanelManager.Instance.IsPanelOpen(PanelType.CreateGame);
		GUITools.PopGUIState();
	}

	private void DrawQuickSearch(Rect rect)
	{
		_searchBar.Draw(rect);
		if (!_refreshGameListOnSortChange && _searchBar.FilterText.Length > 0)
		{
			Singleton<GameStateController>.Instance.Client.RefreshGameLobby();
			RefreshGameList();
			_refreshGameListOnSortChange = true;
		}
		if (_refreshGameListOnSortChange && _searchBar.FilterText.Length == 0)
		{
			RefreshGameList();
			_refreshGameListOnSortChange = false;
		}
	}

	private void DoDropDownList()
	{
		string[] array = null;
		switch (_dropDownList)
		{
		case 1:
			array = _mapsFilter;
			break;
		case 2:
			array = _modesFilter;
			break;
		case 4:
			array = _weaponClassTexts;
			break;
		default:
			Debug.LogError("Nondefined drop down list: " + _dropDownList);
			return;
		}
		GUI.Box(_dropDownRect, string.Empty, BlueStonez.window);
		_filterScroll = GUITools.BeginScrollView(_dropDownRect, _filterScroll, new Rect(0f, 0f, _dropDownRect.width - 20f, 20 * array.Length));
		for (int i = 0; i < array.Length; i++)
		{
			GUI.Label(new Rect(2f, 20 * i, _dropDownRect.width, 20f), array[i], BlueStonez.dropdown_list);
			if (GUI.Button(new Rect(2f, 20 * i, _dropDownRect.width, 20f), string.Empty, BlueStonez.dropdown_list))
			{
				switch (_dropDownList)
				{
				case 1:
					_currentMap = i;
					break;
				case 2:
					_currentMode = i;
					break;
				case 4:
					_currentWeapon = i;
					break;
				}
				_dropDownList = 0;
				_filterScroll.y = 0f;
			}
		}
		GUITools.EndScrollView();
	}

	private void JoinGame(GameRoomData game)
	{
		if (game == null)
		{
			return;
		}
		if (ApplicationDataManager.IsMobile && game.PlayerLimit > 6)
		{
			PopupSystem.ShowMessage(LocalizedStrings.Warning, LocalizedStrings.MobileGameMoreThan8Players, PopupSystem.AlertType.OKCancel, delegate
			{
				Singleton<GameStateController>.Instance.JoinNetworkGame(game);
			}, LocalizedStrings.OkCaps, null, LocalizedStrings.CancelCaps);
		}
		else
		{
			Singleton<GameStateController>.Instance.JoinNetworkGame(game);
		}
	}

	private bool CanPassFilter(GameRoomData game)
	{
		if (game == null)
		{
			return false;
		}
		GameFlags gameFlags = new GameFlags();
		gameFlags.SetFlags(game.GameFlags);
		bool flag = _searchBar.CheckIfPassFilter(game.Name);
		bool flag2 = true;
		bool flag3 = true;
		bool flag4 = true;
		bool flag5 = true;
		if (ApplicationDataManager.IsMobile && GameRoomHelper.HasLevelRestriction(game))
		{
			return false;
		}
		if (!Singleton<MapManager>.Instance.MapExistsWithId(game.MapID))
		{
			return false;
		}
		flag2 = _mapsFilter[_currentMap] == LocalizedStrings.All + " Maps" || Singleton<MapManager>.Instance.GetMapName(game.MapID) == _mapsFilter[_currentMap];
		flag3 = _modesFilter[_currentMode] == LocalizedStrings.All + " Modes" || GameStateHelper.GetModeName(game.GameMode) == _modesFilter[_currentMode];
		if (_gameNotFull)
		{
			flag4 = !game.IsFull;
		}
		if (_noPrivateGames)
		{
			flag5 = !game.IsPasswordProtected;
		}
		if (_showFilters)
		{
			return flag && flag2 && flag3 && flag4 && flag5 && _showFilters;
		}
		return flag;
	}

	private string DisplayMapIcon(int mapId, Rect rect)
	{
		string mapSceneName = Singleton<MapManager>.Instance.GetMapSceneName(mapId);
		string text = ApplicationDataManager.ImagePath + "maps/" + mapSceneName + ".jpg";
		if (_mapImages.ContainsKey(mapId))
		{
			_mapImages[mapId].Draw(rect);
		}
		else
		{
			DynamicTexture dynamicTexture = new DynamicTexture(text);
			_mapImages[mapId] = dynamicTexture;
			dynamicTexture.Draw(rect);
		}
		return text;
	}

	private int DrawAllGames(Rect rect, bool hasVScroll)
	{
		int playerLevel = PlayerDataManager.PlayerLevel;
		List<string> list = new List<string>();
		int num = 0;
		foreach (GameRoomData cachedGame in _cachedGameList)
		{
			if (!CanPassFilter(cachedGame))
			{
				continue;
			}
			bool flag = GameRoomHelper.CanJoinGame(cachedGame);
			bool flag2 = GUI.enabled;
			GUI.enabled = flag2 && flag && _dropDownList == 0;
			int num2 = 70 * num - 1;
			GUI.Box(new Rect(0f, num2, rect.width, 71f), new GUIContent(string.Empty), BlueStonez.box_grey50);
			if (!ApplicationDataManager.IsMobile)
			{
				string tooltip = LocalizedStrings.PlayCaps;
				if (!GameRoomHelper.IsLevelAllowed(cachedGame, playerLevel) && cachedGame.LevelMin > playerLevel)
				{
					tooltip = string.Format(LocalizedStrings.YouHaveToReachLevelNToJoinThisGame, cachedGame.LevelMin);
				}
				else if (!GameRoomHelper.IsLevelAllowed(cachedGame, playerLevel) && cachedGame.LevelMax < playerLevel)
				{
					tooltip = string.Format(LocalizedStrings.YouAlreadyMasteredThisLevel);
				}
				else if (cachedGame.IsFull)
				{
					tooltip = string.Format(LocalizedStrings.ThisGameIsFull);
				}
				GUI.Box(new Rect(0f, num2, rect.width, 70f), new GUIContent(string.Empty, tooltip), BlueStonez.box_grey50);
			}
			if (_selectedGame != null && _selectedGame.Number == cachedGame.Number)
			{
				GUI.color = new Color(1f, 1f, 1f, 0.03f);
				GUI.DrawTexture(new Rect(1f, num2, rect.width + 1f, 70f), UberstrikeIconsHelper.White);
				GUI.color = Color.white;
			}
			GUIStyle style = ((!flag) ? BlueStonez.label_interparkmed_10pt_left : BlueStonez.label_interparkbold_11pt_left);
			GUI.color = ((!GameRoomHelper.HasLevelRestriction(cachedGame)) ? Color.white : new Color(1f, 0.7f, 0f));
			int num3 = 0;
			string item = DisplayMapIcon(cachedGame.MapID, new Rect(num3, num2, 110f, 70f));
			list.Add(item);
			if (cachedGame.IsPermanentGame && GameRoomHelper.HasLevelRestriction(cachedGame))
			{
				if (cachedGame.LevelMax <= 5)
				{
					GUI.DrawTexture(new Rect(80f, num2 + 70 - 30, 25f, 25f), _level1GameIcon);
				}
				else if (cachedGame.LevelMax <= 10)
				{
					GUI.DrawTexture(new Rect(80f, num2 + 70 - 30, 25f, 25f), _level2GameIcon);
				}
				else if (cachedGame.LevelMax <= 20)
				{
					GUI.DrawTexture(new Rect(80f, num2 + 70 - 30, 25f, 25f), _level3GameIcon);
				}
				else if (cachedGame.LevelMin >= 40)
				{
					GUI.DrawTexture(new Rect(80f, num2 + 70 - 30, 25f, 25f), _level20GameIcon);
				}
				else if (cachedGame.LevelMin >= 30)
				{
					GUI.DrawTexture(new Rect(80f, num2 + 70 - 30, 25f, 25f), _level10GameIcon);
				}
				else if (cachedGame.LevelMin >= 20)
				{
					GUI.DrawTexture(new Rect(80f, num2 + 70 - 30, 25f, 25f), _level5GameIcon);
				}
				if (playerLevel > cachedGame.LevelMax)
				{
					GUI.DrawTexture(new Rect(0f, num2 + 70 - 50, 50f, 50f), UberstrikeIcons.LevelMastered);
				}
			}
			if (cachedGame.IsPasswordProtected)
			{
				GUI.DrawTexture(new Rect(80f, num2 + 70 - 30, 25f, 25f), _privateGameIcon);
			}
			GUI.color = ((!GameRoomHelper.HasLevelRestriction(cachedGame)) ? Color.white : new Color(1f, 0.7f, 0f));
			num3 = 120;
			GUI.Label(new Rect(num3, num2, _gameNameWidth, 35f), cachedGame.Name, BlueStonez.label_interparkbold_13pt_left);
			GUI.Label(new Rect(num3, num2 + 35, _gameNameWidth, 35f), Singleton<MapManager>.Instance.GetMapName(cachedGame.MapID) + " " + LevelRestrictionText(cachedGame), BlueStonez.label_interparkmed_10pt_left);
			num3 = 122 + _gameNameWidth - 4;
			int num4 = cachedGame.TimeLimit / 60;
			GUI.Label(new Rect(num3, num2, _gameModeWidth, 35f), GameStateHelper.GetModeName(cachedGame.GameMode) + " - " + num4 + " mins Mods: " + GetGameFlagText(cachedGame), BlueStonez.label_interparkmed_10pt_left);
			GUI.Label(new Rect(num3 + 64, num2 + 35, _gameModeWidth, 35f), string.Format("{0}/{1} players", cachedGame.ConnectedPlayers, cachedGame.PlayerLimit), style);
			GUI.color = Color.white;
			DrawProgressBarLarge(new Rect(num3, num2 + 35 + 5, 58f, 35f), (float)cachedGame.ConnectedPlayers / (float)cachedGame.PlayerLimit);
			num3 = 110 + _gameNameWidth + _gameModeWidth - 6;
			int height = BlueStonez.button.normal.background.height;
			if (GUI.Button(new Rect(num3, num2 + 35 - height / 2, 90f, height), LocalizedStrings.JoinCaps, BlueStonez.button_white))
			{
				JoinGame(cachedGame);
				AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.JoinServer, 0uL, 1f, 1f);
			}
			if (GUI.Button(new Rect(0f, num2, rect.width, 70f), string.Empty, BlueStonez.label_interparkbold_11pt_left))
			{
				Singleton<GameStateController>.Instance.Client.RefreshGameLobby();
				if (_selectedGame != null && _selectedGame.Number == cachedGame.Number && _gameJoinDoubleClick > Time.time)
				{
					_gameJoinDoubleClick = 0f;
					JoinGame(_selectedGame);
					AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.JoinServer, 0uL, 1f, 1f);
				}
				else
				{
					_gameJoinDoubleClick = Time.time + 0.5f;
				}
				_selectedGame = cachedGame;
			}
			num++;
			GUI.color = Color.white;
			GUI.enabled = flag2;
		}
		if (num == 0 && Singleton<GameServerController>.Instance.SelectedServer != null && Singleton<GameServerController>.Instance.SelectedServer.Data.RoomsCreated > 0 && _cachedGameList.Count > 0)
		{
			GUI.Label(new Rect(0f, rect.height * 0.5f, rect.width, 23f), "No games running on this server", BlueStonez.label_interparkmed_11pt);
			if (GUITools.Button(new Rect(rect.width * 0.5f - 70f, rect.height * 0.5f - 30f, 140f, 23f), new GUIContent(LocalizedStrings.CreateGameCaps), BlueStonez.button))
			{
				PanelManager.Instance.OpenPanel(PanelType.CreateGame);
			}
		}
		return num;
	}

	public static string GetGameFlagText(GameRoomData data)
	{
		string result = "None";
		if (GameFlags.IsFlagSet(GameFlags.GAME_FLAGS.NoArmor, data.GameFlags))
		{
			result = "No Armor";
		}
		else if (GameFlags.IsFlagSet(GameFlags.GAME_FLAGS.LowGravity, data.GameFlags))
		{
			result = "Low Gravity";
		}
		else if (GameFlags.IsFlagSet(GameFlags.GAME_FLAGS.QuickSwitch, data.GameFlags))
		{
			result = "Quick Switching";
		}
		else if (GameFlags.IsFlagSet(GameFlags.GAME_FLAGS.MeleeOnly, data.GameFlags))
		{
			result = "Melee Only";
		}
		return result;
	}

	private string LevelRestrictionText(GameRoomData m)
	{
		if (GameRoomHelper.HasLevelRestriction(m))
		{
			if (m.LevelMax == m.LevelMin)
			{
				return string.Format(LocalizedStrings.PlayerLevelNRestriction, m.LevelMin);
			}
			if (m.LevelMax == 0)
			{
				return string.Format(LocalizedStrings.PlayerLevelNPlusRestriction, m.LevelMin);
			}
			if (m.LevelMin == 0)
			{
				return string.Format(LocalizedStrings.PlayerLevelNMinusRestriction, m.LevelMax + 1);
			}
			return string.Format(LocalizedStrings.PlayerLevelNToNRestriction, m.LevelMin, m.LevelMax);
		}
		return string.Empty;
	}

	private void UpdateColumnWidth()
	{
		int num = Screen.width - 40;
		_gameNameWidth = num - 110 - _gameModeWidth - 110;
	}

	public void Show()
	{
		if (_currentSelectedServer != null)
		{
			ShowGameSelection(_currentSelectedServer);
		}
		else
		{
			ShowServerSelection();
		}
	}

	private void ShowGameSelection(PhotonServer server)
	{
		if (server != null)
		{
			Singleton<GameServerController>.Instance.SelectedServer = server;
			_cachedGameList.Clear();
			Singleton<GameStateController>.Instance.Client.EnterGameLobby(Singleton<GameServerController>.Instance.SelectedServer.ConnectionString);
		}
	}

	public void ShowServerSelection()
	{
		Singleton<GameServerController>.Instance.SelectedServer = null;
		Singleton<GameStateController>.Instance.Client.Disconnect();
		if (_lastSortedServerColumn == ServerListColumns.None)
		{
			_lastSortedServerColumn = ServerListColumns.ServerSpeed;
			SortServerList(_lastSortedServerColumn, false);
		}
		RefreshServerLoad();
	}

	public void Hide()
	{
	}

	private void RefreshServerLoad()
	{
		if (_nextServerCheckTime < Time.time)
		{
			_nextServerCheckTime = Time.time + 5f;
			StartCoroutine(Singleton<GameServerManager>.Instance.StartUpdatingServerLoads());
		}
	}
}
