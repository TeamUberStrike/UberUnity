using System;
using System.Collections.Generic;
using System.Text;
using Cmune.DataCenter.Common.Entities;
using UnityEngine;

public class ChatDialog
{
	public delegate bool CanShowMessage(ChatContext c);

	public CanShowMessage CanShow;

	public Queue<InstantMessage> _msgQueue;

	private InstantMessage _lastMessage;

	private float _totalHeight;

	private bool _reset;

	private string _title;

	public Vector2 _frameSize;

	public Vector2 _contentSize;

	public float _heightCache;

	public bool CanChat
	{
		get
		{
			return UserCmid == 0 || AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Lobby.HasPlayer(UserCmid);
		}
	}

	public string Title { get; private set; }

	public string UserName { get; private set; }

	public int UserCmid { get; private set; }

	public UserGroups Group { get; set; }

	public bool HasUnreadMessage { get; set; }

	public ICollection<InstantMessage> AllMessages
	{
		get
		{
			return new List<InstantMessage>(_msgQueue.ToArray());
		}
	}

	public ChatDialog(string title = "")
	{
		UserName = string.Empty;
		Title = title;
		_msgQueue = new Queue<InstantMessage>();
		AddMessage(new InstantMessage(0, "Disclaimer", "Do not share your password or any other confidential information with anybody. The members of Cmune and the Uberstrike Moderators will never ask you to provide such information.", MemberAccessLevel.Admin));
	}

	public ChatDialog(CommUser user, UserGroups group)
		: this(string.Empty)
	{
		Group = group;
		if (user != null)
		{
			UserName = user.ShortName;
			UserCmid = user.Cmid;
			Title = "Chat with " + UserName;
		}
	}

	public void AddMessage(InstantMessage msg)
	{
		_reset = true;
		while (_msgQueue.Count > 200)
		{
			_msgQueue.Dequeue();
		}
		if (_lastMessage != null && _lastMessage.Cmid == msg.Cmid && _lastMessage.ArrivalTime.AddMinutes(1.0) > DateTime.Now && !msg.IsNotification && !_lastMessage.IsNotification)
		{
			_lastMessage.Append(msg.Text);
			return;
		}
		_msgQueue.Enqueue(msg);
		_lastMessage = msg;
	}

	public void Clear()
	{
		_msgQueue.Clear();
		_lastMessage = null;
	}

	public void RecalulateBounds()
	{
		_reset = true;
	}

	public bool CheckSize(Rect rect)
	{
		if (_reset || rect.width != _frameSize.x || rect.height != _frameSize.y)
		{
			_reset = false;
			_frameSize.x = rect.width;
			_frameSize.y = rect.height;
			_contentSize.y = rect.height;
			if (_totalHeight < rect.height)
			{
				_totalHeight = 0f;
				_contentSize.x = rect.width;
				foreach (InstantMessage item in _msgQueue)
				{
					item.UpdateHeight(BlueStonez.label_interparkbold_11pt_left_wrap, _contentSize.x - 8f, 24, Singleton<ChatManager>.Instance.IsMuted(item.Cmid));
					_totalHeight += item.Height;
				}
			}
			else
			{
				_totalHeight = 0f;
				_contentSize.x = rect.width - 17f;
				foreach (InstantMessage item2 in _msgQueue)
				{
					item2.UpdateHeight(BlueStonez.label_interparkbold_11pt_left_wrap, _contentSize.x - 8f, 24, Singleton<ChatManager>.Instance.IsMuted(item2.Cmid));
					_totalHeight += item2.Height;
				}
			}
			_contentSize.y = _totalHeight;
			return true;
		}
		return false;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Title: " + Title);
		stringBuilder.AppendLine("Group: " + Group);
		stringBuilder.AppendLine("User: " + UserName + " " + UserCmid);
		stringBuilder.AppendLine("CanChat: " + CanChat);
		return stringBuilder.ToString();
	}
}
