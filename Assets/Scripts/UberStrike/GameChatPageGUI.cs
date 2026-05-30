using Cmune.DataCenter.Common.Entities;
using UberStrike.Core.Models;
using UberStrike.Realtime.UnitySdk;
using UnityEngine;

public class GameChatPageGUI : PageGUI
{
	private const float TitleHeight = 24f;

	private const int TAB_WIDTH = 150;

	private const int CHAT_USER_HEIGHT = 24;

	private Rect _mainRect;

	private Vector2 _dialogScroll;

	private float _spammingNotificationTime;

	private float _nextNaughtyListUpdate;

	private int _selectedCmid;

	private float _yPosition;

	private float _lastMessageSentTimer = 0.3f;

	private string _currentChatMessage = string.Empty;

	private PopupMenu _playerMenu;

	private float _keyboardOffset;

	private void Awake()
	{
		_playerMenu = new PopupMenu();
		base.IsOnGUIEnabled = true;
	}

	private void Start()
	{
		_playerMenu.AddMenuItem("Add Friend", MenuCmdAddFriend, MenuChkAddFriend);
		_playerMenu.AddMenuItem("Unfriend", MenuCmdRemoveFriend, MenuChkRemoveFriend);
		_playerMenu.AddMenuItem(LocalizedStrings.InviteToClan, MenuCmdInviteClan, MenuChkInviteClan);
		_playerMenu.AddMenuItem(LocalizedStrings.Report + " Cheater", MenuCmdReportPlayer, MenuChkReportPlayer);
		if (PlayerDataManager.AccessLevel >= MemberAccessLevel.Moderator)
		{
			_playerMenu.AddMenuItem("- - - - - - - - - - - - -", null, (CommUser P_0) => true);
			_playerMenu.AddMenuItem("Copy Data", MenuCmdCopyData, (CommUser P_0) => true);
			_playerMenu.AddMenuItem("Moderate Player", MenuCmdModeratePlayer, (CommUser P_0) => true);
		}
	}

	private void MenuCmdCopyData(CommUser user)
	{
		if (user != null)
		{
			TextEditor textEditor = new TextEditor();
			textEditor.text = "<Cmid:" + user.Cmid + "> <Name:" + user.Name + ">";
			textEditor.SelectAll();
			textEditor.Copy();
		}
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
		}
	}

	public override void DrawGUI(Rect rect)
	{
		GUI.BeginGroup(rect, BlueStonez.window);
		if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
		{
			GUIUtility.keyboardControl = 0;
		}
		Rect rect2 = new Rect(0f, 21f, 150f, rect.height - 21f);
		Rect rect3 = new Rect(149f, 0f, rect.width - 150f, 22f);
		Rect rect4 = new Rect(150f, 22f, rect.width - 150f, rect.height - 22f - 36f - _keyboardOffset);
		Rect rect5 = new Rect(149f, rect.height - 37f, rect.width - 150f + 1f, 37f);
		GUITools.PushGUIState();
		GUI.enabled &= !PopupMenu.IsEnabled;
		ChatGroupPanel pane = Singleton<ChatManager>.Instance._commPanes[3];
		DoDialogFooter(rect5, pane, Singleton<ChatManager>.Instance.InGameDialog);
		DrawCommPane(rect2, pane);
		DoDialogHeader(rect3, Singleton<ChatManager>.Instance.InGameDialog);
		DoDialog(rect4, pane, Singleton<ChatManager>.Instance.InGameDialog);
		GUITools.PopGUIState();
		if (PopupMenu.Current != null)
		{
			PopupMenu.Current.Draw();
		}
		GUI.EndGroup();
		GuiManager.DrawTooltip();
	}

	private bool IsMobileChannel(ChannelType channel)
	{
		return channel == ChannelType.Android || channel == ChannelType.IPad || channel == ChannelType.IPhone;
	}

	public void DrawCommPane(Rect rect, ChatGroupPanel pane)
	{
		GUI.BeginGroup(rect);
		pane.WindowHeight = rect.height;
		float height = Mathf.Max(pane.WindowHeight, pane.ContentHeight);
		float num = 0f;
		pane.Scroll = GUITools.BeginScrollView(new Rect(0f, 0f, rect.width, pane.WindowHeight), pane.Scroll, new Rect(0f, 0f, rect.width - 17f, height), false, true);
		GUI.BeginGroup(new Rect(0f, 0f, rect.width, pane.WindowHeight + pane.Scroll.y));
		int num2 = 0;
		string value = pane.SearchText.ToLower();
		GUI.BeginGroup(new Rect(0f, num, rect.width - 17f, GameState.Current.Players.Count * 24));
		foreach (GameActorInfo value2 in GameState.Current.Players.Values)
		{
			if (string.IsNullOrEmpty(value) || value2.PlayerName.ToLower().Contains(value))
			{
				GroupDrawUser(num2++ * 24, rect.width - 17f, value2, true);
			}
		}
		GUI.EndGroup();
		num += 24f + (float)(GameState.Current.Players.Count * 24);
		GUI.EndGroup();
		GUITools.EndScrollView();
		pane.ContentHeight = num;
		GUI.EndGroup();
	}

	private void DoDialog(Rect rect, ChatGroupPanel pane, ChatDialog dialog)
	{
		if (dialog == null)
		{
			return;
		}
		if (dialog.CheckSize(rect) && !Input.GetMouseButton(0))
		{
			_dialogScroll.y = float.MaxValue;
		}
		GUI.BeginGroup(new Rect(rect.x, rect.y + Mathf.Clamp(rect.height - dialog._heightCache, 0f, rect.height), rect.width, rect.height));
		int num = 0;
		float num2 = 0f;
		_dialogScroll = GUITools.BeginScrollView(new Rect(0f, 0f, dialog._frameSize.x, dialog._frameSize.y), _dialogScroll, new Rect(0f, 0f, dialog._contentSize.x, dialog._contentSize.y));
		foreach (InstantMessage item in dialog._msgQueue)
		{
			if (dialog.CanShow == null || dialog.CanShow(item.Context))
			{
				if (num % 2 == 0)
				{
					GUI.Label(new Rect(0f, num2, dialog._contentSize.x - 1f, item.Height), GUIContent.none, BlueStonez.box_grey38);
				}
				if (GUI.Button(new Rect(0f, num2, dialog._contentSize.x - 1f, item.Height), GUIContent.none, BlueStonez.dropdown_list))
				{
					_selectedCmid = item.Cmid;
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
		}
		GUITools.EndScrollView();
		dialog._heightCache = num2;
		GUI.EndGroup();
	}

	private void DoDialogHeader(Rect rect, ChatDialog d)
	{
		GUI.Label(rect, GUIContent.none, BlueStonez.window_standard_grey38);
		GUI.Label(rect, d.Title, BlueStonez.label_interparkbold_11pt);
	}

	private void DoDialogFooter(Rect rect, ChatGroupPanel pane, ChatDialog dialog)
	{
		GUI.BeginGroup(rect, BlueStonez.window_standard_grey38);
		bool flag = GUI.enabled;
		GUI.enabled &= !AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.IsPlayerMuted && dialog != null;
		GUI.SetNextControlName("@CurrentChatMessage");
		_currentChatMessage = GUI.TextField(new Rect(6f, 6f, rect.width - 60f, rect.height - 12f), _currentChatMessage, 140, BlueStonez.textField);
		_currentChatMessage = _currentChatMessage.Trim('\n');
		if (_spammingNotificationTime > Time.time)
		{
			GUI.color = Color.red;
			GUI.Label(new Rect(15f, 6f, rect.width - 66f, rect.height - 12f), LocalizedStrings.DontSpamTheLobbyChat, BlueStonez.label_interparkmed_10pt_left);
			GUI.color = Color.white;
		}
		else
		{
			string empty = string.Empty;
			empty = ((dialog == null || dialog.UserCmid <= 0) ? LocalizedStrings.EnterAMessageHere : ((!dialog.CanChat) ? (dialog.UserName + LocalizedStrings.Offline) : LocalizedStrings.EnterAMessageHere));
			if (string.IsNullOrEmpty(_currentChatMessage) && GUI.GetNameOfFocusedControl() != "@CurrentChatMessage")
			{
				GUI.color = new Color(1f, 1f, 1f, 0.3f);
				GUI.Label(new Rect(10f, 6f, rect.width - 66f, rect.height - 12f), empty, BlueStonez.label_interparkmed_10pt_left);
				GUI.color = Color.white;
			}
		}
		if ((GUITools.Button(new Rect(rect.width - 51f, 6f, 45f, rect.height - 12f), new GUIContent(LocalizedStrings.Send), BlueStonez.buttondark_small) || Event.current.keyCode == KeyCode.Return) && !AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.IsPlayerMuted && _lastMessageSentTimer > 0.29f)
		{
			SendChatMessage();
			GUI.FocusControl("@CurrentChatMessage");
		}
		GUI.enabled = flag;
		GUI.EndGroup();
	}

	private Texture2D GetIcon(GameActorInfo info)
	{
		if (info.IsSpectator)
		{
			return CommunicatorIcons.PresenceOnline;
		}
		if (!info.IsAlive)
		{
			return CommunicatorIcons.SkullCrossbonesIcon;
		}
		return CommunicatorIcons.PresencePlaying;
	}

	private void GroupDrawUser(float vOffset, float width, GameActorInfo user, bool allowSelfSelection = false)
	{
		int cmid = PlayerDataManager.Cmid;
		Rect rect = new Rect(3f, vOffset, width - 3f, 24f);
		if (_selectedCmid == user.Cmid)
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
		GUI.Label(new Rect(10f, vOffset + 3f, 16f, 16f), GetIcon(user), GUIStyle.none);
		GUI.Label(new Rect(23f, vOffset + 3f, 16f, 16f), UberstrikeIconsHelper.GetIconForChannel(user.Channel), GUIStyle.none);
		switch (user.TeamID)
		{
		case TeamID.RED:
			GUI.color = ColorScheme.GuiTeamRed;
			break;
		case TeamID.BLUE:
			GUI.color = ColorScheme.GuiTeamBlue;
			break;
		default:
			GUI.color = Color.white;
			break;
		}
		GUI.Label(new Rect(44f, vOffset, width - 66f, 24f), user.PlayerName, BlueStonez.label_interparkmed_10pt_left);
		GUI.color = Color.white;
		if (user.Cmid != cmid && GUI.Button(new Rect(rect.width - 17f, vOffset + 1f, 18f, 18f), GUIContent.none, BlueStonez.button_context))
		{
			_selectedCmid = user.Cmid;
			_playerMenu.Show(Event.current.mousePosition, new CommUser(user));
		}
		GUI.Box(rect.Expand(0, -1), GUIContent.none, BlueStonez.dropdown_list);
		if (MouseInput.IsMouseClickIn(rect))
		{
			if (_selectedCmid != user.Cmid && (allowSelfSelection || user.Cmid != cmid))
			{
				_selectedCmid = user.Cmid;
			}
		}
		else if (MouseInput.IsMouseClickIn(rect, 1))
		{
			_playerMenu.Show(Event.current.mousePosition, new CommUser(user));
		}
		GUI.enabled = flag;
	}

	private void SendChatMessage()
	{
		if (!string.IsNullOrEmpty(_currentChatMessage))
		{
			_dialogScroll.y = float.MaxValue;
			_currentChatMessage = TextUtilities.ShortenText(TextUtilities.Trim(_currentChatMessage), 140, false);
			GameState.Current.SendChatMessage(_currentChatMessage, ChatContext.Player);
			_lastMessageSentTimer = 0f;
			_currentChatMessage = string.Empty;
		}
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

	private void MenuCmdReportPlayer(CommUser user)
	{
		if (user != null && Singleton<GameStateController>.Instance.Client.IsInsideRoom)
		{
			PopupSystem.ShowMessage(LocalizedStrings.ReportPlayerCaps, "Are you sure you want to report\n" + user.Name + "\nfor cheating?", PopupSystem.AlertType.OKCancel, delegate
			{
				Singleton<GameStateController>.Instance.Client.Operations.SendReportPlayer(user.Cmid, PlayerDataManager.AuthToken);
			}, "Report", null, "Cancel", PopupSystem.ActionType.Negative);
		}
	}

	private bool MenuChkReportPlayer(CommUser user)
	{
		return user != null && user.Cmid != PlayerDataManager.Cmid && user.AccessLevel == MemberAccessLevel.Default;
	}

	private bool MenuChkAddFriend(CommUser user)
	{
		return user != null && user.Cmid != PlayerDataManager.Cmid && user.AccessLevel <= PlayerDataManager.AccessLevel && !PlayerDataManager.IsFriend(user.Cmid);
	}

	private bool MenuChkRemoveFriend(CommUser user)
	{
		return user != null && user.Cmid != PlayerDataManager.Cmid && PlayerDataManager.IsFriend(user.Cmid);
	}

	private bool MenuChkInviteClan(CommUser user)
	{
		return user != null && user.Cmid != PlayerDataManager.Cmid && (user.AccessLevel <= PlayerDataManager.AccessLevel || PlayerDataManager.IsFriend(user.Cmid)) && PlayerDataManager.IsPlayerInClan && PlayerDataManager.CanInviteToClan && !PlayerDataManager.IsClanMember(user.Cmid);
	}
}
