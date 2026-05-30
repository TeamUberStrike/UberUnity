using System.Text.RegularExpressions;
using UberStrike.Realtime.UnitySdk;

public static class ValidationUtilities
{
	public static bool IsValidEmailAddress(string email)
	{
		if (TextUtilities.IsNullOrEmpty(email) || email.Length > 100)
		{
			return false;
		}
		int num = email.IndexOf('@');
		int num2 = email.LastIndexOf('@');
		if (num > 0 && num2 == num && num < email.Length - 1)
		{
			return Regex.IsMatch(email, "^([a-zA-Z0-9_'+*$%\\^&!\\.\\-])+\\@(([a-zA-Z0-9\\-])+\\.)+([a-zA-Z0-9:]{2,4})+$");
		}
		return false;
	}

	public static bool IsValidPassword(string password)
	{
		bool result = false;
		if (!TextUtilities.IsNullOrEmpty(password) && password.Length > 3 && password.Length < 64)
		{
			result = true;
		}
		return result;
	}

	public static bool IsValidMemberName(string memberName)
	{
		return IsValidMemberName(memberName, "en-US");
	}

	public static bool IsValidMemberName(string memberName, string locale)
	{
		bool result = false;
		if (!string.IsNullOrEmpty(memberName))
		{
			memberName = memberName.Trim();
			if (memberName.Equals(TextUtilities.CompleteTrim(memberName)))
			{
				string empty = string.Empty;
				switch (locale)
				{
				case "ko-KR":
					empty = "\\p{IsHangulSyllables}";
					break;
				default:
					empty = string.Empty;
					break;
				}
				result = Regex.IsMatch(memberName, "^[a-zA-Z0-9 .!_\\-<>{}~@#$%^&*()=+|:?" + empty + "]{" + 3 + "," + 18 + "}$");
			}
			if (!memberName.ToLower().IndexOf("admin").Equals(-1) || !memberName.ToLower().IndexOf("cmune").Equals(-1))
			{
				result = false;
			}
		}
		return result;
	}
}
