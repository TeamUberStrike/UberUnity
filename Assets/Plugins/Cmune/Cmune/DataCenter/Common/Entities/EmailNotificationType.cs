namespace Cmune.DataCenter.Common.Entities
{
	public enum EmailNotificationType
	{
		DeleteMember = 1,
		BanMemberPermanent = 2,
		MergeMembers = 3,
		ChangeMemberName = 8,
		ChangeMemberPassword = 9,
		ChangeMemberEmail = 11,
		BanMemberTemporary = 12,
		UnbanMember = 13,
		BanMemberChatPermanent = 14,
		BanMemberChatTemporary = 15,
		UnbanMemberChat = 16,
		ChangeClanTag = 17,
		ChangeClanName = 18,
		ChangeClanMotto = 19
	}
}
