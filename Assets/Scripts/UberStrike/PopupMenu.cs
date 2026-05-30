using System;
using System.Collections.Generic;
using UnityEngine;

public class PopupMenu
{
	private class MenuItem
	{
		public string Caption;

		public Action<CommUser> Callback;

		public Action<CommUser, InstantMessage> CopyMsgCallback;

		public Action<CommUser, InstantMessage> CopyMsg;

		public Action<CommUser, InstantMessage> CopyPlayerName;

		public Func<CommUser, bool> CheckItem;

		public Func<CommUser, string> DynamicCaption;

		public bool Enabled;
	}

	private const int Height = 24;

	private const int Width = 105;

	private Rect _position;

	private List<MenuItem> _items;

	private CommUser _selectedUser;

	public CommUser SelectedUser
	{
		get
		{
			return _selectedUser;
		}
	}

	public InstantMessage msg { get; set; }

	public static PopupMenu Current { get; private set; }

	public static bool IsEnabled
	{
		get
		{
			return Current != null;
		}
	}

	public PopupMenu()
	{
		_items = new List<MenuItem>();
	}

	public void AddMenuCopyItem(string caption, Action<CommUser, InstantMessage> action, Func<CommUser, bool> isEnabledForUser)
	{
		MenuItem menuItem = new MenuItem();
		menuItem.Caption = caption;
		menuItem.CopyMsgCallback = action;
		menuItem.CopyMsg = action;
		menuItem.CheckItem = isEnabledForUser;
		MenuItem item = menuItem;
		_items.Add(item);
	}

	public void AddMenuCopyPlayerName(string caption, Action<CommUser, InstantMessage> action, Func<CommUser, bool> isEnabledForUser)
	{
		MenuItem menuItem = new MenuItem();
		menuItem.Caption = caption;
		menuItem.CopyPlayerName = action;
		menuItem.CopyMsg = action;
		menuItem.CheckItem = isEnabledForUser;
		MenuItem item = menuItem;
		_items.Add(item);
	}

	public void AddMenuItem(Func<CommUser, string> caption, Action<CommUser> action, Func<CommUser, bool> isEnabledForUser)
	{
		MenuItem menuItem = new MenuItem();
		menuItem.DynamicCaption = caption;
		menuItem.Caption = string.Empty;
		menuItem.Callback = action;
		menuItem.CheckItem = isEnabledForUser;
		MenuItem item = menuItem;
		_items.Add(item);
	}

	public void AddMenuItem(string caption, Action<CommUser> action, Func<CommUser, bool> isEnabledForUser)
	{
		MenuItem menuItem = new MenuItem();
		menuItem.Caption = caption;
		menuItem.Callback = action;
		menuItem.CheckItem = isEnabledForUser;
		MenuItem item = menuItem;
		_items.Add(item);
	}

	private void Configure()
	{
		foreach (MenuItem item in _items)
		{
			item.Enabled = item.CheckItem(_selectedUser);
			if (item.DynamicCaption != null)
			{
				item.Caption = item.DynamicCaption(_selectedUser);
			}
		}
	}

	public static void Hide()
	{
		Current = null;
	}

	public void Show(Vector2 screenPos, CommUser user)
	{
		Show(screenPos, user, this);
	}

	public static void Show(Vector2 screenPos, CommUser user, PopupMenu menu)
	{
		if (menu != null)
		{
			menu._selectedUser = user;
			menu.Configure();
			menu._position.height = 24 * menu._items.FindAll((MenuItem i) => i.Enabled).Count;
			menu._position.width = 105f;
			menu._position.x = screenPos.x - 1f;
			if (screenPos.y + menu._position.height > (float)Screen.height)
			{
				menu._position.y = screenPos.y - menu._position.height + 1f;
			}
			else
			{
				menu._position.y = screenPos.y - 1f;
			}
			Current = menu;
		}
	}

	public void Draw()
	{
		GUI.BeginGroup(new Rect(_position.x, _position.y, _position.width, _position.height + 6f), BlueStonez.window);
		GUI.Label(new Rect(1f, 1f, _position.width - 2f, _position.height + 4f), GUIContent.none, BlueStonez.box_grey50);
		GUI.Label(new Rect(0f, 0f, _position.width, _position.height + 6f), GUIContent.none, BlueStonez.box_grey50);
		GUITools.PushGUIState();
		int num = 0;
		foreach (MenuItem item in _items)
		{
			if (item.Enabled)
			{
				if (item.CopyMsgCallback != null)
				{
					GUI.enabled = item.CopyMsgCallback != null;
				}
				else
				{
					GUI.enabled = item.Callback != null;
				}
				GUI.Label(new Rect(8f, 8 + num * 24, _position.width - 8f, 24f), item.Caption, BlueStonez.label_interparkmed_11pt_left);
				if (item.Callback != null && GUI.Button(new Rect(2f, 3 + num * 24, _position.width - 4f, 24f), GUIContent.none, BlueStonez.dropdown_list))
				{
					Current = null;
					item.Callback(_selectedUser);
				}
				else if (item.CopyMsgCallback != null && GUI.Button(new Rect(2f, 3 + num * 24, _position.width - 4f, 24f), GUIContent.none, BlueStonez.dropdown_list))
				{
					Current = null;
					item.CopyMsgCallback(_selectedUser, msg);
				}
				num++;
			}
		}
		GUITools.PopGUIState();
		GUI.EndGroup();
		if (Event.current.type == EventType.MouseUp && !_position.Contains(Event.current.mousePosition))
		{
			Current = null;
		}
	}
}
