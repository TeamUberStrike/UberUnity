using System;
using UnityEngine;

public class TouchControl : TouchBaseControl
{
	public TouchFinger finger;

	private bool enabled;

	protected float _rotationAngle;

	protected Vector2 _rotationPoint = Vector2.zero;

	private bool _inside;

	public override bool Enabled
	{
		get
		{
			return enabled;
		}
		set
		{
			if (value != enabled)
			{
				enabled = value;
				if (!enabled)
				{
					finger.Reset();
					_inside = false;
				}
			}
		}
	}

	public bool IsActive
	{
		get
		{
			return finger.FingerId != -1;
		}
	}

	public event Action<Vector2> OnTouchBegan;

	public event Action<Vector2, Vector2> OnTouchLeftBoundary;

	public event Action<Vector2, Vector2> OnTouchMoved;

	public event Action<Vector2, Vector2> OnTouchEnteredBoundary;

	public event Action<Vector2> OnTouchEnded;

	public TouchControl()
	{
		finger = new TouchFinger();
	}

	public void SetRotation(float angle, Vector2 point)
	{
		_rotationAngle = angle;
		_rotationPoint = point;
	}

	public override void UpdateTouches(Touch touch)
	{
		if ((finger.FingerId != -1 && touch.fingerId != finger.FingerId) || (finger.FingerId == -1 && touch.phase != TouchPhase.Began))
		{
			return;
		}
		Vector2 vector = touch.position;
		if (_rotationAngle != 0f)
		{
			vector = Mathfx.RotateVector2AboutPoint(touch.position, new Vector2(_rotationPoint.x, (float)Screen.height - _rotationPoint.y), 0f - _rotationAngle);
		}
		switch (touch.phase)
		{
		case TouchPhase.Began:
			if (TouchInside(vector))
			{
				finger.StartPos = vector;
				finger.LastPos = vector;
				finger.StartTouchTime = Time.time;
				finger.FingerId = touch.fingerId;
				_inside = true;
				if (this.OnTouchBegan != null)
				{
					this.OnTouchBegan(vector);
				}
			}
			break;
		case TouchPhase.Moved:
		case TouchPhase.Stationary:
		{
			bool flag = TouchInside(vector);
			if (_inside && !flag)
			{
				_inside = false;
				if (this.OnTouchLeftBoundary != null)
				{
					this.OnTouchLeftBoundary(vector, touch.deltaPosition);
				}
			}
			else if (!_inside && flag)
			{
				_inside = true;
				if (this.OnTouchEnteredBoundary != null)
				{
					this.OnTouchEnteredBoundary(vector, touch.deltaPosition);
				}
			}
			if (this.OnTouchMoved != null)
			{
				this.OnTouchMoved(vector, touch.deltaPosition);
			}
			finger.LastPos = vector;
			break;
		}
		case TouchPhase.Ended:
		case TouchPhase.Canceled:
			if (this.OnTouchEnded != null)
			{
				this.OnTouchEnded(vector);
			}
			ResetTouch();
			break;
		}
	}

	protected virtual void ResetTouch()
	{
		finger.Reset();
		_inside = false;
	}

	protected virtual bool TouchInside(Vector2 position)
	{
		return Boundary.ContainsTouch(position);
	}
}
