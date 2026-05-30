using System;

namespace Cmune.DataCenter.Common.Entities
{
	[Serializable]
	public class MemberPositionUpdateView
	{
		public int GroupId { get; set; }

		public string AuthToken { get; set; }

		public int MemberCmid { get; set; }

		public GroupPosition Position { get; set; }

		public MemberPositionUpdateView()
		{
		}

		public MemberPositionUpdateView(int groupId, string authToken, int memberCmid, GroupPosition position)
		{
			GroupId = groupId;
			AuthToken = authToken;
			MemberCmid = memberCmid;
			Position = position;
		}

		public override string ToString()
		{
			string text = "[MemberPositionUpdateView: [GroupId:" + GroupId + "][AuthToken:" + AuthToken + "][MemberCmid:" + MemberCmid;
			string text2 = text;
			return string.Concat(text2, "][Position:", Position, "]]");
		}
	}
}
