using UberStrike.Core.Models;

public class GameDataMapComparer : GameDataBaseComparer
{
	protected override int OnCompare(GameRoomData a, GameRoomData b)
	{
		int num = a.MapID - b.MapID;
		return (num == 0) ? GameDataNameComparer.StaticCompare(a, b) : ((!GameDataComparer.SortAscending) ? (-num) : num);
	}
}
