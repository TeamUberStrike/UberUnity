using UnityEngine;

public class DebugAnimation : IDebugPage
{
	private CharacterConfig config;

	public string Title
	{
		get
		{
			return "Animation";
		}
	}

	public void Draw()
	{
		GUILayout.BeginHorizontal();
		foreach (CharacterConfig value in GameState.Current.Avatars.Values)
		{
			if (GUILayout.Button(value.name))
			{
				config = value;
			}
		}
		GUILayout.EndHorizontal();
		if (config == null)
		{
			GUILayout.Label("Select a player");
		}
		else if (config.Avatar == null)
		{
			GUILayout.Label("Missing Decorator");
		}
	}
}
