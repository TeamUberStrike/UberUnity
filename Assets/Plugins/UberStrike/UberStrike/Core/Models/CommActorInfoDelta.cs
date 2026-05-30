using System.Collections.Generic;
using Cmune.DataCenter.Common.Entities;

namespace UberStrike.Core.Models
{
	public class CommActorInfoDelta
	{
		public enum Keys
		{
			AccessLevel = 0,
			Channel = 1,
			ClanTag = 2,
			Cmid = 3,
			CurrentRoom = 4,
			ModerationFlag = 5,
			ModInformation = 6,
			PlayerName = 7
		}

		public readonly Dictionary<Keys, object> Changes = new Dictionary<Keys, object>();

		public int DeltaMask { get; set; }

		public byte Id { get; set; }

		public void Apply(CommActorInfo instance)
		{
			foreach (KeyValuePair<Keys, object> change in Changes)
			{
				switch (change.Key)
				{
				case Keys.AccessLevel:
					instance.AccessLevel = (MemberAccessLevel)(int)change.Value;
					break;
				case Keys.Channel:
					instance.Channel = (ChannelType)(int)change.Value;
					break;
				case Keys.ClanTag:
					instance.ClanTag = (string)change.Value;
					break;
				case Keys.Cmid:
					instance.Cmid = (int)change.Value;
					break;
				case Keys.CurrentRoom:
					instance.CurrentRoom = (GameRoom)change.Value;
					break;
				case Keys.ModerationFlag:
					instance.ModerationFlag = (byte)change.Value;
					break;
				case Keys.ModInformation:
					instance.ModInformation = (string)change.Value;
					break;
				case Keys.PlayerName:
					instance.PlayerName = (string)change.Value;
					break;
				}
			}
		}
	}
}
