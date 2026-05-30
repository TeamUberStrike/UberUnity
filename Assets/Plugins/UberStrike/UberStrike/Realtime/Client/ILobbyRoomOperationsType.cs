namespace UberStrike.Realtime.Client
{
	public enum ILobbyRoomOperationsType
	{
		FullPlayerListUpdate = 1,
		UpdatePlayerRoom = 2,
		ResetPlayerRoom = 3,
		UpdateFriendsList = 4,
		UpdateClanData = 5,
		UpdateInboxMessages = 6,
		UpdateInboxRequests = 7,
		UpdateClanMembers = 8,
		GetPlayersWithMatchingName = 9,
		ChatMessageToAll = 10,
		ChatMessageToPlayer = 11,
		ChatMessageToClan = 12,
		ModerationMutePlayer = 13,
		ModerationPermanentBan = 14,
		ModerationBanPlayer = 15,
		ModerationKickGame = 16,
		ModerationUnbanPlayer = 17,
		ModerationCustomMessage = 18,
		SpeedhackDetection = 19,
		SpeedhackDetectionNew = 20,
		PlayersReported = 21,
		UpdateNaughtyList = 22,
		ClearModeratorFlags = 23,
		SetContactList = 24,
		UpdateAllActors = 25,
		UpdateContacts = 26
	}
}
