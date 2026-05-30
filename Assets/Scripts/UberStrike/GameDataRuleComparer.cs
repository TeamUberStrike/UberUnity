using UberStrike.Core.Models;

public class GameDataRuleComparer : GameDataBaseComparer
{
	protected override int OnCompare(GameRoomData a, GameRoomData b)
	{
		int num = (short)a.GameMode - (short)b.GameMode;
		return (num == 0) ? GameDataNameComparer.StaticCompare(a, b) : ((!GameDataComparer.SortAscending) ? (-num) : num);
	}
}
