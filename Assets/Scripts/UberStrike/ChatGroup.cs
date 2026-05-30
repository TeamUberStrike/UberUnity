using System.Collections.Generic;

public class ChatGroup
{
	public UserGroups GroupId { get; private set; }

	public string Title { get; private set; }

	public ICollection<CommUser> Players { get; private set; }

	public ChatGroup(UserGroups group, string title, ICollection<CommUser> players)
	{
		GroupId = group;
		Title = title;
		Players = players;
	}

	public bool HasUnreadMessages()
	{
		if (Players != null)
		{
			foreach (CommUser player in Players)
			{
				ChatDialog value;
				if (Singleton<ChatManager>.Instance._dialogsByCmid.TryGetValue(player.Cmid, out value) && value != null && value.HasUnreadMessage)
				{
					return true;
				}
			}
		}
		return false;
	}
}
