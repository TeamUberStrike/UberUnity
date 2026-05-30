using Cmune.DataCenter.Common.Entities;
using UnityEngine;

public class SendMessagePanelGUI : PanelGuiBase
{
	private const string FocusReceiver = "Message Receiver";

	private const string FocusContent = "Message Content";

	private bool _useFixedReceiver;

	private bool _showComposeMessage;

	private bool _showReceiverDropdownList;

	private string _msgReceiver;

	private string _msgContent;

	private string _lastMsgRcvName;

	private int _msgRcvCmid;

	private int _receiverCount;

	private float _rcvDropdownWidth;

	private float _rcvDropdownHeight;

	private Vector2 _friendDropdownScroll;

	private void OnGUI()
	{
		if (_showComposeMessage)
		{
			GUI.depth = 3;
			GUI.skin = BlueStonez.Skin;
			Rect rect = new Rect((Screen.width - 480) / 2, (Screen.height - 300) / 2, 480f, 300f);
			GUI.Box(rect, GUIContent.none, BlueStonez.window);
			DoCompose(rect);
			if (_showReceiverDropdownList)
			{
				DoReceiverDropdownList(rect);
			}
			_rcvDropdownHeight = Mathf.Lerp(_rcvDropdownHeight, _showReceiverDropdownList ? 146 : 0, Time.deltaTime * 9f);
			if (!_showReceiverDropdownList && Mathf.Approximately(_rcvDropdownHeight, 0f))
			{
				_rcvDropdownHeight = 0f;
			}
			GUI.enabled = true;
		}
	}

	private void DoCompose(Rect rect)
	{
		Rect position = new Rect(rect.x + (rect.width - 480f) / 2f, rect.y + (rect.height - 300f) / 2f, 480f, 290f);
		GUI.BeginGroup(position, BlueStonez.window);
		int num = 35;
		int num2 = 120;
		int num3 = 320;
		int num4 = 70;
		int num5 = 100;
		GUI.Label(new Rect(0f, 0f, position.width, 0f), LocalizedStrings.NewMessage, BlueStonez.tab_strip);
		GUI.Box(new Rect(12f, 55f, position.width - 24f, position.height - 101f), GUIContent.none, BlueStonez.window_standard_grey38);
		GUI.Label(new Rect(num, num4, 75f, 20f), LocalizedStrings.To, BlueStonez.label_interparkbold_18pt_right);
		GUI.Label(new Rect(num, num5, 75f, 20f), LocalizedStrings.Message, BlueStonez.label_interparkbold_18pt_right);
		bool flag = GUI.enabled;
		GUI.enabled = flag && !_useFixedReceiver;
		GUI.SetNextControlName("Message Receiver");
		_msgReceiver = GUI.TextField(new Rect(num2, num4, num3, 24f), _msgReceiver, BlueStonez.textField);
		if (string.IsNullOrEmpty(_msgReceiver) && !GUI.GetNameOfFocusedControl().Equals("Message Receiver"))
		{
			GUI.color = new Color(1f, 1f, 1f, 0.3f);
			GUI.Label(new Rect(num2, num4, num3, 24f), " " + LocalizedStrings.StartTypingTheNameOfAFriend, BlueStonez.label_interparkbold_11pt_left);
			GUI.color = Color.white;
		}
		GUI.enabled = flag && !_showReceiverDropdownList;
		GUI.SetNextControlName("Message Content");
		_msgContent = GUI.TextArea(new Rect(num2, num5, num3, 108f), _msgContent, 140, BlueStonez.textArea);
		GUI.color = new Color(1f, 1f, 1f, 0.5f);
		GUI.Label(new Rect(num2, num5 + 110, num3, 24f), (140 - _msgContent.Length).ToString(), BlueStonez.label_interparkbold_11pt_right);
		GUI.color = Color.white;
		GUI.enabled = flag && !_showReceiverDropdownList && _msgRcvCmid != 0 && !string.IsNullOrEmpty(_msgContent);
		if (GUITools.Button(new Rect(position.width - 95f - 100f, position.height - 44f, 90f, 32f), new GUIContent(LocalizedStrings.SendCaps), BlueStonez.button_green))
		{
			Singleton<InboxManager>.Instance.SendPrivateMessage(_msgRcvCmid, _msgReceiver, _msgContent);
			_msgContent = string.Empty;
			_msgReceiver = string.Empty;
			_msgRcvCmid = 0;
			Hide();
		}
		GUI.enabled = flag;
		if (GUITools.Button(new Rect(position.width - 100f, position.height - 44f, 90f, 32f), new GUIContent(LocalizedStrings.DiscardCaps), BlueStonez.button))
		{
			Hide();
		}
		if (!_showReceiverDropdownList && GUI.GetNameOfFocusedControl().Equals("Message Receiver"))
		{
			_showReceiverDropdownList = true;
			_lastMsgRcvName = _msgReceiver;
			_msgReceiver = string.Empty;
		}
		GUI.EndGroup();
		if (_showReceiverDropdownList)
		{
			DoReceiverDropdownList(rect);
		}
	}

	private void DoReceiverDropdownList(Rect rect)
	{
		Rect position = new Rect(rect.x + 120f, rect.y + 94f, 320f, _rcvDropdownHeight);
		GUI.BeginGroup(position, BlueStonez.window);
		if (Singleton<PlayerDataManager>.Instance.FriendsCount > 0)
		{
			int num = 0;
			_friendDropdownScroll = GUITools.BeginScrollView(new Rect(0f, 0f, position.width, position.height), _friendDropdownScroll, new Rect(0f, 0f, _rcvDropdownWidth, _receiverCount * 24));
			foreach (PublicProfileView mergedFriend in Singleton<PlayerDataManager>.Instance.MergedFriends)
			{
				if (_msgReceiver.Length <= 0 || mergedFriend.Name.ToLower().Contains(_msgReceiver.ToLower()))
				{
					Rect position2 = new Rect(0f, num * 24, position.width, 24f);
					if (GUI.enabled && position2.Contains(Event.current.mousePosition) && GUI.Button(position2, GUIContent.none, BlueStonez.box_grey50))
					{
						_msgRcvCmid = mergedFriend.Cmid;
						_msgReceiver = mergedFriend.Name;
						_showReceiverDropdownList = false;
						GUI.FocusControl("Message Content");
					}
					GUI.Label(new Rect(8f, num * 24 + 4, position.width, position.height), mergedFriend.Name, BlueStonez.label_interparkmed_11pt_left);
					num++;
				}
			}
			_receiverCount = num;
			if ((float)(_receiverCount * 24) > position.height)
			{
				_rcvDropdownWidth = position.width - 22f;
			}
			else
			{
				_rcvDropdownWidth = position.width - 8f;
			}
			GUITools.EndScrollView();
		}
		else
		{
			GUI.Label(new Rect(0f, 0f, position.width, position.height), LocalizedStrings.YouHaveNoFriends, BlueStonez.label_interparkmed_11pt);
		}
		GUI.EndGroup();
		if (Event.current.type == EventType.MouseDown && !position.Contains(Event.current.mousePosition))
		{
			_showReceiverDropdownList = false;
			if (_msgRcvCmid == 0)
			{
				_msgReceiver = _lastMsgRcvName;
			}
		}
	}

	public override void Show()
	{
		base.Show();
		_msgRcvCmid = 0;
		_msgContent = string.Empty;
		_msgReceiver = string.Empty;
		_showComposeMessage = true;
		_showReceiverDropdownList = false;
		_useFixedReceiver = false;
		GUI.FocusControl("Message Receiver");
	}

	public override void Hide()
	{
		base.Hide();
		_showComposeMessage = false;
		_showReceiverDropdownList = false;
	}

	public void SelectReceiver(int cmid, string name)
	{
		_useFixedReceiver = true;
		_msgRcvCmid = cmid;
		_msgReceiver = name;
		GUI.FocusControl("Message Content");
	}
}
