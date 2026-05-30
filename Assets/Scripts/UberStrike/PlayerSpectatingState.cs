using System;
using System.Collections.Generic;
using UberStrike.Core.Models;
using UnityEngine;

internal class PlayerSpectatingState : IState
{
	private StateMachine<PlayerStateId> stateMachine;

	private int currentPlayerId;

	private int _currentFollowIndex;

	public PlayerSpectatingState(StateMachine<PlayerStateId> stateMachine)
	{
		this.stateMachine = stateMachine;
	}

	public void OnEnter()
	{
		GamePageManager.Instance.UnloadCurrentPage();
		EventHandler.Global.AddListener<GameEvents.PlayerPause>(OnPlayerPaused);
		EventHandler.Global.AddListener<GameEvents.PlayerUnpause>(OnPlayerUnpaused);
		EventHandler.Global.AddListener<GameEvents.PlayerLeft>(OnPlayerLeft);
		EventHandler.Global.AddListener<GameEvents.FollowPlayer>(FollowNextPlayer);
		EventHandler.Global.AddListener<GlobalEvents.InputChanged>(OnInputChanged);
		LevelCamera.SetMode(LevelCamera.CameraMode.FreeSpectator);
		EnterFreeMoveMode();
		GameState.Current.PlayerData.ResetKeys();
		OnPlayerUnpaused(null);
		EventHandler.Global.Fire(new GameEvents.PlayerSpectator());
	}

	public void OnResume()
	{
	}

	public void OnExit()
	{
		currentPlayerId = 0;
		LevelCamera.SetMode(LevelCamera.CameraMode.Disabled);
		GamePageManager.Instance.UnloadCurrentPage();
		EventHandler.Global.RemoveListener<GlobalEvents.InputChanged>(OnInputChanged);
		EventHandler.Global.RemoveListener<GameEvents.PlayerPause>(OnPlayerPaused);
		EventHandler.Global.RemoveListener<GameEvents.PlayerUnpause>(OnPlayerUnpaused);
		EventHandler.Global.RemoveListener<GameEvents.PlayerLeft>(OnPlayerLeft);
		EventHandler.Global.RemoveListener<GameEvents.FollowPlayer>(FollowNextPlayer);
	}

	public void OnUpdate()
	{
		if (Cursor.lockState != CursorLockMode.Locked && !ApplicationDataManager.IsMobile)
		{
			EventHandler.Global.Fire(new GameEvents.PlayerPause());
		}
		bool flag = (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift)) && Input.GetKey(KeyCode.Tab);
		if (Input.GetKeyDown(KeyCode.Escape) || flag)
		{
			EventHandler.Global.Fire(new GameEvents.PlayerPause());
		}
		if (!GameData.Instance.HUDChatIsTyping && Input.GetKeyDown(KeyCode.Backspace))
		{
			EventHandler.Global.Fire(new GameEvents.PlayerPause());
		}
	}

	private void OnPlayerPaused(GameEvents.PlayerPause ev)
	{
		stateMachine.PushState(PlayerStateId.Paused);
	}

	private void OnPlayerUnpaused(GameEvents.PlayerUnpause ev)
	{
		AutoMonoBehaviour<InputManager>.Instance.IsInputEnabled = true;
		Singleton<QuickItemController>.Instance.IsEnabled = false;
		GameState.Current.Player.EnableWeaponControl = false;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		EventHandler.Global.Fire(new GameEvents.PlayerIngame());
	}

	private void OnPlayerLeft(GameEvents.PlayerLeft ev)
	{
		if (currentPlayerId == ev.Cmid)
		{
			EnterFreeMoveMode();
		}
	}

	private void OnInputChanged(GlobalEvents.InputChanged ev)
	{
		if (AutoMonoBehaviour<InputManager>.Instance.IsInputEnabled && !GameData.Instance.HUDChatIsTyping && Cursor.lockState == CursorLockMode.Locked)
		{
			if (ev.Key == GameInputKey.PrimaryFire && ev.IsDown)
			{
				FollowPrevPlayer();
			}
			else if (ev.Key == GameInputKey.SecondaryFire && ev.IsDown)
			{
				FollowNextPlayer();
			}
			else if (ev.Key == GameInputKey.Jump && ev.IsDown)
			{
				EnterFreeMoveMode();
			}
		}
	}

	private void FollowNextPlayer(GameEvents.FollowPlayer ev)
	{
		FollowNextPlayer();
	}

	private void FollowNextPlayer()
	{
		try
		{
			if (!GameState.Current.HasJoinedGame || GameState.Current.Players.Count <= 0)
			{
				return;
			}
			GameActorInfo[] array = GameState.Current.Players.ValueArray();
			_currentFollowIndex = (_currentFollowIndex + 1) % array.Length;
			int currentFollowIndex = _currentFollowIndex;
			while (array[_currentFollowIndex].Cmid == PlayerDataManager.Cmid || !array[_currentFollowIndex].IsAlive || !GameState.Current.HasAvatarLoaded(array[_currentFollowIndex].Cmid))
			{
				_currentFollowIndex = (_currentFollowIndex + 1) % array.Length;
				if (_currentFollowIndex == currentFollowIndex)
				{
					EnterFreeMoveMode();
					return;
				}
			}
			if (array[_currentFollowIndex] != null)
			{
				ChangeTarget(array[_currentFollowIndex].Cmid);
			}
			else
			{
				EnterFreeMoveMode();
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Failed to follow next player: " + ex.Message);
		}
	}

	private void FollowPrevPlayer()
	{
		try
		{
			if (!GameState.Current.HasJoinedGame || GameState.Current.Players.Count <= 0)
			{
				return;
			}
			List<GameActorInfo> list = new List<GameActorInfo>(GameState.Current.Players.Values);
			_currentFollowIndex = (_currentFollowIndex + list.Count - 1) % list.Count;
			int currentFollowIndex = _currentFollowIndex;
			while (list[_currentFollowIndex].Cmid == PlayerDataManager.Cmid || !list[_currentFollowIndex].IsAlive || !GameState.Current.HasAvatarLoaded(list[_currentFollowIndex].Cmid))
			{
				_currentFollowIndex = (_currentFollowIndex + list.Count - 1) % list.Count;
				if (_currentFollowIndex == currentFollowIndex)
				{
					EnterFreeMoveMode();
					return;
				}
			}
			if (list[_currentFollowIndex] != null)
			{
				ChangeTarget(list[_currentFollowIndex].Cmid);
			}
			else
			{
				EnterFreeMoveMode();
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Failed to follow prev player: " + ex.Message);
		}
	}

	private void ChangeTarget(int cmid)
	{
		if (currentPlayerId == cmid)
		{
			return;
		}
		CharacterConfig character;
		if (GameState.Current.TryGetPlayerAvatar(cmid, out character) && (bool)character.Avatar.Decorator)
		{
			currentPlayerId = cmid;
			LevelCamera.SetMode(LevelCamera.CameraMode.SmoothFollow, character.Avatar.Decorator.transform);
			if (!character.State.Player.IsAlive)
			{
				LevelCamera.SetPosition(character.transform.position);
			}
		}
		else
		{
			EnterFreeMoveMode();
		}
	}

	private void EnterFreeMoveMode()
	{
		if (LevelCamera.CurrentMode != LevelCamera.CameraMode.FreeSpectator)
		{
			currentPlayerId = 0;
			LevelCamera.SetMode(LevelCamera.CameraMode.FreeSpectator);
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}
}
