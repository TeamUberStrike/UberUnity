using System;
using System.Collections.Generic;

namespace Cmune.DataCenter.Common.Entities
{
	[Serializable]
	public class ContactGroupView
	{
		public int GroupId { get; set; }

		public string GroupName { get; set; }

		public List<PublicProfileView> Contacts { get; set; }

		public ContactGroupView()
		{
			Contacts = new List<PublicProfileView>(0);
			GroupName = string.Empty;
		}

		public ContactGroupView(int groupID, string groupName, List<PublicProfileView> contacts)
		{
			GroupId = groupID;
			GroupName = groupName;
			Contacts = contacts;
		}

		public override string ToString()
		{
			string text = "[Contact group: [Group ID: " + GroupId + "][Group Name :" + GroupName + "][Contacts: ";
			foreach (PublicProfileView contact in Contacts)
			{
				text = text + "[Contact: " + contact.ToString() + "]";
			}
			return text + "]]";
		}
	}
}
