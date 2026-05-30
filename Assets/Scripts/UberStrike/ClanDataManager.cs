using Cmune.DataCenter.Common.Entities;
using UberStrike.WebService.Unity;
using UnityEngine;

public class ClanDataManager : Singleton<ClanDataManager>
{
	public bool IsGetMyClanDone { get; set; }

	public bool HaveFriends
	{
		get
		{
			return Singleton<PlayerDataManager>.Instance.FriendsCount >= 1;
		}
	}

	public bool HaveLevel
	{
		get
		{
			return PlayerDataManager.PlayerLevel >= 4;
		}
	}

	public bool HaveLicense
	{
		get
		{
			return Singleton<InventoryManager>.Instance.HasClanLicense();
		}
	}

	public float NextClanRefresh { get; private set; }

	public bool IsProcessingWebservice { get; private set; }

	private ClanDataManager()
	{
	}

	private void HandleWebServiceError()
	{
		Debug.LogError("Error getting Clan data for local player.");
	}

	public void CheckCompleteClanData()
	{
		ClanWebServiceClient.GetMyClanId(PlayerDataManager.AuthToken, delegate(int ev)
		{
			PlayerDataManager.ClanID = ev;
			RefreshClanData(true);
		}, delegate
		{
		});
	}

	public void RefreshClanData(bool force = false)
	{
		if (PlayerDataManager.IsPlayerInClan && (force || NextClanRefresh < Time.time))
		{
			NextClanRefresh = Time.time + 30f;
			ClanWebServiceClient.GetOwnClan(PlayerDataManager.AuthToken, PlayerDataManager.ClanID, delegate(ClanView ev)
			{
				SetClanData(ev);
			}, delegate
			{
			});
		}
	}

	public void SetClanData(ClanView view)
	{
		PlayerDataManager.ClanData = view;
		AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.SendContactList();
		Singleton<ChatManager>.Instance.UpdateClanSection();
	}

	public void LeaveClan()
	{
		IsProcessingWebservice = true;
		ClanWebServiceClient.LeaveAClan(PlayerDataManager.ClanID, PlayerDataManager.AuthToken, delegate(int ev)
		{
			IsProcessingWebservice = false;
			if (ev == 0)
			{
				AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.SendUpdateClanMembers();
				SetClanData(null);
			}
			else
			{
				PopupSystem.ShowMessage("Leave Clan", "There was an error removing you from this clan.\nErrorCode = " + ev, PopupSystem.AlertType.OK);
			}
		}, delegate
		{
			IsProcessingWebservice = false;
		});
	}

	public void DisbanClan()
	{
		IsProcessingWebservice = true;
		ClanWebServiceClient.DisbandGroup(PlayerDataManager.ClanID, PlayerDataManager.AuthToken, delegate(int ev)
		{
			IsProcessingWebservice = false;
			if (ev == 0)
			{
				AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.SendUpdateClanMembers();
				SetClanData(null);
			}
		}, delegate
		{
			IsProcessingWebservice = false;
		});
	}

	public void CreateNewClan(string name, string motto, string tag)
	{
		IsProcessingWebservice = true;
		GroupCreationView groupCreationView = new GroupCreationView();
		groupCreationView.Name = name;
		groupCreationView.Motto = motto;
		groupCreationView.ApplicationId = 1;
		groupCreationView.AuthToken = PlayerDataManager.AuthToken;
		groupCreationView.Tag = tag;
		groupCreationView.Locale = ApplicationDataManager.CurrentLocale.ToString();
		GroupCreationView createClanData = groupCreationView;
		ClanWebServiceClient.CreateClan(createClanData, delegate(ClanCreationReturnView ev)
		{
			IsProcessingWebservice = false;
			if (ev.ResultCode == 0)
			{
				EventHandler.Global.Fire(new GlobalEvents.ClanCreated());
				SetClanData(ev.ClanView);
			}
			else
			{
				switch (ev.ResultCode)
				{
				case 100:
				case 101:
				case 102:
					PopupSystem.ShowMessage("Sorry", "You don't fulfill the minimal requirements to create your own clan.");
					break;
				case 2:
					PopupSystem.ShowMessage("Clan Collision", "You are already member of another clan, please leave first before creating your own.");
					break;
				case 1:
					PopupSystem.ShowMessage("Invalid Clan Name", "The name '" + name + "' is not valid, please modify it.");
					break;
				case 4:
					PopupSystem.ShowMessage("Invalid Clan Tag", "The tag '" + tag + "' is not valid, please modify it.");
					break;
				case 8:
					PopupSystem.ShowMessage("Invalid Clan Motto", "The motto '" + motto + "' is not valid, please modify it.");
					break;
				case 3:
					PopupSystem.ShowMessage("Clan Name", "The name '" + name + "' is already taken, try another one.");
					break;
				case 10:
					PopupSystem.ShowMessage("Clan Tag", "The tag '" + tag + "' is already taken, try another one.");
					break;
				default:
					PopupSystem.ShowMessage("Sorry", "There was an error (code " + ev.ResultCode + "), please visit support.uberstrike.com for help.");
					break;
				}
			}
		}, delegate
		{
			IsProcessingWebservice = false;
			SetClanData(null);
		});
	}

	public void UpdateMemberTo(int cmid, GroupPosition position)
	{
		IsProcessingWebservice = true;
		ClanWebServiceClient.UpdateMemberPosition(new MemberPositionUpdateView(PlayerDataManager.ClanID, PlayerDataManager.AuthToken, cmid, position), delegate(int ev)
		{
			IsProcessingWebservice = false;
			ClanMemberView view;
			if (ev == 0 && PlayerDataManager.TryGetClanMember(cmid, out view))
			{
				view.Position = position;
			}
		}, delegate
		{
			IsProcessingWebservice = false;
		});
	}

	public void TransferOwnershipTo(int cmid)
	{
		IsProcessingWebservice = true;
		ClanWebServiceClient.TransferOwnership(PlayerDataManager.ClanID, PlayerDataManager.AuthToken, cmid, delegate(int ev)
		{
			IsProcessingWebservice = false;
			switch (ev)
			{
			case 0:
			{
				ClanMemberView view;
				if (PlayerDataManager.TryGetClanMember(cmid, out view))
				{
					view.Position = GroupPosition.Leader;
				}
				if (PlayerDataManager.TryGetClanMember(PlayerDataManager.Cmid, out view))
				{
					view.Position = GroupPosition.Member;
				}
				Singleton<PlayerDataManager>.Instance.RankInClan = GroupPosition.Member;
				break;
			}
			case 100:
				PopupSystem.ShowMessage("Sorry", "The player you selected can't be a clan leader, because he is not level 4 yet!");
				break;
			case 101:
				PopupSystem.ShowMessage("Sorry", "The player you selected can't be a clan leader, because has no friends!");
				break;
			case 102:
				PopupSystem.ShowMessage("Sorry", "The player you selected can't be a clan leader, because he doesn't own a clan license.");
				break;
			default:
				PopupSystem.ShowMessage("Sorry", "There was an error (code " + ev + "), please visit support.uberstrike.com for help.");
				break;
			}
		}, delegate
		{
			IsProcessingWebservice = false;
		});
	}

	public void RemoveMemberFromClan(int cmid)
	{
		IsProcessingWebservice = true;
		ClanWebServiceClient.KickMemberFromClan(PlayerDataManager.ClanID, PlayerDataManager.AuthToken, cmid, delegate(int ev)
		{
			IsProcessingWebservice = false;
			if (ev == 0)
			{
				Singleton<PlayerDataManager>.Instance.ClanMembers.RemoveAll((ClanMemberView m) => m.Cmid == cmid);
				AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.SendUpdateClanMembers();
				AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.Operations.SendUpdateClanData(cmid);
				Singleton<ChatManager>.Instance.UpdateClanSection();
			}
		}, delegate
		{
			IsProcessingWebservice = false;
		});
	}
}
