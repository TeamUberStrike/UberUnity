namespace Cmune.DataCenter.Common.Entities
{
	public enum ModerationActionType
	{
		AccountPermanentBan = 0,
		AccountTemporaryBan = 1,
		ChatPermanentBan = 2,
		ChatTemporaryBan = 3,
		Warning = 4,
		Note = 5,
		AccountNameChange = 6,
		InvalidNameChange = 7,
		ItemExchange = 8,
		Refund = 9,
		RescueFromAccountStealing = 10,
		IpBan = 11,
		AccountEmailChange = 12
	}
}
