using System.Collections;
using System.Collections.Generic;
using Cmune.DataCenter.Common.Entities;
using UberStrike.Realtime.UnitySdk;
using UberStrike.WebService.Unity;
using UnityEngine;

public class CommsManager : Singleton<CommsManager>
{
	public float NextFriendsRefresh { get; private set; }

	private CommsManager()
	{
	}

	public void SendFriendRequest(int cmid, string message)
	{
		message = TextUtilities.ShortenText(TextUtilities.Trim(message), 140, false);
		RelationshipWebServiceClient.SendContactRequest(PlayerDataManager.AuthToken, cmid, message, delegate
		{
			AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.Operations.SendUpdateInboxRequests(cmid);
		}, delegate
		{
		});
	}

	public IEnumerator GetContactsByGroups()
	{
		NextFriendsRefresh = Time.time + 30f;
		yield return RelationshipWebServiceClient.GetContactsByGroups(PlayerDataManager.AuthToken, false, delegate(List<ContactGroupView> ev)
		{
			List<PublicProfileView> list = new List<PublicProfileView>();
			foreach (ContactGroupView item in ev)
			{
				foreach (PublicProfileView contact in item.Contacts)
				{
					list.Add(contact);
				}
			}
			Singleton<PlayerDataManager>.Instance.FriendList = list;
			UpdateCommunicator();
		}, delegate
		{
		});
	}

	public void UpdateCommunicator()
	{
		AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.SendContactList();
		Singleton<ChatManager>.Instance.UpdateFriendSection();
	}
}
