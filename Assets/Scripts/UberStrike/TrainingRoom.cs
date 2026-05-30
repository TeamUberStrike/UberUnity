using System;
using System.Collections;
using UberStrike.Core.Models;
using UberStrike.Core.Types;
using UnityEngine;

public class TrainingRoom : IDisposable, IGameMode
{
	private bool isDisposed;

	public GameStateId CurrentState
	{
		get
		{
			return GameState.Current.MatchState.CurrentStateId;
		}
	}

	public GameMode Type
	{
		get
		{
			return GameMode.Training;
		}
	}

	public TrainingRoom()
	{
		GameState.Current.MatchState.RegisterState(GameStateId.PregameLoadout, new PregameLoadoutState(GameState.Current.MatchState));
		GameState.Current.MatchState.RegisterState(GameStateId.MatchRunning, new OfflineMatchState(GameState.Current.MatchState));
		GameState.Current.PlayerState.RegisterState(PlayerStateId.Playing, new PlayerPlayingState(GameState.Current.PlayerState));
		GameState.Current.PlayerState.RegisterState(PlayerStateId.Killed, new PlayerKilledOfflineState(GameState.Current.PlayerState));
		GameState.Current.PlayerState.RegisterState(PlayerStateId.Paused, new PlayerPausedState(GameState.Current.PlayerState));
		GameState.Current.PlayerState.RegisterState(PlayerStateId.Overview, new PlayerOverviewState(GameState.Current.PlayerState));
		GameState.Current.Actions.KillPlayer = delegate
		{
			if (GameState.Current.IsInGame)
			{
				GameState.Current.PlayerKilled(0, PlayerDataManager.Cmid, (UberstrikeItemClass)0, (BodyPart)0, Vector3.zero);
			}
		};
		GameState.Current.Actions.ExplosionHitDamage = delegate(int targetCmid, ushort damage, Vector3 force, byte slot, byte distance)
		{
			GameStateHelper.PlayerHit(targetCmid, damage, BodyPart.Body, force);
			if ((int)GameState.Current.PlayerData.Health <= 0)
			{
				GameState.Current.PlayerData.Set(PlayerStates.Dead, true);
				GameState.Current.PlayerKilled(targetCmid, targetCmid, Singleton<WeaponController>.Instance.GetCurrentWeapon().View.ItemClass, BodyPart.Body, force);
			}
		};
		GameState.Current.Actions.JoinTeam = delegate
		{
			GameActorInfo gameActorInfo = new GameActorInfo
			{
				Cmid = PlayerDataManager.Cmid,
				SkinColor = PlayerDataManager.SkinColor
			};
			GameState.Current.PlayerData.Player = gameActorInfo;
			GameState.Current.Players[gameActorInfo.Cmid] = gameActorInfo;
			GameState.Current.InstantiateAvatar(gameActorInfo);
			GameState.Current.MatchState.SetState(GameStateId.MatchRunning);
			UnityRuntime.StartRoutine(ShowTrainingGameMessages());
		};
		TabScreenPanelGUI.SortPlayersByRank = GameStateHelper.SortDeathMatchPlayers;
		AutoMonoBehaviour<UnityRuntime>.Instance.OnUpdate += OnUpdate;
		GameStateHelper.EnterGameMode();
		GameState.Current.MatchState.SetState(GameStateId.PregameLoadout);
	}

	private IEnumerator ShowTrainingGameMessages()
	{
		if (!ApplicationDataManager.IsMobile)
		{
			float duration = 2f;
			GameData.Instance.OnNotificationFull.Fire(string.Empty, LocalizedStrings.TrainingTutorialMsg01, duration);
			yield return new WaitForSeconds(duration);
			GameData.Instance.OnNotificationFull.Fire(string.Empty, LocalizedStrings.MessageQuickItemsTry, duration);
			yield return new WaitForSeconds(duration);
			GameData.Instance.OnNotificationFull.Fire(string.Empty, LocalizedStrings.TrainingTutorialMsg03, duration);
			yield return new WaitForSeconds(duration);
			GameData.Instance.OnNotificationFull.Fire(string.Empty, LocalizedStrings.TrainingTutorialMsg04, duration);
			yield return new WaitForSeconds(duration);
			GameData.Instance.OnNotificationFull.Fire(string.Empty, string.Format(LocalizedStrings.TrainingTutorialMsg05, AutoMonoBehaviour<InputManager>.Instance.InputChannelForSlot(GameInputKey.Forward), AutoMonoBehaviour<InputManager>.Instance.InputChannelForSlot(GameInputKey.Left), AutoMonoBehaviour<InputManager>.Instance.InputChannelForSlot(GameInputKey.Backward), AutoMonoBehaviour<InputManager>.Instance.InputChannelForSlot(GameInputKey.Right)), duration);
			yield return new WaitForSeconds(duration);
			GameData.Instance.OnNotificationFull.Fire(string.Empty, string.Format(LocalizedStrings.TrainingTutorialMsg06, AutoMonoBehaviour<InputManager>.Instance.InputChannelForSlot(GameInputKey.PrimaryFire)), duration);
			yield return new WaitForSeconds(duration);
			GameData.Instance.OnNotificationFull.Fire(string.Empty, string.Format(LocalizedStrings.TrainingTutorialMsg07, AutoMonoBehaviour<InputManager>.Instance.InputChannelForSlot(GameInputKey.NextWeapon), AutoMonoBehaviour<InputManager>.Instance.InputChannelForSlot(GameInputKey.PrevWeapon)), duration);
			yield return new WaitForSeconds(duration);
			GameData.Instance.OnNotificationFull.Fire(string.Empty, string.Format(LocalizedStrings.TrainingTutorialMsg08, AutoMonoBehaviour<InputManager>.Instance.InputChannelForSlot(GameInputKey.WeaponMelee), AutoMonoBehaviour<InputManager>.Instance.InputChannelForSlot(GameInputKey.Weapon1), AutoMonoBehaviour<InputManager>.Instance.InputChannelForSlot(GameInputKey.Weapon2), AutoMonoBehaviour<InputManager>.Instance.InputChannelForSlot(GameInputKey.Weapon3)), duration);
			yield return new WaitForSeconds(duration);
			GameData.Instance.OnNotificationFull.Fire(string.Empty, string.Format(LocalizedStrings.TrainingTutorialMsg09, AutoMonoBehaviour<InputManager>.Instance.InputChannelForSlot(GameInputKey.Crouch)), duration);
			yield return new WaitForSeconds(duration);
			GameData.Instance.OnNotificationFull.Fire(string.Empty, string.Format(LocalizedStrings.TrainingTutorialMsg10, AutoMonoBehaviour<InputManager>.Instance.InputChannelForSlot(GameInputKey.Fullscreen)), duration);
			yield return new WaitForSeconds(duration);
			GameData.Instance.OnNotificationFull.Fire(string.Empty, LocalizedStrings.TrainingTutorialMsg11, duration);
			yield return new WaitForSeconds(duration);
		}
	}

	public void Dispose()
	{
		if (!isDisposed)
		{
			isDisposed = true;
			AutoMonoBehaviour<UnityRuntime>.Instance.OnUpdate -= OnUpdate;
			GameStateHelper.ExitGameMode();
		}
	}

	private void OnUpdate()
	{
		Singleton<QuickItemController>.Instance.Update();
	}
}
