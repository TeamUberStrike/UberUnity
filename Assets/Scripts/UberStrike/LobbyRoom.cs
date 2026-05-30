using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Cmune.DataCenter.Common.Entities;
using UberStrike.Core.Models;
using UberStrike.Realtime.Client;
using UnityEngine;

public class LobbyRoom : BaseLobbyRoom
{
	private Dictionary<int, CommActorInfo> _actors = new Dictionary<int, CommActorInfo>();

	private CommActorInfo LocalPlayer;

	public Property<bool> IsPlayerMuted = new Property<bool>(false);

	public IEnumerable<CommActorInfo> Players
	{
		get
		{
			return _actors.Values;
		}
	}

	public bool HasPlayer(int cmid)
	{
		return _actors.ContainsKey(cmid);
	}

	public bool TryGetPlayer(int cmid, out CommActorInfo player)
	{
		return _actors.TryGetValue(cmid, out player) && player != null;
	}

	protected override void OnClanChatMessage(int cmid, string name, string message)
	{
		InstantMessage msg = new InstantMessage(cmid, name, message, MemberAccessLevel.Default);
		Singleton<ChatManager>.Instance.AddClanMessage(cmid, msg);
	}

	protected override void OnFullPlayerListUpdate(List<CommActorInfo> players)
	{
		_actors.Clear();
		foreach (CommActorInfo player in players)
		{
			_actors[player.Cmid] = player;
		}
		Singleton<ChatManager>.Instance.RefreshAll(true);
	}

	protected override void OnInGameChatMessage(int cmid, string name, string message, MemberAccessLevel accessLevel, byte context)
	{
		if (ChatManager.CanShowMessage((ChatContext)context))
		{
			GameData.Instance.OnHUDChatMessage.Fire(name, message, accessLevel);
		}
		Singleton<ChatManager>.Instance.InGameDialog.AddMessage(new InstantMessage(cmid, name, message, accessLevel, (ChatContext)context));
	}

	protected override void OnLobbyChatMessage(int cmid, string name, string message)
	{
		MemberAccessLevel level = MemberAccessLevel.Default;
		CommActorInfo value;
		if (_actors.TryGetValue(cmid, out value))
		{
			level = value.AccessLevel;
			if (!string.IsNullOrEmpty(name))
			{
				name = PrependClanTagToPlayerName(value);
			}
		}
		InstantMessage msg = new InstantMessage(cmid, name, message, level, value);
		Singleton<ChatManager>.Instance.LobbyDialog.AddMessage(msg);
	}

	private bool DoModChatCmd(string message)
	{
		string text = message.Substring(1);
		string[] array = (from Match m in Regex.Matches(text, "\\w+|\"[^\\r\\n]*\"")
			select m.Value).ToArray();
		if (text == "h" || text == "help")
		{
			Singleton<ChatManager>.Instance.LobbyDialog.AddMessage(new InstantMessage(0, "[MOD]", " Usage: /cmd user [duration]\nValid commands are [short | long]:\n\tm | mute\n\tg | ghost\n\tu | unmute\n\tk | kick\n\th | help", MemberAccessLevel.Admin));
			Singleton<ChatManager>.Instance.LobbyDialog.AddMessage(new InstantMessage(0, "[MOD]", "Duration defaults to 12 hours if none is given, and applies to mute/ghost only. Use Copy Name menu item to get the name", MemberAccessLevel.Admin));
			return true;
		}
		if (array.Length < 2)
		{
			Singleton<ChatManager>.Instance.LobbyDialog.AddMessage(new InstantMessage(0, "[MOD]", "Error! No player specified!", MemberAccessLevel.Admin));
			return false;
		}
		string text2 = array[0];
		string text3 = array[1];
		string value = "720";
		if (array.Length > 2)
		{
			value = array[2];
		}
		string name;
		if (text3[0] == '"')
		{
			string[] array2 = text3.Substring(1).Split('"');
			name = array2[0];
			if (array2.Length > 2)
			{
				value = text.Split('"')[2].Trim();
			}
		}
		else
		{
			name = array[1];
		}
		int durationInMinutes = Convert.ToInt32(value);
		int cmid;
		string uname;
		if (!FindPlayerByName(name, out cmid, out uname))
		{
			Singleton<ChatManager>.Instance.LobbyDialog.AddMessage(new InstantMessage(0, "[MOD]", "Error! Player not found, or too many players matched that pattern!", MemberAccessLevel.Admin));
			return true;
		}
		switch (text2)
		{
		case "m":
		case "mute":
			AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.Operations.SendModerationMutePlayer(durationInMinutes, cmid, true);
			AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.Operations.SendModerationMutePlayer(durationInMinutes, cmid, false);
			Singleton<ChatManager>.Instance.LobbyDialog.AddMessage(new InstantMessage(0, "[MOD]", "User " + uname + " was muted for " + durationInMinutes + " minutes!", MemberAccessLevel.Admin));
			break;
		case "g":
		case "ghost":
			AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.Operations.SendModerationMutePlayer(durationInMinutes, cmid, false);
			Singleton<ChatManager>.Instance.LobbyDialog.AddMessage(new InstantMessage(0, "[MOD]", "User " + uname + " was ghosted for " + durationInMinutes + " minutes!", MemberAccessLevel.Admin));
			break;
		case "u":
		case "unmute":
			AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.Operations.SendModerationMutePlayer(0, cmid, false);
			Singleton<ChatManager>.Instance.LobbyDialog.AddMessage(new InstantMessage(0, "[MOD]", "User " + uname + " was unmuted!", MemberAccessLevel.Admin));
			break;
		case "k":
		case "kick":
			AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.Operations.SendModerationBanPlayer(cmid);
			Singleton<ChatManager>.Instance.LobbyDialog.AddMessage(new InstantMessage(0, "[MOD]", "User " + uname + " was kicked!", MemberAccessLevel.Admin));
			break;
		}
		return true;
	}

	private bool FindPlayerByName(string name, out int cmid, out string uname)
	{
		int num = 0;
		uname = string.Empty;
		cmid = 0;
		foreach (KeyValuePair<int, CommActorInfo> actor in _actors)
		{
			if (!string.IsNullOrEmpty(actor.Value.PlayerName) && actor.Value.PlayerName.Contains(name))
			{
				if (num > 0)
				{
					return false;
				}
				num = actor.Value.Cmid;
				uname = actor.Value.PlayerName;
			}
		}
		if (num == 0)
		{
			return false;
		}
		cmid = num;
		return true;
	}

	protected override void OnModerationCustomMessage(string message)
	{
		PopupSystem.ShowMessage("Administrator Message", message, PopupSystem.AlertType.OK, delegate
		{
		});
		EventHandler.Global.Fire(new GameEvents.PlayerPause());
	}

	protected override void OnModerationKickGame()
	{
		Singleton<GameStateController>.Instance.LeaveGame();
		PopupSystem.ShowMessage("ADMIN MESSAGE", "You were kicked out of the game!", PopupSystem.AlertType.OK, delegate
		{
		});
	}

	protected override void OnModerationMutePlayer(bool isPlayerMuted)
	{
		IsPlayerMuted.Value = isPlayerMuted;
		if (isPlayerMuted)
		{
			PopupSystem.ShowMessage("ADMIN MESSAGE", "You have been muted!", PopupSystem.AlertType.OK, delegate
			{
			});
		}
	}

	protected override void OnPlayerHide(int cmid)
	{
		if (!PlayerDataManager.IsClanMember(cmid) && !PlayerDataManager.IsFriend(cmid) && !Singleton<ChatManager>.Instance.HasDialogWith(cmid))
		{
			OnPlayerLeft(cmid, true);
		}
	}

	protected override void OnPlayerJoined(CommActorInfo data)
	{
		_actors.Clear();
		Debug.Log("OnPlayerJoined " + data.Cmid);
		_actors[data.Cmid] = data;
		Singleton<ChatManager>.Instance.RefreshAll(true);
	}

	protected override void OnPlayerLeft(int cmid, bool refreshComm)
	{
		CommActorInfo value;
		if (_actors.TryGetValue(cmid, out value))
		{
			_actors.Remove(cmid);
			value.CurrentRoom = null;
		}
		Singleton<ChatManager>.Instance.RefreshAll(refreshComm);
	}

	protected override void OnPlayerUpdate(CommActorInfo data)
	{
		_actors[data.Cmid] = data;
		Singleton<ChatManager>.Instance.RefreshAll();
	}

	protected override void OnPrivateChatMessage(int cmid, string name, string message)
	{
		MemberAccessLevel level = MemberAccessLevel.Default;
		CommActorInfo value;
		if (_actors.TryGetValue(cmid, out value))
		{
			level = value.AccessLevel;
			if (!string.IsNullOrEmpty(name))
			{
				name = PrependClanTagToPlayerName(value);
			}
		}
		InstantMessage msg = new InstantMessage(cmid, name, message, level, value);
		Singleton<ChatManager>.Instance.AddNewPrivateMessage(cmid, msg);
	}

	protected override void OnUpdateActorsForModeration(List<CommActorInfo> naughtyList)
	{
		Singleton<ChatManager>.Instance.SetNaughtyList(naughtyList);
		SendContactList();
	}

	protected override void OnUpdateClanData()
	{
		Singleton<ClanDataManager>.Instance.CheckCompleteClanData();
	}

	protected override void OnUpdateClanMembers()
	{
		Singleton<ClanDataManager>.Instance.RefreshClanData(true);
	}

	protected override void OnUpdateContacts(List<CommActorInfo> updated, List<int> removed)
	{
		foreach (CommActorInfo item in updated)
		{
			_actors[item.Cmid] = item;
		}
		foreach (int item2 in removed)
		{
			OnPlayerLeft(item2, false);
		}
		Singleton<ChatManager>.Instance.RefreshAll(true);
	}

	protected override void OnUpdateFriendsList()
	{
		UnityRuntime.StartRoutine(Singleton<CommsManager>.Instance.GetContactsByGroups());
	}

	protected override void OnUpdateInboxMessages(int messageId)
	{
		Singleton<InboxManager>.Instance.GetMessageWithId(messageId);
	}

	protected override void OnUpdateInboxRequests()
	{
		Singleton<InboxManager>.Instance.RefreshAllRequests();
	}

	public void SendContactList()
	{
		HashSet<int> hashSet = new HashSet<int>();
		foreach (CommUser friendUser in Singleton<ChatManager>.Instance.FriendUsers)
		{
			hashSet.Add(friendUser.Cmid);
		}
		foreach (CommUser clanUser in Singleton<ChatManager>.Instance.ClanUsers)
		{
			hashSet.Add(clanUser.Cmid);
		}
		foreach (CommUser otherUser in Singleton<ChatManager>.Instance.OtherUsers)
		{
			hashSet.Add(otherUser.Cmid);
		}
		foreach (CommUser naughtyUser in Singleton<ChatManager>.Instance.NaughtyUsers)
		{
			hashSet.Add(naughtyUser.Cmid);
		}
		if (hashSet.Count > 0)
		{
			base.Operations.SendSetContactList(hashSet.ToList());
		}
	}

	public void UpdatePlayerRoom(GameRoom room)
	{
		base.Operations.SendUpdatePlayerRoom(room);
	}

	public void SendUpdateClanMembers()
	{
		List<int> list = new List<int>();
		foreach (ClanMemberView clanMember in Singleton<PlayerDataManager>.Instance.ClanMembers)
		{
			if (clanMember.Cmid != PlayerDataManager.Cmid)
			{
				list.Add(clanMember.Cmid);
			}
		}
		list.RemoveAll((int id) => id == PlayerDataManager.Cmid);
		base.Operations.SendUpdateClanMembers(list);
	}

	public void UpdateContacts()
	{
		if (Singleton<ChatManager>.Instance.TotalContacts > 0)
		{
			base.Operations.SendUpdateContacts();
		}
	}

	public void SendUpdateResetLobby()
	{
		_actors.Clear();
		Singleton<ChatManager>.Instance.RefreshAll();
		base.Operations.SendFullPlayerListUpdate();
	}

	public void SendClanChatMessage(string message)
	{
		message = ChatMessageFilter.Cleanup(message);
		if (string.IsNullOrEmpty(message))
		{
			return;
		}
		List<int> list = new List<int>();
		foreach (CommUser clanUser in Singleton<ChatManager>.Instance.ClanUsers)
		{
			if (clanUser.Cmid != PlayerDataManager.Cmid)
			{
				list.Add(clanUser.Cmid);
			}
		}
		OnClanChatMessage(PlayerDataManager.Cmid, PlayerDataManager.Name, message);
		base.Operations.SendChatMessageToClan(list, message);
	}

	public bool SendLobbyChatMessage(string message)
	{
		message = ChatMessageFilter.Cleanup(message);
		if (!string.IsNullOrEmpty(message))
		{
			if (PlayerDataManager.AccessLevel >= MemberAccessLevel.Moderator && message[0] == '/' && DoModChatCmd(message))
			{
				GUI.FocusControl("@CurrentChatMessage");
				return true;
			}
			if (ChatMessageFilter.IsSpamming(message))
			{
				return false;
			}
			OnLobbyChatMessage(PlayerDataManager.Cmid, PlayerDataManager.Name, message);
			base.Operations.SendChatMessageToAll(message);
			return true;
		}
		return false;
	}

	public void SendPrivateChatMessage(int receiverCmid, string receiverName, string message)
	{
		message = ChatMessageFilter.Cleanup(message);
		if (!string.IsNullOrEmpty(message))
		{
			Singleton<ChatManager>.Instance.AddNewPrivateMessage(receiverCmid, new InstantMessage(PlayerDataManager.Cmid, PlayerDataManager.Name, message, PlayerDataManager.AccessLevel));
			base.Operations.SendChatMessageToPlayer(receiverCmid, message);
		}
	}

	private string PrependClanTagToPlayerName(CommActorInfo actor)
	{
		string text = null;
		if (!string.IsNullOrEmpty(actor.ClanTag))
		{
			return string.Concat("[" + actor.ClanTag + "] " + actor.PlayerName);
		}
		return actor.PlayerName;
	}
}
