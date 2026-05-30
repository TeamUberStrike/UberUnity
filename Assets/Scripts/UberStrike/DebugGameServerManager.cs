using UnityEngine;

public class DebugGameServerManager : IDebugPage
{
	public string Title
	{
		get
		{
			return "Requests";
		}
	}

	public void Draw()
	{
		foreach (ServerLoadRequest serverRequest in Singleton<GameServerManager>.Instance.ServerRequests)
		{
			GUILayout.Label(serverRequest.Server.Name + " " + serverRequest.Server.ConnectionString + ", Latency: " + serverRequest.Server.Latency + " - " + serverRequest.Server.IsValid);
			GUILayout.Label(string.Concat("States: ", serverRequest.RequestState, " ", serverRequest.Server.Data.State, ", PeerState: ", serverRequest.Peer.PeerState));
			GUILayout.Space(10f);
		}
	}
}
