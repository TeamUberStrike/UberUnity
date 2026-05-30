using System;
using System.Collections.Generic;
using Cmune.DataCenter.Common.Entities;
using UberStrike.Realtime.UnitySdk;
using UberStrike.WebService.Unity;
using UnityEngine;

public class InboxManager : Singleton<InboxManager>
{
	public Property<int> UnreadMessageCount = new Property<int>(0);

	public Property<List<ContactRequestView>> FriendRequests = new Property<List<ContactRequestView>>(new List<ContactRequestView>());

	public Property<List<GroupInvitationView>> IncomingClanRequests = new Property<List<GroupInvitationView>>(new List<GroupInvitationView>());

	private Dictionary<int, InboxThread> _allThreads = new Dictionary<int, InboxThread>();

	private List<InboxThread> _sortedAllThreads = new List<InboxThread>();

	private int _curThreadsPageIndex;

	public List<GroupInvitationView> _outgoingClanRequests = new List<GroupInvitationView>();

	public bool IsInitialized { get; private set; }

	public IList<InboxThread> AllThreads
	{
		get
		{
			return _sortedAllThreads;
		}
	}

	public int ThreadCount
	{
		get
		{
			return _sortedAllThreads.Count;
		}
	}

	public bool IsLoadingThreads { get; private set; }

	public bool IsNoMoreThreads { get; private set; }

	public float NextInboxRefresh { get; private set; }

	public float NextRequestRefresh { get; private set; }

	private InboxManager()
	{
	}

	public void Initialize()
	{
		if (!IsInitialized)
		{
			IsInitialized = true;
			LoadNextPageThreads();
			RefreshAllRequests();
		}
	}

	public void SendPrivateMessage(int cmidId, string name, string rawMessage)
	{
		string text = TextUtilities.ShortenText(TextUtilities.Trim(rawMessage), 140, false);
		if (!string.IsNullOrEmpty(text))
		{
			if (!_allThreads.ContainsKey(cmidId))
			{
				MessageThreadView messageThreadView = new MessageThreadView();
				messageThreadView.HasNewMessages = false;
				messageThreadView.ThreadName = name;
				messageThreadView.LastMessagePreview = string.Empty;
				messageThreadView.ThreadId = cmidId;
				messageThreadView.LastUpdate = DateTime.Now;
				messageThreadView.MessageCount = 0;
				InboxThread inboxThread = new InboxThread(messageThreadView);
				_allThreads.Add(inboxThread.ThreadId, inboxThread);
				_sortedAllThreads.Add(inboxThread);
			}
			PrivateMessageWebServiceClient.SendMessage(PlayerDataManager.AuthToken, cmidId, text, delegate(PrivateMessageView pm)
			{
				OnPrivateMessageSent(cmidId, pm);
			}, delegate
			{
			});
		}
	}

	public void UpdateNewMessageCount()
	{
		_sortedAllThreads.Sort((InboxThread t1, InboxThread t2) => t2.LastMessageDateTime.CompareTo(t1.LastMessageDateTime));
		UnreadMessageCount.Value = _sortedAllThreads.Reduce((InboxThread el, int acc) => (!el.HasUnreadMessage) ? acc : (acc + 1), 0);
	}

	public void RemoveFriend(int friendCmid)
	{
		Singleton<PlayerDataManager>.Instance.RemoveFriend(friendCmid);
		RelationshipWebServiceClient.DeleteContact(PlayerDataManager.AuthToken, friendCmid, delegate(MemberOperationResult ev)
		{
			if (ev == MemberOperationResult.Ok)
			{
				AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.Operations.SendUpdateFriendsList(friendCmid);
				Singleton<CommsManager>.Instance.UpdateCommunicator();
			}
			else
			{
				Debug.LogError("DeleteContact failed with: " + ev);
			}
		}, delegate
		{
		});
	}

	public void AcceptContactRequest(int requestId)
	{
		FriendRequests.Value.RemoveAll((ContactRequestView r) => r.RequestId == requestId);
		FriendRequests.Fire();
		RelationshipWebServiceClient.AcceptContactRequest(PlayerDataManager.AuthToken, requestId, delegate(PublicProfileView view)
		{
			if (view != null)
			{
				Singleton<PlayerDataManager>.Instance.AddFriend(view);
				AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.Operations.SendUpdateFriendsList(view.Cmid);
				Singleton<CommsManager>.Instance.UpdateCommunicator();
			}
			else
			{
				PopupSystem.ShowMessage(LocalizedStrings.Clan, "Failed accepting friend request", PopupSystem.AlertType.OK);
			}
		}, delegate
		{
		});
	}

	public void DeclineContactRequest(int requestId)
	{
		FriendRequests.Value.RemoveAll((ContactRequestView r) => r.RequestId == requestId);
		FriendRequests.Fire();
		RelationshipWebServiceClient.DeclineContactRequest(PlayerDataManager.AuthToken, requestId, delegate
		{
		}, delegate
		{
		});
	}

	public void AcceptClanRequest(int clanInvitationId)
	{
		IncomingClanRequests.Value.RemoveAll((GroupInvitationView r) => r.GroupInvitationId == clanInvitationId);
		IncomingClanRequests.Fire();
		ClanWebServiceClient.AcceptClanInvitation(clanInvitationId, PlayerDataManager.AuthToken, delegate(ClanRequestAcceptView ev)
		{
			if (ev != null && ev.ActionResult == 0)
			{
				PopupSystem.ShowMessage(LocalizedStrings.Clan, LocalizedStrings.JoinClanSuccessMsg, PopupSystem.AlertType.OKCancel, delegate
				{
					MenuPageManager.Instance.LoadPage(PageType.Clans);
				}, "Go to Clans", null, "Not now", PopupSystem.ActionType.Positive);
				Singleton<ClanDataManager>.Instance.SetClanData(ev.ClanView);
				AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.SendUpdateClanMembers();
			}
			else
			{
				PopupSystem.ShowMessage(LocalizedStrings.Clan, LocalizedStrings.JoinClanErrorMsg, PopupSystem.AlertType.OK);
			}
		}, delegate
		{
		});
	}

	public void DeclineClanRequest(int requestId)
	{
		IncomingClanRequests.Value.RemoveAll((GroupInvitationView r) => r.GroupInvitationId == requestId);
		IncomingClanRequests.Fire();
		ClanWebServiceClient.DeclineClanInvitation(requestId, PlayerDataManager.AuthToken, delegate
		{
		}, delegate
		{
		});
	}

	internal void LoadNextPageThreads()
	{
		if (!IsNoMoreThreads || NextInboxRefresh - Time.time < 0f)
		{
			IsLoadingThreads = true;
			NextInboxRefresh = Time.time + 30f;
			PrivateMessageWebServiceClient.GetAllMessageThreadsForUser(PlayerDataManager.AuthToken, _curThreadsPageIndex, OnFinishLoadingNextPageThreads, delegate
			{
			});
		}
	}

	private void OnFinishLoadingNextPageThreads(List<MessageThreadView> listView)
	{
		IsLoadingThreads = false;
		if (listView.Count > 0)
		{
			_curThreadsPageIndex++;
			OnGetThreads(listView);
			IsNoMoreThreads = false;
		}
		else
		{
			IsNoMoreThreads = true;
		}
	}

	internal void LoadMessagesForThread(InboxThread inboxThread, int pageIndex)
	{
		inboxThread.IsLoading = true;
		PrivateMessageWebServiceClient.GetThreadMessages(PlayerDataManager.AuthToken, inboxThread.ThreadId, pageIndex, delegate(List<PrivateMessageView> list)
		{
			inboxThread.IsLoading = false;
			OnGetMessages(inboxThread.ThreadId, list);
		}, delegate
		{
		});
	}

	private void OnGetThreads(List<MessageThreadView> threadView)
	{
		foreach (MessageThreadView item in threadView)
		{
			InboxThread value;
			if (_allThreads.TryGetValue(item.ThreadId, out value))
			{
				value.UpdateThread(item);
				continue;
			}
			value = new InboxThread(item);
			_allThreads.Add(value.ThreadId, value);
			_sortedAllThreads.Add(value);
		}
		UpdateNewMessageCount();
	}

	private void OnGetMessages(int threadId, List<PrivateMessageView> messages)
	{
		InboxThread value;
		if (_allThreads.TryGetValue(threadId, out value))
		{
			value.AddMessages(messages);
		}
		else
		{
			Debug.LogError("Getting messages of non existing thread " + threadId);
		}
	}

	private void OnPrivateMessageSent(int threadId, PrivateMessageView privateMessage)
	{
		if (privateMessage != null)
		{
			AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.Operations.SendUpdateInboxMessages(privateMessage.ToCmid, privateMessage.PrivateMessageId);
			privateMessage.IsRead = true;
			AddMessageToThread(threadId, privateMessage);
		}
		else
		{
			Debug.LogError("PrivateMessage sending failed");
			PopupSystem.ShowMessage(LocalizedStrings.Error, LocalizedStrings.YourMessageHasNotBeenSent);
		}
	}

	private void AddMessage(PrivateMessageView privateMessage)
	{
		if (privateMessage != null)
		{
			AddMessageToThread(privateMessage.FromCmid, privateMessage);
		}
		else
		{
			Debug.LogError("AddMessage called with NULL message");
		}
	}

	private void AddMessageToThread(int threadId, PrivateMessageView privateMessage)
	{
		InboxThread value;
		if (!_allThreads.TryGetValue(threadId, out value))
		{
			MessageThreadView messageThreadView = new MessageThreadView();
			messageThreadView.ThreadName = privateMessage.FromName;
			messageThreadView.ThreadId = threadId;
			value = new InboxThread(messageThreadView);
			_allThreads.Add(value.ThreadId, value);
			_sortedAllThreads.Add(value);
		}
		value.AddMessage(privateMessage);
		UpdateNewMessageCount();
	}

	internal void MarkThreadAsRead(int threadId)
	{
		PrivateMessageWebServiceClient.MarkThreadAsRead(PlayerDataManager.AuthToken, threadId, delegate
		{
		}, delegate
		{
		});
		UpdateNewMessageCount();
	}

	internal void DeleteThread(int threadId)
	{
		PrivateMessageWebServiceClient.DeleteThread(PlayerDataManager.AuthToken, threadId, delegate
		{
			OnDeleteThread(threadId);
		}, delegate
		{
		});
	}

	private void OnDeleteThread(int threadId)
	{
		_allThreads.Remove(threadId);
		_sortedAllThreads.RemoveAll((InboxThread t) => t.ThreadId == threadId);
		UpdateNewMessageCount();
	}

	internal void GetMessageWithId(int messageId)
	{
		PrivateMessageWebServiceClient.GetMessageWithIdForCmid(PlayerDataManager.AuthToken, messageId, AddMessage, delegate
		{
		});
	}

	internal void RefreshAllRequests()
	{
		NextRequestRefresh = Time.time + 30f;
		RelationshipWebServiceClient.GetContactRequests(PlayerDataManager.AuthToken, OnGetContactRequests, delegate
		{
		});
		ClanWebServiceClient.GetAllGroupInvitations(PlayerDataManager.AuthToken, OnGetAllGroupInvitations, delegate
		{
		});
		if (Singleton<PlayerDataManager>.Instance.RankInClan != GroupPosition.Member)
		{
			ClanWebServiceClient.GetPendingGroupInvitations(PlayerDataManager.ClanID, PlayerDataManager.AuthToken, OnGetPendingGroupInvitations, delegate
			{
			});
		}
	}

	private void OnGetContactRequests(List<ContactRequestView> requests)
	{
		FriendRequests.Value = requests;
		FriendRequests.Fire();
		if (FriendRequests.Value.Count > 0)
		{
			AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.NewRequest, 0uL, 1f, 1f);
		}
	}

	private void OnGetAllGroupInvitations(List<GroupInvitationView> requests)
	{
		IncomingClanRequests.Value = requests;
		IncomingClanRequests.Fire();
		if (IncomingClanRequests.Value.Count > 0)
		{
			AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.NewRequest, 0uL, 1f, 1f);
		}
	}

	private void OnGetPendingGroupInvitations(List<GroupInvitationView> requests)
	{
		_outgoingClanRequests = requests;
	}
}
