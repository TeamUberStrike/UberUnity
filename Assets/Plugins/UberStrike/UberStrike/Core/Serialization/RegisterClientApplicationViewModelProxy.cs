using System.IO;
using Cmune.DataCenter.Common.Entities;
using UberStrike.Core.ViewModel;

namespace UberStrike.Core.Serialization
{
	public static class RegisterClientApplicationViewModelProxy
	{
		public static void Serialize(Stream stream, RegisterClientApplicationViewModel instance)
		{
			int num = 0;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				if (instance.ItemsAttributed != null)
				{
					ListProxy<int>.Serialize(memoryStream, instance.ItemsAttributed, Int32Proxy.Serialize);
				}
				else
				{
					num |= 1;
				}
				EnumProxy<ApplicationRegistrationResult>.Serialize(memoryStream, instance.Result);
				Int32Proxy.Serialize(stream, ~num);
				memoryStream.WriteTo(stream);
			}
		}

		public static RegisterClientApplicationViewModel Deserialize(Stream bytes)
		{
			int num = Int32Proxy.Deserialize(bytes);
			RegisterClientApplicationViewModel registerClientApplicationViewModel = new RegisterClientApplicationViewModel();
			if ((num & 1) != 0)
			{
				registerClientApplicationViewModel.ItemsAttributed = ListProxy<int>.Deserialize(bytes, Int32Proxy.Deserialize);
			}
			registerClientApplicationViewModel.Result = EnumProxy<ApplicationRegistrationResult>.Deserialize(bytes);
			return registerClientApplicationViewModel;
		}
	}
}
