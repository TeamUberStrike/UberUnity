using Cmune.DataCenter.Common.Entities;
using UberStrike.DataCenter.UnitySdk;
using UberStrike.Realtime.UnitySdk;
using UnityEngine;

public class DebugApplication : IDebugPage
{
	public string Title
	{
		get
		{
			return "App";
		}
	}

	public void Draw()
	{
		GUILayout.Label("Channel: " + ApplicationDataManager.Channel);
		GUILayout.Label("Version: 4.7.1");
		GUILayout.Label("Source: " + Application.version);
		GUILayout.Label("WS API: " + UberStrike.DataCenter.UnitySdk.ApiVersion.Current);
		GUILayout.Label("RT API: " + UberStrike.Realtime.UnitySdk.ApiVersion.Current);
		if (PlayerDataManager.AccessLevel > MemberAccessLevel.Default)
		{
			GUILayout.Label("Member Name: " + PlayerDataManager.Name);
			GUILayout.Label("Member Cmid: " + PlayerDataManager.Cmid);
			GUILayout.Label("Member Access: " + PlayerDataManager.AccessLevel);
			GUILayout.Label("Member Tag: " + PlayerDataManager.ClanTag);
			foreach (PhotonServer photonServer in Singleton<GameServerManager>.Instance.PhotonServerList)
			{
				GUILayout.Label("Game Server: " + photonServer.Name + " [" + photonServer.MinLatency + "] " + photonServer.Data.PeersConnected + "/" + photonServer.Data.PlayersConnected);
			}
		}
		GUILayout.Space(10f);
	}
}
