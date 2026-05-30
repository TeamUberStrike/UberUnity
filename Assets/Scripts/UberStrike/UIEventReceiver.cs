using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/CMune Extensions/Event Receiver")]
public class UIEventReceiver : MonoBehaviour
{
	public Action<bool> OnHovered;

	public Action<bool> OnPressed;

	public Action OnReleased;

	public Action<bool> OnSelected;

	public Action OnClicked;

	public Action<Vector2> OnDragging;

	public Action<string> OnInputEntered;

	public Action<KeyCode> OnKeyEntered;

	public Action<bool> OnTooltipActive;

	private void OnHover(bool isOver)
	{
		if (OnHovered != null && base.enabled)
		{
			OnHovered(isOver);
		}
	}

	private void OnPress(bool isPressed)
	{
		if (base.enabled)
		{
			if (OnPressed != null)
			{
				OnPressed(isPressed);
			}
			if (!isPressed && OnReleased != null)
			{
				OnReleased();
			}
		}
	}

	private void OnSelect(bool selected)
	{
		if (OnSelected != null && base.enabled)
		{
			OnSelected(selected);
		}
	}

	private void OnClick()
	{
		if (OnClicked != null && base.enabled)
		{
			OnClicked();
		}
	}

	private void OnDrag(Vector2 delta)
	{
		if (OnDragging != null && base.enabled)
		{
			OnDragging(delta);
		}
	}

	private void OnInput(string text)
	{
		if (OnInputEntered != null && base.enabled)
		{
			OnInputEntered(text);
		}
	}

	private void OnKey(KeyCode key)
	{
		if (OnKeyEntered != null && base.enabled)
		{
			OnKeyEntered(key);
		}
	}

	private void OnTooltip(bool show)
	{
	}
}
