using System;
using UnityEngine;

public static class TagUtil
{
	public static string GetTag(Collider c)
	{
		string result = "Cement";
		try
		{
			if ((bool)c)
			{
				result = c.tag;
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Failed to get tag from collider: " + ex.Message);
		}
		return result;
	}
}
