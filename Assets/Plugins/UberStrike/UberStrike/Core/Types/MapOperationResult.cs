namespace UberStrike.Core.Types
{
	public enum MapOperationResult
	{
		Error = 0,
		InvalidDisplayName = 1,
		DuplicateDisplayName = 2,
		InvalidDescription = 3,
		InvalidSceneName = 4,
		DuplicateSceneName = 5,
		Ok = 6,
		DuplicateMapId = 7,
		InvalidMapId = 8,
		InvalidApplicationVersion = 9,
		DuplicateMapIdDisplayNameSceneName = 10,
		DuplicateMapIdDisplayName = 11,
		DuplicateMapIdSceneName = 12,
		DuplicateDisplayNameSceneName = 13,
		NotFound = 14,
		DuplicateApplicationVersion = 15
	}
}
