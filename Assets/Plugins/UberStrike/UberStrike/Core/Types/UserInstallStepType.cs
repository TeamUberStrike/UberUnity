namespace UberStrike.Core.Types
{
	public enum UserInstallStepType
	{
		InvalidWsCall = 0,
		NoUnity = 1,
		ClickDownload = 2,
		UnityInstalled = 3,
		FullGameLoaded = 4,
		ClickCancel = 5,
		UnityInitialized = 6,
		AccountCreated = 7,
		HasUnity = 8
	}
}
