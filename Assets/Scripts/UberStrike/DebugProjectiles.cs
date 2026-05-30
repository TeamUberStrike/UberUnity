using System.Collections.Generic;
using UnityEngine;

public class DebugProjectiles : IDebugPage
{
	private Vector2 scroll1;

	private Vector2 scroll2;

	public string Title
	{
		get
		{
			return "Projectiles";
		}
	}

	public void Draw()
	{
		scroll1 = GUILayout.BeginScrollView(scroll1);
		foreach (KeyValuePair<int, IProjectile> allProjectile in Singleton<ProjectileManager>.Instance.AllProjectiles)
		{
			GUILayout.Label((allProjectile.Key + " - " + allProjectile.Value == null) ? (ProjectileManager.PrintID(allProjectile.Key) + " (exploded zombie)") : ProjectileManager.PrintID(allProjectile.Key));
		}
		GUILayout.EndScrollView();
		GUILayout.Space(30f);
		scroll2 = GUILayout.BeginScrollView(scroll2);
		foreach (int limitedProjectile in Singleton<ProjectileManager>.Instance.LimitedProjectiles)
		{
			GUILayout.Label("Limited " + ProjectileManager.PrintID(limitedProjectile));
		}
		GUILayout.EndScrollView();
	}
}
