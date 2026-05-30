using UberStrike.Core.Models;

public class GameDataTimeComparer : GameDataBaseComparer
{
	protected override int OnCompare(GameRoomData a, GameRoomData b)
	{
		int num = a.TimeLimit - b.TimeLimit;
		return (!GameDataComparer.SortAscending) ? (-num) : num;
	}
}
