using System.Collections.Generic;
using UnityEngine;

public class AvatarGearParts
{
	public GameObject Base { get; set; }

	public string Avatar { get; set; }

	public List<GameObject> Parts { get; private set; }

	public AvatarGearParts()
	{
		Parts = new List<GameObject>();
	}

	public void DestroyGearParts()
	{
		for (int i = 0; i < Parts.Count; i++)
		{
			Object.Destroy(Parts[i]);
		}
	}
}
