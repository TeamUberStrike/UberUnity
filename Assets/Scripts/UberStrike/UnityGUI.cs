using UnityEngine;

public static class UnityGUI
{
	public static int Toolbar(Rect position, int selected, GUIContent[] contents, int xCount, GUIStyle style)
	{
		int result = GUI.Toolbar(position, selected, contents, style);
		int controlID = GUIUtility.GetControlID(FocusType.Passive, position);
		EventType typeForControl = Event.current.GetTypeForControl(controlID);
		if (typeForControl == EventType.Repaint)
		{
			GUIStyle firstStyle = null;
			GUIStyle midStyle = null;
			GUIStyle lastStyle = null;
			FindStyles(ref style, out firstStyle, out midStyle, out lastStyle, "left", "mid", "right");
			int num = contents.Length;
			int num2 = num / xCount;
			if (num % xCount != 0)
			{
				num2++;
			}
			float num3 = CalcTotalHorizSpacing(xCount, style, firstStyle, midStyle, lastStyle);
			float num4 = Mathf.Max(style.margin.top, style.margin.bottom) * (num2 - 1);
			float elemWidth = (position.width - num3) / (float)xCount;
			float elemHeight = (position.height - num4) / (float)num2;
			Rect[] buttonRects = CalcMouseRects(position, num, xCount, elemWidth, elemHeight, style, firstStyle, midStyle, lastStyle, false);
			int buttonGridMouseSelection = GetButtonGridMouseSelection(buttonRects, Event.current.mousePosition, controlID == GUIUtility.hotControl);
			if (buttonGridMouseSelection >= 0)
			{
				GUI.tooltip = contents[buttonGridMouseSelection].tooltip;
			}
		}
		return result;
	}

	internal static GUIContent[] Temp(string[] texts)
	{
		GUIContent[] array = new GUIContent[texts.Length];
		for (int i = 0; i < texts.Length; i++)
		{
			array[i] = new GUIContent(texts[i]);
		}
		return array;
	}

	public static int Toolbar(Rect position, int selected, string[] contents, int length, GUIStyle style)
	{
		return Toolbar(position, selected, Temp(contents), length, style);
	}

	public static int Toolbar(Rect position, int selected, GUIContent[] contents, GUIStyle style)
	{
		return Toolbar(position, selected, contents, contents.Length, style);
	}

	internal static void FindStyles(ref GUIStyle style, out GUIStyle firstStyle, out GUIStyle midStyle, out GUIStyle lastStyle, string first, string mid, string last)
	{
		if (style == null)
		{
			style = GUI.skin.button;
		}
		string name = style.name;
		midStyle = GUI.skin.FindStyle(name + mid);
		if (midStyle == null)
		{
			midStyle = style;
		}
		firstStyle = GUI.skin.FindStyle(name + first);
		if (firstStyle == null)
		{
			firstStyle = midStyle;
		}
		lastStyle = GUI.skin.FindStyle(name + last);
		if (lastStyle == null)
		{
			lastStyle = midStyle;
		}
	}

	private static Rect[] CalcMouseRects(Rect position, int count, int xCount, float elemWidth, float elemHeight, GUIStyle style, GUIStyle firstStyle, GUIStyle midStyle, GUIStyle lastStyle, bool addBorders)
	{
		int num = 0;
		int num2 = 0;
		float num3 = position.xMin;
		float num4 = position.yMin;
		GUIStyle gUIStyle = style;
		Rect[] array = new Rect[count];
		if (count > 1)
		{
			gUIStyle = firstStyle;
		}
		for (int i = 0; i < count; i++)
		{
			if (addBorders)
			{
				array[i] = gUIStyle.margin.Add(new Rect(num3, num4, elemWidth, elemHeight));
			}
			else
			{
				array[i] = new Rect(num3, num4, elemWidth, elemHeight);
			}
			array[i].width = Mathf.Round(array[i].xMax) - Mathf.Round(array[i].x);
			array[i].x = Mathf.Round(array[i].x);
			GUIStyle gUIStyle2 = midStyle;
			if (i == count - 2)
			{
				gUIStyle2 = lastStyle;
			}
			num3 = num3 + elemWidth + (float)Mathf.Max(gUIStyle.margin.right, gUIStyle2.margin.left);
			num2++;
			if (num2 >= xCount)
			{
				num++;
				num2 = 0;
				num4 = num4 + elemHeight + (float)Mathf.Max(style.margin.top, style.margin.bottom);
				num3 = position.xMin;
			}
		}
		return array;
	}

	private static int GetButtonGridMouseSelection(Rect[] buttonRects, Vector2 mousePos, bool findNearest)
	{
		for (int i = 0; i < buttonRects.Length; i++)
		{
			if (buttonRects[i].Contains(mousePos))
			{
				return i;
			}
		}
		if (findNearest)
		{
			float num = 10000000f;
			int result = -1;
			for (int j = 0; j < buttonRects.Length; j++)
			{
				Rect rect = buttonRects[j];
				Vector2 vector = new Vector2(Mathf.Clamp(mousePos.x, rect.xMin, rect.xMax), Mathf.Clamp(mousePos.y, rect.yMin, rect.yMax));
				float sqrMagnitude = (mousePos - vector).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					result = j;
					num = sqrMagnitude;
				}
			}
			return result;
		}
		return -1;
	}

	internal static int CalcTotalHorizSpacing(int xCount, GUIStyle style, GUIStyle firstStyle, GUIStyle midStyle, GUIStyle lastStyle)
	{
		if (xCount >= 2)
		{
			if (xCount != 2)
			{
				int num = Mathf.Max(midStyle.margin.left, midStyle.margin.right);
				return Mathf.Max(firstStyle.margin.right, midStyle.margin.left) + Mathf.Max(midStyle.margin.right, lastStyle.margin.left) + num * (xCount - 3);
			}
			return Mathf.Max(firstStyle.margin.right, lastStyle.margin.left);
		}
		return 0;
	}
}
