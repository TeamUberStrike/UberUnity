using System.Collections;
using System.Collections.Generic;
using UberStrike.Core.Models;
using UnityEngine;

public class HUDScore : MonoBehaviour
{
	[SerializeField]
	private UIPanel panel;

	[SerializeField]
	private UILabel timerLabel;

	[SerializeField]
	private UILabel blueLabel;

	[SerializeField]
	private UISprite blueBgr;

	[SerializeField]
	private UILabel redLabel;

	[SerializeField]
	private UISprite redBgr;

	[SerializeField]
	private UILabel titleLabel;

	private void OnEnable()
	{
		int num = (((TeamID)GameState.Current.PlayerData.Team != TeamID.BLUE) ? GameState.Current.ScoreRed : GameState.Current.ScoreBlue);
		int num2 = (((TeamID)GameState.Current.PlayerData.Team != TeamID.BLUE) ? GameState.Current.ScoreBlue : GameState.Current.ScoreRed);
		panel.gameObject.SetActive(true);
		blueLabel.text = GameState.Current.ScoreBlue.ToString();
		redLabel.text = GameState.Current.ScoreRed.ToString();
		bool isTeamGame = GameState.Current.IsTeamGame;
		blueLabel.enabled = isTeamGame;
		blueBgr.enabled = isTeamGame;
		redLabel.enabled = isTeamGame;
		redBgr.enabled = isTeamGame;
		if (isTeamGame)
		{
			if (num > num2)
			{
				titleLabel.text = "Your Team Won!";
			}
			else if (num < num2)
			{
				titleLabel.text = "Your Team Lost";
			}
			else
			{
				titleLabel.text = "Draw";
			}
		}
		else
		{
			List<GameActorInfo> list = new List<GameActorInfo>(GameState.Current.Players.Values);
			int maxScore = list.Reduce((GameActorInfo player, int prev) => Mathf.Max(player.Kills, prev), int.MinValue);
			List<GameActorInfo> list2 = list.FindAll((GameActorInfo el) => el.Kills == maxScore);
			string str = string.Empty;
			list2.ForEach(delegate(GameActorInfo el)
			{
				str = str + el.PlayerName + " ";
			});
			titleLabel.text = str + "won!";
		}
		StartCoroutine(Wait5Seconds());
	}

	private IEnumerator Wait5Seconds()
	{
		for (int i = 5; i > 0; i--)
		{
			timerLabel.text = i.ToString();
			UITweener.Begin<TweenScale>(timerLabel.gameObject, 0.5f);
			UITweener.Begin<TweenAlpha>(timerLabel.gameObject, 0.25f);
			yield return new WaitForSeconds(1f);
		}
		panel.gameObject.SetActive(false);
		GameData.Instance.OnEndOfMatchTimer.Fire();
	}
}
