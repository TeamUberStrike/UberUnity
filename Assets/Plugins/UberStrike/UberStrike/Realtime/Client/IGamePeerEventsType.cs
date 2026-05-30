namespace UberStrike.Realtime.Client
{
	public enum IGamePeerEventsType
	{
		HeartbeatChallenge = 1,
		RoomEntered = 2,
		RoomEnterFailed = 3,
		RequestPasswordForRoom = 4,
		RoomLeft = 5,
		FullGameList = 6,
		GameListUpdate = 7,
		GameListUpdateEnd = 8,
		GetGameInformation = 9,
		ServerLoadData = 10,
		DisconnectAndDisablePhoton = 11
	}
}
