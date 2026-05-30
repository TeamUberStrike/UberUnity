using System.Collections.Generic;
using System.IO;
using Cmune.DataCenter.Common.Entities;
using UberStrike.Core.Models;
using UnityEngine;

namespace UberStrike.Core.Serialization
{
	public static class GameActorInfoDeltaProxy
	{
		public static void Serialize(Stream stream, GameActorInfoDelta instance)
		{
			if (instance != null)
			{
				Int32Proxy.Serialize(stream, instance.DeltaMask);
				ByteProxy.Serialize(stream, instance.Id);
				if ((instance.DeltaMask & 1) != 0)
				{
					EnumProxy<MemberAccessLevel>.Serialize(stream, (MemberAccessLevel)(int)instance.Changes[GameActorInfoDelta.Keys.AccessLevel]);
				}
				if ((instance.DeltaMask & 2) != 0)
				{
					ByteProxy.Serialize(stream, (byte)instance.Changes[GameActorInfoDelta.Keys.ArmorPointCapacity]);
				}
				if ((instance.DeltaMask & 4) != 0)
				{
					ByteProxy.Serialize(stream, (byte)instance.Changes[GameActorInfoDelta.Keys.ArmorPoints]);
				}
				if ((instance.DeltaMask & 8) != 0)
				{
					EnumProxy<ChannelType>.Serialize(stream, (ChannelType)(int)instance.Changes[GameActorInfoDelta.Keys.Channel]);
				}
				if ((instance.DeltaMask & 0x10) != 0)
				{
					StringProxy.Serialize(stream, (string)instance.Changes[GameActorInfoDelta.Keys.ClanTag]);
				}
				if ((instance.DeltaMask & 0x20) != 0)
				{
					Int32Proxy.Serialize(stream, (int)instance.Changes[GameActorInfoDelta.Keys.Cmid]);
				}
				if ((instance.DeltaMask & 0x40) != 0)
				{
					EnumProxy<FireMode>.Serialize(stream, (FireMode)(int)instance.Changes[GameActorInfoDelta.Keys.CurrentFiringMode]);
				}
				if ((instance.DeltaMask & 0x80) != 0)
				{
					ByteProxy.Serialize(stream, (byte)instance.Changes[GameActorInfoDelta.Keys.CurrentWeaponSlot]);
				}
				if ((instance.DeltaMask & 0x100) != 0)
				{
					Int16Proxy.Serialize(stream, (short)instance.Changes[GameActorInfoDelta.Keys.Deaths]);
				}
				if ((instance.DeltaMask & 0x200) != 0)
				{
					ListProxy<int>.Serialize(stream, (List<int>)instance.Changes[GameActorInfoDelta.Keys.FunctionalItems], Int32Proxy.Serialize);
				}
				if ((instance.DeltaMask & 0x400) != 0)
				{
					ListProxy<int>.Serialize(stream, (List<int>)instance.Changes[GameActorInfoDelta.Keys.Gear], Int32Proxy.Serialize);
				}
				if ((instance.DeltaMask & 0x800) != 0)
				{
					Int16Proxy.Serialize(stream, (short)instance.Changes[GameActorInfoDelta.Keys.Health]);
				}
				if ((instance.DeltaMask & 0x1000) != 0)
				{
					Int16Proxy.Serialize(stream, (short)instance.Changes[GameActorInfoDelta.Keys.Kills]);
				}
				if ((instance.DeltaMask & 0x2000) != 0)
				{
					Int32Proxy.Serialize(stream, (int)instance.Changes[GameActorInfoDelta.Keys.Level]);
				}
				if ((instance.DeltaMask & 0x4000) != 0)
				{
					UInt16Proxy.Serialize(stream, (ushort)instance.Changes[GameActorInfoDelta.Keys.Ping]);
				}
				if ((instance.DeltaMask & 0x8000) != 0)
				{
					ByteProxy.Serialize(stream, (byte)instance.Changes[GameActorInfoDelta.Keys.PlayerId]);
				}
				if ((instance.DeltaMask & 0x10000) != 0)
				{
					StringProxy.Serialize(stream, (string)instance.Changes[GameActorInfoDelta.Keys.PlayerName]);
				}
				if ((instance.DeltaMask & 0x20000) != 0)
				{
					EnumProxy<PlayerStates>.Serialize(stream, (PlayerStates)(byte)instance.Changes[GameActorInfoDelta.Keys.PlayerState]);
				}
				if ((instance.DeltaMask & 0x40000) != 0)
				{
					ListProxy<int>.Serialize(stream, (List<int>)instance.Changes[GameActorInfoDelta.Keys.QuickItems], Int32Proxy.Serialize);
				}
				if ((instance.DeltaMask & 0x80000) != 0)
				{
					ByteProxy.Serialize(stream, (byte)instance.Changes[GameActorInfoDelta.Keys.Rank]);
				}
				if ((instance.DeltaMask & 0x100000) != 0)
				{
					ColorProxy.Serialize(stream, (Color)instance.Changes[GameActorInfoDelta.Keys.SkinColor]);
				}
				if ((instance.DeltaMask & 0x200000) != 0)
				{
					EnumProxy<SurfaceType>.Serialize(stream, (SurfaceType)(int)instance.Changes[GameActorInfoDelta.Keys.StepSound]);
				}
				if ((instance.DeltaMask & 0x400000) != 0)
				{
					EnumProxy<TeamID>.Serialize(stream, (TeamID)(int)instance.Changes[GameActorInfoDelta.Keys.TeamID]);
				}
				if ((instance.DeltaMask & 0x800000) != 0)
				{
					ListProxy<int>.Serialize(stream, (List<int>)instance.Changes[GameActorInfoDelta.Keys.Weapons], Int32Proxy.Serialize);
				}
			}
			else
			{
				Int32Proxy.Serialize(stream, 0);
			}
		}

		public static GameActorInfoDelta Deserialize(Stream bytes)
		{
			int num = Int32Proxy.Deserialize(bytes);
			byte id = ByteProxy.Deserialize(bytes);
			GameActorInfoDelta gameActorInfoDelta = new GameActorInfoDelta();
			gameActorInfoDelta.Id = id;
			if (num != 0)
			{
				if ((num & 1) != 0)
				{
					gameActorInfoDelta.Changes[GameActorInfoDelta.Keys.AccessLevel] = EnumProxy<MemberAccessLevel>.Deserialize(bytes);
				}
				if ((num & 2) != 0)
				{
					gameActorInfoDelta.Changes[GameActorInfoDelta.Keys.ArmorPointCapacity] = ByteProxy.Deserialize(bytes);
				}
				if ((num & 4) != 0)
				{
					gameActorInfoDelta.Changes[GameActorInfoDelta.Keys.ArmorPoints] = ByteProxy.Deserialize(bytes);
				}
				if ((num & 8) != 0)
				{
					gameActorInfoDelta.Changes[GameActorInfoDelta.Keys.Channel] = EnumProxy<ChannelType>.Deserialize(bytes);
				}
				if ((num & 0x10) != 0)
				{
					gameActorInfoDelta.Changes[GameActorInfoDelta.Keys.ClanTag] = StringProxy.Deserialize(bytes);
				}
				if ((num & 0x20) != 0)
				{
					gameActorInfoDelta.Changes[GameActorInfoDelta.Keys.Cmid] = Int32Proxy.Deserialize(bytes);
				}
				if ((num & 0x40) != 0)
				{
					gameActorInfoDelta.Changes[GameActorInfoDelta.Keys.CurrentFiringMode] = EnumProxy<FireMode>.Deserialize(bytes);
				}
				if ((num & 0x80) != 0)
				{
					gameActorInfoDelta.Changes[GameActorInfoDelta.Keys.CurrentWeaponSlot] = ByteProxy.Deserialize(bytes);
				}
				if ((num & 0x100) != 0)
				{
					gameActorInfoDelta.Changes[GameActorInfoDelta.Keys.Deaths] = Int16Proxy.Deserialize(bytes);
				}
				if ((num & 0x200) != 0)
				{
					gameActorInfoDelta.Changes[GameActorInfoDelta.Keys.FunctionalItems] = ListProxy<int>.Deserialize(bytes, Int32Proxy.Deserialize);
				}
				if ((num & 0x400) != 0)
				{
					gameActorInfoDelta.Changes[GameActorInfoDelta.Keys.Gear] = ListProxy<int>.Deserialize(bytes, Int32Proxy.Deserialize);
				}
				if ((num & 0x800) != 0)
				{
					gameActorInfoDelta.Changes[GameActorInfoDelta.Keys.Health] = Int16Proxy.Deserialize(bytes);
				}
				if ((num & 0x1000) != 0)
				{
					gameActorInfoDelta.Changes[GameActorInfoDelta.Keys.Kills] = Int16Proxy.Deserialize(bytes);
				}
				if ((num & 0x2000) != 0)
				{
					gameActorInfoDelta.Changes[GameActorInfoDelta.Keys.Level] = Int32Proxy.Deserialize(bytes);
				}
				if ((num & 0x4000) != 0)
				{
					gameActorInfoDelta.Changes[GameActorInfoDelta.Keys.Ping] = UInt16Proxy.Deserialize(bytes);
				}
				if ((num & 0x8000) != 0)
				{
					gameActorInfoDelta.Changes[GameActorInfoDelta.Keys.PlayerId] = ByteProxy.Deserialize(bytes);
				}
				if ((num & 0x10000) != 0)
				{
					gameActorInfoDelta.Changes[GameActorInfoDelta.Keys.PlayerName] = StringProxy.Deserialize(bytes);
				}
				if ((num & 0x20000) != 0)
				{
					gameActorInfoDelta.Changes[GameActorInfoDelta.Keys.PlayerState] = EnumProxy<PlayerStates>.Deserialize(bytes);
				}
				if ((num & 0x40000) != 0)
				{
					gameActorInfoDelta.Changes[GameActorInfoDelta.Keys.QuickItems] = ListProxy<int>.Deserialize(bytes, Int32Proxy.Deserialize);
				}
				if ((num & 0x80000) != 0)
				{
					gameActorInfoDelta.Changes[GameActorInfoDelta.Keys.Rank] = ByteProxy.Deserialize(bytes);
				}
				if ((num & 0x100000) != 0)
				{
					gameActorInfoDelta.Changes[GameActorInfoDelta.Keys.SkinColor] = ColorProxy.Deserialize(bytes);
				}
				if ((num & 0x200000) != 0)
				{
					gameActorInfoDelta.Changes[GameActorInfoDelta.Keys.StepSound] = EnumProxy<SurfaceType>.Deserialize(bytes);
				}
				if ((num & 0x400000) != 0)
				{
					gameActorInfoDelta.Changes[GameActorInfoDelta.Keys.TeamID] = EnumProxy<TeamID>.Deserialize(bytes);
				}
				if ((num & 0x800000) != 0)
				{
					gameActorInfoDelta.Changes[GameActorInfoDelta.Keys.Weapons] = ListProxy<int>.Deserialize(bytes, Int32Proxy.Deserialize);
				}
			}
			return gameActorInfoDelta;
		}
	}
}
