using System;

namespace UberStrike.Core.Models
{
	[Flags]
	public enum MoveStates : byte
	{
		None = 0,
		Grounded = 1,
		Jumping = 2,
		Flying = 4,
		Ducked = 8,
		Wading = 0x10,
		Swimming = 0x20,
		Diving = 0x40,
		Landed = 0x80
	}
}
