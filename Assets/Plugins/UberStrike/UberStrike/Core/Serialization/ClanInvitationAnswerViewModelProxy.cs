using System.IO;
using UberStrike.Core.ViewModel;

namespace UberStrike.Core.Serialization
{
	public static class ClanInvitationAnswerViewModelProxy
	{
		public static void Serialize(Stream stream, ClanInvitationAnswerViewModel instance)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Int32Proxy.Serialize(memoryStream, instance.GroupInvitationId);
				BooleanProxy.Serialize(memoryStream, instance.IsInvitationAccepted);
				Int32Proxy.Serialize(memoryStream, instance.ReturnValue);
				memoryStream.WriteTo(stream);
			}
		}

		public static ClanInvitationAnswerViewModel Deserialize(Stream bytes)
		{
			ClanInvitationAnswerViewModel clanInvitationAnswerViewModel = new ClanInvitationAnswerViewModel();
			clanInvitationAnswerViewModel.GroupInvitationId = Int32Proxy.Deserialize(bytes);
			clanInvitationAnswerViewModel.IsInvitationAccepted = BooleanProxy.Deserialize(bytes);
			clanInvitationAnswerViewModel.ReturnValue = Int32Proxy.Deserialize(bytes);
			return clanInvitationAnswerViewModel;
		}
	}
}
