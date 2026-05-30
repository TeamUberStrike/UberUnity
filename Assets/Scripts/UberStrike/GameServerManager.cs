using System;
using System.Collections;
using System.Collections.Generic;
using Cmune.Core.Models.Views;
using Cmune.DataCenter.Common.Entities;
using UberStrike.Core.Models;
using UnityEngine;

public class GameServerManager : Singleton<GameServerManager>
{
	private const int ServerUpdateCycle = 30;

	private Dictionary<int, PhotonServer> _gameServers = new Dictionary<int, PhotonServer>();

	public PhotonServer CommServer = PhotonServer.Empty;

	private List<PhotonServer> _sortedServers = new List<PhotonServer>();

	private IComparer<PhotonServer> _comparer;

	private bool _reverseSorting;

	private Dictionary<int, ServerLoadRequest> _loadRequests = new Dictionary<int, ServerLoadRequest>();

	public int PhotonServerCount
	{
		get
		{
			return _gameServers.Count;
		}
	}

	public int AllPlayersCount { get; private set; }

	public int AllGamesCount { get; private set; }

	public IEnumerable<PhotonServer> PhotonServerList
	{
		get
		{
			return _sortedServers;
		}
	}

	public IEnumerable<ServerLoadRequest> ServerRequests
	{
		get
		{
			return _loadRequests.Values;
		}
	}

	private GameServerManager()
	{
	}

	public void SortServers()
	{
		if (_comparer != null)
		{
			_sortedServers.Sort(_comparer);
			if (_reverseSorting)
			{
				_sortedServers.Reverse();
			}
		}
	}

	public PhotonServer GetBestServer()
	{
		PhotonServer bestServer = GetBestServer(ApplicationDataManager.IsMobile);
		if (ApplicationDataManager.IsMobile && bestServer == null)
		{
			bestServer = GetBestServer(false);
		}
		return bestServer;
	}

	private PhotonServer GetBestServer(bool doMobileFilter)
	{
		List<PhotonServer> list = new List<PhotonServer>(_gameServers.Values);
		list.Sort((PhotonServer s, PhotonServer t) => s.Latency - t.Latency);
		PhotonServer photonServer = null;
		for (int num = 0; num < list.Count; num++)
		{
			PhotonServer photonServer2 = list[num];
			if (photonServer2.Latency != 0 && (!doMobileFilter || photonServer2.UsageType == PhotonUsageType.Mobile))
			{
				if (photonServer == null && photonServer2.CheckLatency())
				{
					photonServer = photonServer2;
				}
				else if (photonServer2.CheckLatency() && photonServer2.Latency < 200 && photonServer.Data.PlayersConnected < photonServer2.Data.PlayersConnected)
				{
					photonServer = photonServer2;
				}
			}
		}
		return photonServer;
	}

	internal string GetServerName(GameRoomData room)
	{
		string result = string.Empty;
		if (room != null && room.Server != null)
		{
			foreach (PhotonServer value in _gameServers.Values)
			{
				if (value.ConnectionString == room.Server.ConnectionString)
				{
					result = value.Name;
					break;
				}
			}
		}
		return result;
	}

	public void SortServers(IComparer<PhotonServer> comparer, bool reverse = false)
	{
		_comparer = comparer;
		_reverseSorting = reverse;
		lock (_sortedServers)
		{
			_sortedServers.Clear();
			_sortedServers.AddRange(_gameServers.Values);
		}
		SortServers();
	}

	public void AddTestPhotonGameServer(int id, PhotonServer photonServer)
	{
		_gameServers[id] = photonServer;
	}

	public void AddPhotonGameServer(PhotonView view)
	{
		_gameServers[view.PhotonId] = new PhotonServer(view);
		if (view.MinLatency > 0)
		{
			view.Name = view.Name + " - " + view.MinLatency + "ms";
		}
		SortServers();
	}

	public void AddPhotonGameServers(List<PhotonView> servers)
	{
		foreach (PhotonView server in servers)
		{
			AddPhotonGameServer(server);
		}
	}

	public int GetServerLatency(string connection)
	{
		foreach (PhotonServer value in _gameServers.Values)
		{
			if (value.ConnectionString == connection)
			{
				return value.Latency;
			}
		}
		return 0;
	}

	public IEnumerator StartUpdatingServerLoads()
	{
		foreach (PhotonServer server in _gameServers.Values)
		{
			ServerLoadRequest request;
			if (!_loadRequests.TryGetValue(server.Id, out request))
			{
				request = ServerLoadRequest.Run(server, delegate
				{
					UpdateGamesAndPlayerCount();
				});
				_loadRequests.Add(server.Id, request);
			}
			if (request.RequestState != ServerLoadRequest.RequestStateType.Waiting)
			{
				request.Run();
			}
			yield return new WaitForSeconds(0.1f);
		}
	}

	public IEnumerator StartUpdatingLatency(Action<float> progressCallback)
	{
		yield return UnityRuntime.StartRoutine(StartUpdatingServerLoads());
		float minTimeout = Time.time + 4f;
		float maxTimeout = Time.time + 10f;
		int count = 0;
		while (count != _loadRequests.Count)
		{
			yield return new WaitForSeconds(1f);
			count = 0;
			foreach (ServerLoadRequest r in _loadRequests.Values)
			{
				if (r.RequestState != ServerLoadRequest.RequestStateType.Waiting)
				{
					count++;
				}
			}
			progressCallback((float)count / (float)_loadRequests.Count);
			if ((count > 0 && Time.time > minTimeout) || Time.time > maxTimeout)
			{
				break;
			}
		}
	}

	private void UpdateGamesAndPlayerCount()
	{
		AllPlayersCount = 0;
		AllGamesCount = 0;
		foreach (PhotonServer value in _gameServers.Values)
		{
			AllPlayersCount += value.Data.PlayersConnected;
			AllGamesCount += value.Data.RoomsCreated;
		}
		SortServers();
	}
}
