using System.Collections.Generic;
using UberStrike.Core.Models;
using UnityEngine;

public class PositionSyncDebugger
{
	private class PositionInfo
	{
		public Vector3 Position;

		public Color Color;

		public MoveStates State;
	}

	private Queue<PositionInfo> positions = new Queue<PositionInfo>();

	public void AddSample(Vector3 position, MoveStates state)
	{
	}
}
