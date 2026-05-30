using System;
using UnityEngine;

public static class LayerUtil
{
	public static int LayerMaskEverything
	{
		get
		{
			return -1;
		}
	}

	public static int LayerMaskNothing
	{
		get
		{
			return 0;
		}
	}

	static LayerUtil()
	{
	}

	public static void ValidateUberstrikeLayers()
	{
		for (int i = 0; i < 32; i++)
		{
			if (i == 2)
			{
				continue;
			}
			if (!string.IsNullOrEmpty(LayerMask.LayerToName(i)))
			{
				if (Enum.GetName(typeof(UberstrikeLayer), i) != LayerMask.LayerToName(i))
				{
					Debug.LogError("Editor layer '" + LayerMask.LayerToName(i) + "' is not defined in the UberstrikeLayer enum!");
				}
			}
			else if (!string.IsNullOrEmpty(Enum.GetName(typeof(UberstrikeLayer), i)))
			{
				throw new Exception("UberstrikeLayer mismatch with Editor on layer: " + Enum.GetName(typeof(UberstrikeLayer), i));
			}
		}
	}

	public static int CreateLayerMask(params UberstrikeLayer[] layers)
	{
		int num = 0;
		for (int i = 0; i < layers.Length; i++)
		{
			int num2 = (int)layers[i];
			num |= 1 << num2;
		}
		return num;
	}

	public static int AddToLayerMask(int mask, params UberstrikeLayer[] layers)
	{
		for (int i = 0; i < layers.Length; i++)
		{
			int num = (int)layers[i];
			mask |= 1 << num;
		}
		return mask;
	}

	public static int RemoveFromLayerMask(int mask, params UberstrikeLayer[] layers)
	{
		for (int i = 0; i < layers.Length; i++)
		{
			int num = (int)layers[i];
			mask &= ~(1 << num);
		}
		return mask;
	}

	public static void SetLayerRecursively(Transform transform, UberstrikeLayer layer)
	{
		SetLayerRecursively(transform, (int)layer);
	}

	public static void SetLayerRecursively(Transform transform, int layer)
	{
		Transform[] componentsInChildren = transform.GetComponentsInChildren<Transform>(true);
		foreach (Transform transform2 in componentsInChildren)
		{
			transform2.gameObject.layer = layer;
		}
	}

	public static int GetLayer(UberstrikeLayer layer)
	{
		return (int)layer;
	}

	public static bool IsLayerInMask(int mask, int layer)
	{
		return (mask & (1 << layer)) != 0;
	}

	public static bool IsLayerInMask(int mask, UberstrikeLayer layer)
	{
		return (mask & (1 << (int)layer)) != 0;
	}
}
