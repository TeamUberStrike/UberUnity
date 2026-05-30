using System;
using System.Collections.Generic;
using Cmune.DataCenter.Common.Entities;
using UnityEngine;

public class ModerationPanelGUI : PanelGuiBase
{
	private enum Actions
	{
		NONE = 0,
		UNMUTE_PLAYER = 1,
		GHOST_PLAYER = 2,
		MUTE_PLAYER = 3,
		KICK_FROM_GAME = 5,
		KICK_FROM_APP = 6
	}

	private class Moderation
	{
		public MemberAccessLevel Level { get; private set; }

		public Actions ID { get; private set; }

		public string Title { get; private set; }

		public string Content { get; private set; }

		public string Option { get; private set; }

		public Action<Moderation, Rect> Draw { get; private set; }

		public GUIContent[] SubSelection { get; private set; }

		public int SubSelectionIndex { get; set; }

		public bool Selected { get; set; }

		public Moderation(MemberAccessLevel level, Actions id, string title, string context, string option, Action<Moderation, Rect> draw)
			: this(level, id, title, context, option, draw, null)
		{
		}

		public Moderation(MemberAccessLevel level, Actions id, string title, string context, string option, Action<Moderation, Rect> draw, GUIContent[] subselection)
		{
			Level = level;
			ID = id;
			Title = title;
			Content = context;
			Draw = draw;
			SubSelection = subselection;
		}
	}

	private float _nextUpdate;

	private CommUser _selectedCommUser;

	private Vector2 _playerScroll = Vector2.zero;

	private Vector2 _moderationScroll = Vector2.zero;

	private Rect _rect;

	private List<Moderation> _moderations;

	private string _filterText = string.Empty;

	private int _banDurationIndex = 1;

	private Actions _moderationSelection;

	private int _playerCount;

	private void Awake()
	{
		_moderations = new List<Moderation>();
		EventHandler.Global.AddListener(delegate(GlobalEvents.Login ev)
		{
			InitModerations(ev.AccessLevel);
		});
	}

	private void OnGUI()
	{
		_rect = new Rect(GUITools.ScreenHalfWidth - 320, GUITools.ScreenHalfHeight - 202, 640f, 404f);
		GUI.BeginGroup(_rect, GUIContent.none, BlueStonez.window_standard_grey38);
		DrawModerationPanel();
		GUI.EndGroup();
	}

	public override void Show()
	{
		base.Show();
		_moderationSelection = Actions.NONE;
	}

	public override void Hide()
	{
		base.Hide();
		_moderationSelection = Actions.NONE;
		_filterText = string.Empty;
	}

	public void SetSelectedUser(CommUser user)
	{
		if (user != null)
		{
			_selectedCommUser = user;
			_filterText = user.Name;
		}
	}

	private void InitModerations(MemberAccessLevel level)
	{
		if (level >= MemberAccessLevel.Moderator)
		{
			Moderation item = new Moderation(MemberAccessLevel.Moderator, Actions.UNMUTE_PLAYER, "Unmute Player", "Player is un-muted and un-ghosted immediately", "Unmute player", DrawModeration);
			_moderations.Add(item);
			Moderation item2 = new Moderation(MemberAccessLevel.Moderator, Actions.GHOST_PLAYER, "Ghost Player", "Chat messages from player only appear in their own chat window, but not the windows of other players.", "Ghost player", DrawModeration, new GUIContent[4]
			{
				new GUIContent("1 min"),
				new GUIContent("5 min"),
				new GUIContent("30 min"),
				new GUIContent("6 hrs")
			});
			_moderations.Add(item2);
			Moderation item3 = new Moderation(MemberAccessLevel.Moderator, Actions.MUTE_PLAYER, "Mute Player", "Chat messages from player do not appear in anyones chat window.", "Mute player", DrawModeration, new GUIContent[4]
			{
				new GUIContent("1 min"),
				new GUIContent("5 min"),
				new GUIContent("30 min"),
				new GUIContent("6 hrs")
			});
			_moderations.Add(item3);
			Moderation item4 = new Moderation(MemberAccessLevel.Moderator, Actions.KICK_FROM_GAME, "Kick from Game", "Player is removed from the game he is currently in and dumped on the home screen.", "Kick player from game", DrawModeration);
			_moderations.Add(item4);
			Moderation item5 = new Moderation(MemberAccessLevel.SeniorQA, Actions.KICK_FROM_APP, "Kick from Application", "Player is disconnected from all realtime connections for the current session.", "Kick player from application", DrawModeration);
			_moderations.Add(item5);
		}
	}

	private void DrawModerationPanel()
	{
		GUI.skin = BlueStonez.Skin;
		GUI.depth = 3;
		GUI.Label(new Rect(0f, 0f, _rect.width, 56f), "MODERATION DASHBOARD", BlueStonez.tab_strip);
		DoModerationDashboard(new Rect(10f, 55f, _rect.width - 20f, _rect.height - 55f - 52f));
		GUI.enabled = _nextUpdate < Time.time;
		if (!GameState.Current.IsMultiplayer && GUITools.Button(new Rect(10f, _rect.height - 10f - 32f, 150f, 32f), new GUIContent((!(_nextUpdate < Time.time)) ? string.Format("Next Update ({0:N0})", _nextUpdate - Time.time) : "GET ALL PLAYERS"), BlueStonez.buttondark_medium))
		{
			ChatPageGUI.IsCompleteLobbyLoaded = true;
			_selectedCommUser = null;
			_filterText = string.Empty;
			_nextUpdate = Time.time + 10f;
			AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.Operations.SendUpdateAllActors();
		}
		GUI.enabled = _selectedCommUser != null && _moderationSelection != Actions.NONE;
		if (GUITools.Button(new Rect(_rect.width - 120f - 140f, _rect.height - 10f - 32f, 140f, 32f), new GUIContent("APPLY ACTION!"), (!GUI.enabled) ? BlueStonez.button : BlueStonez.button_red))
		{
			ApplyModeration();
		}
		GUI.enabled = true;
		if (GUITools.Button(new Rect(_rect.width - 10f - 100f, _rect.height - 10f - 32f, 100f, 32f), new GUIContent("CLOSE"), BlueStonez.button))
		{
			PanelManager.Instance.ClosePanel(PanelType.Moderation);
		}
	}

	private void DoModerationDashboard(Rect position)
	{
		GUI.BeginGroup(position, GUIContent.none, BlueStonez.window_standard_grey38);
		float num = 200f;
		DoPlayerModeration(new Rect(20f + num, 10f, position.width - 30f - num, position.height - 20f));
		DoPlayerSelection(new Rect(10f, 10f, num, position.height - 20f));
		GUI.EndGroup();
	}

	private void DoPlayerSelection(Rect position)
	{
		GUI.BeginGroup(position);
		GUI.Label(new Rect(0f, 0f, position.width, 18f), "SELECT PLAYER", BlueStonez.label_interparkbold_18pt_left);
		bool flag = !string.IsNullOrEmpty(_filterText);
		GUI.SetNextControlName("Filter");
		_filterText = GUI.TextField(new Rect(0f, 26f, (!flag) ? position.width : (position.width - 26f), 24f), _filterText, 20, BlueStonez.textField);
		if (!flag && GUI.GetNameOfFocusedControl() != "Filter")
		{
			GUI.color = new Color(1f, 1f, 1f, 0.3f);
			if (GUI.Button(new Rect(7f, 32f, position.width, 24f), "Enter player name", BlueStonez.label_interparkmed_11pt_left))
			{
				GUI.FocusControl("Filter");
			}
			GUI.color = Color.white;
		}
		if (flag && GUI.Button(new Rect(position.width - 24f, 26f, 24f, 24f), "x", BlueStonez.panelquad_button))
		{
			_filterText = string.Empty;
			GUIUtility.keyboardControl = 0;
		}
		string text = string.Format("PLAYERS ONLINE ({0})", _playerCount);
		GUI.Label(new Rect(0f, 52f, position.width, 25f), GUIContent.none, BlueStonez.box_grey50);
		GUI.Label(new Rect(10f, 52f, position.width, 25f), text, BlueStonez.label_interparkbold_18pt_left);
		GUI.Label(new Rect(0f, 76f, position.width, position.height - 76f), GUIContent.none, BlueStonez.box_grey50);
		_playerScroll = GUITools.BeginScrollView(new Rect(0f, 77f, position.width, position.height - 78f), _playerScroll, new Rect(0f, 0f, position.width - 20f, _playerCount * 20));
		int num = 0;
		string value = _filterText.ToLower();
		ICollection<CommUser> collection;
		if (GameState.Current.IsMultiplayer)
		{
			ICollection<CommUser> gameUsers = Singleton<ChatManager>.Instance.GameUsers;
			collection = gameUsers;
		}
		else
		{
			collection = Singleton<ChatManager>.Instance.LobbyUsers;
		}
		ICollection<CommUser> collection2 = collection;
		foreach (CommUser item in collection2)
		{
			if (string.IsNullOrEmpty(value) || item.Name.ToLower().Contains(value))
			{
				if ((num & 1) == 0)
				{
					GUI.Label(new Rect(1f, num * 20, position.width - 2f, 20f), GUIContent.none, BlueStonez.box_grey38);
				}
				if (_selectedCommUser != null && _selectedCommUser.Cmid == item.Cmid)
				{
					Color uberStrikeBlue = ColorScheme.UberStrikeBlue;
					float r = uberStrikeBlue.r;
					Color uberStrikeBlue2 = ColorScheme.UberStrikeBlue;
					float g = uberStrikeBlue2.g;
					Color uberStrikeBlue3 = ColorScheme.UberStrikeBlue;
					GUI.color = new Color(r, g, uberStrikeBlue3.b, 0.5f);
					GUI.Label(new Rect(1f, num * 20, position.width - 2f, 20f), GUIContent.none, BlueStonez.box_white);
					GUI.color = Color.white;
				}
				if (GUI.Button(new Rect(10f, num * 20, position.width, 20f), "{" + item.Cmid + "} " + item.Name, BlueStonez.label_interparkmed_10pt_left))
				{
					_selectedCommUser = item;
				}
				GUI.color = Color.white;
				num++;
			}
		}
		_playerCount = num;
		GUITools.EndScrollView();
		GUI.EndGroup();
	}

	private void DoPlayerModeration(Rect position)
	{
		int num = _moderations.Count * 100;
		GUI.BeginGroup(position);
		GUI.Label(new Rect(0f, 0f, position.width, position.height), GUIContent.none, BlueStonez.box_grey50);
		_moderationScroll = GUITools.BeginScrollView(new Rect(0f, 0f, position.width, position.height), _moderationScroll, new Rect(0f, 1f, position.width - 20f, num));
		int i = 0;
		int num2 = 0;
		for (; i < _moderations.Count; i++)
		{
			_moderations[i].Draw(_moderations[i], new Rect(10f, num2++ * 100, 360f, 100f));
		}
		GUITools.EndScrollView();
		GUI.EndGroup();
	}

	private void DrawModeration(Moderation moderation, Rect position)
	{
		GUI.BeginGroup(position);
		GUI.Label(new Rect(21f, 0f, position.width, 30f), moderation.Title, BlueStonez.label_interparkbold_13pt);
		GUI.Label(new Rect(0f, 30f, 356f, 40f), moderation.Content, BlueStonez.label_itemdescription);
		GUI.Label(new Rect(0f, 0f, position.width, 1f), GUIContent.none, BlueStonez.horizontal_line_grey95);
		if (GUI.Toggle(new Rect(0f, 7f, position.width, 16f), moderation.Selected, GUIContent.none, BlueStonez.radiobutton) && !moderation.Selected)
		{
			moderation.Selected = true;
			SelectModeration(moderation.ID);
			switch (moderation.SubSelectionIndex)
			{
			case 0:
				_banDurationIndex = 1;
				break;
			case 1:
				_banDurationIndex = 5;
				break;
			case 2:
				_banDurationIndex = 30;
				break;
			case 3:
				_banDurationIndex = 360;
				break;
			default:
				_banDurationIndex = 1;
				break;
			}
			GUIUtility.keyboardControl = 0;
		}
		if (moderation.SubSelection != null)
		{
			GUI.enabled = moderation.Selected;
			GUI.changed = false;
			if (moderation.Selected)
			{
				moderation.SubSelectionIndex = UnityGUI.Toolbar(new Rect(0f, position.height - 25f, position.width, 20f), moderation.SubSelectionIndex, moderation.SubSelection, moderation.SubSelection.Length, BlueStonez.panelquad_toggle);
			}
			else
			{
				UnityGUI.Toolbar(new Rect(0f, position.height - 25f, position.width, 20f), -1, moderation.SubSelection, moderation.SubSelection.Length, BlueStonez.panelquad_toggle);
			}
			if (GUI.changed)
			{
				switch (moderation.SubSelectionIndex)
				{
				case 0:
					_banDurationIndex = 1;
					break;
				case 1:
					_banDurationIndex = 5;
					break;
				case 2:
					_banDurationIndex = 30;
					break;
				case 3:
					_banDurationIndex = 360;
					break;
				default:
					_banDurationIndex = 1;
					break;
				}
			}
			GUI.enabled = true;
		}
		GUI.EndGroup();
	}

	private void SelectModeration(Actions id)
	{
		_moderationSelection = id;
		for (int i = 0; i < _moderations.Count; i++)
		{
			if (id != _moderations[i].ID)
			{
				_moderations[i].Selected = false;
			}
		}
	}

	private void ApplyModeration()
	{
		if (PlayerDataManager.AccessLevel < MemberAccessLevel.Moderator || !_moderations.Exists((Moderation m) => m.ID == _moderationSelection))
		{
			return;
		}
		switch (_moderationSelection)
		{
		case Actions.UNMUTE_PLAYER:
			AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.Operations.SendModerationMutePlayer(0, _selectedCommUser.Cmid, false);
			PopupSystem.ShowMessage("Action Executed", string.Format("The Player '{0}' was unmuted.", _selectedCommUser.Name));
			break;
		case Actions.GHOST_PLAYER:
			AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.Operations.SendModerationMutePlayer(_banDurationIndex, _selectedCommUser.Cmid, false);
			PopupSystem.ShowMessage("Action Executed", string.Format("The Player '{0}' was ghosted for {1} minutes.", _selectedCommUser.Name, _banDurationIndex));
			break;
		case Actions.MUTE_PLAYER:
			AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.Operations.SendModerationMutePlayer(_banDurationIndex, _selectedCommUser.Cmid, true);
			AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.Operations.SendModerationMutePlayer(_banDurationIndex, _selectedCommUser.Cmid, false);
			PopupSystem.ShowMessage("Action Executed", string.Format("The Player '{0}' was muted for {1} minutes.", _selectedCommUser.Name, _banDurationIndex));
			break;
		case Actions.KICK_FROM_GAME:
			if (_selectedCommUser.CurrentGame != null && _selectedCommUser.CurrentGame.Server != null)
			{
				GamePeerAction.KickPlayer(_selectedCommUser.CurrentGame.Server.ConnectionString, _selectedCommUser.Cmid);
				PopupSystem.ShowMessage("Action Executed", string.Format("The Player '{0}' was kicked out of his current game!", _selectedCommUser.Name));
			}
			else
			{
				PopupSystem.ShowMessage("Warning", string.Format("The Player '{0}' is currently not in a game!", _selectedCommUser.Name));
			}
			break;
		case Actions.KICK_FROM_APP:
			AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.Operations.SendModerationBanPlayer(_selectedCommUser.Cmid);
			PopupSystem.ShowMessage("Action Executed", string.Format("The Player '{0}' was disconnected from all servers!", _selectedCommUser.Name));
			break;
		}
		_moderationSelection = Actions.NONE;
		foreach (Moderation moderation in _moderations)
		{
			moderation.Selected = false;
		}
	}
}
