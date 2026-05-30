using System;

namespace UberStrike.DataCenter.Common.Entities
{
	[Serializable]
	public class UberstrikeMemberView
	{
		public PlayerCardView PlayerCardView { get; set; }

		public PlayerStatisticsView PlayerStatisticsView { get; set; }

		public UberstrikeMemberView()
		{
		}

		public UberstrikeMemberView(PlayerCardView playerCardView, PlayerStatisticsView playerStatisticsView)
		{
			PlayerCardView = playerCardView;
			PlayerStatisticsView = playerStatisticsView;
		}

		public override string ToString()
		{
			string text = "[Uberstrike member view: ";
			text = ((PlayerCardView == null) ? (text + "null") : (text + PlayerCardView.ToString()));
			text = ((PlayerStatisticsView == null) ? (text + "null") : (text + PlayerStatisticsView.ToString()));
			return text + "]";
		}
	}
}
