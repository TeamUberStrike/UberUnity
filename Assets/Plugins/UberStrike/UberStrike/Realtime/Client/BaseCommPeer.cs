using System.IO;
using UberStrike.Core.Serialization;
using UberStrike.Core.ViewModel;

namespace UberStrike.Realtime.Client
{
	public abstract class BaseCommPeer : BasePeer, IEventDispatcher
	{
		public CommPeerOperations Operations { get; private set; }

		protected BaseCommPeer(int syncFrequency, bool monitorTraffic = false)
			: base(syncFrequency, monitorTraffic)
		{
			Operations = new CommPeerOperations(1);
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
				LoadData(data);
				break;
			case 3:
				LobbyEntered(data);
				break;
			case 4:
				DisconnectAndDisablePhoton(data);
				break;
			}
		}

		protected abstract void OnHeartbeatChallenge(string challengeHash);

		protected abstract void OnLoadData(ServerConnectionView data);

		protected abstract void OnLobbyEntered();

		protected abstract void OnDisconnectAndDisablePhoton(string message);

		private void HeartbeatChallenge(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				string challengeHash = StringProxy.Deserialize(bytes);
				OnHeartbeatChallenge(challengeHash);
			}
		}

		private void LoadData(byte[] _bytes)
		{
			using (MemoryStream bytes = new MemoryStream(_bytes))
			{
				ServerConnectionView data = ServerConnectionViewProxy.Deserialize(bytes);
				OnLoadData(data);
			}
		}

		private void LobbyEntered(byte[] _bytes)
		{
			using (new MemoryStream(_bytes))
			{
				OnLobbyEntered();
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
