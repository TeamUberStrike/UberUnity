using UnityEngine;

public class DebugMaps : IDebugPage
{
	private Vector2 scroll;

	public string Title
	{
		get
		{
			return "Maps";
		}
	}

	public void Draw()
	{
		scroll = GUILayout.BeginScrollView(scroll);
		foreach (UberstrikeMap allMap in Singleton<MapManager>.Instance.AllMaps)
		{
			GUILayout.Label(allMap.Id + ", Modes: " + allMap.View.SupportedGameModes + ", Item: " + allMap.View.RecommendedItemId + ", Scene: " + allMap.SceneName);
		}
		GUILayout.EndScrollView();
	}
}
