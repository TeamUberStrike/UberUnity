using System;

namespace UberStrike.Realtime.UnitySdk
{
	public class SecureMemoryMonitor
	{
		public static readonly SecureMemoryMonitor Instance = new SecureMemoryMonitor();

		private event Action _sender;

		internal event Action AddToMonitor
		{
			add
			{
				this._sender = (Action)Delegate.Combine(this._sender, value);
			}
			remove
			{
				this._sender = (Action)Delegate.Remove(this._sender, value);
			}
		}

		private SecureMemoryMonitor()
		{
		}

		public void PerformCheck()
		{
			if (this._sender != null)
			{
				this._sender();
			}
		}
	}
}
