using System;
using UnityEngine;

public abstract class BasePopupDialog : IPopupDialog
{
	protected Vector2 _size = new Vector2(320f, 240f);

	protected PopupSystem.ActionType _actionType;

	protected PopupSystem.AlertType _alertType;

	protected string _okCaption = string.Empty;

	protected string _cancelCaption = string.Empty;

	protected bool _allowAudio = true;

	protected Action _callbackOk;

	protected Action _callbackCancel;

	protected Action _onGUIAction;

	public string Text { get; set; }

	public string Title { get; set; }

	public GuiDepth Depth
	{
		get
		{
			return GuiDepth.Popup;
		}
	}

	protected virtual bool IsOkButtonEnabled
	{
		get
		{
			return true;
		}
	}

	public virtual void OnHide()
	{
	}

	public void SetAlertType(PopupSystem.AlertType type)
	{
		_alertType = type;
	}

	public void OnGUI()
	{
		Rect position = new Rect(((float)Screen.width - _size.x) * 0.5f, ((float)Screen.height - _size.y - 56f) * 0.5f, _size.x, _size.y);
		GUI.BeginGroup(position, GUIContent.none, PopupSkin.window);
		GUI.Label(new Rect(0f, 0f, _size.x, 56f), Title, PopupSkin.title);
		DrawPopupWindow();
		switch (_alertType)
		{
		case PopupSystem.AlertType.OK:
			DoOKButton();
			break;
		case PopupSystem.AlertType.Cancel:
			DoCancelButton();
			break;
		case PopupSystem.AlertType.OKCancel:
			DoOKCancelButtons();
			break;
		}
		GUI.EndGroup();
	}

	protected abstract void DrawPopupWindow();

	private void DoOKButton()
	{
		GUIStyle style = PopupSkin.button;
		switch (_actionType)
		{
		case PopupSystem.ActionType.Negative:
			style = PopupSkin.button_red;
			break;
		case PopupSystem.ActionType.Positive:
			style = PopupSkin.button_green;
			break;
		}
		Rect rect = new Rect((_size.x - 120f) * 0.5f, _size.y - 40f, 120f, 32f);
		GUIContent content = new GUIContent((!string.IsNullOrEmpty(_okCaption)) ? _okCaption : LocalizedStrings.OkCaps);
		if ((!_allowAudio) ? GUI.Button(rect, content, style) : GUITools.Button(rect, content, style))
		{
			PopupSystem.HideMessage(this);
			if (_callbackOk != null)
			{
				_callbackOk();
			}
		}
	}

	private void DoOKCancelButtons()
	{
		Rect rect = new Rect(_size.x * 0.5f + 5f, _size.y - 40f, 120f, 32f);
		GUIContent content = new GUIContent((!string.IsNullOrEmpty(_cancelCaption)) ? _cancelCaption : LocalizedStrings.CancelCaps);
		GUI.color = Color.white;
		if ((!_allowAudio) ? GUI.Button(rect, content, PopupSkin.button) : GUITools.Button(rect, content, PopupSkin.button))
		{
			PopupSystem.HideMessage(this);
			if (_callbackCancel != null)
			{
				_callbackCancel();
			}
		}
		GUIStyle style = PopupSkin.button;
		switch (_actionType)
		{
		case PopupSystem.ActionType.Negative:
			style = PopupSkin.button_red;
			break;
		case PopupSystem.ActionType.Positive:
			style = PopupSkin.button_green;
			break;
		}
		rect = new Rect(_size.x * 0.5f - 125f, _size.y - 40f, 120f, 32f);
		content = new GUIContent((!string.IsNullOrEmpty(_okCaption)) ? _okCaption : LocalizedStrings.OkCaps);
		GUI.enabled = IsOkButtonEnabled;
		if ((!_allowAudio) ? GUI.Button(rect, content, style) : GUITools.Button(rect, content, style))
		{
			PopupSystem.HideMessage(this);
			if (_callbackOk != null)
			{
				_callbackOk();
			}
		}
	}

	private void DoCancelButton()
	{
		GUIStyle style = PopupSkin.button;
		switch (_actionType)
		{
		case PopupSystem.ActionType.Negative:
			style = PopupSkin.button_red;
			break;
		case PopupSystem.ActionType.Positive:
			style = PopupSkin.button_green;
			break;
		}
		Rect rect = new Rect((_size.x - 120f) * 0.5f, _size.y - 40f, 120f, 32f);
		GUIContent content = new GUIContent((!string.IsNullOrEmpty(_cancelCaption)) ? _cancelCaption : LocalizedStrings.CancelCaps);
		if ((!_allowAudio) ? GUI.Button(rect, content, style) : GUITools.Button(rect, content, style))
		{
			PopupSystem.HideMessage(this);
			if (_callbackCancel != null)
			{
				_callbackCancel();
			}
		}
	}
}
