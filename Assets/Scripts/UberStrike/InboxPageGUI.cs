using Cmune.DataCenter.Common.Entities;
using UberStrike.WebService.Unity;
using UnityEngine;

public class InboxPageGUI : MonoBehaviour
{
	private const int PanelHeight = 50;

	private const int TAB_MESSAGE = 0;

	private const int TAB_REQUEST = 1;

	[SerializeField]
	private Texture2D _newMessage;

	private int _threadWidth;

	private int _selectedTab;

	private GUIContent[] _tabContents;

	private int _threadViewWidth;

	private int _threadViewHeight;

	private Vector2 _threadScroll;

	private Vector2 _requestScroll;

	private int _messageViewWidth;

	private int _messageViewHeight;

	private string _replyMessage = string.Empty;

	private string _searchMessage = string.Empty;

	private int _requestWidth;

	private int _requestHeight;

	private void Start()
	{
		_tabContents = new GUIContent[2]
		{
			new GUIContent(LocalizedStrings.MessagesCaps),
			new GUIContent(LocalizedStrings.RequestsCaps)
		};
	}

	private void OnGUI()
	{
		GUI.depth = 11;
		GUI.skin = BlueStonez.Skin;
		Rect position = new Rect(0f, GlobalUIRibbon.Instance.Height(), Screen.width, Screen.height - GlobalUIRibbon.Instance.Height());
		_threadWidth = (int)position.width / 4;
		GUI.BeginGroup(position, BlueStonez.window_standard_grey38);
		GUI.enabled = PlayerDataManager.IsPlayerLoggedIn && IsNoPanelOpen();
		DrawInbox(new Rect(0f, 0f, position.width, position.height));
		GUI.enabled = true;
		GUI.EndGroup();
	}

	private void DrawInbox(Rect rect)
	{
		DoTitle(new Rect(1f, 0f, rect.width - 2f, 72f));
		switch (_selectedTab)
		{
		case 0:
			DoToolbarMessage(new Rect(1f, 72f, rect.width - 2f, 40f));
			DoThreads(new Rect(1f, 112f, _threadWidth, rect.height - 112f));
			DoMessages(new Rect(_threadWidth, 110f, rect.width - (float)_threadWidth, rect.height - 112f));
			break;
		case 1:
		{
			float num = Mathf.Max(Singleton<InboxManager>.Instance.NextRequestRefresh - Time.time, 0f);
			GUITools.PushGUIState();
			GUI.enabled &= num == 0f;
			if (GUITools.Button(new Rect(rect.width - 131f, 80f, 123f, 24f), new GUIContent(string.Format(LocalizedStrings.Refresh + " {0}", (!(num > 0f)) ? string.Empty : ("(" + num.ToString("N0") + ")"))), BlueStonez.buttondark_medium))
			{
				Singleton<InboxManager>.Instance.RefreshAllRequests();
			}
			GUITools.PopGUIState();
			DoRequests(new Rect(0f, 112f, rect.width, rect.height - 112f));
			break;
		}
		}
	}

	private void DoTitle(Rect rect)
	{
		GUI.BeginGroup(rect, BlueStonez.tab_strip_large);
		int num = UnityGUI.Toolbar(new Rect(1f, 32f, 508f, 40f), _selectedTab, _tabContents, _tabContents.Length, BlueStonez.tab_large);
		if (GUI.changed)
		{
			GUI.changed = false;
			AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.ButtonClick, 0uL, 1f, 1f);
		}
		if (num != _selectedTab)
		{
			GUIUtility.keyboardControl = 0;
			_selectedTab = num;
		}
		if ((int)Singleton<InboxManager>.Instance.UnreadMessageCount > 0)
		{
			GUI.DrawTexture(new Rect(133f, 32f, 20f, 20f), _newMessage);
		}
		if (Singleton<InboxManager>.Instance.IncomingClanRequests.Value.Count > 0 || Singleton<InboxManager>.Instance.FriendRequests.Value.Count > 0)
		{
			GUI.DrawTexture(new Rect(311f, 32f, 20f, 20f), _newMessage);
		}
		GUI.EndGroup();
	}

	private void DoToolbarMessage(Rect rect)
	{
		GUI.BeginGroup(rect);
		GUI.Label(new Rect(8f, 8f, 206f, 24f), string.Format(LocalizedStrings.YouHaveNNewMessages, Singleton<InboxManager>.Instance.UnreadMessageCount), BlueStonez.label_interparkbold_16pt_left);
		if (_selectedTab == 0)
		{
			Rect position = new Rect(rect.width - 368f, 8f, 140f, 24f);
			GUI.SetNextControlName("SearchMessage");
			_searchMessage = GUI.TextField(position, _searchMessage, BlueStonez.textField);
			if (string.IsNullOrEmpty(_searchMessage) && GUI.GetNameOfFocusedControl() != "SearchMessage")
			{
				GUI.color = new Color(1f, 1f, 1f, 0.3f);
				GUI.Label(position, " " + LocalizedStrings.SearchMessages, BlueStonez.label_interparkbold_11pt_left);
				GUI.color = Color.white;
			}
		}
		if (GUITools.Button(new Rect(rect.width - 224f, 8f, 106f, 24f), new GUIContent(LocalizedStrings.NewMessage), BlueStonez.buttondark_medium))
		{
			PanelManager.Instance.OpenPanel(PanelType.SendMessage);
		}
		float num = Mathf.Max(Singleton<InboxManager>.Instance.NextInboxRefresh - Time.time, 0f);
		GUITools.PushGUIState();
		GUI.enabled &= num == 0f;
		if (GUITools.Button(new Rect(rect.width - 114f, 8f, 106f, 24f), new GUIContent(string.Format(LocalizedStrings.CheckMail + " {0}", (!(num > 0f)) ? string.Empty : ("(" + num.ToString("N0") + ")"))), BlueStonez.buttondark_medium))
		{
			Singleton<InboxManager>.Instance.LoadNextPageThreads();
		}
		GUITools.PopGUIState();
		GUI.EndGroup();
	}

	private bool IsNoPanelOpen()
	{
		return !PanelManager.IsAnyPanelOpen;
	}

	private void DoThreads(Rect rect)
	{
		rect = new Rect(rect.x + 8f, rect.y, rect.width - 8f, rect.height - 8f);
		GUI.Box(rect, GUIContent.none, BlueStonez.window);
		if (Singleton<InboxManager>.Instance.ThreadCount > 0)
		{
			Vector2 threadScroll = GUITools.BeginScrollView(rect, _threadScroll, new Rect(0f, 0f, _threadViewWidth, _threadViewHeight));
			bool flag = threadScroll.y > _threadScroll.y;
			_threadScroll = threadScroll;
			int num = 0;
			for (int i = 0; i < Singleton<InboxManager>.Instance.ThreadCount; i++)
			{
				InboxThread inboxThread = Singleton<InboxManager>.Instance.AllThreads[i];
				if (string.IsNullOrEmpty(_searchMessage) || inboxThread.Contains(_searchMessage))
				{
					num = inboxThread.DrawThread(num, _threadViewWidth);
					GUI.Label(new Rect(4f, num, _threadViewWidth, 1f), GUIContent.none, BlueStonez.horizontal_line_grey95);
				}
			}
			if (Singleton<InboxManager>.Instance.IsLoadingThreads)
			{
				GUI.Label(new Rect(0f, num, rect.width, 30f), "Loading threads...", BlueStonez.label_interparkmed_11pt);
				num += 30;
			}
			else
			{
				if (Singleton<InboxManager>.Instance.IsNoMoreThreads)
				{
					GUI.contentColor = Color.gray;
					GUI.Label(new Rect(0f, num, rect.width, 30f), "No more threads", BlueStonez.label_interparkmed_11pt);
					GUI.contentColor = Color.white;
				}
				num += 30;
				float num2 = Mathf.Max((float)num - rect.height, 0f);
				if (flag && _threadScroll.y >= num2)
				{
					Singleton<InboxManager>.Instance.LoadNextPageThreads();
				}
			}
			_threadViewHeight = num;
			_threadViewWidth = (int)((!((float)_threadViewHeight > rect.height)) ? (rect.width - 8f) : (rect.width - 22f));
			GUITools.EndScrollView();
		}
		else if (Singleton<InboxManager>.Instance.IsLoadingThreads)
		{
			GUI.Label(rect, "Loading threads...", BlueStonez.label_interparkbold_13pt);
		}
		else
		{
			GUI.Label(rect, LocalizedStrings.Empty, BlueStonez.label_interparkmed_11pt);
		}
	}

	private void DoMessages(Rect rect)
	{
		InboxThread current = InboxThread.Current;
		bool flag = current != null && current.IsAdmin;
		Rect position = new Rect(rect.x + 8f, rect.y + 2f, rect.width - 16f, rect.height - 8f);
		GUI.Box(position, GUIContent.none, BlueStonez.box_grey50);
		string text = LocalizedStrings.NoConversationSelected;
		if (current != null)
		{
			text = string.Format(LocalizedStrings.BetweenYouAndN, current.Name);
			if (GUI.Button(new Rect(position.x + 10f, position.y + 10f, 150f, 20f), "Delete Conversation", BlueStonez.buttondark_medium))
			{
				InboxThread.Current = null;
				Singleton<InboxManager>.Instance.DeleteThread(current.ThreadId);
			}
		}
		GUI.contentColor = new Color(1f, 1f, 1f, 0.75f);
		GUI.Label(new Rect(position.x + 10f, position.y, position.width - 20f, 40f), text, BlueStonez.label_interparkmed_11pt_right);
		GUI.contentColor = Color.white;
		GUI.Label(new Rect(position.x + 4f, position.y + 40f, position.width - 8f, 1f), GUIContent.none, BlueStonez.horizontal_line_grey95);
		int y = 8;
		Rect position2 = new Rect(position.x + 8f, position.y + 48f, position.width - 8f, position.height - (float)((!flag) ? 90 : 49));
		if (InboxThread.Current != null)
		{
			current.Scroll = GUITools.BeginScrollView(position2, current.Scroll, new Rect(0f, 0f, _messageViewWidth, _messageViewHeight));
			y = current.DrawMessageList(y, _messageViewWidth, position2.height, current.Scroll.y);
			if ((float)y > position2.height)
			{
				_messageViewHeight = y;
				_messageViewWidth = (int)(position2.width - 22f);
			}
			else
			{
				_messageViewHeight = (int)position2.height;
				_messageViewWidth = (int)position2.width - 8;
			}
			GUITools.EndScrollView();
		}
		else
		{
			GUI.Label(position2, "Select a message thread", BlueStonez.label_interparkbold_13pt);
		}
		if (!flag)
		{
			GUITools.PushGUIState();
			GUI.enabled &= InboxThread.Current != null;
			GUI.Box(new Rect(rect.x + 8f, rect.y + rect.height - 51f, rect.width - 16f, 45f), GUIContent.none, BlueStonez.window_standard_grey38);
			DoReply(new Rect(rect.x, rect.y + rect.height - 51f, rect.width, 45f));
			GUITools.PopGUIState();
		}
	}

	private void DoReply(Rect rect)
	{
		Rect position = new Rect(rect.x + (rect.width - 420f) / 2f, rect.y + 12f, 420f, rect.height);
		GUI.BeginGroup(position);
		GUI.SetNextControlName("Reply Edit");
		_replyMessage = GUI.TextField(new Rect(0f, 0f, position.width - 64f, 24f), _replyMessage, 140, BlueStonez.textField);
		_replyMessage = _replyMessage.Trim('\n');
		if (GUI.GetNameOfFocusedControl().Equals("Reply Edit") && !string.IsNullOrEmpty(_replyMessage) && Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Return)
		{
			SendMessage();
		}
		GUITools.PushGUIState();
		GUI.enabled &= !string.IsNullOrEmpty(_replyMessage);
		if (GUITools.Button(new Rect(position.width - 64f, 0f, 64f, 24f), new GUIContent(LocalizedStrings.Reply), BlueStonez.buttondark_medium))
		{
			SendMessage();
		}
		GUITools.PopGUIState();
		GUI.EndGroup();
	}

	private void SendMessage()
	{
		if (InboxThread.Current != null)
		{
			Singleton<InboxManager>.Instance.SendPrivateMessage(InboxThread.Current.ThreadId, InboxThread.Current.Name, _replyMessage);
			_replyMessage = string.Empty;
			GUIUtility.keyboardControl = 0;
		}
	}

	private void DoRequests(Rect rect)
	{
		Rect position = new Rect(rect.x + 8f, rect.y, rect.width - 16f, rect.height - 8f);
		GUI.BeginGroup(position, BlueStonez.window);
		int num = 5;
		_requestHeight = 180 + Singleton<InboxManager>.Instance.FriendRequests.Value.Count * 60 + Singleton<InboxManager>.Instance.IncomingClanRequests.Value.Count * 60 + Singleton<InboxManager>.Instance._outgoingClanRequests.Count * 60;
		_requestWidth = (int)position.width - ((!((float)_requestHeight > position.height)) ? 8 : 22);
		_requestScroll = GUITools.BeginScrollView(new Rect(0f, num, position.width, position.height), _requestScroll, new Rect(0f, 0f, _requestWidth, _requestHeight));
		GUI.Box(new Rect(4f, 0f, _requestWidth, 50f), GUIContent.none, BlueStonez.box_grey38);
		GUI.Label(new Rect(14f, 0f, _requestWidth - 10, 50f), string.Format(LocalizedStrings.FriendRequestsYouHaveNPendingRequests, Singleton<InboxManager>.Instance.FriendRequests.Value.Count.ToString(), (Singleton<InboxManager>.Instance.FriendRequests.Value.Count == 1) ? string.Empty : "s"), BlueStonez.label_interparkmed_18pt_left);
		num += 50;
		for (int i = 0; i < Singleton<InboxManager>.Instance.FriendRequests.Value.Count; i++)
		{
			DrawFriendRequestView(Singleton<InboxManager>.Instance.FriendRequests.Value[i], num, _requestWidth);
			GUI.Label(new Rect(25f, num + Mathf.RoundToInt(9f), 32f, 32f), (i + 1).ToString(), BlueStonez.label_interparkbold_32pt);
			num += 60;
		}
		GUI.Box(new Rect(4f, num, _requestWidth, 50f), GUIContent.none, BlueStonez.box_grey38);
		GUI.Label(new Rect(14f, num, _requestWidth - 10, 50f), string.Format("Clan Requests - You have {0} incoming invite{1}", Singleton<InboxManager>.Instance.IncomingClanRequests.Value.Count, (Singleton<InboxManager>.Instance.IncomingClanRequests.Value.Count == 1) ? string.Empty : "s"), BlueStonez.label_interparkmed_18pt_left);
		num += 55;
		for (int j = 0; j < Singleton<InboxManager>.Instance.IncomingClanRequests.Value.Count; j++)
		{
			DrawIncomingClanInvitation(Singleton<InboxManager>.Instance.IncomingClanRequests.Value[j], num, _requestWidth);
			GUI.Label(new Rect(25f, num + Mathf.RoundToInt(9f), 32f, 32f), (j + 1).ToString(), BlueStonez.label_interparkbold_32pt);
			num += 60;
		}
		GUI.Box(new Rect(4f, num, _requestWidth, 50f), GUIContent.none, BlueStonez.box_grey38);
		GUI.Label(new Rect(14f, num, _requestWidth - 10, 50f), string.Format("Clan Requests - You have {0} outgoing invite{1}", Singleton<InboxManager>.Instance._outgoingClanRequests.Count, (Singleton<InboxManager>.Instance._outgoingClanRequests.Count == 1) ? string.Empty : "s"), BlueStonez.label_interparkmed_18pt_left);
		num += 55;
		for (int k = 0; k < Singleton<InboxManager>.Instance._outgoingClanRequests.Count; k++)
		{
			DrawOutgoingClanInvitation(Singleton<InboxManager>.Instance._outgoingClanRequests[k], num, _requestWidth);
			GUI.Label(new Rect(25f, num + Mathf.RoundToInt(9f), 32f, 32f), (k + 1).ToString(), BlueStonez.label_interparkbold_32pt);
			num += 60;
		}
		GUITools.EndScrollView();
		GUI.EndGroup();
	}

	public void DrawFriendRequestView(ContactRequestView request, float y, int width)
	{
		Rect position = new Rect(4f, y + 4f, width - 1, 50f);
		GUI.BeginGroup(position);
		Rect position2 = new Rect(0f, 0f, position.width, position.height - 1f);
		if (GUI.enabled && position2.Contains(Event.current.mousePosition))
		{
			GUI.Box(position2, GUIContent.none, BlueStonez.box_grey50);
		}
		GUI.Label(new Rect(80f, 5f, position.width - 250f, 20f), string.Format("{0}: {1}", LocalizedStrings.FriendRequest, request.InitiatorName), BlueStonez.label_interparkbold_13pt_left);
		GUI.Label(new Rect(80f, 30f, position.width - 250f, 20f), "> " + request.InitiatorMessage, BlueStonez.label_interparkmed_11pt_left);
		if (GUITools.Button(new Rect(position.width - 120f - 18f, 5f, 60f, 20f), new GUIContent(LocalizedStrings.Accept), BlueStonez.buttondark_medium))
		{
			Singleton<InboxManager>.Instance.AcceptContactRequest(request.RequestId);
		}
		if (GUITools.Button(new Rect(position.width - 50f - 18f, 5f, 60f, 20f), new GUIContent(LocalizedStrings.Ignore), BlueStonez.buttondark_medium))
		{
			Singleton<InboxManager>.Instance.DeclineContactRequest(request.RequestId);
		}
		GUI.EndGroup();
		GUI.Label(new Rect(4f, y + 50f + 8f, width, 1f), GUIContent.none, BlueStonez.horizontal_line_grey95);
	}

	private void DrawIncomingClanInvitation(GroupInvitationView view, int y, int width)
	{
		Rect position = new Rect(4f, y + 4, width - 1, 50f);
		GUI.BeginGroup(position);
		Rect position2 = new Rect(0f, 0f, position.width, position.height - 1f);
		if (GUI.enabled && position2.Contains(Event.current.mousePosition))
		{
			GUI.Box(position2, GUIContent.none, BlueStonez.box_grey50);
		}
		GUI.Label(new Rect(80f, 5f, position.width - 250f, 20f), string.Format("{0}: {1}", LocalizedStrings.ClanInvite, view.GroupName), BlueStonez.label_interparkbold_13pt_left);
		GUI.Label(new Rect(80f, 30f, position.width - 250f, 20f), "> " + view.Message, BlueStonez.label_interparkmed_11pt_left);
		if (GUITools.Button(new Rect(position.width - 120f - 18f, 5f, 60f, 20f), new GUIContent(LocalizedStrings.Accept), BlueStonez.buttondark_medium))
		{
			if (PlayerDataManager.IsPlayerInClan)
			{
				PopupSystem.ShowMessage(LocalizedStrings.Error, LocalizedStrings.YouAlreadyInClanMsg, PopupSystem.AlertType.OK);
			}
			else
			{
				int requestId = view.GroupInvitationId;
				PopupSystem.ShowMessage(LocalizedStrings.Accept, "Do you want to accept this clan invitation?", PopupSystem.AlertType.OKCancel, delegate
				{
					Singleton<InboxManager>.Instance.AcceptClanRequest(requestId);
				}, "Join", null, LocalizedStrings.Cancel, PopupSystem.ActionType.Positive);
			}
		}
		if (GUITools.Button(new Rect(position.width - 50f - 18f, 5f, 60f, 20f), new GUIContent(LocalizedStrings.Ignore), BlueStonez.buttondark_medium))
		{
			Singleton<InboxManager>.Instance.DeclineClanRequest(view.GroupInvitationId);
		}
		GUI.EndGroup();
		GUI.Label(new Rect(4f, y + 50 + 8, width, 1f), GUIContent.none, BlueStonez.horizontal_line_grey95);
	}

	private void DrawOutgoingClanInvitation(GroupInvitationView view, int y, int width)
	{
		Rect position = new Rect(4f, y + 4, width - 1, 50f);
		GUI.BeginGroup(position);
		Rect position2 = new Rect(0f, 0f, position.width, position.height - 1f);
		if (GUI.enabled && position2.Contains(Event.current.mousePosition))
		{
			GUI.Box(position2, GUIContent.none, BlueStonez.box_grey50);
		}
		GUI.Label(new Rect(80f, 5f, position.width - 250f, 20f), string.Format("You invited: {0}", view.InviteeName), BlueStonez.label_interparkbold_13pt_left);
		GUI.Label(new Rect(80f, 30f, position.width - 250f, 20f), "> " + view.Message, BlueStonez.label_interparkmed_11pt_left);
		if (GUITools.Button(new Rect(position.width - 140f, 5f, 120f, 20f), new GUIContent(LocalizedStrings.CancelInvite), BlueStonez.buttondark_medium))
		{
			int groupInvitationId = view.GroupInvitationId;
			if (Singleton<InboxManager>.Instance._outgoingClanRequests.Remove(view))
			{
				ClanWebServiceClient.CancelInvitation(groupInvitationId, PlayerDataManager.AuthToken, null, delegate
				{
				});
			}
		}
		GUI.EndGroup();
		GUI.Label(new Rect(4f, y + 50 + 8, width, 1f), GUIContent.none, BlueStonez.horizontal_line_grey95);
	}
}
