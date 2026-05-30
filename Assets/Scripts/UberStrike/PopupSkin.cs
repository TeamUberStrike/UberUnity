using UnityEngine;

public static class PopupSkin
{
	public static GUIStyle box = GUIStyle.none;

	public static GUIStyle label = GUIStyle.none;

	public static GUIStyle textField = GUIStyle.none;

	public static GUIStyle textArea = GUIStyle.none;

	public static GUIStyle button = GUIStyle.none;

	public static GUIStyle toggle = GUIStyle.none;

	public static GUIStyle window = GUIStyle.none;

	public static GUIStyle horizontalSlider = GUIStyle.none;

	public static GUIStyle horizontalSliderThumb = GUIStyle.none;

	public static GUIStyle verticalSlider = GUIStyle.none;

	public static GUIStyle verticalSliderThumb = GUIStyle.none;

	public static GUIStyle horizontalScrollbar = GUIStyle.none;

	public static GUIStyle horizontalScrollbarThumb = GUIStyle.none;

	public static GUIStyle horizontalScrollbarLeftButton = GUIStyle.none;

	public static GUIStyle horizontalScrollbarRightButton = GUIStyle.none;

	public static GUIStyle verticalScrollbar = GUIStyle.none;

	public static GUIStyle verticalScrollbarThumb = GUIStyle.none;

	public static GUIStyle verticalScrollbarUpButton = GUIStyle.none;

	public static GUIStyle verticalScrollbarDownButton = GUIStyle.none;

	public static GUIStyle scrollView = GUIStyle.none;

	public static GUIStyle title = GUIStyle.none;

	public static GUIStyle button_green = GUIStyle.none;

	public static GUIStyle button_red = GUIStyle.none;

	public static GUIStyle label_loading = GUIStyle.none;

	public static GUISkin Skin { get; private set; }

	private static GUIStyle SafeGetStyle(GUISkin skin, string name)
	{
		try { return skin.GetStyle(name); }
		catch { return GUIStyle.none; }
	}

	public static void Initialize(GUISkin skin)
	{
		Skin = skin;
		box = SafeGetStyle(skin, "box");
		label = SafeGetStyle(skin, "label");
		textField = SafeGetStyle(skin, "textField");
		textArea = SafeGetStyle(skin, "textArea");
		button = SafeGetStyle(skin, "button");
		toggle = SafeGetStyle(skin, "toggle");
		window = SafeGetStyle(skin, "window");
		horizontalSlider = SafeGetStyle(skin, "horizontalSlider");
		horizontalSliderThumb = SafeGetStyle(skin, "horizontalSliderThumb");
		verticalSlider = SafeGetStyle(skin, "verticalSlider");
		verticalSliderThumb = SafeGetStyle(skin, "verticalSliderThumb");
		horizontalScrollbar = SafeGetStyle(skin, "horizontalScrollbar");
		horizontalScrollbarThumb = SafeGetStyle(skin, "horizontalScrollbarThumb");
		horizontalScrollbarLeftButton = SafeGetStyle(skin, "horizontalScrollbarLeftButton");
		horizontalScrollbarRightButton = SafeGetStyle(skin, "horizontalScrollbarRightButton");
		verticalScrollbar = SafeGetStyle(skin, "verticalScrollbar");
		verticalScrollbarThumb = SafeGetStyle(skin, "verticalScrollbarThumb");
		verticalScrollbarUpButton = SafeGetStyle(skin, "verticalScrollbarUpButton");
		verticalScrollbarDownButton = SafeGetStyle(skin, "verticalScrollbarDownButton");
		scrollView = SafeGetStyle(skin, "scrollView");
		title = SafeGetStyle(skin, "title");
		button_green = SafeGetStyle(skin, "button_green");
		button_red = SafeGetStyle(skin, "button_red");
		label_loading = SafeGetStyle(skin, "label_loading");
	}
}
