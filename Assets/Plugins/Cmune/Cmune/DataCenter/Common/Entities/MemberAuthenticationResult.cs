namespace Cmune.DataCenter.Common.Entities
{
	public enum MemberAuthenticationResult
	{
		Ok = 0,
		InvalidData = 1,
		InvalidName = 2,
		InvalidEmail = 3,
		InvalidPassword = 4,
		IsBanned = 5,
		InvalidHandle = 6,
		InvalidEsns = 7,
		InvalidCookie = 8,
		IsIpBanned = 9,
		UnknownError = 10
	}
}
