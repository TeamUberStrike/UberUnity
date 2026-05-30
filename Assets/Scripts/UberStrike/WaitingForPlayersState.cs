using UberStrike.Core.Types;
using UnityEngine;

internal class WaitingForPlayersState : IState
{
	public WaitingForPlayersState(StateMachine<GameStateId> stateMachine)
	{
	}

	public void OnEnter()
	{
		GamePageManager.Instance.UnloadCurrentPage();
		if (GameState.Current.Players.ContainsKey(PlayerDataManager.Cmid))
		{
			GameStateHelper.RespawnLocalPlayerAtRandom();
			GameState.Current.PlayerState.SetState(PlayerStateId.Playing);
		}
		else
		{
			GameState.Current.PlayerState.SetState(PlayerStateId.Spectating);
		}
		EventHandler.Global.Fire(new GameEvents.PlayerIngame());
		EventHandler.Global.AddListener<GameEvents.PlayerDied>(OnPlayerKilled);
		EventHandler.Global.AddListener<GameEvents.PlayerRespawn>(OnPlayerRespawn);
	}

	public void OnResume()
	{
	}

	public void OnExit()
	{
		GamePageManager.Instance.UnloadCurrentPage();
		EventHandler.Global.RemoveListener<GameEvents.PlayerDied>(OnPlayerKilled);
		EventHandler.Global.RemoveListener<GameEvents.PlayerRespawn>(OnPlayerRespawn);
	}

	public void OnUpdate()
	{
		string empty = string.Empty;
		PropertyExt.Fire(v2: (GameState.Current.GameMode != GameModeType.DeathMatch) ? "Get as many kills for your team as you can\nbefore the time runs out" : "Get as many kills as you can before the time runs out", property: GameData.Instance.OnNotificationFull, v1: LocalizedStrings.WaitingForOtherPlayers, v3: 0f);
	}

	private void OnPlayerRespawn(GameEvents.PlayerRespawn ev)
	{
		GameState.Current.RespawnLocalPlayerAt(ev.Position, Quaternion.Euler(0f, ev.Rotation, 0f));
		GameState.Current.PlayerState.SetState(PlayerStateId.Playing);
	}

	private void OnPlayerKilled(GameEvents.PlayerDied ev)
	{
		GameState.Current.PlayerState.SetState(PlayerStateId.Killed);
	}
}
