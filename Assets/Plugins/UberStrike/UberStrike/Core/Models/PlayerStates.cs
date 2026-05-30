using System;

namespace UberStrike.Core.Models
{
	[Flags]
	public enum PlayerStates : byte
	{
		None = 0,
		Spectator = 1,
		Dead = 2,
		Paused = 4,
		Sniping = 8,
		Shooting = 0x10,
		Ready = 0x20,
		Offline = 0x40
	}
}
