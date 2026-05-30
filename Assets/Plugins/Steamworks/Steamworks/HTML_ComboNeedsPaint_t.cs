using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	[CallbackIdentity(4517)]
	public struct HTML_ComboNeedsPaint_t
	{
		public const int k_iCallback = 4517;

		public HHTMLBrowser unBrowserHandle;

		public IntPtr pBGRA;

		public uint unWide;

		public uint unTall;
	}
}
