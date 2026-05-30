using UberStrike.Core.Models;
using UberStrike.Core.Types;

internal static class ExtensionMethods
{
	public static void Copy(this CommActorInfo original, CommActorInfo data)
	{
		original.ClanTag = data.ClanTag;
		original.CurrentRoom = data.CurrentRoom;
		original.ModerationFlag = data.ModerationFlag;
		original.ModInformation = data.ModInformation;
		original.PlayerName = data.PlayerName;
	}

	public static GameModeType GetGameMode(int id)
	{
		switch (id)
		{
		case 101:
			return GameModeType.DeathMatch;
		case 100:
			return GameModeType.TeamDeathMatch;
		case 106:
			return GameModeType.EliminationMode;
		default:
			return GameModeType.None;
		}
	}
}
