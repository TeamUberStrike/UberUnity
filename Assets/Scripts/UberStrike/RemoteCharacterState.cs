using System;
using UberStrike.Core.Models;
using UberStrike.Realtime.UnitySdk;
using UnityEngine;

public class RemoteCharacterState : ICharacterState
{
	private class PositionInterpolator
	{
		private class Packet
		{
			public Vector3 Position;

			public float Time;

			public int GameFrame;

			public float ArrivalTime;
		}

		private Packet sampleA;

		private Packet sampleB;

		private Packet sampleC;

		private float timeWindow = 1.5f;

		private int baseGameFrame;

		private float baseTime;

		private float avgFrameTime;

		public float LastTime
		{
			get
			{
				return sampleA.Time;
			}
		}

		public PositionInterpolator()
		{
			Reset(Vector3.zero);
		}

		public void Add(PlayerMovement m, ushort gameFrame)
		{
			if (Mathf.Abs(gameFrame - sampleA.GameFrame) > 5)
			{
				baseGameFrame = gameFrame;
				baseTime = Time.time;
				avgFrameTime = 0.1f;
			}
			else
			{
				avgFrameTime = (Time.time - baseTime) / (float)(gameFrame - baseGameFrame);
				if (float.IsNaN(avgFrameTime) || float.IsInfinity(avgFrameTime) || avgFrameTime < 0.01f)
				{
					avgFrameTime = 0.1f;
				}
			}
			Vector3 vector = Lerp();
			if (gameFrame == sampleA.GameFrame)
			{
				sampleA = new Packet
				{
					Position = m.Position,
					Time = baseTime + (float)(gameFrame - baseGameFrame) * avgFrameTime + timeWindow * avgFrameTime,
					GameFrame = gameFrame
				};
			}
			else
			{
				sampleC = sampleB;
				sampleB = sampleA;
				sampleA = new Packet
				{
					Position = m.Position,
					Time = baseTime + (float)(gameFrame - baseGameFrame) * avgFrameTime + timeWindow * avgFrameTime,
					GameFrame = gameFrame
				};
				Vector3 vector2 = Lerp();
			}
		}

		public Vector3 Lerp()
		{
			if (Time.time > sampleB.Time)
			{
				float t = 1f - Mathf.Clamp01(sampleA.Time - Time.time) / Mathf.Max(sampleA.Time - sampleB.Time, 0.05f);
				return Vector3.Lerp(sampleB.Position, sampleA.Position, t);
			}
			float t2 = 1f - Mathf.Clamp01(sampleB.Time - Time.time) / Mathf.Max(sampleB.Time - sampleC.Time, 0.05f);
			return Vector3.Lerp(sampleC.Position, sampleB.Position, t2);
		}

		public void Reset(Vector3 pos)
		{
			sampleA = new Packet
			{
				Position = pos,
				Time = Time.time + timeWindow * avgFrameTime
			};
			sampleB = new Packet
			{
				Position = pos,
				Time = Time.time + timeWindow * avgFrameTime - 1f * avgFrameTime
			};
			sampleC = new Packet
			{
				Position = pos,
				Time = Time.time + timeWindow * avgFrameTime - 2f * avgFrameTime
			};
			baseGameFrame = 0;
		}

		internal void Extrapolate()
		{
			Add(new PlayerMovement
			{
				Position = sampleA.Position + (sampleA.Position - sampleB.Position)
			}, (ushort)(sampleA.GameFrame + 1));
		}
	}

	private PositionSyncDebugger debugger;

	private float hRotationTarget;

	private float vRotationTarget;

	private PositionInterpolator sampler = new PositionInterpolator();

	public GameActorInfo Player { get; private set; }

	public Vector3 Velocity { get; set; }

	public Vector3 Position { get; set; }

	public Quaternion HorizontalRotation { get; set; }

	public float VerticalRotation { get; set; }

	public MoveStates MovementState { get; set; }

	public KeyState KeyState { get; set; }

	public event Action<GameActorInfoDelta> OnDeltaUpdate = delegate
	{
	};

	public event Action<PlayerMovement> OnPositionUpdate = delegate
	{
	};

	public RemoteCharacterState(GameActorInfo info, PlayerMovement update)
	{
		debugger = new PositionSyncDebugger();
		Player = info;
		sampler.Reset(update.Position);
		PositionUpdate(update, 0);
		HorizontalRotation = Quaternion.Euler(0f, hRotationTarget, 0f);
		VerticalRotation = vRotationTarget;
	}

	public bool Is(MoveStates state)
	{
		return (MovementState & state) != 0;
	}

	public void PositionUpdate(PlayerMovement update, ushort gameFrame)
	{
		MovementState = (MoveStates)update.MovementState;
		KeyState = (KeyState)update.KeyState;
		Velocity = update.Velocity;
		hRotationTarget = Conversion.Byte2Angle(update.HorizontalRotation);
		vRotationTarget = Conversion.Byte2Angle(update.VerticalRotation);
		sampler.Add(update, gameFrame);
		debugger.AddSample(update.Position, MovementState);
		InterpolateMovement();
		this.OnPositionUpdate(update);
	}

	public void DeltaUpdate(GameActorInfoDelta update)
	{
		update.Apply(Player);
		this.OnDeltaUpdate(update);
	}

	public void SetPosition(Vector3 pos)
	{
		sampler.Reset(pos);
		InterpolateMovement();
	}

	public void InterpolateMovement()
	{
		if (Time.time > sampler.LastTime && KeyState != KeyState.Still)
		{
			sampler.Extrapolate();
		}
		Position = sampler.Lerp();
		HorizontalRotation = Quaternion.Lerp(HorizontalRotation, Quaternion.Euler(0f, hRotationTarget, 0f), Time.deltaTime * 5f);
		VerticalRotation = Mathf.LerpAngle(VerticalRotation, vRotationTarget, Time.deltaTime * 20f);
		if (VerticalRotation > 180f)
		{
			VerticalRotation -= 360f;
		}
	}
}
