using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button")]
public class UIButton : UIButtonColor
{
	public Color disabledColor = Color.grey;

	public Action OnPressed;

	public Action OnRelease;

	public Action OnHovered;

	public bool isEnabled
	{
		get
		{
			Collider collider = base.GetComponent<Collider>();
			return (bool)collider && collider.enabled;
		}
		set
		{
			Collider collider = base.GetComponent<Collider>();
			if ((bool)collider && collider.enabled != value)
			{
				collider.enabled = value;
				UpdateColor(value, false);
			}
		}
	}

	protected override void OnEnable()
	{
		if (isEnabled)
		{
			base.OnEnable();
		}
		else
		{
			UpdateColor(false, true);
		}
	}

	public override void OnHover(bool isOver)
	{
		if (isEnabled)
		{
			if (isOver && OnHovered != null)
			{
				OnHovered();
			}
			base.OnHover(isOver);
		}
	}

	public override void OnPress(bool isPressed)
	{
		if (!isEnabled)
		{
			return;
		}
		if (isPressed)
		{
			if (OnPressed != null)
			{
				OnPressed();
			}
		}
		else if (OnRelease != null)
		{
			OnRelease();
		}
		base.OnPress(isPressed);
	}

	public void UpdateColor(bool shouldBeEnabled, bool immediate)
	{
		if ((bool)tweenTarget)
		{
			if (!mStarted)
			{
				mStarted = true;
				Init();
			}
			Color color = ((!shouldBeEnabled) ? disabledColor : base.defaultColor);
			TweenColor tweenColor = TweenColor.Begin(tweenTarget, 0.15f, color);
			if (immediate)
			{
				tweenColor.color = color;
				tweenColor.enabled = false;
			}
		}
	}
}
