using System;

namespace UberStrike.Core.Types
{
	[Flags]
	public enum ModerationTag
	{
		None = 0,
		Muted = 1,
		Ghosted = 2,
		Banned = 4,
		Speedhacking = 8,
		Spamming = 0x10,
		Language = 0x20,
		Name = 0x40
	}
}
