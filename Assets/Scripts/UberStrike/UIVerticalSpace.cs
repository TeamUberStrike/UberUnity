using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/CMune Extensions/Vertical Space")]
public class UIVerticalSpace : UIWidget
{
	public float Width;

	public override Vector2 relativeSize
	{
		get
		{
			return new Vector2(0f, Width);
		}
	}
}
