namespace UberStrike.Realtime.Client
{
	public enum IGamePeerOperationsType
	{
		SendHeartbeatResponse = 1,
		GetServerLoad = 2,
		GetGameInformation = 3,
		GetGameListUpdates = 4,
		EnterRoom = 5,
		CreateRoom = 6,
		LeaveRoom = 7,
		CloseRoom = 8,
		InspectRoom = 9,
		ReportPlayer = 10,
		KickPlayer = 11,
		UpdateLoadout = 12,
		UpdatePing = 13,
		UpdateKeyState = 14,
		RefreshBackendData = 15
	}
}
