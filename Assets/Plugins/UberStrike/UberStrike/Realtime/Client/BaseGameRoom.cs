using System.Collections.Generic;
using System.IO;
using Cmune.DataCenter.Common.Entities;
using UberStrike.Core.Models;
using UberStrike.Core.Serialization;
using UberStrike.Core.Types;
using UnityEngine;

namespace UberStrike.Realtime.Client
{
	public abstract class BaseGameRoom : IEventDispatcher, IRoomLogic
	{
		IOperationSender IRoomLogic.Operations
		{
			get
			{
				return Operations;
			}
		}

		public GameRoomOperations Operations { get; private set; }

		protected BaseGameRoom()
		{
			Operations = new GameRoomOperations();
		}

		public void OnEvent(byte id, byte[] data)
		{
			switch (id)
			{
			case 12:
				PowerUpPicked(data);
				break;
			case 13:
				SetPowerupState(data);
				break;
			case 14:
				ResetAllPowerups(data);
				break;
			case 15:
				DoorOpen(data);
				break;
			case 16:
				DisconnectCountdown(data);
				break;
			case 17:
				MatchStartCountdown(data);
				break;
			case 18:
				MatchStart(data);
				break;
			case 19:
				MatchEnd(data);
				break;
			case 20:
				TeamWins(data);
				break;
			case 21:
				WaitingForPlayers(data);
				break;
			case 22:
				PrepareNextRound(data);
				break;
			case 23:
				AllPlayers(data);
				break;
			case 24:
				AllPlayerDeltas(data);
				break;
			case 25:
				AllPlayerPositions(data);
				break;
			case 26:
				PlayerDelta(data);
				break;
			case 27:
				PlayerJumped(data);
				break;
			case 28:
				PlayerRespawnCountdown(data);
				break;
			case 29:
				PlayerRespawned(data);
				break;
			case 30:
				PlayerJoinedGame(data);
				break;
			case 31:
				JoinGameFailed(data);
				break;
			case 32:
				PlayerLeftGame(data);
				break;
			case 33:
				PlayerChangedTeam(data);
				break;
			case 34:
				JoinedAsSpectator(data);
				break;
			case 35:
				PlayersReadyUpdated(data);
				break;
			case 36:
				DamageEvent(data);
				break;
			case 37:
				PlayerKilled(data);
				break;
			case 38:
				UpdateRoundScore(data);
				break;
			case 39:
				KillsRemaining(data);
				break;
			case 40:
				LevelUp(data);
				break;
			case 41:
				KickPlayer(data);
				break;
			case 42:
				QuickItemEvent(data);
				break;
			case 43:
				SingleBulletFire(data);
				break;
			case 44:
				PlayerHit(data);
				break;
			case 45:
				RemoveProjectile(data);
				break;
			case 46:
				EmitProjectile(data);
				break;
			case 47:
				EmitQuickItem(data);
				break;
			case 48:
				ActivateQuickItem(data);
				break;
			case 49:
				ChatMessage(data);
				break;
			}
		}

		protected abstract void OnPowerUpPicked(int id, byte flag);

		protected abstract void OnSetPowerupState(List<int> states);

		protected abstract void OnResetAllPowerups();

		protected abstract void OnDoorOpen(int id);

		protected abstract void OnDisconnectCountdown(byte countdown);

		protected abstract void OnMatchStartCountdown(byte countdown);

		protected abstract void OnMatchStart(int roundNumber, int endTime);

		protected abstract void OnMatchEnd(EndOfMatchData data);

		protected abstract void OnTeamWins(TeamID team);

		protected abstract void OnWaitingForPlayers();

		protected abstract void OnPrepareNextRound();

		protected abstract void OnAllPlayers(List<GameActorInfo> allPlayers, List<PlayerMovement> allPositions, ushort gameframe);

		protected abstract void OnAllPlayerDeltas(List<GameActorInfoDelta> allDeltas);

		protected abstract void OnAllPlayerPositions(List<PlayerMovement> allPositions, ushort gameframe);

		protected abstract void OnPlayerDelta(GameActorInfoDelta delta);

		protected abstract void OnPlayerJumped(int cmid, Vector3 position);

		protected abstract void OnPlayerRespawnCountdown(byte countdown);

		protected abstract void OnPlayerRespawned(int cmid, Vector3 position, byte rotation);

		protected abstract void OnPlayerJoinedGame(GameActorInfo player, PlayerMovement position);

		protected abstract void OnJoinGameFailed(string message);

		protected abstract void OnPlayerLeftGame(int cmid);

		protected abstract void OnPlayerChangedTeam(int cmid, TeamID team);

		protected abstract void OnJoinedAsSpectator();

		protected abstract void OnPlayersReadyUpdated();

		protected abstract void OnDamageEvent(DamageEvent damageEvent);

		protected abstract void OnPlayerKilled(int shooter, int target, byte weaponClass, ushort damage, byte bodyPart, Vector3 direction);

		protected abstract void OnUpdateRoundScore(int round, short blue, short red);

		protected abstract void OnKillsRemaining(int killsRemaining, int leaderCmid);

		protected abstract void OnLevelUp(int newLevel);

		protected abstract void OnKickPlayer(string message);

		protected abstract void OnQuickItemEvent(int cmid, byte eventType, int robotLifeTime, int scrapsLifeTime, bool isInstant);

		protected abstract void OnSingleBulletFire(int cmid);

		protected abstract void OnPlayerHit(Vector3 force);

		protected abstract void OnRemoveProjectile(int projectileId, bool explode);

		protected abstract void OnEmitProjectile(int cmid, Vector3 origin, Vector3 direction, byte slot, int projectileID, bool explode);

		protected abstract void OnEmitQuickItem(Vector3 origin, Vector3 direction, int itemId, byte playerNumber, int projectileID);

		protected abstract void OnActivateQuickItem(int cmid, QuickItemLogic logic, int robotLifeTime, int scrapsLifeTime, bool isInstant);

		protected abstract void OnChatMessage(int cmid, string name, string message, MemberAccessLevel accessLevel, byte context);

		private void PowerUpPicked(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				int id = Int32Proxy.Deserialize(bytes);
				byte flag = ByteProxy.Deserialize(bytes);
				OnPowerUpPicked(id, flag);
			}
		}

		private void SetPowerupState(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				List<int> states = ListProxy<int>.Deserialize(bytes, Int32Proxy.Deserialize);
				OnSetPowerupState(states);
			}
		}

		private void ResetAllPowerups(byte[] _bytes)
		{
			using (new MemoryStream(_bytes))
			{
				OnResetAllPowerups();
			}
		}

		private void DoorOpen(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				int id = Int32Proxy.Deserialize(bytes);
				OnDoorOpen(id);
			}
		}

		private void DisconnectCountdown(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				byte countdown = ByteProxy.Deserialize(bytes);
				OnDisconnectCountdown(countdown);
			}
		}

		private void MatchStartCountdown(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				byte countdown = ByteProxy.Deserialize(bytes);
				OnMatchStartCountdown(countdown);
			}
		}

		private void MatchStart(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				int roundNumber = Int32Proxy.Deserialize(bytes);
				int endTime = Int32Proxy.Deserialize(bytes);
				OnMatchStart(roundNumber, endTime);
			}
		}

		private void MatchEnd(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				EndOfMatchData data = EndOfMatchDataProxy.Deserialize(bytes);
				OnMatchEnd(data);
			}
		}

		private void TeamWins(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				TeamID team = EnumProxy<TeamID>.Deserialize(bytes);
				OnTeamWins(team);
			}
		}

		private void WaitingForPlayers(byte[] _bytes)
		{
			using (new MemoryStream(_bytes))
			{
				OnWaitingForPlayers();
			}
		}

		private void PrepareNextRound(byte[] _bytes)
		{
			using (new MemoryStream(_bytes))
			{
				OnPrepareNextRound();
			}
		}

		private void AllPlayers(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				List<GameActorInfo> allPlayers = ListProxy<GameActorInfo>.Deserialize(bytes, GameActorInfoProxy.Deserialize);
				List<PlayerMovement> allPositions = ListProxy<PlayerMovement>.Deserialize(bytes, PlayerMovementProxy.Deserialize);
				ushort gameframe = UInt16Proxy.Deserialize(bytes);
				OnAllPlayers(allPlayers, allPositions, gameframe);
			}
		}

		private void AllPlayerDeltas(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				List<GameActorInfoDelta> allDeltas = ListProxy<GameActorInfoDelta>.Deserialize(bytes, GameActorInfoDeltaProxy.Deserialize);
				OnAllPlayerDeltas(allDeltas);
			}
		}

		private void AllPlayerPositions(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				List<PlayerMovement> allPositions = ListProxy<PlayerMovement>.Deserialize(bytes, PlayerMovementProxy.Deserialize);
				ushort gameframe = UInt16Proxy.Deserialize(bytes);
				OnAllPlayerPositions(allPositions, gameframe);
			}
		}

		private void PlayerDelta(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				GameActorInfoDelta delta = GameActorInfoDeltaProxy.Deserialize(bytes);
				OnPlayerDelta(delta);
			}
		}

		private void PlayerJumped(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				int cmid = Int32Proxy.Deserialize(bytes);
				Vector3 position = Vector3Proxy.Deserialize(bytes);
				OnPlayerJumped(cmid, position);
			}
		}

		private void PlayerRespawnCountdown(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				byte countdown = ByteProxy.Deserialize(bytes);
				OnPlayerRespawnCountdown(countdown);
			}
		}

		private void PlayerRespawned(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				int cmid = Int32Proxy.Deserialize(bytes);
				Vector3 position = Vector3Proxy.Deserialize(bytes);
				byte rotation = ByteProxy.Deserialize(bytes);
				OnPlayerRespawned(cmid, position, rotation);
			}
		}

		private void PlayerJoinedGame(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				GameActorInfo player = GameActorInfoProxy.Deserialize(bytes);
				PlayerMovement position = PlayerMovementProxy.Deserialize(bytes);
				OnPlayerJoinedGame(player, position);
			}
		}

		private void JoinGameFailed(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				string message = StringProxy.Deserialize(bytes);
				OnJoinGameFailed(message);
			}
		}

		private void PlayerLeftGame(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				int cmid = Int32Proxy.Deserialize(bytes);
				OnPlayerLeftGame(cmid);
			}
		}

		private void PlayerChangedTeam(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				int cmid = Int32Proxy.Deserialize(bytes);
				TeamID team = EnumProxy<TeamID>.Deserialize(bytes);
				OnPlayerChangedTeam(cmid, team);
			}
		}

		private void JoinedAsSpectator(byte[] _bytes)
		{
			using (new MemoryStream(_bytes))
			{
				OnJoinedAsSpectator();
			}
		}

		private void PlayersReadyUpdated(byte[] _bytes)
		{
			using (new MemoryStream(_bytes))
			{
				OnPlayersReadyUpdated();
			}
		}

		private void DamageEvent(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				DamageEvent damageEvent = DamageEventProxy.Deserialize(bytes);
				OnDamageEvent(damageEvent);
			}
		}

		private void PlayerKilled(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				int shooter = Int32Proxy.Deserialize(bytes);
				int target = Int32Proxy.Deserialize(bytes);
				byte weaponClass = ByteProxy.Deserialize(bytes);
				ushort damage = UInt16Proxy.Deserialize(bytes);
				byte bodyPart = ByteProxy.Deserialize(bytes);
				Vector3 direction = Vector3Proxy.Deserialize(bytes);
				OnPlayerKilled(shooter, target, weaponClass, damage, bodyPart, direction);
			}
		}

		private void UpdateRoundScore(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				int round = Int32Proxy.Deserialize(bytes);
				short blue = Int16Proxy.Deserialize(bytes);
				short red = Int16Proxy.Deserialize(bytes);
				OnUpdateRoundScore(round, blue, red);
			}
		}

		private void KillsRemaining(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				int killsRemaining = Int32Proxy.Deserialize(bytes);
				int leaderCmid = Int32Proxy.Deserialize(bytes);
				OnKillsRemaining(killsRemaining, leaderCmid);
			}
		}

		private void LevelUp(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				int newLevel = Int32Proxy.Deserialize(bytes);
				OnLevelUp(newLevel);
			}
		}

		private void KickPlayer(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				string message = StringProxy.Deserialize(bytes);
				OnKickPlayer(message);
			}
		}

		private void QuickItemEvent(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				int cmid = Int32Proxy.Deserialize(bytes);
				byte eventType = ByteProxy.Deserialize(bytes);
				int robotLifeTime = Int32Proxy.Deserialize(bytes);
				int scrapsLifeTime = Int32Proxy.Deserialize(bytes);
				bool isInstant = BooleanProxy.Deserialize(bytes);
				OnQuickItemEvent(cmid, eventType, robotLifeTime, scrapsLifeTime, isInstant);
			}
		}

		private void SingleBulletFire(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				int cmid = Int32Proxy.Deserialize(bytes);
				OnSingleBulletFire(cmid);
			}
		}

		private void PlayerHit(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				Vector3 force = Vector3Proxy.Deserialize(bytes);
				OnPlayerHit(force);
			}
		}

		private void RemoveProjectile(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				int projectileId = Int32Proxy.Deserialize(bytes);
				bool explode = BooleanProxy.Deserialize(bytes);
				OnRemoveProjectile(projectileId, explode);
			}
		}

		private void EmitProjectile(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				int cmid = Int32Proxy.Deserialize(bytes);
				Vector3 origin = Vector3Proxy.Deserialize(bytes);
				Vector3 direction = Vector3Proxy.Deserialize(bytes);
				byte slot = ByteProxy.Deserialize(bytes);
				int projectileID = Int32Proxy.Deserialize(bytes);
				bool explode = BooleanProxy.Deserialize(bytes);
				OnEmitProjectile(cmid, origin, direction, slot, projectileID, explode);
			}
		}

		private void EmitQuickItem(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				Vector3 origin = Vector3Proxy.Deserialize(bytes);
				Vector3 direction = Vector3Proxy.Deserialize(bytes);
				int itemId = Int32Proxy.Deserialize(bytes);
				byte playerNumber = ByteProxy.Deserialize(bytes);
				int projectileID = Int32Proxy.Deserialize(bytes);
				OnEmitQuickItem(origin, direction, itemId, playerNumber, projectileID);
			}
		}

		private void ActivateQuickItem(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				int cmid = Int32Proxy.Deserialize(bytes);
				QuickItemLogic logic = EnumProxy<QuickItemLogic>.Deserialize(bytes);
				int robotLifeTime = Int32Proxy.Deserialize(bytes);
				int scrapsLifeTime = Int32Proxy.Deserialize(bytes);
				bool isInstant = BooleanProxy.Deserialize(bytes);
				OnActivateQuickItem(cmid, logic, robotLifeTime, scrapsLifeTime, isInstant);
			}
		}

		private void ChatMessage(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				int cmid = Int32Proxy.Deserialize(bytes);
				string name = StringProxy.Deserialize(bytes);
				string message = StringProxy.Deserialize(bytes);
				MemberAccessLevel accessLevel = EnumProxy<MemberAccessLevel>.Deserialize(bytes);
				byte context = ByteProxy.Deserialize(bytes);
				OnChatMessage(cmid, name, message, accessLevel, context);
			}
		}
	}
}
