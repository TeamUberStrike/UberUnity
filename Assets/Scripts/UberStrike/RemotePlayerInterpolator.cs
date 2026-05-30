using System.Collections.Generic;
using UberStrike.Core.Models;
using UnityEngine;

public class RemotePlayerInterpolator
{
	private Dictionary<byte, RemoteCharacterState> remoteStates;

	public RemotePlayerInterpolator()
	{
		remoteStates = new Dictionary<byte, RemoteCharacterState>(20);
	}

	public void Update()
	{
		foreach (RemoteCharacterState value in remoteStates.Values)
		{
			value.InterpolateMovement();
		}
	}

	public void PositionUpdate(PlayerMovement update, ushort gameFrame)
	{
		RemoteCharacterState value;
		if (remoteStates.TryGetValue(update.Number, out value))
		{
			value.PositionUpdate(update, gameFrame);
		}
	}

	public void DeltaUpdate(GameActorInfoDelta delta)
	{
		RemoteCharacterState value;
		if (remoteStates.TryGetValue(delta.Id, out value))
		{
			value.DeltaUpdate(delta);
		}
	}

	public void UpdatePositionHard(byte playerNumber, Vector3 pos)
	{
		RemoteCharacterState value;
		if (remoteStates.TryGetValue(playerNumber, out value))
		{
			value.SetPosition(pos);
		}
	}

	public void AddCharacterInfo(GameActorInfo player, PlayerMovement position)
	{
		remoteStates[player.PlayerId] = new RemoteCharacterState(player, position);
	}

	public void Reset()
	{
		remoteStates.Clear();
	}

	public void RemoveCharacterInfo(byte playerID)
	{
		RemoteCharacterState value;
		if (remoteStates.TryGetValue(playerID, out value))
		{
			remoteStates.Remove(value.Player.PlayerId);
		}
	}

	public RemoteCharacterState GetState(byte playerID)
	{
		RemoteCharacterState value = null;
		remoteStates.TryGetValue(playerID, out value);
		return value;
	}
}
