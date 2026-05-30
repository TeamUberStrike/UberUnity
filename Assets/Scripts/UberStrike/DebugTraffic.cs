using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DebugTraffic : IDebugPage
{
	public string Title
	{
		get
		{
			return "Traffic";
		}
	}

	public void Draw()
	{
		if (GUILayout.Button("Clear"))
		{
			AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Monitor.AllEvents.Clear();
			Singleton<GameStateController>.Instance.Client.Monitor.AllEvents.Clear();
		}
		GUILayout.BeginHorizontal();
		GUILayout.TextArea(Debug(AutoMonoBehaviour<CommConnectionManager>.Instance.Client.Monitor.AllEvents));
		GUILayout.TextArea(Debug(Singleton<GameStateController>.Instance.Client.Monitor.AllEvents));
		GUILayout.EndHorizontal();
	}

	private string Debug(LinkedList<string> list)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string item in list)
		{
			stringBuilder.AppendLine(item);
		}
		return stringBuilder.ToString();
	}
}
