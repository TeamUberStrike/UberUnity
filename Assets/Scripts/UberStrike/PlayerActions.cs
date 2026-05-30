using System;

public class PlayerActions
{
	public Action<byte> UpdateKeyState;

	public Action<byte> UpdateMovementState;

	public Action<byte> SwitchWeapon;

	public Action<ushort> UpdatePing;

	public Action<bool> PausePlayer;

	public Action<bool> AutomaticFire;

	public Action<bool> SniperMode;

	public Action<bool> SetReadyForNextGame;

	public PlayerActions()
	{
		Clear();
	}

	public void Clear()
	{
		UpdateKeyState = delegate
		{
		};
		UpdateMovementState = delegate
		{
		};
		SwitchWeapon = delegate
		{
		};
		UpdatePing = delegate
		{
		};
		PausePlayer = delegate
		{
		};
		AutomaticFire = delegate
		{
		};
		SniperMode = delegate
		{
		};
		SetReadyForNextGame = delegate
		{
		};
	}
}
