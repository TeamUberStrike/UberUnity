using System.IO;
using Cmune.DataCenter.Common.Entities;

namespace UberStrike.Core.Serialization
{
	public static class ClanRequestDeclineViewProxy
	{
		public static void Serialize(Stream stream, ClanRequestDeclineView instance)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Int32Proxy.Serialize(memoryStream, instance.ActionResult);
				Int32Proxy.Serialize(memoryStream, instance.ClanRequestId);
				memoryStream.WriteTo(stream);
			}
		}

		public static ClanRequestDeclineView Deserialize(Stream bytes)
		{
			ClanRequestDeclineView clanRequestDeclineView = new ClanRequestDeclineView();
			clanRequestDeclineView.ActionResult = Int32Proxy.Deserialize(bytes);
			clanRequestDeclineView.ClanRequestId = Int32Proxy.Deserialize(bytes);
			return clanRequestDeclineView;
		}
	}
}
