using System;

namespace Cmune.DataCenter.Common.Entities
{
	[Serializable]
	public enum MemberOperationResult
	{
		Ok = 0,
		DuplicateEmail = 2,
		DuplicateName = 3,
		DuplicateHandle = 4,
		DuplicateEmailName = 5,
		MemberNotFound = 6,
		InvalidData = 9,
		InvalidHandle = 10,
		InvalidEsns = 11,
		InvalidCmid = 12,
		InvalidName = 13,
		InvalidEmail = 14,
		InvalidPassword = 15,
		OffensiveName = 16,
		NameChangeNotInInventory = 17,
		AlreadyHasAnESNSAccountOfThisTypeAttached = 18
	}
}
