using System;
using System.Collections.Generic;
using UberStrike.Core.Models;
using UberStrike.Core.Types;
using UberStrike.Realtime.UnitySdk;
using UnityEngine;

public class PlayerData : ICharacterState
{
	public readonly PlayerActions Actions = new PlayerActions();

	private float posSyncFrame;

	private float shotSyncFrame;

	public IntegerProperty Health = new IntegerProperty(0, 0, 200);

	public IntegerProperty ArmorPoints = new IntegerProperty(0, 0, 200);

	public IntegerProperty ArmorCarried = new IntegerProperty(0, 0, 200);

	public IntegerProperty RemainingTime = new IntegerProperty(0, 0);

	public IntegerProperty RemainingKills = new IntegerProperty(0, 0);

	public Property<TeamID> Team = new Property<TeamID>(TeamID.NONE);

	public IntegerProperty BlueTeamScore = new IntegerProperty(0, 0);

	public IntegerProperty RedTeamScore = new IntegerProperty(0, 0);

	public Property<Dictionary<LoadoutSlotType, IUnityItem>> LoadoutWeapons = new Property<Dictionary<LoadoutSlotType, IUnityItem>>();

	public Property<WeaponSlot> ActiveWeapon = new Property<WeaponSlot>();

	public Property<WeaponSlot> NextActiveWeapon = new Property<WeaponSlot>();

	public IntegerProperty Ammo = new IntegerProperty(0, 0);

	public Property<WeaponSlot> WeaponFired = new Property<WeaponSlot>();

	public Property<TeamID> FocusedPlayerTeam = new Property<TeamID>(TeamID.NONE);

	public Property<DamageInfo> AppliedDamage = new Property<DamageInfo>();

	public Property<bool> IsIronSighted = new Property<bool>(false);

	public Property<bool> IsZoomedIn = new Property<bool>(false);

	public static readonly Vector3 StandingOffset = new Vector3(0f, 0.65f, 0f);

	public static readonly Vector3 CrouchingOffset = new Vector3(0f, 0.1f, 0f);

	private MovementUpdateCache cache = new MovementUpdateCache();

	public int ArmorPointCapacity
	{
		get
		{
			return Player.ArmorPointCapacity;
		}
	}

	public PlayerStates PlayerState
	{
		get
		{
			return Player.PlayerState;
		}
	}

	public MoveStates MovementState { get; set; }

	public KeyState KeyState { get; private set; }

	public Vector3 Velocity { get; set; }

	public Vector3 Position
	{
		get
		{
			return (!(GameState.Current.Player != null)) ? Vector3.zero : GameState.Current.Player.transform.position;
		}
	}

	public Quaternion HorizontalRotation
	{
		get
		{
			return UserInput.HorizontalRotation;
		}
	}

	public float VerticalRotation
	{
		get
		{
			return UserInput.VerticalRotation.eulerAngles.x;
		}
	}

	public bool IsOnline
	{
		get
		{
			return !Is(PlayerStates.Offline);
		}
	}

	public bool IsAlive
	{
		get
		{
			return (PlayerState & PlayerStates.Dead) == 0;
		}
	}

	public bool IsSpectator
	{
		get
		{
			return (PlayerState & PlayerStates.Spectator) != 0;
		}
	}

	public bool IsUnderWater
	{
		get
		{
			return (MovementState & (MoveStates.Swimming | MoveStates.Diving)) != 0;
		}
	}

	public Vector3 CurrentOffset
	{
		get
		{
			return ((MovementState & MoveStates.Ducked) != MoveStates.None) ? CrouchingOffset : StandingOffset;
		}
	}

	public Vector3 ShootingPoint
	{
		get
		{
			return GameState.Current.Player.transform.position + CurrentOffset;
		}
	}

	public Vector3 ShootingDirection
	{
		get
		{
			return UserInput.Rotation * Vector3.forward;
		}
	}

	public GameActorInfo Player { get; set; }

	public event Action<GameActorInfoDelta> OnDeltaUpdate = delegate
	{
	};

	public event Action<PlayerMovement> OnPositionUpdate = delegate
	{
	};

	public event Action<Vector3> SendJumpUpdate = delegate
	{
	};

	public event Action<ShortVector3, ShortVector3, byte, byte, byte> SendMovementUpdate = delegate
	{
	};

	public PlayerData()
	{
		Player = new GameActorInfo();
		Health.AddEvent(delegate(int el)
		{
			Player.Health = (short)el;
		}, null);
		ArmorPoints.AddEvent(delegate(int el)
		{
			Player.ArmorPoints = (byte)el;
		}, null);
	}

	public void Reset()
	{
		int armorPoints = 0;
		Singleton<LoadoutManager>.Instance.GetArmorValues(out armorPoints);
		Player.ArmorPointCapacity = (byte)armorPoints;
		Player.PlayerState = PlayerStates.None;
		KeyState = KeyState.Still;
		MovementState = MoveStates.None;
		Velocity = Vector3.zero;
		Health.Value = 100;
		ArmorPoints.Value = armorPoints;
	}

	public void InitializePlayer()
	{
		Reset();
	}

	public void SwitchWeaponSlot(int slot)
	{
		Actions.SwitchWeapon((byte)slot);
		Player.CurrentWeaponSlot = (byte)slot;
	}

	public void SetPing(int ping)
	{
		if (Player.Ping != ping)
		{
			Player.Ping = (ushort)ping;
			Actions.UpdatePing(Player.Ping);
		}
	}

	public float GetAbsorptionRate()
	{
		return 0.66f;
	}

	public void GetArmorDamage(short damage, BodyPart part, out short healthDamage, out byte armorDamage)
	{
		if ((int)ArmorPoints > 0)
		{
			int value = Mathf.CeilToInt(GetAbsorptionRate() * (float)damage);
			armorDamage = (byte)Mathf.Clamp(value, 0, ArmorPoints);
			healthDamage = (short)(damage - armorDamage);
		}
		else
		{
			armorDamage = 0;
			healthDamage = damage;
		}
	}

	public void Set(MoveStates state, bool on)
	{
		if (on)
		{
			MovementState |= state;
		}
		else
		{
			MovementState &= (MoveStates)(byte)(~(int)state);
		}
	}

	public void Set(PlayerStates state, bool on)
	{
		if (on)
		{
			Player.PlayerState |= state;
			switch (state)
			{
			case PlayerStates.Paused:
				Actions.PausePlayer(true);
				break;
			case PlayerStates.Sniping:
				Actions.SniperMode(true);
				break;
			case PlayerStates.Shooting:
				Actions.AutomaticFire(true);
				break;
			}
		}
		else
		{
			Player.PlayerState &= (PlayerStates)(byte)(~(int)state);
			switch (state)
			{
			case PlayerStates.Paused:
				Actions.PausePlayer(false);
				break;
			case PlayerStates.Sniping:
				Actions.SniperMode(false);
				break;
			case PlayerStates.Shooting:
				Actions.AutomaticFire(false);
				break;
			}
		}
	}

	public void Set(KeyState state, bool on)
	{
		if (on && !Is(state))
		{
			KeyState |= state;
			Actions.UpdateKeyState((byte)KeyState);
		}
		else if (!on && Is(state))
		{
			KeyState = (KeyState)((uint)KeyState & (uint)(byte)(~(int)state));
			Actions.UpdateKeyState((byte)KeyState);
		}
	}

	public void ResetKeys()
	{
		if (KeyState != KeyState.Still)
		{
			KeyState = KeyState.Still;
			Actions.UpdateKeyState((byte)KeyState);
		}
	}

	public bool Is(MoveStates state)
	{
		return (MovementState & state) != 0;
	}

	public bool Is(PlayerStates state)
	{
		return (PlayerState & state) != 0;
	}

	public bool Is(KeyState state)
	{
		return (KeyState & state) != 0;
	}

	public void DeltaUpdate(GameActorInfoDelta delta)
	{
		foreach (KeyValuePair<GameActorInfoDelta.Keys, object> change in delta.Changes)
		{
			switch (change.Key)
			{
			case GameActorInfoDelta.Keys.Health:
				Health.Value = (short)change.Value;
				break;
			case GameActorInfoDelta.Keys.PlayerState:
				Player.PlayerState = (PlayerStates)(byte)change.Value;
				break;
			case GameActorInfoDelta.Keys.ArmorPoints:
				ArmorPoints.Value = (byte)change.Value;
				break;
			}
		}
		delta.Apply(Player);
		this.OnDeltaUpdate(delta);
	}

	public void PositionUpdate(PlayerMovement update, ushort gameFrame)
	{
	}

	public void LandingUpdate()
	{
		posSyncFrame = Time.realtimeSinceStartup + 0.05f;
		byte arg = (byte)(MovementState | MoveStates.Landed | MoveStates.Grounded);
		this.SendMovementUpdate(GameState.Current.Player.transform.position, GameState.Current.Player.MoveController.Velocity, Conversion.Angle2Byte(HorizontalRotation.eulerAngles.y), Conversion.Angle2Byte(VerticalRotation), arg);
	}

	public void JumpingUpdate()
	{
		Set(MoveStates.Jumping, true);
		if (GameState.Current.Player != null)
		{
			this.SendJumpUpdate(GameState.Current.Player.transform.position);
		}
	}

	public void SendUpdates()
	{
		if (GameState.Current.IsInGame && IsAlive && posSyncFrame <= Time.realtimeSinceStartup && this.SendMovementUpdate != null)
		{
			posSyncFrame = Time.realtimeSinceStartup + 0.05f;
			if (cache.Update(GameState.Current.Player.transform.position, Conversion.Angle2Byte(HorizontalRotation.eulerAngles.y), Conversion.Angle2Byte(VerticalRotation), (byte)MovementState))
			{
				this.SendMovementUpdate(cache.Position, GameState.Current.Player.MoveController.Velocity, cache.HRotation, cache.VRotation, cache.MovementState);
				Singleton<GameStateController>.Instance.Client.Peer.SendOutgoingCommands();
			}
		}
	}
}
