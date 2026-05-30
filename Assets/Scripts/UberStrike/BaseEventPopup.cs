using System;
using UnityEngine;

public abstract class BaseEventPopup : IPopupDialog
{
	private const float BerpSpeed = 2.5f;

	protected int Width = 650;

	protected int Height = 330;

	protected bool ClickAnywhereToExit = true;

	private float _startTime;

	protected Action _onCloseButtonClicked;

	public string Text { get; set; }

	public string Title { get; set; }

	public GuiDepth Depth
	{
		get
		{
			return GuiDepth.Event;
		}
	}

	public float Scale
	{
		get
		{
			if (_startTime > Time.time - 1f)
			{
				return Mathfx.Berp(0.01f, 1f, (Time.time - _startTime) * 2.5f);
			}
			return 1f;
		}
	}

	protected abstract void DrawGUI(Rect rect);

	public virtual void OnHide()
	{
	}

	public void OnGUI()
	{
		if (_startTime == 0f)
		{
			_startTime = Time.time;
		}
		GUI.color = Color.white.SetAlpha(Scale);
		float num = (float)(Screen.width - Width) * 0.5f;
		float num2 = (float)GlobalUIRibbon.Instance.Height() + (float)(Screen.height - GlobalUIRibbon.Instance.Height() - Height) * 0.5f;
		Rect rect = new Rect(num, num2, Width, (float)(64 + Height) - 64f * Scale);
		GUI.Box(new Rect(num - 1f, num2 - 1f, rect.width + 2f, rect.height + 2f), GUIContent.none, BlueStonez.window);
		GUI.BeginGroup(rect);
		if (GUI.Button(new Rect(rect.width - 20f, 0f, 20f, 20f), "X", BlueStonez.friends_hidden_button))
		{
			Close();
		}
		DrawGUI(rect);
		GUI.EndGroup();
		GUI.color = Color.white;
		if (ClickAnywhereToExit && Event.current.type == EventType.MouseDown && !rect.Contains(Event.current.mousePosition))
		{
			Event.current.Use();
			Close();
		}
		OnAfterGUI();
	}

	public virtual void OnAfterGUI()
	{
	}

	private void Close()
	{
		PopupSystem.HideMessage(this);
		if (_onCloseButtonClicked != null)
		{
			_onCloseButtonClicked();
		}
	}
}
