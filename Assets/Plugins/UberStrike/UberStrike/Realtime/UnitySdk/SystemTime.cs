using System;

namespace UberStrike.Realtime.UnitySdk
{
	public static class SystemTime
	{
		public static int Running
		{
			get
			{
				return Environment.TickCount & 0x7FFFFFFF;
			}
		}
	}
}
