using System.IO;
using Cmune.Core.Models;

namespace UberStrike.Core.Serialization
{
	public static class PhotonServerLoadProxy
	{
		public static void Serialize(Stream stream, PhotonServerLoad instance)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				SingleProxy.Serialize(memoryStream, instance.MaxPlayerCount);
				Int32Proxy.Serialize(memoryStream, instance.PeersConnected);
				Int32Proxy.Serialize(memoryStream, instance.PlayersConnected);
				Int32Proxy.Serialize(memoryStream, instance.RoomsCreated);
				memoryStream.WriteTo(stream);
			}
		}

		public static PhotonServerLoad Deserialize(Stream bytes)
		{
			PhotonServerLoad photonServerLoad = new PhotonServerLoad();
			photonServerLoad.MaxPlayerCount = SingleProxy.Deserialize(bytes);
			photonServerLoad.PeersConnected = Int32Proxy.Deserialize(bytes);
			photonServerLoad.PlayersConnected = Int32Proxy.Deserialize(bytes);
			photonServerLoad.RoomsCreated = Int32Proxy.Deserialize(bytes);
			return photonServerLoad;
		}
	}
}
