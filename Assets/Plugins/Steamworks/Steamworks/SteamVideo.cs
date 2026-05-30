namespace Steamworks
{
	public static class SteamVideo
	{
		public static void GetVideoURL(AppId_t unVideoAppID)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamVideo_GetVideoURL(unVideoAppID);
		}
	}
}
