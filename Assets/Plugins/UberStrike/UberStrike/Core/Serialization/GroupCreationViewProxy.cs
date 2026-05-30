using System.IO;
using Cmune.DataCenter.Common.Entities;

namespace UberStrike.Core.Serialization
{
	public static class GroupCreationViewProxy
	{
		public static void Serialize(Stream stream, GroupCreationView instance)
		{
			int num = 0;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				if (instance.Address != null)
				{
					StringProxy.Serialize(memoryStream, instance.Address);
				}
				else
				{
					num |= 1;
				}
				Int32Proxy.Serialize(memoryStream, instance.ApplicationId);
				if (instance.AuthToken != null)
				{
					StringProxy.Serialize(memoryStream, instance.AuthToken);
				}
				else
				{
					num |= 2;
				}
				if (instance.Description != null)
				{
					StringProxy.Serialize(memoryStream, instance.Description);
				}
				else
				{
					num |= 4;
				}
				BooleanProxy.Serialize(memoryStream, instance.HasPicture);
				if (instance.Locale != null)
				{
					StringProxy.Serialize(memoryStream, instance.Locale);
				}
				else
				{
					num |= 8;
				}
				if (instance.Motto != null)
				{
					StringProxy.Serialize(memoryStream, instance.Motto);
				}
				else
				{
					num |= 0x10;
				}
				if (instance.Name != null)
				{
					StringProxy.Serialize(memoryStream, instance.Name);
				}
				else
				{
					num |= 0x20;
				}
				if (instance.Tag != null)
				{
					StringProxy.Serialize(memoryStream, instance.Tag);
				}
				else
				{
					num |= 0x40;
				}
				Int32Proxy.Serialize(stream, ~num);
				memoryStream.WriteTo(stream);
			}
		}

		public static GroupCreationView Deserialize(Stream bytes)
		{
			int num = Int32Proxy.Deserialize(bytes);
			GroupCreationView groupCreationView = new GroupCreationView();
			if ((num & 1) != 0)
			{
				groupCreationView.Address = StringProxy.Deserialize(bytes);
			}
			groupCreationView.ApplicationId = Int32Proxy.Deserialize(bytes);
			if ((num & 2) != 0)
			{
				groupCreationView.AuthToken = StringProxy.Deserialize(bytes);
			}
			if ((num & 4) != 0)
			{
				groupCreationView.Description = StringProxy.Deserialize(bytes);
			}
			groupCreationView.HasPicture = BooleanProxy.Deserialize(bytes);
			if ((num & 8) != 0)
			{
				groupCreationView.Locale = StringProxy.Deserialize(bytes);
			}
			if ((num & 0x10) != 0)
			{
				groupCreationView.Motto = StringProxy.Deserialize(bytes);
			}
			if ((num & 0x20) != 0)
			{
				groupCreationView.Name = StringProxy.Deserialize(bytes);
			}
			if ((num & 0x40) != 0)
			{
				groupCreationView.Tag = StringProxy.Deserialize(bytes);
			}
			return groupCreationView;
		}
	}
}
