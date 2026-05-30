namespace Cmune.DataCenter.Common.Entities
{
	public enum MemberRegistrationResult
	{
		Ok = 0,
		InvalidEmail = 1,
		InvalidName = 2,
		InvalidPassword = 3,
		DuplicateEmail = 4,
		DuplicateName = 5,
		DuplicateEmailName = 6,
		InvalidData = 7,
		InvalidHandle = 8,
		DuplicateHandle = 9,
		InvalidEsns = 10,
		MemberNotFound = 11,
		OffensiveName = 12,
		IsIpBanned = 13,
		EmailAlreadyLinkedToActualEsns = 14,
		Error_MemberNotCreated = 15
	}
}
