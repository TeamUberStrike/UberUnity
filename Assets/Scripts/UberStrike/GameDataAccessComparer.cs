using UberStrike.Core.Models;

public class GameDataAccessComparer : GameDataBaseComparer
{
	protected override int OnCompare(GameRoomData a, GameRoomData b)
	{
		int result = 0;
		if (GameDataComparer.SortAscending)
		{
			if (!a.IsPasswordProtected && !b.IsPasswordProtected)
			{
				result = 2;
			}
			else if (!a.IsPasswordProtected)
			{
				result = 1;
			}
			else if (!b.IsPasswordProtected)
			{
				result = -1;
			}
		}
		else if (a.IsPasswordProtected && b.IsPasswordProtected)
		{
			result = 2;
		}
		else if (a.IsPasswordProtected)
		{
			result = 1;
		}
		else if (b.IsPasswordProtected)
		{
			result = -1;
		}
		return result;
	}
}
