using Cmune.DataCenter.Common.Entities;
using UberStrike.Core.Models;
using UnityEngine;

public class HUDJoinButtons : MonoBehaviour
{
	[SerializeField]
	private UIPanel panel;

	[SerializeField]
	private UIEventReceiver joinBlue;

	[SerializeField]
	private UIEventReceiver joinRed;

	[SerializeField]
	private UIEventReceiver join;

	[SerializeField]
	private UIEventReceiver spectate;

	[SerializeField]
	private UISprite[] blueBars;

	[SerializeField]
	private UISprite[] redBars;

	[SerializeField]
	private UISprite[] bars;

	private void Start()
	{
		spectate.gameObject.SetActive(PlayerDataManager.AccessLevel >= MemberAccessLevel.QA);
		PropertyExt.AddEvent(GameData.Instance.OnEndOfMatchTimer, delegate
		{
			panel.gameObject.SetActive(true);
		}, this);
		joinBlue.OnClicked = delegate
		{
			GamePageManager.Instance.UnloadCurrentPage();
			GameState.Current.Actions.JoinTeam(TeamID.BLUE);
		};
		joinRed.OnClicked = delegate
		{
			GamePageManager.Instance.UnloadCurrentPage();
			GameState.Current.Actions.JoinTeam(TeamID.RED);
		};
		join.OnClicked = delegate
		{
			GamePageManager.Instance.UnloadCurrentPage();
			GameState.Current.Actions.JoinTeam(TeamID.NONE);
		};
		spectate.OnClicked = delegate
		{
			GamePageManager.Instance.UnloadCurrentPage();
			GameState.Current.PlayerData.Team = new Property<TeamID>(TeamID.NONE);
			GameState.Current.Actions.JoinAsSpectator();
		};
	}

	private void OnEnable()
	{
		bool isTeamGame = GameState.Current.IsTeamGame;
		if (GameState.Current.MatchState.CurrentStateId != GameStateId.PregameLoadout)
		{
			panel.gameObject.SetActive(false);
		}
		joinBlue.gameObject.SetActive(isTeamGame);
		joinRed.gameObject.SetActive(isTeamGame);
		join.gameObject.SetActive(!isTeamGame);
		UISprite[] array = blueBars;
		foreach (UISprite uISprite in array)
		{
			uISprite.enabled = isTeamGame;
		}
		UISprite[] array2 = redBars;
		foreach (UISprite uISprite2 in array2)
		{
			uISprite2.enabled = isTeamGame;
		}
		UISprite[] array3 = bars;
		foreach (UISprite uISprite3 in array3)
		{
			uISprite3.enabled = !isTeamGame;
		}
	}

	private void Update()
	{
		if (GameState.Current.IsTeamGame)
		{
			int value = Mathf.CeilToInt((float)GameState.Current.RoomData.PlayerLimit / 2f);
			joinBlue.GetComponent<UIButton>().isEnabled = GameState.Current.CanJoinBlueTeam;
			int blueTeamPlayerCount = GameState.Current.BlueTeamPlayerCount;
			int num = Mathf.Clamp(value, 0, blueBars.Length);
			for (int i = 0; i < blueBars.Length; i++)
			{
				blueBars[i].enabled = i < num;
				blueBars[i].color = ((i >= blueTeamPlayerCount) ? GUIUtils.ColorBlack : GUIUtils.ColorBlue);
			}
			joinRed.GetComponent<UIButton>().isEnabled = GameState.Current.CanJoinRedTeam;
			int redTeamPlayerCount = GameState.Current.RedTeamPlayerCount;
			int num2 = Mathf.Clamp(value, 0, redBars.Length);
			for (int j = 0; j < redBars.Length; j++)
			{
				redBars[j].enabled = j < num2;
				redBars[j].color = ((j >= redTeamPlayerCount) ? GUIUtils.ColorBlack : GUIUtils.ColorRed);
			}
		}
		else
		{
			int playerLimit = GameState.Current.RoomData.PlayerLimit;
			int count = GameState.Current.Players.Count;
			for (int k = 0; k < bars.Length; k++)
			{
				bars[k].enabled = k < playerLimit;
				bars[k].color = ((k >= count) ? GUIUtils.ColorBlack : GUIUtils.ColorBlue);
			}
		}
	}
}
