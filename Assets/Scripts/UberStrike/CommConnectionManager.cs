using System.Collections;
using UnityEngine;

public class CommConnectionManager : AutoMonoBehaviour<CommConnectionManager>
{
	private float _pollFriendsOnlineStatus;

	public CommPeer Client { get; private set; }

	private void Awake()
	{
		Client = new CommPeer();
		StartCoroutine(StartCheckingCommServerConnection());
		EventHandler.Global.AddListener<GlobalEvents.Login>(OnLoginEvent);
	}

	private void OnLoginEvent(GlobalEvents.Login ev)
	{
	}

	private void Update()
	{
		if (_pollFriendsOnlineStatus < Time.time)
		{
			_pollFriendsOnlineStatus = Time.time + 30f;
			if (MenuPageManager.Instance != null && (MenuPageManager.Instance.IsCurrentPage(PageType.Chat) || MenuPageManager.Instance.IsCurrentPage(PageType.Clans)))
			{
				Client.Lobby.UpdateContacts();
			}
		}
	}

	public void Reconnect()
	{
		Stop();
		Awake();
	}

	private IEnumerator StartCheckingCommServerConnection()
	{
		while (true)
		{
			yield return new WaitForSeconds(5f);
			if (Client.IsEnabled && !Client.IsConnected && Singleton<GameServerManager>.Instance.CommServer.IsValid && PlayerDataManager.IsPlayerLoggedIn)
			{
				Client.Connect(Singleton<GameServerManager>.Instance.CommServer.ConnectionString);
			}
		}
	}

	public void Stop()
	{
		Client.Disconnect();
	}

	internal void DisableNetworkConnection(string message)
	{
		Debug.LogError("DisableNetworkConnection");
		if (GameState.Current.HasJoinedGame)
		{
			EventHandler.Global.Fire(new GameEvents.PlayerPause());
		}
		AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Dispose();
		Singleton<GameStateController>.Instance.Client.Dispose();
		ApplicationDataManager.LockApplication(message);
	}
}
