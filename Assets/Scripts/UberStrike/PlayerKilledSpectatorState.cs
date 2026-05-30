using UnityEngine;

internal class PlayerKilledSpectatorState : IState
{
	public PlayerKilledSpectatorState(StateMachine<PlayerStateId> stateMachine, bool showInGameHelp = true)
	{
	}

	public void OnEnter()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		Singleton<QuickItemController>.Instance.IsEnabled = false;
		AutoMonoBehaviour<InputManager>.Instance.IsInputEnabled = false;
		LevelCamera.SetMode(LevelCamera.CameraMode.Ragdoll);
	}

	public void OnResume()
	{
	}

	public void OnExit()
	{
	}

	public void OnUpdate()
	{
	}
}
