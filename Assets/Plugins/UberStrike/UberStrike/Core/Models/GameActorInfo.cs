using System;
using System.Collections.Generic;
using Cmune.DataCenter.Common.Entities;
using UnityEngine;

namespace UberStrike.Core.Models
{
	[Serializable]
	[Synchronizable]
	public class GameActorInfo
	{
		public int Cmid { get; set; }

		public string PlayerName { get; set; }

		public MemberAccessLevel AccessLevel { get; set; }

		public ChannelType Channel { get; set; }

		public string ClanTag { get; set; }

		public byte Rank { get; set; }

		public byte PlayerId { get; set; }

		public PlayerStates PlayerState { get; set; }

		public short Health { get; set; }

		public TeamID TeamID { get; set; }

		public int Level { get; set; }

		public ushort Ping { get; set; }

		public byte CurrentWeaponSlot { get; set; }

		public FireMode CurrentFiringMode { get; set; }

		public byte ArmorPoints { get; set; }

		public byte ArmorPointCapacity { get; set; }

		public Color SkinColor { get; set; }

		public short Kills { get; set; }

		public short Deaths { get; set; }

		public List<int> Weapons { get; set; }

		public List<int> Gear { get; set; }

		public List<int> FunctionalItems { get; set; }

		public List<int> QuickItems { get; set; }

		public SurfaceType StepSound { get; set; }

		public bool IsFiring
		{
			get
			{
				return Is(PlayerStates.Shooting);
			}
		}

		public bool IsReadyForGame
		{
			get
			{
				return Is(PlayerStates.Ready);
			}
		}

		public bool IsOnline
		{
			get
			{
				return !Is(PlayerStates.Offline);
			}
		}

		public int CurrentWeaponID
		{
			get
			{
				return (Weapons != null && Weapons.Count > CurrentWeaponSlot) ? Weapons[CurrentWeaponSlot] : 0;
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

		public GameActorInfo()
		{
			Weapons = new List<int> { 0, 0, 0, 0 };
			Gear = new List<int> { 0, 0, 0, 0, 0, 0, 0 };
			QuickItems = new List<int> { 0, 0, 0 };
			FunctionalItems = new List<int> { 0, 0, 0 };
		}

		public bool Is(PlayerStates state)
		{
			return (PlayerState & state) != 0;
		}

		public float GetAbsorptionRate()
		{
			return 0.66f;
		}

		public void Damage(short damage, BodyPart part, out short healthDamage, out byte armorDamage)
		{
			if (ArmorPoints > 0)
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
	}
}
