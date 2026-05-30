namespace Cmune.DataCenter.Common.Entities
{
	public class ConvertEntities
	{
		public static MemberOperationResult ConvertMemberRegistration(MemberRegistrationResult memberRegistration)
		{
			MemberOperationResult result = MemberOperationResult.Ok;
			switch (memberRegistration)
			{
			case MemberRegistrationResult.InvalidHandle:
				result = MemberOperationResult.InvalidHandle;
				break;
			case MemberRegistrationResult.InvalidName:
				result = MemberOperationResult.InvalidName;
				break;
			case MemberRegistrationResult.InvalidEsns:
				result = MemberOperationResult.InvalidEsns;
				break;
			case MemberRegistrationResult.DuplicateName:
				result = MemberOperationResult.DuplicateName;
				break;
			case MemberRegistrationResult.DuplicateHandle:
				result = MemberOperationResult.DuplicateHandle;
				break;
			case MemberRegistrationResult.Ok:
				result = MemberOperationResult.Ok;
				break;
			}
			return result;
		}

		public static MemberRegistrationResult ConvertMemberOperation(MemberOperationResult memberOperation)
		{
			MemberRegistrationResult result = MemberRegistrationResult.Ok;
			switch (memberOperation)
			{
			case MemberOperationResult.DuplicateEmail:
				result = MemberRegistrationResult.DuplicateEmail;
				break;
			case MemberOperationResult.DuplicateEmailName:
				result = MemberRegistrationResult.DuplicateEmailName;
				break;
			case MemberOperationResult.DuplicateName:
				result = MemberRegistrationResult.DuplicateName;
				break;
			case MemberOperationResult.Ok:
				result = MemberRegistrationResult.Ok;
				break;
			case MemberOperationResult.InvalidName:
				result = MemberRegistrationResult.InvalidName;
				break;
			case MemberOperationResult.OffensiveName:
				result = MemberRegistrationResult.OffensiveName;
				break;
			}
			return result;
		}
	}
}
