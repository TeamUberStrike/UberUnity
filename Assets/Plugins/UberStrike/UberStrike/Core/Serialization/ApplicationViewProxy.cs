using System;
using System.IO;
using Cmune.Core.Models.Views;
using Cmune.DataCenter.Common.Entities;

namespace UberStrike.Core.Serialization
{
	public static class ApplicationViewProxy
	{
		public static void Serialize(Stream stream, ApplicationView instance)
		{
			int num = 0;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Int32Proxy.Serialize(memoryStream, instance.ApplicationVersionId);
				EnumProxy<BuildType>.Serialize(memoryStream, instance.Build);
				EnumProxy<ChannelType>.Serialize(memoryStream, instance.Channel);
				if (instance.ExpirationDate.HasValue)
				{
					DateTime? expirationDate = instance.ExpirationDate;
					DateTimeProxy.Serialize(memoryStream, (!expirationDate.HasValue) ? default(DateTime) : expirationDate.Value);
				}
				else
				{
					num |= 1;
				}
				if (instance.FileName != null)
				{
					StringProxy.Serialize(memoryStream, instance.FileName);
				}
				else
				{
					num |= 2;
				}
				BooleanProxy.Serialize(memoryStream, instance.IsCurrent);
				Int32Proxy.Serialize(memoryStream, instance.PhotonGroupId);
				if (instance.PhotonGroupName != null)
				{
					StringProxy.Serialize(memoryStream, instance.PhotonGroupName);
				}
				else
				{
					num |= 4;
				}
				DateTimeProxy.Serialize(memoryStream, instance.ReleaseDate);
				Int32Proxy.Serialize(memoryStream, instance.RemainingTime);
				if (instance.Servers != null)
				{
					ListProxy<PhotonView>.Serialize(memoryStream, instance.Servers, PhotonViewProxy.Serialize);
				}
				else
				{
					num |= 8;
				}
				if (instance.SupportUrl != null)
				{
					StringProxy.Serialize(memoryStream, instance.SupportUrl);
				}
				else
				{
					num |= 0x10;
				}
				if (instance.Version != null)
				{
					StringProxy.Serialize(memoryStream, instance.Version);
				}
				else
				{
					num |= 0x20;
				}
				Int32Proxy.Serialize(stream, ~num);
				memoryStream.WriteTo(stream);
			}
		}

		public static ApplicationView Deserialize(Stream bytes)
		{
			int num = Int32Proxy.Deserialize(bytes);
			ApplicationView applicationView = new ApplicationView();
			applicationView.ApplicationVersionId = Int32Proxy.Deserialize(bytes);
			applicationView.Build = EnumProxy<BuildType>.Deserialize(bytes);
			applicationView.Channel = EnumProxy<ChannelType>.Deserialize(bytes);
			if ((num & 1) != 0)
			{
				applicationView.ExpirationDate = DateTimeProxy.Deserialize(bytes);
			}
			if ((num & 2) != 0)
			{
				applicationView.FileName = StringProxy.Deserialize(bytes);
			}
			applicationView.IsCurrent = BooleanProxy.Deserialize(bytes);
			applicationView.PhotonGroupId = Int32Proxy.Deserialize(bytes);
			if ((num & 4) != 0)
			{
				applicationView.PhotonGroupName = StringProxy.Deserialize(bytes);
			}
			applicationView.ReleaseDate = DateTimeProxy.Deserialize(bytes);
			applicationView.RemainingTime = Int32Proxy.Deserialize(bytes);
			if ((num & 8) != 0)
			{
				applicationView.Servers = ListProxy<PhotonView>.Deserialize(bytes, PhotonViewProxy.Deserialize);
			}
			if ((num & 0x10) != 0)
			{
				applicationView.SupportUrl = StringProxy.Deserialize(bytes);
			}
			if ((num & 0x20) != 0)
			{
				applicationView.Version = StringProxy.Deserialize(bytes);
			}
			return applicationView;
		}
	}
}
