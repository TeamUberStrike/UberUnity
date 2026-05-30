using System.IO;
using UberStrike.Core.Models;

namespace UberStrike.Core.Serialization
{
	public static class PlayerMovementProxy
	{
		public static void Serialize(Stream stream, PlayerMovement instance)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				ByteProxy.Serialize(memoryStream, instance.HorizontalRotation);
				ByteProxy.Serialize(memoryStream, instance.KeyState);
				ByteProxy.Serialize(memoryStream, instance.MovementState);
				ByteProxy.Serialize(memoryStream, instance.Number);
				ShortVector3Proxy.Serialize(memoryStream, instance.Position);
				ShortVector3Proxy.Serialize(memoryStream, instance.Velocity);
				ByteProxy.Serialize(memoryStream, instance.VerticalRotation);
				memoryStream.WriteTo(stream);
			}
		}

		public static PlayerMovement Deserialize(Stream bytes)
		{
			PlayerMovement playerMovement = new PlayerMovement();
			playerMovement.HorizontalRotation = ByteProxy.Deserialize(bytes);
			playerMovement.KeyState = ByteProxy.Deserialize(bytes);
			playerMovement.MovementState = ByteProxy.Deserialize(bytes);
			playerMovement.Number = ByteProxy.Deserialize(bytes);
			playerMovement.Position = ShortVector3Proxy.Deserialize(bytes);
			playerMovement.Velocity = ShortVector3Proxy.Deserialize(bytes);
			playerMovement.VerticalRotation = ByteProxy.Deserialize(bytes);
			return playerMovement;
		}
	}
}
