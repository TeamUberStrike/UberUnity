using UnityEngine;

public static class WaitingTexture
{
	public static int Angle
	{
		get
		{
			return Mathf.RoundToInt(Time.time * 10f) * 30;
		}
	}

	public static void Draw(Vector2 position, int size = 0)
	{
		size = ((size > 0) ? Mathf.Clamp(size, 1, 32) : 32);
		GUIUtility.RotateAroundPivot(Angle, position);
		GUI.DrawTexture(new Rect(position.x - (float)size * 0.5f, position.y - (float)size * 0.5f, size, size), UberstrikeIcons.Waiting);
		GUI.matrix = Matrix4x4.identity;
	}
}
