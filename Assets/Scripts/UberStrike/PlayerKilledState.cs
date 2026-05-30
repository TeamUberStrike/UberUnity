using UnityEngine;

internal class PlayerKilledState : IState
{
	private const int DisconnectionTimeout = 120;

	private const int DisconnectionTimeoutAdmin = 1200;

	private StateMachine<PlayerStateId> stateMachine;

	public PlayerKilledState(StateMachine<PlayerStateId> stateMachine)
	{
		this.stateMachine = stateMachine;
	}

	public void OnEnter()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		Singleton<QuickItemController>.Instance.IsEnabled = false;
		AutoMonoBehaviour<InputManager>.Instance.IsInputEnabled = false;
		LevelCamera.SetMode(LevelCamera.CameraMode.Ragdoll);
		stateMachine.Events.AddListener<GameEvents.RespawnCountdown>(OnRespawnCountdown);
		EventHandler.Global.AddListener<GameEvents.PlayerUnpause>(OnUnpause);
	}

	public void OnResume()
	{
	}

	public void OnExit()
	{
		EventHandler.Global.RemoveListener<GameEvents.PlayerUnpause>(OnUnpause);
		stateMachine.Events.RemoveListener<GameEvents.RespawnCountdown>(OnRespawnCountdown);
	}

	public void OnUpdate()
	{
		GameStateHelper.UpdateMatchTime();
		if (Input.GetKeyDown(KeyCode.L) && !GameData.Instance.HUDChatIsTyping)
		{
			if (GamePageManager.IsCurrentPage(IngamePageType.None))
			{
				GamePageManager.Instance.LoadPage(IngamePageType.PausedWaiting);
			}
			else
			{
				GamePageManager.Instance.UnloadCurrentPage();
			}
		}
	}

	private void OnUnpause(GameEvents.PlayerUnpause ev)
	{
	}

	private void OnRespawnCountdown(GameEvents.RespawnCountdown ev)
	{
		GameData.Instance.OnRespawnCountdown.Fire(ev.Countdown);
	}
}
