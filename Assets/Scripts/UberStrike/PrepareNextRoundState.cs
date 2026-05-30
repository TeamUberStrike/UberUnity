using UnityEngine;

internal class PrepareNextRoundState : IState
{
	public PrepareNextRoundState(StateMachine<GameStateId> stateMachine)
	{
	}

	public void OnEnter()
	{
		EventHandler.Global.AddListener<GameEvents.MatchCountdown>(OnMatchStartCountdownEvent);
		EventHandler.Global.AddListener<GameEvents.PlayerRespawn>(OnPlayerRespawn);
		GameState.Current.PlayerState.SetState(PlayerStateId.PrepareForMatch);
		EventHandler.Global.Fire(new GameEvents.PlayerIngame());
	}

	public void OnResume()
	{
	}

	public void OnExit()
	{
		EventHandler.Global.RemoveListener<GameEvents.MatchCountdown>(OnMatchStartCountdownEvent);
		EventHandler.Global.RemoveListener<GameEvents.PlayerRespawn>(OnPlayerRespawn);
	}

	public void OnUpdate()
	{
	}

	private void OnPlayerRespawn(GameEvents.PlayerRespawn ev)
	{
		GameState.Current.RespawnLocalPlayerAt(ev.Position, Quaternion.Euler(0f, ev.Rotation, 0f));
		GameState.Current.PlayerState.SetState(PlayerStateId.PrepareForMatch);
	}

	private void OnMatchStartCountdownEvent(GameEvents.MatchCountdown ev)
	{
		switch (ev.Countdown)
		{
		case 5:
			AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.MatchEndingCountdown5, 0uL, 1f, 1f);
			break;
		case 4:
			AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.MatchEndingCountdown4, 0uL, 1f, 1f);
			break;
		case 3:
			AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.MatchEndingCountdown3, 0uL, 1f, 1f);
			break;
		case 2:
			AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.MatchEndingCountdown2, 0uL, 1f, 1f);
			break;
		case 1:
			AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.MatchEndingCountdown1, 0uL, 1f, 1f);
			break;
		}
	}
}
