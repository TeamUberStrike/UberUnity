using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	[CallbackIdentity(4518)]
	public struct HTML_ShowPopup_t
	{
		public const int k_iCallback = 4518;

		public HHTMLBrowser unBrowserHandle;
	}
}
