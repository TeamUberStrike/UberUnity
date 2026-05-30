using System.Globalization;
using UnityEngine;

public static class ColorConverter
{
	public static float GetHue(Color c)
	{
		float num = 0f;
		if (c.r == 0f)
		{
			num = 2f;
			return num + ((!(c.b < 1f)) ? (2f - c.g) : c.b);
		}
		if (c.g == 0f)
		{
			num = 4f;
			return num + ((!(c.r < 1f)) ? (2f - c.b) : c.r);
		}
		num = 0f;
		return num + ((!(c.g < 1f)) ? (2f - c.r) : c.g);
	}

	public static Color GetColor(float hue)
	{
		hue %= 6f;
		Color white = Color.white;
		return (hue < 1f) ? new Color(1f, hue, 0f) : ((hue < 2f) ? new Color(2f - hue, 1f, 0f) : ((hue < 3f) ? new Color(0f, 1f, hue - 2f) : ((hue < 4f) ? new Color(0f, 4f - hue, 1f) : ((!(hue < 5f)) ? new Color(1f, 0f, 6f - hue) : new Color(hue - 4f, 0f, 1f)))));
	}

	public static Color HexToColor(string hexString)
	{
		int num;
		try
		{
			num = int.Parse(hexString.Substring(0, 2), NumberStyles.HexNumber);
		}
		catch
		{
			num = 255;
		}
		int num2;
		try
		{
			num2 = int.Parse(hexString.Substring(2, 2), NumberStyles.HexNumber);
		}
		catch
		{
			num2 = 255;
		}
		int num3;
		try
		{
			num3 = int.Parse(hexString.Substring(4, 2), NumberStyles.HexNumber);
		}
		catch
		{
			num3 = 255;
		}
		return new Color((float)num / 255f, (float)num2 / 255f, (float)num3 / 255f);
	}

	public static string ColorToHex(Color color)
	{
		string text = ((int)(color.r * 255f)).ToString("X2");
		string text2 = ((int)(color.g * 255f)).ToString("X2");
		string text3 = ((int)(color.b * 255f)).ToString("X2");
		return text + text2 + text3;
	}

	public static Color RgbToColor(float r, float g, float b)
	{
		return new Color(r / 255f, g / 255f, b / 255f);
	}

	public static Color RgbaToColor(float r, float g, float b, float a)
	{
		return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
	}
}
