using UnityEngine;

internal class EndOfMatchState : IState
{
	public EndOfMatchState(StateMachine<GameStateId> stateMachine)
	{
	}

	public void OnEnter()
	{
		AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.EndOfRound, 0uL, 1f, 1f);
		Singleton<QuickItemController>.Instance.Restriction.RenewGameUses();
		EventHandler.Global.AddListener<GameEvents.MatchCountdown>(OnMatchCountdown);
		EventHandler.Global.AddListener<GameEvents.PlayerRespawn>(OnPlayerRespawn);
		GamePageManager.Instance.LoadPage(IngamePageType.EndOfMatch);
		EventHandler.Global.Fire(new GameEvents.MatchEnd());
		SpawnLocalAvatar();
	}

	private void SpawnLocalAvatar()
	{
		if ((bool)GameState.Current.Avatar.Decorator)
		{
			GameState.Current.Player.SpawnPlayerAt(GameState.Current.Map.DefaultSpawnPoint.position, GameState.Current.Map.DefaultSpawnPoint.rotation);
			if ((bool)GameState.Current.Player.Character)
			{
				GameState.Current.PlayerData.Reset();
				GameState.Current.Player.Character.Reset();
			}
		}
		GameState.Current.PlayerState.SetState(PlayerStateId.Overview);
	}

	public void OnExit()
	{
		EventHandler.Global.RemoveListener<GameEvents.MatchCountdown>(OnMatchCountdown);
		EventHandler.Global.RemoveListener<GameEvents.PlayerRespawn>(OnPlayerRespawn);
	}

	private void OnPlayerRespawn(GameEvents.PlayerRespawn ev)
	{
		GameState.Current.RespawnLocalPlayerAt(ev.Position, Quaternion.Euler(0f, ev.Rotation, 0f));
		GameState.Current.PlayerState.SetState(PlayerStateId.PrepareForMatch);
	}

	private void OnMatchCountdown(GameEvents.MatchCountdown ev)
	{
		if (ev.Countdown <= 3 && ev.Countdown > 0)
		{
			AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.CountdownTonal1, 0uL, 1f, 1f);
		}
	}

	public void OnResume()
	{
	}

	public void OnUpdate()
	{
	}
}
