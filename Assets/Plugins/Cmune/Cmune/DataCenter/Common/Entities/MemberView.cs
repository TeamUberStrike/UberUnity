using System;
using System.Collections.Generic;
using System.Text;

namespace Cmune.DataCenter.Common.Entities
{
	[Serializable]
	public class MemberView
	{
		public PublicProfileView PublicProfile { get; set; }

		public MemberWalletView MemberWallet { get; set; }

		public List<int> MemberItems { get; set; }

		public MemberView()
		{
			PublicProfile = new PublicProfileView();
			MemberWallet = new MemberWalletView();
			MemberItems = new List<int>(0);
		}

		public MemberView(PublicProfileView publicProfile, MemberWalletView memberWallet, List<int> memberItems)
		{
			PublicProfile = publicProfile;
			MemberWallet = memberWallet;
			MemberItems = memberItems;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder("[Member view: ");
			if (PublicProfile != null && MemberWallet != null)
			{
				stringBuilder.Append(PublicProfile);
				stringBuilder.Append(MemberWallet);
				stringBuilder.Append("[items: ");
				if (MemberItems != null && MemberItems.Count > 0)
				{
					int num = MemberItems.Count;
					foreach (int memberItem in MemberItems)
					{
						stringBuilder.Append(memberItem);
						if (--num > 0)
						{
							stringBuilder.Append(", ");
						}
					}
				}
				else
				{
					stringBuilder.Append("No items");
				}
				stringBuilder.Append("]");
			}
			else
			{
				stringBuilder.Append("No member");
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}
	}
}
