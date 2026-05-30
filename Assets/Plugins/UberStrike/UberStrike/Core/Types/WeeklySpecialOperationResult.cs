namespace UberStrike.Core.Types
{
	public enum WeeklySpecialOperationResult
	{
		Error = 0,
		InvalidTitle = 1,
		InvalidText = 2,
		InvalidImageUrl = 3,
		Ok = 4,
		InvalidItemId = 5,
		ExistingWeeklySpecial = 6,
		NonExistingWeeklySpecial = 7
	}
}
