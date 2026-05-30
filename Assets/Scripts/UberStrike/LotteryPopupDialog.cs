using UnityEngine;

public abstract class LotteryPopupDialog : IPopupDialog
{
	protected enum State
	{
		Normal = 0,
		Rolled = 1
	}

	public const int IMG_WIDTH = 282;

	public const int IMG_HEIGHT = 317;

	private const float BerpSpeed = 2.5f;

	protected int Width = 650;

	protected int Height = 330;

	public bool ClickAnywhereToExit = true;

	protected State _state;

	protected bool _showExitButton = true;

	public string Text { get; set; }

	public string Title { get; set; }

	public bool IsVisible { get; set; }

	public bool IsUIDisabled { get; set; }

	public bool IsWaiting { get; set; }

	public GuiDepth Depth
	{
		get
		{
			return GuiDepth.Event;
		}
	}

	public void OnGUI()
	{
		Rect position = GetPosition();
		GUI.Box(position, GUIContent.none, BlueStonez.window);
		GUITools.PushGUIState();
		GUI.enabled = !IsUIDisabled;
		GUI.BeginGroup(position);
		if (_showExitButton && GUI.Button(new Rect(position.width - 20f, 0f, 20f, 20f), "X", BlueStonez.friends_hidden_button))
		{
			PopupSystem.HideMessage(this);
		}
		DrawPlayGUI(position);
		GUI.EndGroup();
		GUITools.PopGUIState();
		if (IsWaiting)
		{
			WaitingTexture.Draw(position.center);
		}
		if (ClickAnywhereToExit && Event.current.type == EventType.MouseDown && !position.Contains(Event.current.mousePosition))
		{
			ClosePopup();
			Event.current.Use();
		}
		OnAfterGUI();
	}

	public virtual void OnAfterGUI()
	{
	}

	public void OnHide()
	{
	}

	protected abstract void DrawPlayGUI(Rect rect);

	protected void ClosePopup()
	{
		PopupSystem.HideMessage(this);
	}

	private Rect GetPosition()
	{
		float left = (float)(Screen.width - Width) * 0.5f;
		float top = (float)GlobalUIRibbon.Instance.Height() + (float)(Screen.height - GlobalUIRibbon.Instance.Height() - Height) * 0.5f;
		return new Rect(left, top, Width, Height);
	}
}
