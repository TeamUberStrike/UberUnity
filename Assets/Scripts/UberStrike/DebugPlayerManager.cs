using UberStrike.Core.Models;
using UberStrike.Realtime.UnitySdk;
using UnityEngine;

public class DebugPlayerManager : IDebugPage
{
	private Vector2 v1;

	public string Title
	{
		get
		{
			return "Players";
		}
	}

	public void Draw()
	{
		v1 = GUILayout.BeginScrollView(v1);
		GUILayout.BeginHorizontal();
		foreach (GameActorInfo value in GameState.Current.Players.Values)
		{
			ICharacterState characterState = GameState.Current.RemotePlayerStates.GetState(value.PlayerId);
			if (value.Cmid == PlayerDataManager.Cmid)
			{
				characterState = GameState.Current.PlayerData;
			}
			GUILayout.BeginVertical();
			GUILayout.Label(value.ToCustomString());
			if (characterState != null)
			{
				GUILayout.Label("Keys: " + CmunePrint.Flag(characterState.KeyState));
				GUILayout.Label("Move: " + CmunePrint.Flag(characterState.MovementState));
				float num = Mathf.Clamp(characterState.VerticalRotation + 90f, 0f, 180f) / 180f;
				GUILayout.Label(string.Concat("Rotation: ", characterState.HorizontalRotation, "/", characterState.VerticalRotation.ToString("f2"), "/", num.ToString("f2")));
				GUILayout.Label("Position: " + characterState.Position);
				GUILayout.Label("Velocity: " + characterState.Velocity);
			}
			GUI.contentColor = ((value.TeamID == TeamID.RED) ? Color.red : ((value.TeamID != TeamID.BLUE) ? Color.white : Color.blue));
			GUILayout.Label("AVATAR: " + GameState.Current.Avatars.ContainsKey(value.Cmid));
			if (value.Cmid != PlayerDataManager.Cmid && GUILayout.Button("Kick"))
			{
				GameState.Current.Actions.KickPlayer(value.Cmid);
			}
			GUI.contentColor = Color.white;
			GUILayout.EndVertical();
		}
		GUILayout.EndHorizontal();
		GUILayout.EndScrollView();
	}
}
