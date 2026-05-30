using UberStrike.Core.Models;
using UberStrike.Core.Types;
using UberStrike.Realtime.UnitySdk;
using UnityEngine;

public class GameStateController : Singleton<GameStateController>
{
	private IGameMode currentGameMode;

	public GameMode CurrentGameMode
	{
		get
		{
			return (currentGameMode != null) ? currentGameMode.Type : GameMode.None;
		}
	}

	public GamePeer Client { get; private set; }

	private GameStateController()
	{
		Client = new GamePeer();
	}

	public void CreateNetworkGame(string server, int mapId, GameModeType mode, string name, string password, int timeMinutes, int killLimit, int playerLimit, int minLevel, int maxLevel, GameFlags.GAME_FLAGS flags)
	{
		GameRoomData gameRoomData = new GameRoomData();
		gameRoomData.Name = name;
		gameRoomData.Server = new ConnectionAddress(server);
		gameRoomData.MapID = mapId;
		gameRoomData.TimeLimit = timeMinutes;
		gameRoomData.PlayerLimit = playerLimit;
		gameRoomData.GameMode = mode;
		gameRoomData.GameFlags = (int)flags;
		gameRoomData.KillLimit = killLimit;
		gameRoomData.LevelMin = (byte)Mathf.Clamp(minLevel, 0, 255);
		gameRoomData.LevelMax = (byte)Mathf.Clamp(maxLevel, 0, 255);
		GameRoomData data = gameRoomData;
		float time = Time.time;
		ProgressPopupDialog dialog = PopupSystem.ShowProgress("Authentication", "Connecting to Server", () => Mathf.Clamp(Time.time - time, 0f, 3f));
		dialog.SetCancelable(delegate
		{
			PopupSystem.HideMessage(dialog);
		});
		Client.CreateGame(data, password);
	}

	public void JoinNetworkGame(GameRoomData data)
	{
		if (data.Server != null)
		{
			float time = Time.time;
			ProgressPopupDialog dialog = PopupSystem.ShowProgress("Authentication", "Connecting to Server", () => Mathf.Clamp(Time.time - time, 0f, 3f));
			dialog.SetCancelable(delegate
			{
				PopupSystem.HideMessage(dialog);
			});
			Singleton<ChatManager>.Instance.InGameDialog.Clear();
			Client.JoinGame(data.Server.ConnectionString, data.Number, string.Empty);
		}
		else
		{
			PopupSystem.ShowError("Game not found", "The game doesn't exist anymore.", PopupSystem.AlertType.OK);
		}
	}

	public void JoinNetworkGame(GameRoom data)
	{
		if (data.Server != null)
		{
			float time = Time.time;
			ProgressPopupDialog dialog = PopupSystem.ShowProgress("Authentication", "Connecting to Server", () => Mathf.Clamp(Time.time - time, 0f, 3f));
			dialog.SetCancelable(delegate
			{
				PopupSystem.HideMessage(dialog);
			});
			Singleton<ChatManager>.Instance.InGameDialog.Clear();
			Client.JoinGame(data.Server.ConnectionString, data.Number, string.Empty);
		}
		else
		{
			PopupSystem.ShowError("Game not found", "The game doesn't exist anymore.", PopupSystem.AlertType.OK);
		}
	}

	public void LeaveGame(bool warnBeforeLeaving = false)
	{
		if (warnBeforeLeaving && GameState.Current.IsMultiplayer && GameState.Current.IsMatchRunning)
		{
			PopupSystem.ShowMessage(LocalizedStrings.LeavingGame, LocalizedStrings.LeaveGameWarningMsg, PopupSystem.AlertType.OKCancel, BackToMenu, LocalizedStrings.LeaveCaps, null, LocalizedStrings.CancelCaps, PopupSystem.ActionType.Negative);
		}
		else
		{
			BackToMenu();
		}
	}

	public void ResetClient()
	{
		Client.Dispose();
		Client = new GamePeer();
	}

	private void BackToMenu()
	{
		GamePageManager.Instance.UnloadCurrentPage();
		UnloadGameMode();
		if (Singleton<SceneLoader>.Instance.CurrentScene != "Menu")
		{
			Singleton<SceneLoader>.Instance.LoadLevel("Menu");
		}
	}

	public void UnloadGameMode()
	{
		SetGameMode(null);
	}

	public void SetGameMode(IGameMode mode)
	{
		if (currentGameMode != null)
		{
			Client.LeaveGame();
			currentGameMode.Dispose();
		}
		currentGameMode = mode;
	}
}
