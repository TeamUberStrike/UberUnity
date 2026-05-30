using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	[CallbackIdentity(4520)]
	public struct HTML_SizePopup_t
	{
		public const int k_iCallback = 4520;

		public HHTMLBrowser unBrowserHandle;

		public uint unX;

		public uint unY;

		public uint unWide;

		public uint unTall;
	}
}
