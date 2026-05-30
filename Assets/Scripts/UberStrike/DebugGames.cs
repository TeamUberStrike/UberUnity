using UberStrike.Core.Models;
using UnityEngine;

public class DebugGames : IDebugPage
{
	private Vector2 scroll;

	public string Title
	{
		get
		{
			return "Games";
		}
	}

	public void Draw()
	{
		if (Singleton<GameStateController>.Instance.Client.IsConnected)
		{
			if (Singleton<GameStateController>.Instance.Client.IsConnectedToLobby)
			{
				scroll = GUILayout.BeginScrollView(scroll);
				foreach (GameRoomData game in Singleton<GameListManager>.Instance.GameList)
				{
					GUILayout.BeginHorizontal();
					GUILayout.Label("[ID: " + game.Number + "] [Name: " + game.Name + "] [Players: " + game.ConnectedPlayers + "/" + game.PlayerLimit + "] [Time: " + game.TimeLimit + "]");
					if (GUILayout.Button("Close", GUILayout.Width(200f)))
					{
						Singleton<GameStateController>.Instance.Client.CloseGame(game.Number);
					}
					GUILayout.EndHorizontal();
				}
				GUILayout.EndScrollView();
			}
			else
			{
				GUILayout.FlexibleSpace();
				GUILayout.Label("Reconnect to the game lobby");
				if (GUILayout.Button(LocalizedStrings.Refresh, BlueStonez.buttondark_medium))
				{
					Singleton<GameStateController>.Instance.Client.RefreshGameLobby();
				}
				GUILayout.FlexibleSpace();
			}
		}
		else
		{
			GUILayout.FlexibleSpace();
			GUILayout.Label("You're not connected to a game server");
			GUILayout.FlexibleSpace();
		}
	}
}
