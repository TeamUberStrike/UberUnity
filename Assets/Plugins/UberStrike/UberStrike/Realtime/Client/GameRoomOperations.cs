using System;
using System.Collections.Generic;
using System.IO;
using UberStrike.Core.Models;
using UberStrike.Core.Serialization;
using UberStrike.Core.Types;
using UnityEngine;

namespace UberStrike.Realtime.Client
{
	public sealed class GameRoomOperations : IOperationSender
	{
		private byte __id;

		private RemoteProcedureCall sendOperation;

		public event RemoteProcedureCall SendOperation
		{
			add
			{
				sendOperation = (RemoteProcedureCall)Delegate.Combine(sendOperation, value);
			}
			remove
			{
				sendOperation = (RemoteProcedureCall)Delegate.Remove(sendOperation, value);
			}
		}

		public GameRoomOperations(byte id = 0)
		{
			__id = id;
		}

		public void SendJoinGame(TeamID team)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				EnumProxy<TeamID>.Serialize(memoryStream, team);
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(__id, memoryStream.ToArray());
				Dictionary<byte, object> customOpParameters = dictionary;
				if (sendOperation != null)
				{
					sendOperation(1, customOpParameters);
				}
			}
		}

		public void SendJoinAsSpectator()
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(__id, memoryStream.ToArray());
				Dictionary<byte, object> customOpParameters = dictionary;
				if (sendOperation != null)
				{
					sendOperation(2, customOpParameters);
				}
			}
		}

		public void SendPowerUpRespawnTimes(List<ushort> respawnTimes)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				ListProxy<ushort>.Serialize(memoryStream, respawnTimes, UInt16Proxy.Serialize);
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(__id, memoryStream.ToArray());
				Dictionary<byte, object> customOpParameters = dictionary;
				if (sendOperation != null)
				{
					sendOperation(3, customOpParameters);
				}
			}
		}

		public void SendPowerUpPicked(int powerupId, byte type, byte value)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Int32Proxy.Serialize(memoryStream, powerupId);
				ByteProxy.Serialize(memoryStream, type);
				ByteProxy.Serialize(memoryStream, value);
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(__id, memoryStream.ToArray());
				Dictionary<byte, object> customOpParameters = dictionary;
				if (sendOperation != null)
				{
					sendOperation(4, customOpParameters);
				}
			}
		}

		public void SendIncreaseHealthAndArmor(byte health, byte armor)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				ByteProxy.Serialize(memoryStream, health);
				ByteProxy.Serialize(memoryStream, armor);
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(__id, memoryStream.ToArray());
				Dictionary<byte, object> customOpParameters = dictionary;
				if (sendOperation != null)
				{
					sendOperation(5, customOpParameters);
				}
			}
		}

		public void SendOpenDoor(int doorId)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Int32Proxy.Serialize(memoryStream, doorId);
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(__id, memoryStream.ToArray());
				Dictionary<byte, object> customOpParameters = dictionary;
				if (sendOperation != null)
				{
					sendOperation(6, customOpParameters);
				}
			}
		}

		public void SendSpawnPositions(TeamID team, List<Vector3> positions, List<byte> rotations)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				EnumProxy<TeamID>.Serialize(memoryStream, team);
				ListProxy<Vector3>.Serialize(memoryStream, positions, Vector3Proxy.Serialize);
				ListProxy<byte>.Serialize(memoryStream, rotations, ByteProxy.Serialize);
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(__id, memoryStream.ToArray());
				Dictionary<byte, object> customOpParameters = dictionary;
				if (sendOperation != null)
				{
					sendOperation(7, customOpParameters);
				}
			}
		}

		public void SendRespawnRequest()
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(__id, memoryStream.ToArray());
				Dictionary<byte, object> customOpParameters = dictionary;
				if (sendOperation != null)
				{
					sendOperation(8, customOpParameters);
				}
			}
		}

		public void SendDirectHitDamage(int target, byte bodyPart, byte bullets)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Int32Proxy.Serialize(memoryStream, target);
				ByteProxy.Serialize(memoryStream, bodyPart);
				ByteProxy.Serialize(memoryStream, bullets);
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(__id, memoryStream.ToArray());
				Dictionary<byte, object> customOpParameters = dictionary;
				if (sendOperation != null)
				{
					sendOperation(9, customOpParameters);
				}
			}
		}

		public void SendExplosionDamage(int target, byte slot, byte distance, Vector3 force)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Int32Proxy.Serialize(memoryStream, target);
				ByteProxy.Serialize(memoryStream, slot);
				ByteProxy.Serialize(memoryStream, distance);
				Vector3Proxy.Serialize(memoryStream, force);
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(__id, memoryStream.ToArray());
				Dictionary<byte, object> customOpParameters = dictionary;
				if (sendOperation != null)
				{
					sendOperation(10, customOpParameters);
				}
			}
		}

		public void SendDirectDamage(ushort damage)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				UInt16Proxy.Serialize(memoryStream, damage);
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(__id, memoryStream.ToArray());
				Dictionary<byte, object> customOpParameters = dictionary;
				if (sendOperation != null)
				{
					sendOperation(11, customOpParameters);
				}
			}
		}

		public void SendDirectDeath()
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(__id, memoryStream.ToArray());
				Dictionary<byte, object> customOpParameters = dictionary;
				if (sendOperation != null)
				{
					sendOperation(12, customOpParameters);
				}
			}
		}

		public void SendJump(Vector3 position)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Vector3Proxy.Serialize(memoryStream, position);
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(__id, memoryStream.ToArray());
				Dictionary<byte, object> customOpParameters = dictionary;
				if (sendOperation != null)
				{
					sendOperation(13, customOpParameters);
				}
			}
		}

		public void SendUpdatePositionAndRotation(ShortVector3 position, ShortVector3 velocity, byte hrot, byte vrot, byte moveState)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				ShortVector3Proxy.Serialize(memoryStream, position);
				ShortVector3Proxy.Serialize(memoryStream, velocity);
				ByteProxy.Serialize(memoryStream, hrot);
				ByteProxy.Serialize(memoryStream, vrot);
				ByteProxy.Serialize(memoryStream, moveState);
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(__id, memoryStream.ToArray());
				Dictionary<byte, object> customOpParameters = dictionary;
				if (sendOperation != null)
				{
					sendOperation(14, customOpParameters, false);
				}
			}
		}

		public void SendKickPlayer(int cmid)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Int32Proxy.Serialize(memoryStream, cmid);
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(__id, memoryStream.ToArray());
				Dictionary<byte, object> customOpParameters = dictionary;
				if (sendOperation != null)
				{
					sendOperation(15, customOpParameters);
				}
			}
		}

		public void SendIsFiring(bool on)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BooleanProxy.Serialize(memoryStream, on);
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(__id, memoryStream.ToArray());
				Dictionary<byte, object> customOpParameters = dictionary;
				if (sendOperation != null)
				{
					sendOperation(16, customOpParameters);
				}
			}
		}

		public void SendIsReadyForNextMatch(bool on)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BooleanProxy.Serialize(memoryStream, on);
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(__id, memoryStream.ToArray());
				Dictionary<byte, object> customOpParameters = dictionary;
				if (sendOperation != null)
				{
					sendOperation(17, customOpParameters);
				}
			}
		}

		public void SendIsPaused(bool on)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BooleanProxy.Serialize(memoryStream, on);
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(__id, memoryStream.ToArray());
				Dictionary<byte, object> customOpParameters = dictionary;
				if (sendOperation != null)
				{
					sendOperation(18, customOpParameters);
				}
			}
		}

		public void SendIsInSniperMode(bool on)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BooleanProxy.Serialize(memoryStream, on);
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(__id, memoryStream.ToArray());
				Dictionary<byte, object> customOpParameters = dictionary;
				if (sendOperation != null)
				{
					sendOperation(19, customOpParameters);
				}
			}
		}

		public void SendSingleBulletFire()
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(__id, memoryStream.ToArray());
				Dictionary<byte, object> customOpParameters = dictionary;
				if (sendOperation != null)
				{
					sendOperation(20, customOpParameters);
				}
			}
		}

		public void SendSwitchWeapon(byte weaponSlot)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				ByteProxy.Serialize(memoryStream, weaponSlot);
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(__id, memoryStream.ToArray());
				Dictionary<byte, object> customOpParameters = dictionary;
				if (sendOperation != null)
				{
					sendOperation(21, customOpParameters);
				}
			}
		}

		public void SendSwitchTeam()
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(__id, memoryStream.ToArray());
				Dictionary<byte, object> customOpParameters = dictionary;
				if (sendOperation != null)
				{
					sendOperation(22, customOpParameters);
				}
			}
		}

		public void SendChangeGear(int head, int face, int upperBody, int lowerBody, int gloves, int boots, int holo)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Int32Proxy.Serialize(memoryStream, head);
				Int32Proxy.Serialize(memoryStream, face);
				Int32Proxy.Serialize(memoryStream, upperBody);
				Int32Proxy.Serialize(memoryStream, lowerBody);
				Int32Proxy.Serialize(memoryStream, gloves);
				Int32Proxy.Serialize(memoryStream, boots);
				Int32Proxy.Serialize(memoryStream, holo);
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(__id, memoryStream.ToArray());
				Dictionary<byte, object> customOpParameters = dictionary;
				if (sendOperation != null)
				{
					sendOperation(23, customOpParameters);
				}
			}
		}

		public void SendEmitProjectile(Vector3 origin, Vector3 direction, byte slot, int projectileID, bool explode)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Vector3Proxy.Serialize(memoryStream, origin);
				Vector3Proxy.Serialize(memoryStream, direction);
				ByteProxy.Serialize(memoryStream, slot);
				Int32Proxy.Serialize(memoryStream, projectileID);
				BooleanProxy.Serialize(memoryStream, explode);
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(__id, memoryStream.ToArray());
				Dictionary<byte, object> customOpParameters = dictionary;
				if (sendOperation != null)
				{
					sendOperation(24, customOpParameters);
				}
			}
		}

		public void SendEmitQuickItem(Vector3 origin, Vector3 direction, int itemId, byte playerNumber, int projectileID)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Vector3Proxy.Serialize(memoryStream, origin);
				Vector3Proxy.Serialize(memoryStream, direction);
				Int32Proxy.Serialize(memoryStream, itemId);
				ByteProxy.Serialize(memoryStream, playerNumber);
				Int32Proxy.Serialize(memoryStream, projectileID);
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(__id, memoryStream.ToArray());
				Dictionary<byte, object> customOpParameters = dictionary;
				if (sendOperation != null)
				{
					sendOperation(25, customOpParameters);
				}
			}
		}

		public void SendRemoveProjectile(int projectileId, bool explode)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Int32Proxy.Serialize(memoryStream, projectileId);
				BooleanProxy.Serialize(memoryStream, explode);
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(__id, memoryStream.ToArray());
				Dictionary<byte, object> customOpParameters = dictionary;
				if (sendOperation != null)
				{
					sendOperation(26, customOpParameters);
				}
			}
		}

		public void SendHitFeedback(int targetCmid, Vector3 force)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Int32Proxy.Serialize(memoryStream, targetCmid);
				Vector3Proxy.Serialize(memoryStream, force);
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(__id, memoryStream.ToArray());
				Dictionary<byte, object> customOpParameters = dictionary;
				if (sendOperation != null)
				{
					sendOperation(27, customOpParameters);
				}
			}
		}

		public void SendActivateQuickItem(QuickItemLogic logic, int robotLifeTime, int scrapsLifeTime, bool isInstant)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				EnumProxy<QuickItemLogic>.Serialize(memoryStream, logic);
				Int32Proxy.Serialize(memoryStream, robotLifeTime);
				Int32Proxy.Serialize(memoryStream, scrapsLifeTime);
				BooleanProxy.Serialize(memoryStream, isInstant);
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(__id, memoryStream.ToArray());
				Dictionary<byte, object> customOpParameters = dictionary;
				if (sendOperation != null)
				{
					sendOperation(28, customOpParameters);
				}
			}
		}

		public void SendChatMessage(string message, byte context)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				StringProxy.Serialize(memoryStream, message);
				ByteProxy.Serialize(memoryStream, context);
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(__id, memoryStream.ToArray());
				Dictionary<byte, object> customOpParameters = dictionary;
				if (sendOperation != null)
				{
					sendOperation(29, customOpParameters);
				}
			}
		}
	}
}
