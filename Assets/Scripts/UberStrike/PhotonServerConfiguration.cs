using System;
using Cmune.Core.Models.Views;
using Cmune.DataCenter.Common.Entities;
using UnityEngine;

public class PhotonServerConfiguration : MonoBehaviour
{
	[Serializable]
	public class LocalRealtimeServer
	{
		public string Ip = string.Empty;

		public int Port;

		public bool IsEnabled;

		public string Address
		{
			get
			{
				return Ip + ":" + Port;
			}
		}
	}

	[SerializeField]
	private LocalRealtimeServer _localGameServer = new LocalRealtimeServer
	{
		Ip = "127.0.0.1",
		Port = 5155
	};

	[SerializeField]
	private LocalRealtimeServer _localCommServer = new LocalRealtimeServer
	{
		Ip = "127.0.0.1",
		Port = 5055
	};

	[SerializeField]
	private bool simEnabled;

	private float incomingLag;

	private float outgoingLag;

	private float incomingLoss;

	private float outgoingLoss;

	public LocalRealtimeServer CustomGameServer
	{
		get
		{
			return _localGameServer;
		}
	}

	public LocalRealtimeServer CustomCommServer
	{
		get
		{
			return _localCommServer;
		}
	}

	private void Awake()
	{
		if (CustomGameServer.IsEnabled)
		{
			for (int i = 0; i < 20; i += 5)
			{
				Singleton<GameServerManager>.Instance.AddPhotonGameServer(new PhotonView
				{
					IP = CustomGameServer.Ip,
					Port = CustomGameServer.Port,
					Name = "CUSTOM GAME SERVER",
					PhotonId = UnityEngine.Random.Range(-1, -100),
					Region = RegionType.AsiaPacific,
					UsageType = PhotonUsageType.All,
					MinLatency = i
				});
			}
		}
		if (_localCommServer.IsEnabled)
		{
			Singleton<GameServerManager>.Instance.CommServer = new PhotonServer(_localCommServer.Address, PhotonUsageType.CommServer);
		}
	}
}
