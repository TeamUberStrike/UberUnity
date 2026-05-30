using System.Collections.Generic;
using System.Linq;

namespace UberStrike.DataCenter.Common.Entities
{
	public class FacebookBasicStatisticView : EsnsBasicStatisticView
	{
		private string _picturePath;

		public long FacebookId { get; set; }

		public string FirstName { get; set; }

		public string PicturePath
		{
			get
			{
				return _picturePath;
			}
			set
			{
				if (value.StartsWith("http:"))
				{
					value = value.Replace("http:", "https:");
				}
				_picturePath = value;
			}
		}

		public FacebookBasicStatisticView(long facebookId, string firstName, string picturePath, string name, int xp, int level, int cmid)
			: base(name, xp, level, cmid)
		{
			FacebookId = facebookId;
			FirstName = firstName;
			PicturePath = picturePath;
		}

		public FacebookBasicStatisticView(long facebookId, string firstName, string picturePath)
		{
			FacebookId = facebookId;
			FirstName = firstName;
			PicturePath = picturePath;
		}

		public FacebookBasicStatisticView()
		{
			FacebookId = 0L;
		}

		public static List<FacebookBasicStatisticView> Rank(List<FacebookBasicStatisticView> views, int friendsDisplayedCount)
		{
			List<FacebookBasicStatisticView> list = new List<FacebookBasicStatisticView>();
			FacebookBasicStatisticView facebookBasicStatisticView = null;
			if (views.Count > 0)
			{
				facebookBasicStatisticView = views[0];
			}
			views = views.OrderByDescending((FacebookBasicStatisticView v) => v.XP).ToList();
			int num = 1;
			foreach (FacebookBasicStatisticView view in views)
			{
				if (view.Cmid != 0)
				{
					view.SocialRank = num;
					num++;
				}
			}
			list.Add(facebookBasicStatisticView);
			num = 0;
			for (int num2 = 0; num2 < friendsDisplayedCount && num2 < views.Count; num2++)
			{
				if (views[num2].FacebookId != facebookBasicStatisticView.FacebookId)
				{
					list.Add(views[num2]);
					num++;
				}
			}
			while (list.Count < friendsDisplayedCount + 1)
			{
				list.Add(new FacebookBasicStatisticView());
			}
			return list;
		}
	}
}
