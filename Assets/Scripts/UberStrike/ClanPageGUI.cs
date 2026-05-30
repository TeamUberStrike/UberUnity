using System;
using System.Collections.Generic;
using Cmune.DataCenter.Common.Entities;
using UnityEngine;

public class ClanPageGUI : MonoBehaviour
{
	private class ClanSorter : IComparer<ClanMemberView>
	{
		public int Compare(ClanMemberView a, ClanMemberView b)
		{
			return CompareClanFunctionList.CompareClanMembers(a, b);
		}
	}

	private static class CompareClanFunctionList
	{
		public static int CompareClanMembers(ClanMemberView a, ClanMemberView b)
		{
			int num = ComparePosition(a, b);
			return (num == 0) ? CompareName(a, b) : num;
		}

		public static int ComparePosition(ClanMemberView a, ClanMemberView b)
		{
			int num = 0;
			int num2 = 0;
			if (a.Position == GroupPosition.Leader)
			{
				num = 1;
			}
			else if (a.Position == GroupPosition.Officer)
			{
				num = 2;
			}
			else if (a.Position == GroupPosition.Member)
			{
				num = 3;
			}
			if (b.Position == GroupPosition.Leader)
			{
				num2 = 1;
			}
			else if (b.Position == GroupPosition.Officer)
			{
				num2 = 2;
			}
			else if (b.Position == GroupPosition.Member)
			{
				num2 = 3;
			}
			return (num != num2) ? ((num > num2) ? 1 : (-1)) : 0;
		}

		public static int CompareName(ClanMemberView a, ClanMemberView b)
		{
			return string.Compare(a.Name, b.Name);
		}
	}

	private const int _indicatorWidth = 25;

	private const int _positionWidth = 70;

	private const int _joinDateWidth = 80;

	private const int _nameWidth = 200;

	[SerializeField]
	private Texture2D _level4Icon;

	[SerializeField]
	private Texture2D _licenseIcon;

	[SerializeField]
	private Texture2D _friendsIcon;

	private bool createAClan;

	private int _onlineMemberCount;

	private int _statusWidth;

	private Vector2 _clanMembersScrollView;

	private string _newClanName = string.Empty;

	private string _newClanTag = string.Empty;

	private string _newClanMotto = string.Empty;

	private void Awake()
	{
		EventHandler.Global.AddListener<GlobalEvents.ClanCreated>(OnClanCreated);
	}

	private void OnClanCreated(GlobalEvents.ClanCreated ev)
	{
		createAClan = false;
		_newClanMotto = string.Empty;
		_newClanName = string.Empty;
		_newClanTag = string.Empty;
	}

	private void OnGUI()
	{
		GUI.depth = 11;
		GUI.skin = BlueStonez.Skin;
		Rect rect = new Rect(0f, GlobalUIRibbon.Instance.Height(), Screen.width, Screen.height - GlobalUIRibbon.Instance.Height());
		GUI.BeginGroup(rect, BlueStonez.box_grey31);
		GUI.enabled = PlayerDataManager.IsPlayerLoggedIn && IsNoPopupOpen() && !Singleton<ClanDataManager>.Instance.IsProcessingWebservice;
		if (PlayerDataManager.IsPlayerInClan)
		{
			float num = 73f;
			float num2 = 40f;
			float num3 = rect.height - num - num2;
			DrawClanRosterHeader(new Rect(0f, 0f, rect.width, num));
			DrawMembersView(new Rect(0f, num, rect.width, num3));
			DrawClanRosterFooter(new Rect(0f, num + num3, rect.width, num2));
		}
		else
		{
			GUI.Box(rect, GUIContent.none, BlueStonez.box_grey38);
			if (createAClan)
			{
				DrawCreateClanMessage(rect);
			}
			else
			{
				DrawNoClanMessage(rect);
			}
		}
		GuiManager.DrawTooltip();
		GUI.enabled = true;
		GUI.EndGroup();
	}

	private void DrawClanRosterHeader(Rect rect)
	{
		int num = (int)rect.width;
		GUI.BeginGroup(rect, BlueStonez.box_grey31);
		GUI.Label(new Rect(10f, 5f, rect.width - 20f, 18f), string.Format("{0}: {1}", LocalizedStrings.YourClan, PlayerDataManager.ClanName), BlueStonez.label_interparkbold_16pt_left);
		float num2 = Mathf.Max(Singleton<ClanDataManager>.Instance.NextClanRefresh - Time.time, 0f);
		GUITools.PushGUIState();
		GUI.enabled &= num2 == 0f;
		if (GUITools.Button(new Rect(rect.width - 130f, 5f, 120f, 19f), new GUIContent(string.Format(LocalizedStrings.Refresh + " {0}", (!(num2 > 0f)) ? string.Empty : ("(" + num2.ToString("N0") + ")"))), BlueStonez.buttondark_medium))
		{
			Singleton<ClanDataManager>.Instance.RefreshClanData();
		}
		GUITools.PopGUIState();
		GUI.Label(new Rect(rect.width - 340f, 5f, 200f, 18f), string.Format(LocalizedStrings.NMembersNOnline, Singleton<PlayerDataManager>.Instance.ClanMembersCount, PlayerDataManager.ClanMembersLimit, _onlineMemberCount), BlueStonez.label_interparkmed_11pt_right);
		GUI.BeginGroup(new Rect(0f, 25f, rect.width, 50f), BlueStonez.box_grey50);
		GUI.Label(new Rect(10f, 7f, rect.width / 2f, 16f), string.Format("Tag: {0}", PlayerDataManager.ClanTag), BlueStonez.label_interparkmed_11pt_left);
		GUI.Label(new Rect(10f, 28f, rect.width / 2f, 16f), string.Format(LocalizedStrings.MottoN, PlayerDataManager.ClanMotto), BlueStonez.label_interparkmed_11pt_left);
		GUI.Label(new Rect(rect.width / 2f, 7f, rect.width / 2f, 16f), string.Format(LocalizedStrings.CreatedN, PlayerDataManager.ClanFoundingDate.ToShortDateString()), BlueStonez.label_interparkmed_11pt_left);
		GUI.Label(new Rect(rect.width / 2f, 28f, rect.width / 2f, 16f), string.Format(LocalizedStrings.LeaderN, PlayerDataManager.ClanOwnerName), BlueStonez.label_interparkmed_11pt_left);
		GUI.EndGroup();
		if (Singleton<PlayerDataManager>.Instance.RankInClan != GroupPosition.Member)
		{
			num = (int)(rect.width - 10f - 120f);
			if (GUITools.Button(new Rect(num, 40f, 120f, 20f), new GUIContent(LocalizedStrings.InvitePlayer), BlueStonez.buttondark_medium))
			{
				PanelManager.Instance.OpenPanel(PanelType.ClanRequest);
			}
		}
		GUI.EndGroup();
	}

	private void DrawMembersView(Rect rect)
	{
		GUI.BeginGroup(rect, BlueStonez.box_grey38);
		UpdateColumnWidth();
		int num = 0;
		GUI.Box(new Rect(num, 0f, 25f, 25f), string.Empty, BlueStonez.box_grey50);
		num = 24;
		GUI.Box(new Rect(num, 0f, 200f, 25f), string.Empty, BlueStonez.box_grey50);
		GUI.Label(new Rect(num + 5, 5f, 200f, 25f), LocalizedStrings.Player, BlueStonez.label_interparkmed_11pt_left);
		num = 223;
		GUI.Box(new Rect(num, 0f, 70f, 25f), string.Empty, BlueStonez.box_grey50);
		GUI.Label(new Rect(num + 5, 5f, 70f, 25f), LocalizedStrings.Position, BlueStonez.label_interparkmed_11pt_left);
		num = 292;
		GUI.Box(new Rect(num, 0f, 80f, 25f), string.Empty, BlueStonez.box_grey50);
		GUI.Label(new Rect(num + 5, 5f, 80f, 25f), LocalizedStrings.JoinDate, BlueStonez.label_interparkmed_11pt_left);
		num = 371;
		GUI.Box(new Rect(num, 0f, _statusWidth, 25f), string.Empty, BlueStonez.box_grey50);
		int num2 = 0;
		int num3 = Singleton<PlayerDataManager>.Instance.ClanMembersCount * 50;
		_clanMembersScrollView = GUITools.BeginScrollView(new Rect(0f, 25f, rect.width, rect.height - 25f), _clanMembersScrollView, new Rect(0f, 0f, rect.width - 20f, num3));
		_onlineMemberCount = 0;
		foreach (ClanMemberView clanMember in Singleton<PlayerDataManager>.Instance.ClanMembers)
		{
			DrawClanMembers(new Rect(0f, 50 * num2++ - 1, rect.width - 20f, 50f), clanMember);
		}
		GUITools.EndScrollView();
		GUI.EndGroup();
	}

	private void DrawClanMembers(Rect rect, ClanMemberView member)
	{
		GUIStyle style = ((!rect.Contains(Event.current.mousePosition)) ? BlueStonez.box_grey38 : BlueStonez.box_grey50);
		GUI.BeginGroup(rect, style);
		CommUser user;
		if (Singleton<ChatManager>.Instance.TryGetClanUsers(member.Cmid, out user))
		{
			GUI.DrawTexture(new Rect(5f, 12f, 14f, 20f), ChatManager.GetPresenceIcon(user.PresenceIndex));
		}
		int num = 28;
		GUI.Label(new Rect(num, 12f, 200f, 25f), member.Name, BlueStonez.label_interparkbold_13pt_left);
		num = 228;
		GUI.Label(new Rect(num, 20f, 70f, 25f), ConvertClanPosition(member.Position), BlueStonez.label_interparkmed_11pt_left);
		num = 298;
		GUI.Label(new Rect(num, 20f, 80f, 25f), member.JoiningDate.ToString("d"), BlueStonez.label_interparkmed_11pt_left);
		float num2 = rect.width - 20f;
		if (member.Cmid != PlayerDataManager.Cmid)
		{
			if (user != null && user.IsOnline)
			{
				_onlineMemberCount++;
				if (GUITools.Button(new Rect(num2 - 120f, 4f, 100f, 20f), new GUIContent(LocalizedStrings.PrivateChat), BlueStonez.buttondark_medium))
				{
					MenuPageManager.Instance.LoadPage(PageType.Chat);
					Singleton<ChatManager>.Instance.CreatePrivateChat(member.Cmid);
				}
			}
			else
			{
				int days = DateTime.Now.Subtract(member.Lastlogin).Days;
				string text = string.Format(LocalizedStrings.LastOnlineN, (days > 1) ? (days + " " + LocalizedStrings.DaysAgo) : ((days != 0) ? LocalizedStrings.Yesterday : LocalizedStrings.Today));
				GUI.Label(new Rect(num2 - 120f, 4f, 100f, 25f), text, BlueStonez.label_interparkmed_11pt_left);
			}
			if (GUITools.Button(new Rect(num2 - 120f, 28f, 100f, 20f), new GUIContent(LocalizedStrings.SendMessage), BlueStonez.buttondark_medium))
			{
				SendMessagePanelGUI sendMessagePanelGUI = PanelManager.Instance.OpenPanel(PanelType.SendMessage) as SendMessagePanelGUI;
				if ((bool)sendMessagePanelGUI)
				{
					sendMessagePanelGUI.SelectReceiver(member.Cmid, member.Name);
				}
			}
		}
		if (HasHigherPermissionThan(member.Position))
		{
			if (GUITools.Button(new Rect(num2 - 10f, 14f, 20f, 20f), new GUIContent("x"), BlueStonez.buttondark_medium))
			{
				int removeFromClanCmid = member.Cmid;
				string text2 = string.Format(LocalizedStrings.RemoveNFromClanN, member.Name, PlayerDataManager.ClanName) + "\n\n" + LocalizedStrings.RemoveMemberWarningMsg;
				PopupSystem.ShowMessage(LocalizedStrings.RemoveMember, text2, PopupSystem.AlertType.OKCancel, delegate
				{
					Singleton<ClanDataManager>.Instance.RemoveMemberFromClan(removeFromClanCmid);
				}, "OK", null, LocalizedStrings.Cancel, PopupSystem.ActionType.Negative);
			}
			num2 -= 160f;
		}
		num = 378;
		if (Singleton<PlayerDataManager>.Instance.RankInClan == GroupPosition.Leader && HasHigherPermissionThan(member.Position))
		{
			if (GUITools.Button(new Rect(num, 4f, 130f, 20f), new GUIContent(LocalizedStrings.TransferLeadership), BlueStonez.buttondark_medium))
			{
				int newLeader = member.Cmid;
				string text3 = string.Format(LocalizedStrings.TransferClanLeaderhsipToN, member.Name) + "\n\n" + LocalizedStrings.TransferClanWarningMsg;
				PopupSystem.ShowMessage(LocalizedStrings.TransferLeadership, text3, PopupSystem.AlertType.OKCancel, delegate
				{
					Singleton<ClanDataManager>.Instance.TransferOwnershipTo(newLeader);
				}, LocalizedStrings.TransferCaps, null, LocalizedStrings.Cancel, PopupSystem.ActionType.Negative);
			}
			num2 -= 160f;
		}
		if (Singleton<PlayerDataManager>.Instance.RankInClan == GroupPosition.Leader && HasHigherPermissionThan(member.Position))
		{
			if (member.Position == GroupPosition.Member && GUITools.Button(new Rect(num, 28f, 130f, 20f), new GUIContent(LocalizedStrings.PromoteMember), BlueStonez.buttondark_medium))
			{
				int memberCmid = member.Cmid;
				PopupSystem.ShowMessage(LocalizedStrings.PromoteMember, string.Format(LocalizedStrings.ThisWillChangeNPositionToN, member.Name, LocalizedStrings.Officer), PopupSystem.AlertType.OKCancel, delegate
				{
					Singleton<ClanDataManager>.Instance.UpdateMemberTo(memberCmid, GroupPosition.Officer);
				}, LocalizedStrings.PromoteCaps, null, LocalizedStrings.Cancel, PopupSystem.ActionType.Positive);
			}
			else if (member.Position == GroupPosition.Officer && GUITools.Button(new Rect(num, 28f, 130f, 20f), new GUIContent(LocalizedStrings.DemoteMember), BlueStonez.buttondark_medium))
			{
				int memberCmid2 = member.Cmid;
				PopupSystem.ShowMessage(LocalizedStrings.DemoteMember, string.Format(LocalizedStrings.ThisWillChangeNPositionToN, member.Name, LocalizedStrings.Member), PopupSystem.AlertType.OKCancel, delegate
				{
					Singleton<ClanDataManager>.Instance.UpdateMemberTo(memberCmid2, GroupPosition.Member);
				}, LocalizedStrings.DemoteCaps, null, LocalizedStrings.Cancel, PopupSystem.ActionType.Negative);
			}
			num2 -= 160f;
		}
		GUI.Label(new Rect(1f, rect.height - 2f, rect.width - 2f, 1f), string.Empty, BlueStonez.horizontal_line_grey95);
		GUI.EndGroup();
	}

	private void DrawClanRosterFooter(Rect rect)
	{
		GUI.BeginGroup(rect, BlueStonez.box_grey31);
		if (Singleton<PlayerDataManager>.Instance.RankInClan == GroupPosition.Leader)
		{
			if (GUITools.Button(new Rect(rect.width - 110f, 10f, 100f, 20f), new GUIContent(LocalizedStrings.DisbandClan), BlueStonez.buttondark_medium))
			{
				string text = string.Format(LocalizedStrings.DisbandClanN, PlayerDataManager.ClanName) + "\n\n" + LocalizedStrings.DisbandClanWarningMsg;
				PopupSystem.ShowMessage(LocalizedStrings.DisbandClan, text, PopupSystem.AlertType.OKCancel, delegate
				{
					Singleton<ClanDataManager>.Instance.DisbanClan();
				}, LocalizedStrings.DisbandCaps, null, LocalizedStrings.Cancel, PopupSystem.ActionType.Negative);
			}
		}
		else if (GUITools.Button(new Rect(rect.width - 110f, 10f, 100f, 20f), new GUIContent(LocalizedStrings.LeaveClan), BlueStonez.buttondark_medium))
		{
			string text2 = string.Format(LocalizedStrings.LeaveClanN, PlayerDataManager.ClanName) + "\n\n" + LocalizedStrings.LeaveClanWarningMsg;
			PopupSystem.ShowMessage(LocalizedStrings.LeaveClan, text2, PopupSystem.AlertType.OKCancel, delegate
			{
				Singleton<ClanDataManager>.Instance.LeaveClan();
			}, LocalizedStrings.LeaveCaps, null, LocalizedStrings.Cancel, PopupSystem.ActionType.Negative);
		}
		GUI.EndGroup();
	}

	private bool HasHigherPermissionThan(GroupPosition gp)
	{
		switch (Singleton<PlayerDataManager>.Instance.RankInClan)
		{
		case GroupPosition.Leader:
			return gp != GroupPosition.Leader;
		case GroupPosition.Officer:
			return gp == GroupPosition.Member;
		default:
			return false;
		}
	}

	private string ConvertClanPosition(GroupPosition gp)
	{
		string empty = string.Empty;
		switch (gp)
		{
		case GroupPosition.Officer:
			return LocalizedStrings.Officer;
		case GroupPosition.Member:
			return LocalizedStrings.Member;
		case GroupPosition.Leader:
			return LocalizedStrings.Leader;
		default:
			return LocalizedStrings.Unknown;
		}
	}

	private void UpdateColumnWidth()
	{
		int width = Screen.width;
		int num = width - 25 - 70 - 80;
		_statusWidth = num - 200 + 4;
	}

	private void DrawNoClanMessage(Rect rect)
	{
		Rect position = new Rect((rect.width - 480f) / 2f, (rect.height - 240f) / 2f, 480f, 240f);
		GUI.BeginGroup(position, BlueStonez.window_standard_grey38);
		GUI.Label(new Rect(0f, 0f, position.width, 56f), LocalizedStrings.ClansCaps, BlueStonez.tab_strip);
		GUI.Box(new Rect(position.width / 2f - 82f, 60f, 48f, 48f), new GUIContent(_level4Icon), BlueStonez.item_slot_large);
		if (Singleton<ClanDataManager>.Instance.HaveLevel)
		{
			GUI.Box(new Rect(position.width / 2f - 82f, 60f, 48f, 48f), new GUIContent(UberstrikeIcons.LevelMastered));
		}
		GUI.Box(new Rect(position.width / 2f - 24f, 60f, 48f, 48f), new GUIContent(_licenseIcon), BlueStonez.item_slot_large);
		if (Singleton<ClanDataManager>.Instance.HaveLicense)
		{
			GUI.Box(new Rect(position.width / 2f - 24f, 60f, 48f, 48f), new GUIContent(UberstrikeIcons.LevelMastered));
		}
		GUI.Box(new Rect(position.width / 2f + 34f, 60f, 48f, 48f), new GUIContent(_friendsIcon), BlueStonez.item_slot_large);
		if (Singleton<ClanDataManager>.Instance.HaveFriends)
		{
			GUI.Box(new Rect(position.width / 2f + 34f, 60f, 48f, 48f), new GUIContent(UberstrikeIcons.LevelMastered));
		}
		if (!Singleton<ClanDataManager>.Instance.HaveLevel || !Singleton<ClanDataManager>.Instance.HaveLicense || !Singleton<ClanDataManager>.Instance.HaveFriends)
		{
			bool flag = GUI.enabled;
			GUI.Label(new Rect(position.width / 2f - 90f, 110f, 210f, 14f), LocalizedStrings.ToCreateAClanYouStillNeedTo, BlueStonez.label_interparkbold_11pt_left);
			GUI.enabled = flag && !Singleton<ClanDataManager>.Instance.HaveLevel;
			GUI.Label(new Rect(position.width / 2f - 90f, 124f, 200f, 14f), LocalizedStrings.ReachLevelFour + ((!Singleton<ClanDataManager>.Instance.HaveLevel) ? string.Empty : string.Format(" ({0})", LocalizedStrings.Done)), BlueStonez.label_interparkbold_11pt_left);
			GUI.enabled = flag && !Singleton<ClanDataManager>.Instance.HaveFriends;
			GUI.Label(new Rect(position.width / 2f - 90f, 138f, 200f, 14f), LocalizedStrings.HaveAtLeastOneFriend + ((!Singleton<ClanDataManager>.Instance.HaveFriends) ? string.Empty : string.Format(" ({0})", LocalizedStrings.Done)), BlueStonez.label_interparkbold_11pt_left);
			GUI.enabled = flag && !Singleton<ClanDataManager>.Instance.HaveLicense;
			GUI.Label(new Rect(position.width / 2f - 90f, 152f, 240f, 14f), LocalizedStrings.BuyAClanLicense + ((!Singleton<ClanDataManager>.Instance.HaveLicense) ? string.Empty : string.Format(" ({0})", LocalizedStrings.Done)), BlueStonez.label_interparkbold_11pt_left);
			GUI.enabled = flag;
			if (!Singleton<ClanDataManager>.Instance.HaveLicense && GUITools.Button(new Rect((position.width - 200f) / 2f, 170f, 200f, 22f), new GUIContent(LocalizedStrings.BuyAClanLicense), BlueStonez.buttondark_medium))
			{
				IUnityItem itemInShop = Singleton<ItemManager>.Instance.GetItemInShop(1234);
				if (itemInShop != null && itemInShop.View != null)
				{
					BuyPanelGUI buyPanelGUI = PanelManager.Instance.OpenPanel(PanelType.BuyItem) as BuyPanelGUI;
					if ((bool)buyPanelGUI)
					{
						buyPanelGUI.SetItem(itemInShop, BuyingLocationType.Shop, BuyingRecommendationType.None);
					}
				}
			}
		}
		else
		{
			GUI.Label(new Rect(0f, 140f, position.width, 14f), LocalizedStrings.CreateAClanAndInviteYourFriends, BlueStonez.label_interparkbold_11pt);
		}
		GUITools.PushGUIState();
		GUI.enabled &= Singleton<ClanDataManager>.Instance.HaveLevel && Singleton<ClanDataManager>.Instance.HaveLicense && Singleton<ClanDataManager>.Instance.HaveFriends;
		if (GUITools.Button(new Rect((position.width - 200f) / 2f, 200f, 200f, 30f), new GUIContent(LocalizedStrings.CreateAClanCaps), BlueStonez.button_green))
		{
			createAClan = true;
		}
		GUITools.PopGUIState();
		GUI.EndGroup();
	}

	private void DrawCreateClanMessage(Rect rect)
	{
		Rect position = new Rect((rect.width - 480f) / 2f, (rect.height - 360f) / 2f, 480f, 360f);
		GUI.BeginGroup(position, BlueStonez.window_standard_grey38);
		int num = 35;
		int num2 = 120;
		int num3 = 320;
		int num4 = 130;
		int num5 = 190;
		int num6 = 250;
		GUI.Label(new Rect(0f, 0f, position.width, 56f), LocalizedStrings.CreateAClan, BlueStonez.tab_strip);
		GUI.Label(new Rect(0f, 60f, position.width, 20f), LocalizedStrings.HereYouCanCreateYourOwnClan, BlueStonez.label_interparkbold_18pt);
		GUI.Label(new Rect(0f, 80f, position.width, 40f), LocalizedStrings.YouCantChangeYourClanInfoOnceCreated, BlueStonez.label_interparkmed_11pt);
		GUI.Label(new Rect(num, num4, 100f, 20f), LocalizedStrings.Name, BlueStonez.label_interparkbold_18pt_left);
		GUI.Label(new Rect(num, num5, 100f, 20f), LocalizedStrings.Tag, BlueStonez.label_interparkbold_18pt_left);
		GUI.Label(new Rect(num, num6, 100f, 20f), LocalizedStrings.Motto, BlueStonez.label_interparkbold_18pt_left);
		_newClanName = GUI.TextField(new Rect(num2, num4, num3, 24f), _newClanName, BlueStonez.textField);
		_newClanTag = GUI.TextField(new Rect(num2, num5, num3, 24f), _newClanTag, BlueStonez.textField);
		_newClanMotto = GUI.TextField(new Rect(num2, num6, num3, 24f), _newClanMotto, BlueStonez.textField);
		GUI.Label(new Rect(num2, num4 + 25, 300f, 20f), LocalizedStrings.ThisIsTheOfficialNameOfYourClan, BlueStonez.label_interparkmed_10pt_left);
		GUI.Label(new Rect(num2, num5 + 25, 300f, 20f), LocalizedStrings.ThisTagGetsDisplayedNextToYourName, BlueStonez.label_interparkmed_10pt_left);
		GUI.Label(new Rect(num2, num6 + 25, 300f, 20f), LocalizedStrings.ThisIsYourOfficialClanMotto, BlueStonez.label_interparkmed_10pt_left);
		if (_newClanName.Length > 25)
		{
			_newClanName = _newClanName.Remove(25);
		}
		if (_newClanTag.Length > 5)
		{
			_newClanTag = _newClanTag.Remove(5);
		}
		if (_newClanMotto.Length > 25)
		{
			_newClanMotto = _newClanMotto.Remove(25);
		}
		GUITools.PushGUIState();
		GUI.enabled &= _newClanName.Length >= 3 && _newClanTag.Length >= 2 && _newClanMotto.Length >= 3;
		if (GUITools.Button(new Rect(position.width - 110f - 110f, 310f, 100f, 30f), new GUIContent(LocalizedStrings.CreateCaps), BlueStonez.button_green))
		{
			Singleton<ClanDataManager>.Instance.CreateNewClan(_newClanName, _newClanMotto, _newClanTag);
		}
		GUITools.PopGUIState();
		if (GUITools.Button(new Rect(position.width - 110f, 310f, 100f, 30f), new GUIContent(LocalizedStrings.CancelCaps), BlueStonez.button))
		{
			createAClan = false;
		}
		GUI.EndGroup();
	}

	private void SortClanMembers()
	{
		if (Singleton<PlayerDataManager>.Instance.ClanMembers != null)
		{
			Singleton<PlayerDataManager>.Instance.ClanMembers.Sort(new ClanSorter());
		}
	}

	private bool IsNoPopupOpen()
	{
		return !PanelManager.IsAnyPanelOpen && !PopupSystem.IsAnyPopupOpen;
	}
}
