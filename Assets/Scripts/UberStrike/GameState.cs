using System;
using System.Collections.Generic;
using Cmune.DataCenter.Common.Entities;
using UberStrike.Core.Models;
using UberStrike.Core.Types;
using UberStrike.Realtime.UnitySdk;
using UnityEngine;

public class GameState
{
	public static readonly GameState Current = new GameState();

	public readonly Dictionary<int, CharacterConfig> Avatars = new Dictionary<int, CharacterConfig>();

	public readonly Dictionary<int, GameActorInfo> Players = new Dictionary<int, GameActorInfo>();

	public readonly StateMachine<GameStateId> MatchState = new StateMachine<GameStateId>();

	public readonly StateMachine<PlayerStateId> PlayerState = new StateMachine<PlayerStateId>();

	public readonly GameActions Actions = new GameActions();

	public readonly RemotePlayerInterpolator RemotePlayerStates = new RemotePlayerInterpolator();

	public readonly PlayerLeadAudio LeadStatus = new PlayerLeadAudio();

	public readonly Avatar Avatar = new Avatar(Loadout.Empty, true);

	public readonly EndOfMatchStats Statistics = new EndOfMatchStats();

	private LocalPlayer player;

	private int roundStartTime;

	public LocalPlayer Player
	{
		get
		{
			if (player == null && PrefabManager.Instance != null)
			{
				player = PrefabManager.Instance.InstantiateLocalPlayer();
			}
			return player;
		}
	}

	public PlayerData PlayerData { get; private set; }

	public MapConfiguration Map { get; set; }

	public GameRoomData RoomData { get; set; }

	public int RoundsPlayed { get; set; }

	public int ScoreRed { get; private set; }

	public int ScoreBlue { get; private set; }

	public int BlueTeamPlayerCount { get; private set; }

	public int RedTeamPlayerCount { get; private set; }

	public int PlayerCountReadyForNextRound { get; private set; }

	public bool IsInGame
	{
		get
		{
			switch (MatchState.CurrentStateId)
			{
			case GameStateId.None:
			case GameStateId.PregameLoadout:
			case GameStateId.EndOfMatch:
				return false;
			default:
				return true;
			}
		}
	}

	public bool IsMatchRunning
	{
		get
		{
			return MatchState.CurrentStateId == GameStateId.MatchRunning;
		}
	}

	public bool IsEndOfMatchState
	{
		get
		{
			return MatchState.CurrentStateId == GameStateId.EndOfMatch;
		}
	}

	public bool IsInAnyGameState
	{
		get
		{
			return MatchState.CurrentStateId != GameStateId.None;
		}
	}

	public bool IsPlayerPaused
	{
		get
		{
			return PlayerState.CurrentStateId == PlayerStateId.Paused;
		}
	}

	public bool IsPlayerDead
	{
		get
		{
			return PlayerState.CurrentStateId == PlayerStateId.Killed || PlayerState.CurrentStateId == PlayerStateId.Spectating;
		}
	}

	public bool IsPlaying
	{
		get
		{
			return PlayerState.CurrentStateId == PlayerStateId.Playing || PlayerState.CurrentStateId == PlayerStateId.Spectating;
		}
	}

	public bool IsWaitingForPlayers
	{
		get
		{
			return MatchState.CurrentStateId == GameStateId.WaitingForPlayers;
		}
	}

	public bool HasJoinedGame
	{
		get
		{
			return MatchState.CurrentStateId != GameStateId.None;
		}
	}

	public bool IsLocalAvatarLoaded
	{
		get
		{
			return Avatars.ContainsKey(PlayerDataManager.Cmid);
		}
	}

	public bool IsSinglePlayer
	{
		get
		{
			return !IsMultiplayer;
		}
	}

	public bool IsGameAboutToEnd
	{
		get
		{
			return GameTime >= (float)(RoomData.TimeLimit - 1);
		}
	}

	public bool CanJoinRedTeam
	{
		get
		{
			return IsAccessAllowed || (!IsGameFull && RedTeamPlayerCount <= BlueTeamPlayerCount);
		}
	}

	public bool CanJoinBlueTeam
	{
		get
		{
			return IsAccessAllowed || (!IsGameFull && BlueTeamPlayerCount <= RedTeamPlayerCount);
		}
	}

	public bool CanJoinGame
	{
		get
		{
			return IsAccessAllowed || !IsGameFull;
		}
	}

	public bool IsGameFull
	{
		get
		{
			return RoomData.ConnectedPlayers >= RoomData.PlayerLimit;
		}
	}

	public bool IsAccessAllowed
	{
		get
		{
			return PlayerDataManager.AccessLevel >= MemberAccessLevel.Moderator;
		}
	}

	public float GameTime
	{
		get
		{
			return Mathf.Max((float)((double)(Singleton<GameStateController>.Instance.Client.ServerTimeTicks - roundStartTime) / 1000.0), 0f);
		}
	}

	public GameModeType GameMode
	{
		get
		{
			return RoomData.GameMode;
		}
	}

	public bool IsMultiplayer
	{
		get
		{
			return RoomData.GameMode != GameModeType.None;
		}
	}

	public bool IsTeamGame
	{
		get
		{
			return GameMode == GameModeType.TeamDeathMatch || GameMode == GameModeType.EliminationMode;
		}
	}

	private GameState()
	{
		MatchState.OnChanged += delegate(GameStateId el)
		{
			GameData.Instance.GameState.Value = el;
		};
		PlayerState.OnChanged += delegate(PlayerStateId el)
		{
			GameData.Instance.PlayerState.Value = el;
		};
		PlayerData = new PlayerData();
		Reset();
		AutoMonoBehaviour<UnityRuntime>.Instance.OnUpdate += delegate
		{
			if (IsInGame)
			{
				if (IsLocalAvatarLoaded)
				{
					PlayerData.SendUpdates();
				}
				RemotePlayerStates.Update();
			}
			MatchState.Update();
			PlayerState.Update();
		};
	}

	public bool HasAvatarLoaded(int cmid)
	{
		return Avatars.ContainsKey(cmid);
	}

	public void ResetRoundStartTime()
	{
		roundStartTime = Singleton<GameStateController>.Instance.Client.ServerTimeTicks;
	}

	public void Reset()
	{
		Actions.Clear();
		PlayerData.Reset();
		MatchState.Reset();
		PlayerState.Reset();
		RoomData = new GameRoomData
		{
			GameMode = GameModeType.None
		};
		foreach (CharacterConfig value in Avatars.Values)
		{
			value.Destroy();
		}
		RemotePlayerStates.Reset();
		Avatars.Clear();
		Players.Clear();
	}

	public bool TryGetPlayerAvatar(int cmid, out CharacterConfig character)
	{
		return Avatars.TryGetValue(cmid, out character) && character != null;
	}

	public bool TryGetActorInfo(int cmid, out GameActorInfo player)
	{
		return Players.TryGetValue(cmid, out player) && player != null;
	}

	public void UnloadAvatar(int cmid)
	{
		CharacterConfig value;
		if (Avatars.TryGetValue(cmid, out value))
		{
			if ((bool)value)
			{
				value.Destroy();
			}
			Avatars.Remove(cmid);
		}
		Players.Remove(cmid);
	}

	public void EmitRemoteProjectile(int cmid, Vector3 origin, Vector3 direction, byte slot, int projectileID, bool explode)
	{
		CharacterConfig character;
		if (TryGetPlayerAvatar(cmid, out character))
		{
			if ((bool)character.Avatar.Decorator.AnimationController)
			{
				character.Avatar.Decorator.AnimationController.Shoot();
			}
			IProjectile projectile = character.WeaponSimulator.EmitProjectile(cmid, character.State.Player.PlayerId, origin, direction, (LoadoutSlotType)slot, projectileID, explode);
			if (projectile != null)
			{
				Singleton<ProjectileManager>.Instance.AddProjectile(projectile, projectileID);
			}
		}
	}

	public void UpdateTeamCounter()
	{
		int redTeamPlayerCount = (BlueTeamPlayerCount = 0);
		RedTeamPlayerCount = redTeamPlayerCount;
		foreach (GameActorInfo value in Players.Values)
		{
			if (value.TeamID == TeamID.BLUE)
			{
				BlueTeamPlayerCount++;
			}
			else if (value.TeamID == TeamID.RED)
			{
				RedTeamPlayerCount++;
			}
		}
	}

	public void SingleBulletFire(int cmid)
	{
		CharacterConfig character;
		if (TryGetPlayerAvatar(cmid, out character) && character.State.Player.IsAlive && !character.IsLocal)
		{
			if ((bool)character.Avatar.Decorator.AnimationController)
			{
				character.Avatar.Decorator.AnimationController.Shoot();
			}
			character.WeaponSimulator.Shoot(character.State);
		}
	}

	public void QuickItemEvent(int cmid, byte eventType, int robotLifeTime, int scrapsLifeTime, bool isInstant)
	{
		CharacterConfig character;
		if (TryGetPlayerAvatar(cmid, out character))
		{
			Singleton<QuickItemSfxController>.Instance.ShowThirdPersonEffect(character, (QuickItemLogic)eventType, robotLifeTime, scrapsLifeTime, isInstant);
		}
	}

	public void ActivateQuickItem(int cmid, QuickItemLogic logic, int robotLifeTime, int scrapsLifeTime, bool isInstant)
	{
		CharacterConfig character;
		if (TryGetPlayerAvatar(cmid, out character))
		{
			Singleton<QuickItemSfxController>.Instance.ShowThirdPersonEffect(character, logic, robotLifeTime, scrapsLifeTime, isInstant);
		}
	}

	public void UpdatePlayersReady()
	{
		PlayerCountReadyForNextRound = 0;
		foreach (GameActorInfo value in Players.Values)
		{
			if (value.IsReadyForGame)
			{
				PlayerCountReadyForNextRound++;
			}
		}
	}

	public void UpdateTeamScore(int blueScore, int redScore)
	{
		ScoreRed = redScore;
		ScoreBlue = blueScore;
		Current.PlayerData.BlueTeamScore.Value = blueScore;
		Current.PlayerData.RedTeamScore.Value = redScore;
		int num = RoomData.KillLimit - Math.Max(redScore, blueScore);
		Current.PlayerData.RemainingKills.Value = num;
		if (MatchState.CurrentStateId == GameStateId.MatchRunning)
		{
			LeadStatus.PlayKillsLeftAudio(num);
		}
		switch (PlayerData.Player.TeamID)
		{
		case TeamID.RED:
			LeadStatus.UpdateLeadStatus(redScore, blueScore, num > 0 && MatchState.CurrentStateId == GameStateId.MatchRunning);
			break;
		case TeamID.BLUE:
			LeadStatus.UpdateLeadStatus(blueScore, redScore, num > 0 && MatchState.CurrentStateId == GameStateId.MatchRunning);
			break;
		}
	}

	private void UpdateDeathmatchScore()
	{
		int num = 0;
		foreach (GameActorInfo value in Players.Values)
		{
			if (value.Cmid != PlayerDataManager.Cmid && num < value.Kills)
			{
				num = value.Kills;
			}
		}
	}

	public void PlayerKilled(int shooter, int target, UberstrikeItemClass weaponClass, BodyPart bodyPart, Vector3 direction)
	{
		CharacterConfig value;
		if (Avatars.TryGetValue(target, out value) && !value.IsDead)
		{
			Avatars[target].SetDead(direction, BodyPart.Body, target, weaponClass);
			GameActorInfo valueOrDefault = Players.GetValueOrDefault(shooter);
			GameActorInfo valueOrDefault2 = Players.GetValueOrDefault(target);
			if (valueOrDefault2 == null)
			{
				Debug.LogError("Kill target is null " + target);
			}
			GameData.Instance.OnPlayerKilled.Fire(valueOrDefault, valueOrDefault2, weaponClass, bodyPart);
			if (target == PlayerDataManager.Cmid)
			{
				EventHandler.Global.Fire(new GameEvents.PlayerDied());
			}
		}
	}

	public void PlayerDamaged(DamageEvent damageEvent)
	{
		if (!(Player != null))
		{
			return;
		}
		foreach (KeyValuePair<byte, byte> item in damageEvent.Damage)
		{
			EventHandler.Global.Fire(new GameEvents.PlayerDamage
			{
				Angle = Conversion.Byte2Angle(item.Key),
				DamageValue = (int)item.Value
			});
			if ((damageEvent.DamageEffectFlag & 1) != 0)
			{
				Player.DamageFactor = damageEvent.DamgeEffectValue;
			}
		}
	}

	public void StartMatch(int roundNumber, int endTime)
	{
		roundStartTime = endTime - RoomData.TimeLimit * 1000;
		LeadStatus.Reset();
		Singleton<GameStateController>.Instance.Client.Peer.FetchServerTimestamp();
		CheatDetection.SyncSystemTime();
		LevelCamera.ResetFeedback();
		Current.PlayerData.RemainingKills.Value = RoomData.KillLimit;
		Current.PlayerData.RemainingTime.Value = 0;
	}

	public void UpdatePlayerStatistics(StatsCollection totalStats, StatsCollection bestPerLife)
	{
		int playerLevel = PlayerDataManager.PlayerLevel;
		if (playerLevel > 0 && playerLevel < XpPointsUtil.Config.MaxLevel)
		{
			Singleton<PlayerDataManager>.Instance.UpdatePlayerStats(totalStats, bestPerLife);
			if (PlayerDataManager.PlayerLevel != playerLevel)
			{
				PopupSystem.Show(new LevelUpPopup(PlayerDataManager.PlayerLevel, playerLevel));
			}
			GlobalUIRibbon.Instance.AddXPEvent(totalStats.Xp);
		}
		if (totalStats.Points > 0)
		{
			PlayerDataManager.Points += totalStats.Points;
			GlobalUIRibbon.Instance.AddPointsEvent(totalStats.Points);
		}
	}

	public AchievementType GetPlayersFirstAchievement(EndOfMatchData endOfMatchData)
	{
		AchievementType result = AchievementType.None;
		StatsSummary statsSummary = endOfMatchData.MostValuablePlayers.Find((StatsSummary p) => p.Cmid == PlayerDataManager.Cmid);
		if (statsSummary != null)
		{
			List<AchievementType> list = new List<AchievementType>();
			foreach (KeyValuePair<byte, ushort> achievement in statsSummary.Achievements)
			{
				list.Add((AchievementType)achievement.Key);
			}
			if (list.Count > 0)
			{
				result = list[0];
			}
		}
		return result;
	}

	public void EmitRemoteQuickItem(Vector3 origin, Vector3 direction, int itemId, byte playerNumber, int projectileID)
	{
		IUnityItem itemInShop = Singleton<ItemManager>.Instance.GetItemInShop(itemId);
		if (itemInShop != null)
		{
			if (!itemInShop.Prefab)
			{
				return;
			}
			IGrenadeProjectile grenadeProjectile = itemInShop.Prefab.GetComponent<QuickItem>() as IGrenadeProjectile;
			try
			{
				IGrenadeProjectile grenadeProjectile2 = grenadeProjectile.Throw(origin, direction);
				if (playerNumber == PlayerData.Player.PlayerId)
				{
					grenadeProjectile2.SetLayer(UberstrikeLayer.LocalProjectile);
				}
				else
				{
					grenadeProjectile2.SetLayer(UberstrikeLayer.RemoteProjectile);
				}
				Singleton<ProjectileManager>.Instance.AddProjectile(grenadeProjectile2, projectileID);
				return;
			}
			catch (Exception exception)
			{
				Debug.LogWarning("OnEmitQuickItem failed because Item is not a projectile: " + itemId + "/" + playerNumber + "/" + projectileID);
				Debug.LogException(exception);
				return;
			}
		}
		Debug.LogError("OnEmitQuickItem failed because item not found: " + itemId + "/" + playerNumber + "/" + projectileID);
	}

	public void PlayerLeftGame(int cmid)
	{
		try
		{
			EventHandler.Global.Fire(new GameEvents.PlayerLeft
			{
				Cmid = cmid
			});
			GameActorInfo value;
			if (Players.TryGetValue(cmid, out value))
			{
				GameData.Instance.OnHUDStreamMessage.Fire(value, LocalizedStrings.LeftTheGame, null);
				Debug.Log("<< OnPlayerLeftGame " + value.PlayerName + " " + MatchState.CurrentStateId);
				if (value.Cmid == PlayerDataManager.Cmid)
				{
					Player.SetCurrentCharacterConfig(null);
				}
				else
				{
					RemotePlayerStates.RemoveCharacterInfo(value.PlayerId);
				}
			}
			UnloadAvatar(cmid);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		finally
		{
			Singleton<ChatManager>.Instance.SetGameSection(RoomData.Server.ConnectionString, RoomData.Number, RoomData.MapID, Players.Values);
		}
	}

	public void AllPlayerDeltas(List<GameActorInfoDelta> players)
	{
		bool flag = false;
		bool flag2 = false;
		foreach (GameActorInfoDelta player in players)
		{
			try
			{
				if (player.Changes.Count > 0)
				{
					PlayerDelta(player);
					if (player.Changes.ContainsKey(GameActorInfoDelta.Keys.TeamID))
					{
						flag = true;
					}
					if (player.Changes.ContainsKey(GameActorInfoDelta.Keys.Kills))
					{
						flag2 = true;
					}
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}
		if (flag)
		{
			UpdateTeamCounter();
		}
		if (flag2 && GameMode == GameModeType.DeathMatch)
		{
			UpdateDeathmatchScore();
		}
	}

	public void PlayerDelta(GameActorInfoDelta update)
	{
		if (update.Id == PlayerData.Player.PlayerId)
		{
			PlayerData.DeltaUpdate(update);
		}
		else
		{
			RemotePlayerStates.DeltaUpdate(update);
		}
	}

	public void AllPositionUpdate(List<PlayerMovement> positions, ushort gameFrame)
	{
		foreach (PlayerMovement position in positions)
		{
			if (position.Number != PlayerData.Player.PlayerId)
			{
				RemotePlayerStates.PositionUpdate(position, gameFrame);
			}
		}
	}

	public void RespawnLocalPlayerAt(Vector3 position, Quaternion rotation)
	{
		Player.SpawnPlayerAt(position, rotation);
		CharacterConfig value;
		GameActorInfo info;
		if (Avatars.TryGetValue(PlayerDataManager.Cmid, out value))
		{
			value.Reset();
		}
		else if (TryGetActorInfo(PlayerDataManager.Cmid, out info))
		{
			InstantiateAvatar(info);
		}
	}

	public void PlayerRespawned(int cmid, Vector3 position, byte rotation)
	{
		GameActorInfo gameActorInfo;
		if (TryGetActorInfo(cmid, out gameActorInfo))
		{
			if (gameActorInfo.Cmid == PlayerDataManager.Cmid && gameActorInfo.TeamID == TeamID.NONE && GameMode != GameModeType.DeathMatch)
			{
				Debug.LogWarning("PlayerRespawned failed, invalid team for gamemode");
				Singleton<GameStateController>.Instance.LeaveGame();
				return;
			}
			if (!Avatars.ContainsKey(cmid))
			{
				InstantiateAvatar(gameActorInfo);
			}
			CharacterConfig value;
			if (Avatars.TryGetValue(cmid, out value))
			{
				RemotePlayerStates.UpdatePositionHard(value.State.Player.PlayerId, position);
				value.Reset();
			}
			if (cmid == PlayerDataManager.Cmid)
			{
				EventHandler.Global.Fire(new GameEvents.PlayerRespawn
				{
					Position = position,
					Rotation = Conversion.Byte2Angle(rotation)
				});
			}
		}
		else
		{
			Debug.LogError(string.Format("PlayerRespawned failed {0} because not found in the list of players!", cmid));
		}
	}

	public void InstantiateAvatar(GameActorInfo info)
	{
		if (!Avatars.ContainsKey(info.Cmid))
		{
			if (info.Cmid == PlayerDataManager.Cmid)
			{
				CharacterConfig characterConfig = PrefabManager.Instance.InstantiateLocalCharacter();
				Avatars.Add(info.Cmid, characterConfig);
				ConfigureAvatar(info, characterConfig, true);
			}
			else
			{
				CharacterConfig characterConfig2 = PrefabManager.Instance.InstantiateRemoteCharacter();
				Avatars.Add(info.Cmid, characterConfig2);
				ConfigureAvatar(info, characterConfig2, false);
			}
		}
		else
		{
			Debug.LogError(string.Format("Failed call of InstantiateAvatar {0} because already existing!", info.Cmid));
		}
	}

	private void ConfigureAvatar(GameActorInfo info, CharacterConfig character, bool isLocal)
	{
		if (character != null && info != null)
		{
			if (isLocal)
			{
				Player.SetCurrentCharacterConfig(character);
				Player.MoveController.IsLowGravity = GameFlags.IsFlagSet(GameFlags.GAME_FLAGS.LowGravity, RoomData.GameFlags);
				character.Initialize(PlayerData, Avatar);
			}
			else
			{
				Avatar avatar = new Avatar(new Loadout(info.Gear, info.Weapons), false);
				avatar.SetDecorator(AvatarBuilder.CreateRemoteAvatar(avatar.Loadout.GetAvatarGear(), info.SkinColor));
				character.Initialize(RemotePlayerStates.GetState(info.PlayerId), avatar);
				GameData.Instance.OnHUDStreamMessage.Fire(info, LocalizedStrings.JoinedTheGame, null);
			}
			if (!info.IsAlive)
			{
				character.SetDead(Vector3.zero);
			}
		}
		else
		{
			Debug.LogError(string.Format("OnAvatarLoaded failed because loaded Avatar is {0} and Info is {1}", character != null, info != null));
		}
	}

	public bool SendChatMessage(string message, ChatContext context)
	{
		message = ChatMessageFilter.Cleanup(message);
		if (!string.IsNullOrEmpty(message) && !ChatMessageFilter.IsSpamming(message))
		{
			GameStateHelper.OnChatMessage(PlayerDataManager.Cmid, PlayerDataManager.Name, message, PlayerDataManager.AccessLevel, (byte)ChatManager.CurrentChatContext);
			Actions.ChatMessage(message, (byte)ChatManager.CurrentChatContext);
			return true;
		}
		return false;
	}
}
