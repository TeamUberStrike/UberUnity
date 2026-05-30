using System;

namespace Cmune.DataCenter.Common.Entities
{
	[Serializable]
	public class MemberWalletView
	{
		public int Cmid { get; set; }

		public int Credits { get; set; }

		public int Points { get; set; }

		public DateTime CreditsExpiration { get; set; }

		public DateTime PointsExpiration { get; set; }

		public MemberWalletView()
		{
			CreditsExpiration = DateTime.Today;
			PointsExpiration = DateTime.Today;
		}

		public MemberWalletView(int cmid, int? credits, int? points, DateTime? creditsExpiration, DateTime? pointsExpiration)
		{
			if (!credits.HasValue)
			{
				credits = 0;
			}
			if (!points.HasValue)
			{
				points = 0;
			}
			if (!creditsExpiration.HasValue)
			{
				creditsExpiration = DateTime.MinValue;
			}
			if (!pointsExpiration.HasValue)
			{
				pointsExpiration = DateTime.MinValue;
			}
			SetMemberWallet(cmid, credits.Value, points.Value, creditsExpiration.Value, pointsExpiration.Value);
		}

		public MemberWalletView(int cmid, int credits, int points, DateTime creditsExpiration, DateTime pointsExpiration)
		{
			SetMemberWallet(cmid, credits, points, creditsExpiration, pointsExpiration);
		}

		private void SetMemberWallet(int cmid, int credits, int points, DateTime creditsExpiration, DateTime pointsExpiration)
		{
			Cmid = cmid;
			Credits = credits;
			Points = points;
			CreditsExpiration = creditsExpiration;
			PointsExpiration = pointsExpiration;
		}

		public override string ToString()
		{
			string text = "[Wallet: ";
			string text2 = text;
			text = string.Concat(text2, "[CMID:", Cmid, "][Credits:", Credits, "][Credits Expiration:", CreditsExpiration, "][Points:", Points, "][Points Expiration:", PointsExpiration, "]");
			return text + "]";
		}
	}
}
