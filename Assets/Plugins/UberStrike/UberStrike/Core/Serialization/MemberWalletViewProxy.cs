using System.IO;
using Cmune.DataCenter.Common.Entities;

namespace UberStrike.Core.Serialization
{
	public static class MemberWalletViewProxy
	{
		public static void Serialize(Stream stream, MemberWalletView instance)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Int32Proxy.Serialize(memoryStream, instance.Cmid);
				Int32Proxy.Serialize(memoryStream, instance.Credits);
				DateTimeProxy.Serialize(memoryStream, instance.CreditsExpiration);
				Int32Proxy.Serialize(memoryStream, instance.Points);
				DateTimeProxy.Serialize(memoryStream, instance.PointsExpiration);
				memoryStream.WriteTo(stream);
			}
		}

		public static MemberWalletView Deserialize(Stream bytes)
		{
			MemberWalletView memberWalletView = new MemberWalletView();
			memberWalletView.Cmid = Int32Proxy.Deserialize(bytes);
			memberWalletView.Credits = Int32Proxy.Deserialize(bytes);
			memberWalletView.CreditsExpiration = DateTimeProxy.Deserialize(bytes);
			memberWalletView.Points = Int32Proxy.Deserialize(bytes);
			memberWalletView.PointsExpiration = DateTimeProxy.Deserialize(bytes);
			return memberWalletView;
		}
	}
}
