using UnityEngine;

public class TouchFinger
{
	public Vector2 StartPos;

	public Vector2 LastPos;

	public float StartTouchTime;

	public int FingerId;

	public TouchFinger()
	{
		Reset();
	}

	public void Reset()
	{
		StartPos = Vector2.zero;
		LastPos = Vector2.zero;
		StartTouchTime = 0f;
		FingerId = -1;
	}
}
