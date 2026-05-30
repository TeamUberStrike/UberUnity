using System;
using System.Collections;
using UnityEngine;

public class TouchFollow : TouchBaseControl
{
	private bool enabled;

	private TouchFinger _finger;

	private ArrayList _ignoreTouches;

	private float _totalMoved;

	public Vector2 Aim { get; private set; }

	public override bool Enabled
	{
		get
		{
			return enabled;
		}
		set
		{
			if (enabled != value)
			{
				enabled = value;
				if (enabled)
				{
					Aim = Vector2.zero;
					_finger = new TouchFinger();
				}
			}
		}
	}

	public event Action OnFired;

	public TouchFollow()
	{
		_finger = new TouchFinger();
		_ignoreTouches = new ArrayList();
	}

	public override void UpdateTouches(Touch touch)
	{
		if (touch.phase == TouchPhase.Began)
		{
			if (_finger.FingerId == -1)
			{
				_finger = new TouchFinger
				{
					StartPos = touch.position,
					StartTouchTime = Time.time,
					LastPos = touch.position,
					FingerId = touch.fingerId
				};
				_totalMoved = 0f;
			}
		}
		else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
		{
			if (_finger.FingerId == touch.fingerId)
			{
				Aim = touch.deltaPosition * 500f / Screen.width;
				_totalMoved += Mathf.Abs(touch.deltaPosition.x) + Mathf.Abs(touch.deltaPosition.y);
			}
		}
		else if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && _finger.FingerId == touch.fingerId)
		{
			if (_totalMoved < 10f && this.OnFired != null)
			{
				this.OnFired();
			}
			_finger.Reset();
			Aim = Vector2.zero;
		}
	}

	public void IgnoreRect(Rect r)
	{
		if (!_ignoreTouches.Contains(r))
		{
			_ignoreTouches.Add(r);
		}
	}

	private bool ValidArea(Vector2 pos)
	{
		if (_ignoreTouches.Count == 0)
		{
			return true;
		}
		foreach (Rect ignoreTouch in _ignoreTouches)
		{
			if (ignoreTouch.ContainsTouch(pos))
			{
				return false;
			}
		}
		return true;
	}
}
