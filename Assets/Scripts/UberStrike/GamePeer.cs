using System;
using System.Collections;
using System.Collections.Generic;
using Cmune.Core.Models;
using ExitGames.Client.Photon;
using UberStrike.Core.Models;
using UberStrike.Core.Types;
using UberStrike.Realtime.Client;
using UnityEngine;

public class GamePeer : BaseGamePeer
{
	private Action onConnectAction;

	private int lastRoomJoined;

	private BaseGameRoom currentRoom;

	public ushort Ping
	{
		get
		{
			return (ushort)Mathf.Clamp(base.Peer.RoundTripTime / 2, 0, 65535);
		}
	}

	public bool IsConnectedToLobby { get; private set; }

	public bool IsInsideRoom
	{
		get
		{
			return base.IsConnected && lastRoomJoined != 0;
		}
	}

	public event Action<PhotonServerLoad> OnServerLoad;

	public GamePeer()
		: base(50, Application.isEditor)
	{
		base.Operations.SendGetGameListUpdates();
	}

	protected override void OnConnected()
	{
		Debug.Log("OnConnected");
		if (onConnectAction != null)
		{
			onConnectAction();
			onConnectAction = null;
		}
	}

	protected override void OnHeartbeatChallenge(string challengeHash)
	{
		string responseHash = Heartbeat.Instance.CheckHeartbeat(challengeHash);
		base.Operations.SendSendHeartbeatResponse(PlayerDataManager.AuthToken, responseHash);
	}

	protected override void OnDisconnected(StatusCode status)
	{
		Debug.LogWarning("#### OnDisconnected");
		OnRoomLeft();
		onConnectAction = null;
		if (base.IsEnabled && lastRoomJoined != 0)
		{
			PopupSystem.ClearAll();
			PopupSystem.ShowMessage("Server Disconnection", "You were disconnected from the game.\n Do you want to try to reconnect?", PopupSystem.AlertType.OKCancel, delegate
			{
				ReconnectToCurrentGame();
			}, delegate
			{
				lastRoomJoined = 0;
				Singleton<GameStateController>.Instance.LeaveGame();
			});
		}
	}

	protected override void OnError(string message)
	{
		Singleton<GameStateController>.Instance.UnloadGameMode();
		if (Singleton<SceneLoader>.Instance.CurrentScene != "Menu")
		{
			Singleton<SceneLoader>.Instance.LoadLevel("Menu");
		}
		PopupSystem.ClearAll();
		PopupSystem.ShowMessage("Server Disconnection", message, PopupSystem.AlertType.OK);
	}

	protected override void OnFullGameList(List<GameRoomData> gameList)
	{
		IsConnectedToLobby = true;
		Singleton<GameListManager>.Instance.SetGameList(gameList);
		if ((bool)PlayPageGUI.Instance)
		{
			PlayPageGUI.Instance.RefreshGameList();
		}
	}

	protected override void OnGameListUpdate(List<GameRoomData> updatedGames, List<int> removedGames)
	{
		IsConnectedToLobby = true;
		foreach (GameRoomData updatedGame in updatedGames)
		{
			Singleton<GameListManager>.Instance.AddGame(updatedGame);
		}
		foreach (int removedGame in removedGames)
		{
			Singleton<GameListManager>.Instance.RemoveGame(removedGame);
		}
		if ((bool)PlayPageGUI.Instance)
		{
			PlayPageGUI.Instance.RefreshGameList();
		}
	}

	protected override void OnGameListUpdateEnd()
	{
		IsConnectedToLobby = false;
		Singleton<GameListManager>.Instance.Clear();
		if ((bool)PlayPageGUI.Instance)
		{
			PlayPageGUI.Instance.RefreshGameList();
		}
	}

	protected override void OnRequestPasswordForRoom(string server, int roomId)
	{
		PopupSystem.ClearAll();
		PopupSystem.Show(new PasswordPopupDialog("Secured Area", "Please enter the password below:", delegate(string password)
		{
			JoinGame(server, roomId, password);
		}, delegate
		{
			Singleton<GameStateController>.Instance.LeaveGame();
		}));
	}

	protected override void OnRoomEnterFailed(string server, int roomId, string message)
	{
		PopupSystem.ClearAll();
		PopupSystem.ShowMessage("Failed to join game", message, PopupSystem.AlertType.OK, delegate
		{
			Singleton<GameStateController>.Instance.LeaveGame();
		});
	}

	protected override void OnRoomEntered(GameRoomData data)
	{
		Debug.Log("OnRoomJoined " + lastRoomJoined);
		GameState.Current.Reset();
		PopupSystem.ClearAll();
		lastRoomJoined = data.Number;
		GameState.Current.ResetRoundStartTime();
		base.Peer.FetchServerTimestamp();
		switch (data.GameMode)
		{
		case GameModeType.DeathMatch:
		{
			DeathMatchRoom gameMode3 = new DeathMatchRoom(data, this);
			Singleton<GameStateController>.Instance.SetGameMode(gameMode3);
			currentRoom = gameMode3;
			break;
		}
		case GameModeType.TeamDeathMatch:
		{
			TeamDeathMatchRoom gameMode2 = new TeamDeathMatchRoom(data, this);
			Singleton<GameStateController>.Instance.SetGameMode(gameMode2);
			currentRoom = gameMode2;
			break;
		}
		case GameModeType.EliminationMode:
		{
			TeamEliminationRoom gameMode = new TeamEliminationRoom(data, this);
			Singleton<GameStateController>.Instance.SetGameMode(gameMode);
			currentRoom = gameMode;
			break;
		}
		default:
			throw new NotImplementedException("GameMode not supported: " + data.GameMode);
		}
		AddRoomLogic(currentRoom, currentRoom.Operations);
		UberstrikeMap mapWithId = Singleton<MapManager>.Instance.GetMapWithId(data.MapID);
		if (mapWithId != null)
		{
			Singleton<MapManager>.Instance.LoadMap(mapWithId, delegate
			{
				GameStateHelper.EnterGameMode();
				GameState.Current.MatchState.SetState(GameStateId.PregameLoadout);
				foreach (GameActorInfo value in GameState.Current.Players.Values)
				{
					if (!value.IsSpectator)
					{
						GameState.Current.InstantiateAvatar(value);
					}
				}
				currentRoom.Operations.SendPowerUpRespawnTimes(PickupItem.GetRespawnDurations());
				List<Vector3> positions;
				List<byte> angles;
				Singleton<SpawnPointManager>.Instance.GetAllSpawnPoints(data.GameMode, TeamID.NONE, out positions, out angles);
				currentRoom.Operations.SendSpawnPositions(TeamID.NONE, positions, angles);
				Singleton<SpawnPointManager>.Instance.GetAllSpawnPoints(data.GameMode, TeamID.RED, out positions, out angles);
				currentRoom.Operations.SendSpawnPositions(TeamID.RED, positions, angles);
				Singleton<SpawnPointManager>.Instance.GetAllSpawnPoints(data.GameMode, TeamID.BLUE, out positions, out angles);
				currentRoom.Operations.SendSpawnPositions(TeamID.BLUE, positions, angles);
				AvatarBuilder.UpdateLocalAvatar(Singleton<LoadoutManager>.Instance.Loadout.GetAvatarGear());
				GameState.Current.RoomData = data;
				AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.Operations.SendUpdatePlayerRoom(new GameRoom
				{
					Server = new ConnectionAddress(data.Server.ConnectionString),
					Number = data.Number,
					MapId = data.MapID
				});
			});
		}
		else
		{
			Debug.LogError("Map not found");
		}
	}

	protected override void OnRoomLeft()
	{
		Debug.Log("OnRoomLeft " + (currentRoom != null));
		if (currentRoom != null)
		{
			RemoveRoomLogic(currentRoom, currentRoom.Operations);
			currentRoom = null;
			AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.Operations.SendResetPlayerRoom();
		}
	}

	protected override void OnServerLoadData(PhotonServerLoad data)
	{
		if (this.OnServerLoad != null)
		{
			this.OnServerLoad(data);
		}
	}

	protected override void OnGetGameInformation(GameRoomData room, List<GameActorInfo> players, int endTime)
	{
	}

	protected override void OnDisconnectAndDisablePhoton(string message)
	{
		AutoMonoBehaviour<CommConnectionManager>.Instance.DisableNetworkConnection(message);
	}

	public new void Disconnect()
	{
		Debug.Log("Disconnect");
		if (base.IsConnected)
		{
			lastRoomJoined = 0;
			base.Disconnect();
		}
	}

	internal void CloseGame(int gameId)
	{
		if (base.IsConnected)
		{
			base.Operations.SendCloseRoom(gameId, PlayerDataManager.AuthToken, PlayerDataManager.MagicHash);
		}
		else
		{
			Debug.Log("You are currently not connected to the game server");
		}
	}

	internal void InspectGame(int gameId)
	{
		Debug.Log("InspectGame operation is not implemented");
	}

	public void CreateGame(GameRoomData data, string password)
	{
		if (base.IsConnected)
		{
			base.Operations.SendCreateRoom(data, password, "4.7.1", PlayerDataManager.AuthToken, PlayerDataManager.MagicHash);
			return;
		}
		onConnectAction = delegate
		{
			base.Operations.SendCreateRoom(data, password, "4.7.1", PlayerDataManager.AuthToken, PlayerDataManager.MagicHash);
		};
		Connect(data.Server.ConnectionString);
	}

	public void JoinGame(string server, int roomId, string password = "")
	{
		Debug.Log("JoinGame " + server + ":" + roomId + "[current:" + base.Peer.ServerAddress + "]");
		if (base.IsConnected)
		{
			base.Operations.SendEnterRoom(roomId, password, "4.7.1", PlayerDataManager.AuthToken, PlayerDataManager.MagicHash);
			return;
		}
		onConnectAction = delegate
		{
			base.Operations.SendEnterRoom(roomId, password, "4.7.1", PlayerDataManager.AuthToken, PlayerDataManager.MagicHash);
		};
		Connect(server);
	}

	public void LeaveGame()
	{
		base.Operations.SendLeaveRoom();
		OnRoomLeft();
	}

	public void RefreshGameLobby()
	{
		if (base.IsConnected)
		{
			base.Operations.SendGetGameListUpdates();
		}
	}

	public void EnterGameLobby(string serverAddress)
	{
		IsConnectedToLobby = true;
		if (base.IsConnected)
		{
			base.Operations.SendGetGameListUpdates();
			return;
		}
		onConnectAction = delegate
		{
			base.Operations.SendGetGameListUpdates();
		};
		Connect(serverAddress);
	}

	private void ReconnectToCurrentGame()
	{
		if (lastRoomJoined != 0)
		{
			Singleton<GameStateController>.Instance.UnloadGameMode();
			JoinGame(base.Peer.ServerAddress, lastRoomJoined, string.Empty);
		}
		else
		{
			Singleton<GameStateController>.Instance.LeaveGame();
		}
	}

	private IEnumerator StartReconnectionInSeconds(string server, int roomId, int seconds)
	{
		yield return new WaitForSeconds(seconds);
		if (roomId != 0)
		{
			JoinGame(server, roomId, string.Empty);
		}
		else
		{
			Debug.LogError("Failed to reconnect because GameRoom is null!");
		}
	}
}
