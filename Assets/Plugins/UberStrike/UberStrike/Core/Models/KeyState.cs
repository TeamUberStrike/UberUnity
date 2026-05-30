using System;

namespace UberStrike.Core.Models
{
	[Flags]
	public enum KeyState : byte
	{
		Still = 0,
		Forward = 1,
		Backward = 2,
		Left = 4,
		Right = 8,
		Jump = 0x10,
		Crouch = 0x20,
		Vertical = 3,
		Horizontal = 0xC,
		Walking = 0xF
	}
}
