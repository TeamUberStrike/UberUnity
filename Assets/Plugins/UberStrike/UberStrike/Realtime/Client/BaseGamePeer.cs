using System.Collections.Generic;
using System.IO;
using Cmune.Core.Models;
using UberStrike.Core.Models;
using UberStrike.Core.Serialization;

namespace UberStrike.Realtime.Client
{
	public abstract class BaseGamePeer : BasePeer, IEventDispatcher
	{
		public GamePeerOperations Operations { get; private set; }

		protected BaseGamePeer(int syncFrequency, bool monitorTraffic = false)
			: base(syncFrequency, monitorTraffic)
		{
			Operations = new GamePeerOperations(1);
			AddRoomLogic(this, Operations);
		}

		public void OnEvent(byte id, byte[] data)
		{
			switch (id)
			{
			case 1:
				HeartbeatChallenge(data);
				break;
			case 2:
				RoomEntered(data);
				break;
			case 3:
				RoomEnterFailed(data);
				break;
			case 4:
				RequestPasswordForRoom(data);
				break;
			case 5:
				RoomLeft(data);
				break;
			case 6:
				FullGameList(data);
				break;
			case 7:
				GameListUpdate(data);
				break;
			case 8:
				GameListUpdateEnd(data);
				break;
			case 9:
				GetGameInformation(data);
				break;
			case 10:
				ServerLoadData(data);
				break;
			case 11:
				DisconnectAndDisablePhoton(data);
				break;
			}
		}

		protected abstract void OnHeartbeatChallenge(string challengeHash);

		protected abstract void OnRoomEntered(GameRoomData game);

		protected abstract void OnRoomEnterFailed(string server, int roomId, string message);

		protected abstract void OnRequestPasswordForRoom(string server, int roomId);

		protected abstract void OnRoomLeft();

		protected abstract void OnFullGameList(List<GameRoomData> gameList);

		protected abstract void OnGameListUpdate(List<GameRoomData> updatedGames, List<int> removedGames);

		protected abstract void OnGameListUpdateEnd();

		protected abstract void OnGetGameInformation(GameRoomData room, List<GameActorInfo> players, int endTime);

		protected abstract void OnServerLoadData(PhotonServerLoad data);

		protected abstract void OnDisconnectAndDisablePhoton(string message);

		private void HeartbeatChallenge(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				string challengeHash = StringProxy.Deserialize(bytes);
				OnHeartbeatChallenge(challengeHash);
			}
		}

		private void RoomEntered(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				GameRoomData game = GameRoomDataProxy.Deserialize(bytes);
				OnRoomEntered(game);
			}
		}

		private void RoomEnterFailed(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				string server = StringProxy.Deserialize(bytes);
				int roomId = Int32Proxy.Deserialize(bytes);
				string message = StringProxy.Deserialize(bytes);
				OnRoomEnterFailed(server, roomId, message);
			}
		}

		private void RequestPasswordForRoom(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				string server = StringProxy.Deserialize(bytes);
				int roomId = Int32Proxy.Deserialize(bytes);
				OnRequestPasswordForRoom(server, roomId);
			}
		}

		private void RoomLeft(byte[] _bytes)
		{
			using (new MemoryStream(_bytes))
			{
				OnRoomLeft();
			}
		}

		private void FullGameList(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				List<GameRoomData> gameList = ListProxy<GameRoomData>.Deserialize(bytes, GameRoomDataProxy.Deserialize);
				OnFullGameList(gameList);
			}
		}

		private void GameListUpdate(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				List<GameRoomData> updatedGames = ListProxy<GameRoomData>.Deserialize(bytes, GameRoomDataProxy.Deserialize);
				List<int> removedGames = ListProxy<int>.Deserialize(bytes, Int32Proxy.Deserialize);
				OnGameListUpdate(updatedGames, removedGames);
			}
		}

		private void GameListUpdateEnd(byte[] _bytes)
		{
			using (new MemoryStream(_bytes))
			{
				OnGameListUpdateEnd();
			}
		}

		private void GetGameInformation(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				GameRoomData room = GameRoomDataProxy.Deserialize(bytes);
				List<GameActorInfo> players = ListProxy<GameActorInfo>.Deserialize(bytes, GameActorInfoProxy.Deserialize);
				int endTime = Int32Proxy.Deserialize(bytes);
				OnGetGameInformation(room, players, endTime);
			}
		}

		private void ServerLoadData(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				PhotonServerLoad data = PhotonServerLoadProxy.Deserialize(bytes);
				OnServerLoadData(data);
			}
		}

		private void DisconnectAndDisablePhoton(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				string message = StringProxy.Deserialize(bytes);
				OnDisconnectAndDisablePhoton(message);
			}
		}
	}
}
