using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Cmune.DataCenter.Common.Entities;
using ExitGames.Client.Photon;
using UberStrike.Realtime.UnitySdk;
using UnityEngine;

public class ChatPageGUI : PageGUI
{
	private const int SEARCH_HEIGHT = 36;

	private const float TitleHeight = 24f;

	private const int TAB_WIDTH = 300;

	private const int CHAT_USER_HEIGHT = 24;

	private GUIStyle label_notification;

	private Rect _mainRect;

	private Vector2 _dialogScroll;

	public static bool IsCompleteLobbyLoaded;

	private float _nextFullLobbyUpdate;

	private bool autoScroll;

	private float _spammingNotificationTime;

	private float _nextNaughtyListUpdate;

	private float _yPosition;

	private float _lastMessageSentTimer = 0.3f;

	private string _currentChatMessage = string.Empty;

	private PopupMenu _playerMenu;

	private float _keyboardOffset;

	public static TabArea SelectedTab { get; set; }

	public static bool IsChatActive
	{
		get
		{
			return GUI.GetNameOfFocusedControl() == "@CurrentChatMessage";
		}
	}

	private void Awake()
	{
		_playerMenu = new PopupMenu();
		base.IsOnGUIEnabled = true;
	}

	private void Start()
	{
		label_notification = new GUIStyle(BlueStonez.label_interparkbold_18pt);
		_playerMenu.AddMenuItem(LocalizedStrings.JoinGame, MenuCmdJoinGame, MenuChkJoinGame);
		_playerMenu.AddMenuItem("Add Friend", MenuCmdAddFriend, MenuChkAddFriend);
		_playerMenu.AddMenuItem(LocalizedStrings.PrivateChat, MenuCmdChat, MenuChkChat);
		_playerMenu.AddMenuItem(LocalizedStrings.SendMessage, MenuCmdSendMessage, MenuChkSendMessage);
		_playerMenu.AddMenuItem(LocalizedStrings.InviteToClan, MenuCmdInviteClan, MenuChkInviteClan);
		_playerMenu.AddMenuItem(MenuCaptionMute, MenuCmdMute, MenuChkMute);
		_playerMenu.AddMenuItem("Unfriend", MenuCmdRemoveFriend, MenuChkRemoveFriend);
		if (PlayerDataManager.AccessLevel >= MemberAccessLevel.Moderator)
		{
			_playerMenu.AddMenuItem("- - - - - - - - - - - - -", null, (CommUser P_0) => true);
			_playerMenu.AddMenuItem("Copy Data", MenuCmdCopyData, (CommUser P_0) => true);
			_playerMenu.AddMenuCopyItem("Copy Message", MenuCmdCopyMsg, (CommUser P_0) => true);
			_playerMenu.AddMenuCopyItem("Copy Name", MenuCmdCopyPlayerName, (CommUser P_0) => true);
			_playerMenu.AddMenuItem("Moderate Player", MenuCmdModeratePlayer, (CommUser P_0) => true);
		}
	}

	private void OnEnable()
	{
		if (_nextFullLobbyUpdate < Time.time)
		{
			_nextFullLobbyUpdate = Time.time + 20f;
			AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.Operations.SendFullPlayerListUpdate();
		}
	}

	private void Update()
	{
		if (_lastMessageSentTimer < 0.3f)
		{
			_lastMessageSentTimer += Time.deltaTime;
		}
		if (_yPosition < 0f)
		{
			_yPosition = Mathf.Lerp(_yPosition, 0.1f, Time.deltaTime * 8f);
		}
		else
		{
			_yPosition = 0f;
		}
	}

	private void OnGUI()
	{
		if (base.IsOnGUIEnabled)
		{
			GUI.skin = BlueStonez.Skin;
			GUI.depth = 9;
			_mainRect = new Rect(0f, GlobalUIRibbon.Instance.Height(), Screen.width, Screen.height - GlobalUIRibbon.Instance.Height());
			DrawGUI(_mainRect);
			if (PopupMenu.Current != null)
			{
				PopupMenu.Current.Draw();
			}
		}
	}

	public override void DrawGUI(Rect rect)
	{
		GUI.BeginGroup(rect, BlueStonez.window);
		if (AutoMonoBehaviour<CommConnectionManager>.Instance.Client.IsConnected)
		{
			DoTabs(new Rect(10f, 0f, 300f, 30f));
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
			{
				GUIUtility.keyboardControl = 0;
			}
			Rect rect2 = new Rect(0f, 21f, 300f, rect.height - 21f);
			Rect rect3 = new Rect(299f, 0f, rect.width - 300f, 22f);
			Rect rect4 = new Rect(300f, 22f, rect.width - 300f, rect.height - 22f - 36f - _keyboardOffset);
			Rect rect5 = new Rect(299f, rect.height - 37f, rect.width - 300f + 1f, 37f);
			ChatGroupPanel pane = Singleton<ChatManager>.Instance._commPanes[(int)SelectedTab];
			GUITools.PushGUIState();
			GUI.enabled &= !PopupMenu.IsEnabled;
			switch (SelectedTab)
			{
			case TabArea.Lobby:
				DoDialogFooter(rect5, pane, Singleton<ChatManager>.Instance.LobbyDialog);
				DoLobbyCommPane(rect2, pane);
				DoDialogHeader(rect3, Singleton<ChatManager>.Instance.LobbyDialog);
				DoDialog(rect4, pane, Singleton<ChatManager>.Instance.LobbyDialog);
				break;
			case TabArea.Private:
				DoDialogFooter(rect5, pane, Singleton<ChatManager>.Instance.SelectedDialog);
				DrawCommPane(rect2, pane);
				DoPrivateDialogHeader(rect3, Singleton<ChatManager>.Instance.SelectedDialog);
				DoDialog(rect4, pane, Singleton<ChatManager>.Instance.SelectedDialog);
				break;
			case TabArea.Clan:
				DoDialogFooter(rect5, pane, Singleton<ChatManager>.Instance.ClanDialog);
				DrawCommPane(rect2, pane);
				DoDialogHeader(rect3, Singleton<ChatManager>.Instance.ClanDialog);
				DoDialog(rect4, pane, Singleton<ChatManager>.Instance.ClanDialog);
				break;
			case TabArea.InGame:
				DoDialogFooter(rect5, pane, Singleton<ChatManager>.Instance.InGameDialog);
				DrawCommPane(rect2, pane);
				DoDialogHeader(rect3, Singleton<ChatManager>.Instance.InGameDialog);
				DoDialog(rect4, pane, Singleton<ChatManager>.Instance.InGameDialog);
				break;
			case TabArea.Moderation:
				DoModeratorPaneFooter(rect5, pane);
				DrawModeratorCommPane(rect2, pane);
				DoDialogHeader(rect3, Singleton<ChatManager>.Instance.ModerationDialog);
				DoModeratorDialog(rect4, pane);
				break;
			}
			GUITools.PopGUIState();
		}
		else
		{
			GUI.color = Color.gray;
			PeerStateValue peerState = AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Peer.PeerState;
			if (peerState == PeerStateValue.Connecting)
			{
				GUI.Label(new Rect(0f, rect.height / 2f, rect.width, 20f), LocalizedStrings.ConnectingToServer, BlueStonez.label_interparkbold_11pt);
			}
			else
			{
				GUI.Label(new Rect(0f, rect.height / 2f, rect.width, 20f), LocalizedStrings.ServerIsNotReachable, BlueStonez.label_interparkbold_11pt);
			}
			GUI.color = Color.white;
		}
		GUI.EndGroup();
		GuiManager.DrawTooltip();
	}

	private bool IsMobileChannel(ChannelType channel)
	{
		return channel == ChannelType.Android || channel == ChannelType.IPad || channel == ChannelType.IPhone;
	}

	private int DoModeratorControlPanel(Rect rect, ChatGroupPanel pane)
	{
		if (PlayerDataManager.AccessLevel >= MemberAccessLevel.Moderator)
		{
			int num = 0;
			bool flag = PlayerDataManager.AccessLevel >= MemberAccessLevel.Moderator;
			bool flag2 = flag && IsCompleteLobbyLoaded;
			rect = new Rect(rect.x, rect.yMax - 36f - (float)(flag2 ? 60 : (flag ? 30 : 0)) - 1f, rect.width, 37 + (flag2 ? 60 : (flag ? 30 : 0)));
			GUI.BeginGroup(rect, GUIContent.none, BlueStonez.window_standard_grey38);
			if (flag)
			{
				GUI.enabled = _nextNaughtyListUpdate < Time.time;
				if (GUITools.Button(new Rect(6f, rect.height - 61f, (rect.width - 12f) * 0.5f, 26f), new GUIContent((!(_nextNaughtyListUpdate < Time.time)) ? string.Format("Next Update ({0:N0})", _nextNaughtyListUpdate - Time.time) : "Update Naughty List"), BlueStonez.buttondark_medium))
				{
					_nextNaughtyListUpdate = Time.time + 10f;
					AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.Operations.SendUpdateNaughtyList();
				}
				GUI.enabled = true;
				GUI.enabled = _nextNaughtyListUpdate < Time.time;
				if (GUITools.Button(new Rect(6f + (rect.width - 12f) * 0.5f, rect.height - 61f, (rect.width - 12f) * 0.5f, 26f), new GUIContent((!(_nextNaughtyListUpdate < Time.time)) ? string.Format("Next Update ({0:N0})", _nextNaughtyListUpdate - Time.time) : "Unban Next 50"), BlueStonez.buttondark_medium))
				{
					List<CommUser> list = new List<CommUser>(Singleton<ChatManager>.Instance.NaughtyUsers);
					int num2 = 0;
					foreach (CommUser item in list)
					{
						if (item.Name.StartsWith("Banned:"))
						{
							AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.Operations.SendClearModeratorFlags(item.Cmid);
							Singleton<ChatManager>.Instance.SelectedCmid = 0;
							Singleton<ChatManager>.Instance._naughtyUsers.Remove(item.Cmid);
							if (++num2 > 50)
							{
								break;
							}
						}
					}
				}
				GUI.enabled = true;
				num += ((!IsCompleteLobbyLoaded) ? 30 : 60);
			}
			bool flag3 = !string.IsNullOrEmpty(pane.SearchText);
			GUI.SetNextControlName("@ModSearch");
			GUI.changed = false;
			pane.SearchText = GUI.TextField(new Rect(6f, rect.height - 30f, rect.width - (float)((!flag3) ? 12 : 37), 24f), pane.SearchText, 20, BlueStonez.textField);
			if (!flag3 && GUI.GetNameOfFocusedControl() != "@ModSearch")
			{
				GUI.color = new Color(1f, 1f, 1f, 0.3f);
				GUI.Label(new Rect(12f, rect.height - 30f, rect.width - 20f, 24f), LocalizedStrings.Search, BlueStonez.label_interparkmed_10pt_left);
				GUI.color = Color.white;
			}
			if (flag3 && GUITools.Button(new Rect(rect.width - 28f, rect.height - 30f, 22f, 22f), new GUIContent("x"), BlueStonez.panelquad_button))
			{
				pane.SearchText = string.Empty;
				GUIUtility.keyboardControl = 0;
			}
			GUI.EndGroup();
			return num + 36;
		}
		return 0;
	}

	public void DrawCommPane(Rect rect, ChatGroupPanel pane)
	{
		GUI.BeginGroup(rect);
		pane.WindowHeight = rect.height;
		float height = Mathf.Max(pane.WindowHeight, pane.ContentHeight);
		float num = 0f;
		pane.Scroll = GUITools.BeginScrollView(new Rect(0f, 0f, rect.width, pane.WindowHeight), pane.Scroll, new Rect(0f, 0f, rect.width - 17f, height), false, true);
		GUI.BeginGroup(new Rect(0f, 0f, rect.width, pane.WindowHeight + pane.Scroll.y));
		foreach (ChatGroup group in pane.Groups)
		{
			num += DrawPlayerGroup(group, num, rect.width - 17f, pane.SearchText.Trim().ToLower());
		}
		GUI.EndGroup();
		GUITools.EndScrollView();
		pane.ContentHeight = num;
		GUI.EndGroup();
	}

	private void DoLobbyCommPane(Rect rect, ChatGroupPanel pane)
	{
		GUI.BeginGroup(rect);
		bool flag = GUI.enabled;
		int num = DoLobbyControlPanel(new Rect(0f, 0f, rect.width, rect.height), pane);
		pane.WindowHeight = rect.height - (float)num;
		float height = Mathf.Max(pane.WindowHeight, pane.ContentHeight);
		float num2 = 0f;
		pane.Scroll = GUITools.BeginScrollView(new Rect(0f, 0f, rect.width, pane.WindowHeight), pane.Scroll, new Rect(0f, 0f, rect.width - 17f, height), false, true);
		GUI.BeginGroup(new Rect(0f, 0f, rect.width, pane.WindowHeight + pane.Scroll.y));
		foreach (ChatGroup group in pane.Groups)
		{
			num2 += DrawPlayerGroup(group, num2, rect.width - 17f, pane.SearchText.Trim().ToLower());
		}
		GUI.EndGroup();
		GUITools.EndScrollView();
		pane.ContentHeight = num2;
		GUI.enabled = flag;
		GUI.EndGroup();
	}

	private void DrawModeratorCommPane(Rect rect, ChatGroupPanel pane)
	{
		GUI.BeginGroup(rect);
		int num = 0;
		num = DoModeratorControlPanel(new Rect(0f, 0f, rect.width, rect.height), pane);
		pane.WindowHeight = rect.height - (float)num;
		float height = Mathf.Max(pane.WindowHeight, pane.ContentHeight);
		float num2 = 0f;
		pane.Scroll = GUITools.BeginScrollView(new Rect(0f, 0f, rect.width, pane.WindowHeight), pane.Scroll, new Rect(0f, 0f, rect.width - 17f, height), false, true);
		GUI.BeginGroup(new Rect(0f, 0f, rect.width, pane.WindowHeight + pane.Scroll.y));
		foreach (ChatGroup group in pane.Groups)
		{
			num2 += DrawPlayerGroup(group, num2, rect.width - 17f, pane.SearchText.Trim().ToLower(), true);
			if (num2 > pane.Scroll.y + pane.WindowHeight)
			{
				break;
			}
		}
		GUI.EndGroup();
		GUITools.EndScrollView();
		pane.ContentHeight = num2;
		GUI.EndGroup();
	}

	private int DoLobbyControlPanel(Rect rect, ChatGroupPanel pane)
	{
		int num = 0;
		bool flag = PlayerDataManager.AccessLevel >= MemberAccessLevel.Moderator;
		bool flag2 = flag && IsCompleteLobbyLoaded;
		rect = new Rect(rect.x, rect.yMax - 36f - (float)(flag2 ? 60 : (flag ? 30 : 0)) - 1f, rect.width, 37 + (flag2 ? 60 : (flag ? 30 : 0)));
		GUI.BeginGroup(rect, GUIContent.none, BlueStonez.window_standard_grey38);
		if (flag)
		{
			GUI.enabled = _nextFullLobbyUpdate < Time.time;
			if (flag2 && GUITools.Button(new Rect(6f, 5f, rect.width - 12f, 26f), new GUIContent("Reset Lobby"), BlueStonez.buttondark_medium))
			{
				IsCompleteLobbyLoaded = false;
				_nextFullLobbyUpdate = Time.time + 10f;
				AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.SendUpdateResetLobby();
			}
			if (GUITools.Button(new Rect(6f, rect.height - 61f, rect.width - 12f, 26f), new GUIContent((!(_nextFullLobbyUpdate < Time.time)) ? string.Format("Next Update ({0:N0})", _nextFullLobbyUpdate - Time.time) : "Get All Players "), BlueStonez.buttondark_medium))
			{
				IsCompleteLobbyLoaded = true;
				_nextFullLobbyUpdate = Time.time + 10f;
				AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.Operations.SendUpdateAllActors();
			}
			GUI.enabled = true;
			num += ((!IsCompleteLobbyLoaded) ? 30 : 60);
		}
		bool flag3 = !string.IsNullOrEmpty(pane.SearchText);
		if (!flag3)
		{
			pane.SearchText = " ";
		}
		GUI.SetNextControlName("@LobbySearch");
		GUI.changed = false;
		pane.SearchText = GUI.TextField(new Rect(6f, rect.height - 30f, rect.width - (float)((!flag3) ? 12 : 37), 24f), pane.SearchText, 20, BlueStonez.textField);
		if (!flag3 && GUI.GetNameOfFocusedControl() != "@LobbySearch")
		{
			GUI.color = new Color(1f, 1f, 1f, 0.3f);
			GUI.Label(new Rect(12f, rect.height - 30f, rect.width - 20f, 24f), LocalizedStrings.Search, BlueStonez.label_interparkmed_10pt_left);
			GUI.color = Color.white;
		}
		if (flag3 && GUITools.Button(new Rect(rect.width - 28f, rect.height - 30f, 22f, 22f), new GUIContent("x"), BlueStonez.panelquad_button))
		{
			pane.SearchText = string.Empty;
			GUIUtility.keyboardControl = 0;
		}
		GUI.EndGroup();
		return num + 36;
	}

	public float DrawPlayerGroup(ChatGroup group, float vOffset, float width, string search, bool allowSelfSelection = false)
	{
		Rect position = new Rect(0f, vOffset, width, 24f);
		GUI.Label(position, GUIContent.none, BlueStonez.window_standard_grey38);
		if (group.Players != null)
		{
			GUI.Label(position, group.Title + " (" + group.Players.Count + ")", BlueStonez.label_interparkbold_11pt);
		}
		vOffset += 24f;
		int num = 0;
		if (group.Players != null)
		{
			GUI.BeginGroup(new Rect(0f, vOffset, width, group.Players.Count * 24));
			foreach (CommUser player in group.Players)
			{
				if (string.IsNullOrEmpty(search) || player.Name.ToLower().Contains(search))
				{
					GroupDrawUser(num++ * 24, width, player, allowSelfSelection);
				}
			}
			GUI.EndGroup();
		}
		return 24f + (float)(group.Players.Count * 24);
	}

	private void DoTabs(Rect rect)
	{
		float num = Mathf.Floor(rect.width / (float)Singleton<ChatManager>.Instance.TabCounter);
		bool flag = false;
		bool flag2 = false;
		int num2 = 0;
		if (GUI.Toggle(new Rect(rect.x + (float)num2 * num, rect.y, num, rect.height), SelectedTab == TabArea.Lobby, LocalizedStrings.Lobby, BlueStonez.tab_medium) && SelectedTab != TabArea.Lobby)
		{
			SelectedTab = TabArea.Lobby;
			flag = true;
		}
		num2++;
		if (GUI.Toggle(new Rect(rect.x + (float)num2 * num, rect.y, num, rect.height), SelectedTab == TabArea.Private, LocalizedStrings.Private, BlueStonez.tab_medium) && SelectedTab != TabArea.Private)
		{
			SelectedTab = TabArea.Private;
			flag = true;
			Singleton<ChatManager>.Instance.HasUnreadPrivateMessage.Value = false;
		}
		if ((bool)Singleton<ChatManager>.Instance.HasUnreadPrivateMessage)
		{
			GUI.DrawTexture(new Rect(rect.x + (float)num2 * num, rect.y + 1f, 18f, 18f), CommunicatorIcons.NewInboxMessage);
		}
		num2++;
		if (Singleton<ChatManager>.Instance.ShowTab(TabArea.Clan))
		{
			if (GUI.Toggle(new Rect(rect.x + (float)num2 * num, rect.y, num, rect.height), SelectedTab == TabArea.Clan, LocalizedStrings.Clan, BlueStonez.tab_medium) && SelectedTab != TabArea.Clan)
			{
				SelectedTab = TabArea.Clan;
				flag = true;
				Singleton<ChatManager>.Instance.HasUnreadClanMessage.Value = false;
			}
			if (PlayerDataManager.IsPlayerInClan && (bool)Singleton<ChatManager>.Instance.HasUnreadClanMessage)
			{
				GUI.DrawTexture(new Rect(rect.x + (float)num2 * num, rect.y + 1f, 18f, 18f), CommunicatorIcons.NewInboxMessage);
			}
			num2++;
		}
		if (Singleton<ChatManager>.Instance.ShowTab(TabArea.InGame))
		{
			if (GUI.Toggle(new Rect(rect.x + (float)num2 * num, rect.y, num, rect.height), SelectedTab == TabArea.InGame, LocalizedStrings.Game, BlueStonez.tab_medium) && SelectedTab != TabArea.InGame)
			{
				SelectedTab = TabArea.InGame;
				_currentChatMessage = string.Empty;
				flag = true;
			}
			num2++;
		}
		if (Singleton<ChatManager>.Instance.ShowTab(TabArea.Moderation))
		{
			if (GUI.Toggle(new Rect(rect.x + (float)num2 * num, rect.y, num, rect.height), SelectedTab == TabArea.Moderation, LocalizedStrings.Admin, BlueStonez.tab_medium) && SelectedTab != TabArea.Moderation && PlayerDataManager.AccessLevel >= MemberAccessLevel.Moderator)
			{
				SelectedTab = TabArea.Moderation;
				_currentChatMessage = string.Empty;
				flag = true;
			}
			num2++;
		}
		if (flag)
		{
			_currentChatMessage = string.Empty;
			PopupMenu.Hide();
			GUIUtility.keyboardControl = 0;
		}
	}

	private void DoDialog(Rect rect, ChatGroupPanel pane, ChatDialog dialog)
	{
		if (dialog == null)
		{
			return;
		}
		dialog.CheckSize(rect);
		if (!Input.GetMouseButton(0))
		{
			if (autoScroll)
			{
				_dialogScroll.y = float.MaxValue;
			}
		}
		else
		{
			autoScroll = false;
		}
		Rect position = new Rect(rect.x, rect.y + Mathf.Clamp(rect.height - dialog._heightCache, 0f, rect.height), rect.width, rect.height);
		GUI.BeginGroup(position);
		int num = 0;
		float num2 = 0f;
		_dialogScroll = GUITools.BeginScrollView(new Rect(0f, 0f, dialog._frameSize.x, dialog._frameSize.y), _dialogScroll, new Rect(0f, 0f, dialog._contentSize.x, dialog._contentSize.y));
		foreach (InstantMessage item in dialog._msgQueue)
		{
			if (Singleton<ChatManager>.Instance.IsMuted(item.Cmid) || (dialog.CanShow != null && !dialog.CanShow(item.Context)))
			{
				continue;
			}
			if (num % 2 == 0)
			{
				GUI.Label(new Rect(0f, num2, dialog._contentSize.x - 1f, item.Height), GUIContent.none, BlueStonez.box_grey38);
			}
			if (GUI.Button(new Rect(0f, num2, dialog._contentSize.x - 1f, item.Height), GUIContent.none, BlueStonez.dropdown_list))
			{
				SelectUser(item.Cmid);
				ScrollToUser(item.Cmid);
				if (PlayerDataManager.AccessLevel >= MemberAccessLevel.Moderator)
				{
					CommUser user = new CommUser(item.Actor);
					if (Event.current.button == 1)
					{
						_playerMenu.msg = item;
						_playerMenu.Show(GUIUtility.GUIToScreenPoint(Event.current.mousePosition), user);
					}
				}
			}
			if (string.IsNullOrEmpty(item.PlayerName))
			{
				GUI.color = new Color(0.6f, 0.6f, 0.6f);
				GUI.Label(new Rect(4f, num2, dialog._contentSize.x - 8f, 20f), item.Text, BlueStonez.label_interparkbold_11pt_left);
			}
			else
			{
				GUI.color = GetNameColor(item);
				GUI.Label(new Rect(4f, num2, dialog._contentSize.x - 8f, 20f), item.PlayerName + ":", BlueStonez.label_interparkbold_11pt_left);
				GUI.color = new Color(0.9f, 0.9f, 0.9f);
				GUI.Label(new Rect(4f, num2 + 20f, dialog._contentSize.x - 8f, item.Height - 20f), item.Text, BlueStonez.label_interparkmed_11pt_left);
			}
			GUI.color = new Color(1f, 1f, 1f, 0.5f);
			GUI.Label(new Rect(4f, num2, dialog._contentSize.x - 8f, 20f), item.TimeString, BlueStonez.label_interparkmed_10pt_right);
			GUI.color = Color.white;
			num2 += item.Height;
			num++;
		}
		GUITools.EndScrollView();
		dialog._heightCache = num2;
		GUI.EndGroup();
		if (dialog.UserCmid > 0 && !AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.HasPlayer(dialog.UserCmid))
		{
			GUI.Label(rect, dialog.UserName + " is currently offline", label_notification);
		}
	}

	private void DoModeratorDialog(Rect rect, ChatGroupPanel pane)
	{
		if (PlayerDataManager.AccessLevel < MemberAccessLevel.Moderator)
		{
			return;
		}
		GUI.BeginGroup(rect, GUIContent.none);
		CommUser value;
		if (Singleton<ChatManager>.Instance._naughtyUsers.TryGetValue(Singleton<ChatManager>.Instance.SelectedCmid, out value) && value != null)
		{
			GUI.TextField(new Rect(10f, 15f, rect.width, 20f), "Name: " + value.Name, BlueStonez.label_interparkbold_11pt_left);
			GUI.TextField(new Rect(10f, 37f, rect.width, 20f), "Cmid: " + value.Cmid, BlueStonez.label_interparkmed_11pt_left);
			float num = rect.width - 20f;
			GUI.BeginGroup(new Rect(10f, 80f, num, rect.height - 70f), GUIContent.none, BlueStonez.box_grey50);
			if (GUITools.Button(new Rect(5f, 5f, num - 10f, 20f), new GUIContent("Clear and Unban"), BlueStonez.buttondark_medium))
			{
				AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.Operations.SendClearModeratorFlags(value.Cmid);
				Singleton<ChatManager>.Instance.SelectedCmid = 0;
				Singleton<ChatManager>.Instance._naughtyUsers.Remove(value.Cmid);
			}
			int num2 = 40;
			if ((value.ModerationFlag & 4) != 0)
			{
				GUI.Label(new Rect(8f, num2, num - 10f, 20f), "- BANNED", BlueStonez.label_interparkbold_11pt_left);
				num2 += 20;
			}
			if ((value.ModerationFlag & 2) != 0)
			{
				GUI.Label(new Rect(8f, num2, num - 10f, 20f), "- Ghosted", BlueStonez.label_interparkmed_11pt_left);
				num2 += 20;
			}
			if ((value.ModerationFlag & 1) != 0)
			{
				GUI.Label(new Rect(8f, num2, num - 10f, 20f), "- Muted", BlueStonez.label_interparkmed_11pt_left);
				num2 += 20;
			}
			if ((value.ModerationFlag & 8) != 0)
			{
				GUI.Label(new Rect(8f, num2, num - 10f, 20f), "- Speed " + value.ModerationInfo, BlueStonez.label_interparkmed_11pt_left);
				num2 += 20;
			}
			if ((value.ModerationFlag & 0x10) != 0)
			{
				GUI.Label(new Rect(8f, num2, num - 10f, 20f), "- Spamming", BlueStonez.label_interparkmed_11pt_left);
				num2 += 20;
			}
			if ((value.ModerationFlag & 0x20) != 0)
			{
				GUI.Label(new Rect(8f, num2, num - 10f, 20f), "- CrudeLanguage", BlueStonez.label_interparkmed_11pt_left);
				num2 += 20;
			}
			GUI.Label(new Rect(8f, num2 + 20, num - 10f, 100f), value.ModerationInfo, BlueStonez.label_interparkmed_11pt_left);
			GUI.EndGroup();
		}
		else
		{
			GUI.Label(new Rect(0f, rect.height / 2f, rect.width, 20f), "No user selected", BlueStonez.label_interparkmed_11pt);
		}
		GUI.EndGroup();
	}

	private void DoDialogHeader(Rect rect, ChatDialog d)
	{
		GUI.Label(rect, GUIContent.none, BlueStonez.window_standard_grey38);
		GUI.Label(rect, d.Title, BlueStonez.label_interparkbold_11pt);
	}

	private void DoPrivateDialogHeader(Rect rect, ChatDialog d)
	{
		GUI.Label(rect, GUIContent.none, BlueStonez.window_standard_grey38);
		if (d != null && d.UserCmid > 0)
		{
			GUI.Label(rect, d.Title, BlueStonez.label_interparkbold_11pt);
			if (GUITools.Button(new Rect(rect.x + rect.width - 20f, rect.y + 3f, 16f, 16f), new GUIContent("x"), BlueStonez.panelquad_button))
			{
				Singleton<ChatManager>.Instance.RemoveDialog(d);
			}
		}
		else
		{
			GUI.Label(rect, LocalizedStrings.PrivateChat, BlueStonez.label_interparkbold_11pt);
		}
	}

	private void DoModeratorPaneFooter(Rect rect, ChatGroupPanel pane)
	{
		GUI.BeginGroup(rect, BlueStonez.window_standard_grey38);
		CommUser user;
		if (Singleton<ChatManager>.Instance.SelectedCmid > 0 && Singleton<ChatManager>.Instance.TryGetLobbyCommUser(Singleton<ChatManager>.Instance.SelectedCmid, out user) && user != null)
		{
			if (GUITools.Button(new Rect(5f, 6f, rect.width - 10f, rect.height - 12f), new GUIContent("Moderate User"), BlueStonez.buttondark_medium))
			{
				ModerationPanelGUI moderationPanelGUI = PanelManager.Instance.OpenPanel(PanelType.Moderation) as ModerationPanelGUI;
				if ((bool)moderationPanelGUI)
				{
					moderationPanelGUI.SetSelectedUser(user);
				}
			}
		}
		else if (GUITools.Button(new Rect(5f, 6f, rect.width - 10f, rect.height - 12f), new GUIContent("Open Moderator"), BlueStonez.buttondark_medium))
		{
			PanelManager.Instance.OpenPanel(PanelType.Moderation);
		}
		GUI.EndGroup();
	}

	private void DoDialogFooter(Rect rect, ChatGroupPanel pane, ChatDialog dialog)
	{
		GUI.BeginGroup(rect, BlueStonez.window_standard_grey38);
		bool flag = GUI.enabled;
		GUI.enabled &= !AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.IsPlayerMuted && dialog != null && dialog.CanChat;
		if (SelectedTab == TabArea.InGame)
		{
			GUI.enabled &= GameState.Current.HasJoinedGame && GameState.Current.IsInGame;
		}
		GUI.SetNextControlName("@CurrentChatMessage");
		_currentChatMessage = GUI.TextField(new Rect(6f, 6f, rect.width - 60f, rect.height - 12f), _currentChatMessage, 140, BlueStonez.textField);
		_currentChatMessage = _currentChatMessage.Trim('\n');
		if (_spammingNotificationTime > Time.time)
		{
			GUI.color = Color.red;
			GUI.Label(new Rect(15f, 6f, rect.width - 66f, rect.height - 12f), LocalizedStrings.DontSpamTheLobbyChat, BlueStonez.label_interparkmed_10pt_left);
			GUI.color = Color.white;
		}
		else if (string.IsNullOrEmpty(_currentChatMessage) && GUI.GetNameOfFocusedControl() != "@CurrentChatMessage")
		{
			string empty = string.Empty;
			empty = ((dialog == null || dialog.UserCmid <= 0) ? LocalizedStrings.EnterAMessageHere : ((!dialog.CanChat) ? (dialog.UserName + LocalizedStrings.Offline) : LocalizedStrings.EnterAMessageHere));
			GUI.color = new Color(1f, 1f, 1f, 0.3f);
			GUI.Label(new Rect(10f, 6f, rect.width - 66f, rect.height - 12f), empty, BlueStonez.label_interparkmed_10pt_left);
			GUI.color = Color.white;
		}
		if ((GUITools.Button(new Rect(rect.width - 51f, 6f, 45f, rect.height - 12f), new GUIContent(LocalizedStrings.Send), BlueStonez.buttondark_small) || Event.current.keyCode == KeyCode.Return) && !AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.IsPlayerMuted && _lastMessageSentTimer > 0.29f)
		{
			SendChatMessage();
			GUI.FocusControl("@CurrentChatMessage");
		}
		GUI.enabled = flag;
		GUI.EndGroup();
	}

	private void GroupDrawUser(float vOffset, float width, CommUser user, bool allowSelfSelection = false)
	{
		int cmid = PlayerDataManager.Cmid;
		Rect rect = new Rect(3f, vOffset, width - 3f, 24f);
		if (Singleton<ChatManager>.Instance.SelectedCmid == user.Cmid)
		{
			Color uberStrikeBlue = ColorScheme.UberStrikeBlue;
			float r = uberStrikeBlue.r;
			Color uberStrikeBlue2 = ColorScheme.UberStrikeBlue;
			float g = uberStrikeBlue2.g;
			Color uberStrikeBlue3 = ColorScheme.UberStrikeBlue;
			GUI.color = new Color(r, g, uberStrikeBlue3.b, 0.5f);
			GUI.Label(rect, GUIContent.none, BlueStonez.box_white);
			GUI.color = Color.white;
		}
		bool flag = GUI.enabled;
		GUI.Label(new Rect(10f, vOffset + 3f, 11.2f, 16f), ChatManager.GetPresenceIcon(user.PresenceIndex), GUIStyle.none);
		GUI.Label(new Rect(23f, vOffset + 3f, 16f, 16f), UberstrikeIconsHelper.GetIconForChannel(user.Channel), GUIStyle.none);
		if (user.Cmid == PlayerDataManager.Cmid)
		{
			GUI.color = ColorScheme.ChatNameCurrentUser;
		}
		else if (user.IsFriend || user.IsClanMember)
		{
			GUI.color = ColorScheme.ChatNameFriendsUser;
		}
		else if (user.IsFacebookFriend)
		{
			GUI.color = ColorScheme.ChatNameFacebookFriendUser;
		}
		else
		{
			GUI.color = ColorScheme.GetNameColorByAccessLevel(user.AccessLevel);
		}
		if (PlayerDataManager.AccessLevel >= MemberAccessLevel.Moderator)
		{
			GUI.Label(new Rect(44f, vOffset, width - 66f, 24f), "{" + user.Cmid + "} " + user.Name, BlueStonez.label_interparkmed_10pt_left);
		}
		else
		{
			GUI.Label(new Rect(44f, vOffset, width - 66f, 24f), user.Name, BlueStonez.label_interparkmed_10pt_left);
		}
		GUI.color = Color.white;
		if (user.Cmid != cmid && GUI.Button(new Rect(rect.width - 17f, vOffset + 1f, 18f, 18f), GUIContent.none, BlueStonez.button_context))
		{
			SelectUser(user.Cmid);
			_playerMenu.Show(GUIUtility.GUIToScreenPoint(Event.current.mousePosition), user);
		}
		GUI.Box(rect.Expand(0, -1), GUIContent.none, BlueStonez.dropdown_list);
		if (MouseInput.IsMouseClickIn(rect))
		{
			if (Singleton<ChatManager>.Instance.SelectedCmid != user.Cmid)
			{
				if (allowSelfSelection || user.Cmid != cmid)
				{
					SelectUser(user.Cmid);
				}
				if (SelectedTab == TabArea.Private)
				{
					Singleton<ChatManager>.Instance.CreatePrivateChat(user.Cmid);
				}
			}
			else if (MouseInput.IsDoubleClick() && user.Cmid != cmid && SelectedTab != TabArea.Private)
			{
				Singleton<ChatManager>.Instance.CreatePrivateChat(user.Cmid);
				ScrollToUser(user.Cmid);
			}
		}
		else if (MouseInput.IsMouseClickIn(rect, 1))
		{
			_playerMenu.Show(GUIUtility.GUIToScreenPoint(Event.current.mousePosition), user);
		}
		ChatDialog value;
		if (SelectedTab == TabArea.Private && Singleton<ChatManager>.Instance._dialogsByCmid.TryGetValue(user.Cmid, out value) && value != null && value.HasUnreadMessage && value != Singleton<ChatManager>.Instance.SelectedDialog)
		{
			GUI.Label(new Rect(rect.width - 50f, vOffset, 25f, 25f), CommunicatorIcons.NewInboxMessage);
		}
		if (PlayerDataManager.AccessLevel >= MemberAccessLevel.Moderator)
		{
			int num = user.ModerationFlag & 0xC;
			if (num == 8)
			{
				GUI.Label(new Rect(width - 50f, vOffset + 3f, 20f, 20f), CommunicatorIcons.TagLightningBolt);
			}
		}
		GUI.enabled = flag;
	}

	private void SelectUser(int cmid)
	{
		Singleton<ChatManager>.Instance.SelectedCmid = cmid;
		ChatDialog value;
		if (SelectedTab == TabArea.Private && Singleton<ChatManager>.Instance._dialogsByCmid.TryGetValue(cmid, out value))
		{
			value.HasUnreadMessage = false;
			_currentChatMessage = string.Empty;
			Singleton<ChatManager>.Instance.SelectedDialog = value;
			_dialogScroll.y = float.MaxValue;
			autoScroll = true;
		}
	}

	private void ScrollToUser(int cmid)
	{
		ChatGroupPanel chatGroupPanel = Singleton<ChatManager>.Instance._commPanes[(int)SelectedTab];
		chatGroupPanel.ScrollToUser(cmid);
	}

	private void SendChatMessage()
	{
		if (string.IsNullOrEmpty(_currentChatMessage))
		{
			return;
		}
		_dialogScroll.y = float.MaxValue;
		autoScroll = true;
		_currentChatMessage = TextUtilities.ShortenText(TextUtilities.Trim(_currentChatMessage), 140, false);
		switch (SelectedTab)
		{
		case TabArea.InGame:
			GameState.Current.SendChatMessage(_currentChatMessage, ChatContext.Player);
			break;
		case TabArea.Lobby:
			if (!AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.SendLobbyChatMessage(_currentChatMessage))
			{
				_spammingNotificationTime = Time.time + 5f;
			}
			break;
		case TabArea.Clan:
			AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.SendClanChatMessage(_currentChatMessage);
			break;
		case TabArea.Private:
		{
			ChatDialog selectedDialog = Singleton<ChatManager>.Instance.SelectedDialog;
			if (selectedDialog != null)
			{
				AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.SendPrivateChatMessage(selectedDialog.UserCmid, selectedDialog.UserName, _currentChatMessage);
			}
			break;
		}
		}
		_lastMessageSentTimer = 0f;
		_currentChatMessage = string.Empty;
	}

	private Color GetNameColor(InstantMessage msg)
	{
		if (msg.Cmid == PlayerDataManager.Cmid)
		{
			return ColorScheme.ChatNameCurrentUser;
		}
		if (msg.IsFriend || msg.IsClan)
		{
			return ColorScheme.ChatNameFriendsUser;
		}
		if (msg.IsFacebookFriend)
		{
			return ColorScheme.ChatNameFacebookFriendUser;
		}
		return ColorScheme.GetNameColorByAccessLevel(msg.AccessLevel);
	}

	private void MenuCmdRemoveFriend(CommUser user)
	{
		if (user != null)
		{
			int friendCmid = user.Cmid;
			PopupSystem.ShowMessage(LocalizedStrings.RemoveFriendCaps, string.Format(LocalizedStrings.DoYouReallyWantToRemoveNFromYourFriendsList, user.Name), PopupSystem.AlertType.OKCancel, delegate
			{
				Singleton<InboxManager>.Instance.RemoveFriend(friendCmid);
			}, LocalizedStrings.Remove, null, LocalizedStrings.Cancel, PopupSystem.ActionType.Negative);
		}
	}

	private void MenuCmdAddFriend(CommUser user)
	{
		if (user != null)
		{
			FriendRequestPanelGUI friendRequestPanelGUI = PanelManager.Instance.OpenPanel(PanelType.FriendRequest) as FriendRequestPanelGUI;
			if ((bool)friendRequestPanelGUI)
			{
				friendRequestPanelGUI.SelectReceiver(user.Cmid, user.Name);
			}
		}
	}

	private string MenuCaptionMute(CommUser user)
	{
		if (user != null && Singleton<ChatManager>.Instance.IsMuted(user.Cmid))
		{
			return "Show Messages";
		}
		return "Hide Messages";
	}

	private bool MenuChkMute(CommUser user)
	{
		if (user != null)
		{
			return user.Cmid != PlayerDataManager.Cmid;
		}
		return false;
	}

	private void MenuCmdMute(CommUser user)
	{
		if (user != null)
		{
			if (Singleton<ChatManager>.Instance.IsMuted(user.Cmid))
			{
				Singleton<ChatManager>.Instance.ShowConversations(user.Cmid);
			}
			else
			{
				Singleton<ChatManager>.Instance.HideConversations(user.Cmid);
			}
		}
	}

	private void MenuCmdChat(CommUser user)
	{
		if (user != null)
		{
			Singleton<ChatManager>.Instance.CreatePrivateChat(user.Cmid);
			ScrollToUser(user.Cmid);
		}
	}

	private void MenuCmdSendMessage(CommUser user)
	{
		if (user != null)
		{
			SendMessagePanelGUI sendMessagePanelGUI = PanelManager.Instance.OpenPanel(PanelType.SendMessage) as SendMessagePanelGUI;
			if ((bool)sendMessagePanelGUI)
			{
				sendMessagePanelGUI.SelectReceiver(user.Cmid, user.Name);
			}
		}
	}

	private void MenuCmdJoinGame(CommUser user)
	{
		if (user != null && user.CurrentGame != null)
		{
			Singleton<GameStateController>.Instance.JoinNetworkGame(user.CurrentGame);
		}
	}

	private void MenuCmdInviteClan(CommUser user)
	{
		if (user != null)
		{
			InviteToClanPanelGUI inviteToClanPanelGUI = PanelManager.Instance.OpenPanel(PanelType.ClanRequest) as InviteToClanPanelGUI;
			if ((bool)inviteToClanPanelGUI)
			{
				inviteToClanPanelGUI.SelectReceiver(user.Cmid, user.ShortName);
			}
		}
	}

	private void MenuCmdCopyData(CommUser user)
	{
		if (user != null)
		{
			TextEditor textEditor = new TextEditor();
			textEditor.text = "<Cmid=" + user.Cmid + "> <Name=" + user.Name + ">";
			textEditor.SelectAll();
			textEditor.Copy();
		}
	}

	private void MenuCmdCopyMsg(CommUser user, InstantMessage msg)
	{
		if (msg != null)
		{
			TextEditor textEditor = new TextEditor();
			DateTime dateTime = msg.ArrivalTime.ToUniversalTime();
			textEditor.text = string.Concat("LobbyMessage from <name=", msg.PlayerName, "> <cmid=", msg.Cmid, ">: <", msg.Text, "> <", dateTime, ">");
			textEditor.SelectAll();
			textEditor.Copy();
		}
	}

	private void MenuCmdCopyPlayerName(CommUser user, InstantMessage msg)
	{
		Regex regex = new Regex("[^\"\\r\\n]*[\\[\\]][ ]");
		TextEditor textEditor = new TextEditor();
		if (msg != null)
		{
			if (Regex.IsMatch(msg.PlayerName, "[^\"\\r\\n]*[\\[\\]][ ]"))
			{
				string playerName = msg.PlayerName;
				string[] array = regex.Split(playerName);
				textEditor.text = "\"" + array[1] + "\"";
			}
			else
			{
				textEditor.text = "\"" + msg.PlayerName + "\"";
			}
		}
		else if (msg == null && user != null)
		{
			if (Regex.IsMatch(user.Name, "[^\"\\r\\n]*[\\[\\]][ ]"))
			{
				string input = user.Name;
				string[] array2 = regex.Split(input);
				textEditor.text = "\"" + array2[1] + "\"";
			}
			else
			{
				textEditor.text = "\"" + user.Name + "\"";
			}
		}
		textEditor.SelectAll();
		textEditor.Copy();
	}

	private void MenuCmdModeratePlayer(CommUser user)
	{
		if (user != null)
		{
			ModerationPanelGUI moderationPanelGUI = PanelManager.Instance.OpenPanel(PanelType.Moderation) as ModerationPanelGUI;
			if ((bool)moderationPanelGUI)
			{
				moderationPanelGUI.SetSelectedUser(user);
			}
		}
	}

	private bool MenuChkIsModerator(CommUser user)
	{
		return user != null && PlayerDataManager.AccessLevel >= MemberAccessLevel.Moderator;
	}

	private bool MenuChkAddFriend(CommUser user)
	{
		return user != null && user.Cmid != PlayerDataManager.Cmid && user.AccessLevel <= PlayerDataManager.AccessLevel && !PlayerDataManager.IsFriend(user.Cmid) && !PlayerDataManager.IsFacebookFriend(user.Cmid);
	}

	private bool MenuChkRemoveFriend(CommUser user)
	{
		return user != null && user.Cmid != PlayerDataManager.Cmid && PlayerDataManager.IsFriend(user.Cmid);
	}

	private bool MenuChkChat(CommUser user)
	{
		return user != null && user.Cmid != PlayerDataManager.Cmid && user.IsOnline;
	}

	private bool MenuChkSendMessage(CommUser user)
	{
		return user != null && user.Cmid != PlayerDataManager.Cmid && !GameState.Current.HasJoinedGame;
	}

	private bool MenuChkJoinGame(CommUser user)
	{
		return user != null && user.Cmid != PlayerDataManager.Cmid && user.IsInGame;
	}

	private bool MenuChkInviteClan(CommUser user)
	{
		return user != null && user.Cmid != PlayerDataManager.Cmid && (user.AccessLevel <= PlayerDataManager.AccessLevel || PlayerDataManager.IsFriend(user.Cmid) || PlayerDataManager.IsFacebookFriend(user.Cmid)) && PlayerDataManager.IsPlayerInClan && PlayerDataManager.CanInviteToClan && !PlayerDataManager.IsClanMember(user.Cmid);
	}
}
