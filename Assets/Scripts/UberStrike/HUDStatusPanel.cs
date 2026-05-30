using UberStrike.Core.Models;
using UnityEngine;

public class HUDStatusPanel : MonoBehaviour
{
	[SerializeField]
	private GameObject scores;

	[SerializeField]
	private UILabel timerLabel;

	[SerializeField]
	private UILabel statusLabel;

	[SerializeField]
	private UILabel blueLabel;

	[SerializeField]
	private UILabel redLabel;

	[SerializeField]
	private UILabel countDownLabel;

	[SerializeField]
	private UISprite blueBgr;

	[SerializeField]
	private UISprite redBgr;

	[SerializeField]
	private UISprite blueTriangle;

	[SerializeField]
	private UISprite redTriangle;

	[SerializeField]
	private UITweener timerTween;

	[SerializeField]
	private UIPanel mainPanel;

	private static int WARNING_TIME_LOW_VALUE = 30;

	private int remainingSeconds;

	private int remainingKillsRounds;

	private int RemainingSeconds
	{
		get
		{
			return remainingSeconds;
		}
		set
		{
			if (remainingSeconds != value)
			{
				remainingSeconds = Mathf.Max(value, 0);
				timerLabel.text = GetClockString(remainingSeconds);
				OnUpdateRemainingSeconds();
			}
		}
	}

	private int KillsRemaining
	{
		set
		{
			int num = Mathf.Max(value, 0);
			if (remainingKillsRounds != num)
			{
				remainingKillsRounds = value;
				statusLabel.text = GetRemainingKillString(value);
			}
		}
	}

	private void OnEnable()
	{
		GameState.Current.PlayerData.Team.Fire();
		EventHandler.Global.AddListener<GameEvents.MatchCountdown>(OnMatchStartCountdownEvent);
	}

	private void OnDisable()
	{
		EventHandler.Global.RemoveListener<GameEvents.MatchCountdown>(OnMatchStartCountdownEvent);
	}

	private void Start()
	{
		countDownLabel.gameObject.SetActive(false);
		GameState.Current.PlayerData.RemainingTime.AddEventAndFire(delegate(int el)
		{
			RemainingSeconds = el;
		}, this);
		GameState.Current.PlayerData.RemainingKills.AddEventAndFire(delegate(int el)
		{
			KillsRemaining = el;
		}, this);
		GameState.Current.PlayerData.Team.AddEventAndFire(delegate(TeamID el)
		{
			blueBgr.color = ((el != TeamID.BLUE) ? GUIUtils.ColorBlue.SetAlpha(16f / 51f) : GUIUtils.ColorBlue.SetAlpha(1f));
			redBgr.color = ((el != TeamID.RED) ? GUIUtils.ColorRed.SetAlpha(16f / 51f) : GUIUtils.ColorRed.SetAlpha(1f));
			blueTriangle.enabled = el == TeamID.BLUE;
			redTriangle.enabled = el == TeamID.RED;
			SetupGameMode(GameState.Current.IsTeamGame);
		}, this);
	}

	private void SetupGameMode(bool isTeamGame)
	{
		scores.SetActive(isTeamGame);
		if (isTeamGame)
		{
			GameState.Current.PlayerData.BlueTeamScore.AddEventAndFire(delegate(int el)
			{
				blueLabel.text = el.ToString();
			}, this);
			GameState.Current.PlayerData.RedTeamScore.AddEventAndFire(delegate(int el)
			{
				redLabel.text = el.ToString();
			}, this);
		}
	}

	private void OnMatchStartCountdownEvent(GameEvents.MatchCountdown ev)
	{
		scores.SetActive(ev.Countdown < 1);
		timerLabel.gameObject.SetActive(ev.Countdown < 1);
		statusLabel.text = LocalizedStrings.StartsInCaps;
		countDownLabel.text = ev.Countdown.ToString();
		UITweener.Begin<TweenScale>(countDownLabel.gameObject, 0.5f);
		UITweener.Begin<TweenAlpha>(countDownLabel.gameObject, 0.25f);
		countDownLabel.gameObject.SetActive(ev.Countdown > 0);
	}

	private void OnUpdateRemainingSeconds()
	{
		if (remainingSeconds > WARNING_TIME_LOW_VALUE || remainingSeconds <= 0)
		{
			StopPulse();
			return;
		}
		UITweener.Begin<TweenScale>(timerLabel.gameObject, 0.5f);
		switch (RemainingSeconds)
		{
		case 5:
			AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.MatchEndingCountdown5, 0uL, 1f, 1f);
			break;
		case 4:
			AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.MatchEndingCountdown4, 0uL, 1f, 1f);
			break;
		case 3:
			AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.MatchEndingCountdown3, 0uL, 1f, 1f);
			break;
		case 2:
			AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.MatchEndingCountdown2, 0uL, 1f, 1f);
			break;
		case 1:
			AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.MatchEndingCountdown1, 0uL, 1f, 1f);
			break;
		}
	}

	private void StopPulse()
	{
		if (timerTween.enabled)
		{
			timerTween.Reset();
			timerTween.enabled = false;
		}
	}

	public void IsOnPaused(bool isPaused)
	{
		SpringPosition.Begin(mainPanel.gameObject, new Vector3(0f, (!isPaused) ? 0f : (-60f), 0f), 10f).onFinished = delegate(SpringPosition el)
		{
			el.enabled = false;
		};
	}

	private string GetRemainingKillString(int remainingKills)
	{
		if (remainingKills > 1)
		{
			return string.Format(LocalizedStrings.NKillsLeft, remainingKills);
		}
		return LocalizedStrings.OneKillLeft;
	}

	private string GetRemainingRoundsString(int remainingRounds)
	{
		if (remainingRounds == 1)
		{
			if (GameState.Current.ScoreBlue > GameState.Current.ScoreRed)
			{
				return string.Format(LocalizedStrings.FinalRoundX, LocalizedStrings.BlueCaps);
			}
			if (GameState.Current.ScoreRed > GameState.Current.ScoreBlue)
			{
				return string.Format(LocalizedStrings.FinalRoundX, LocalizedStrings.RedCaps);
			}
			return LocalizedStrings.FinalRoundCaps;
		}
		return string.Format(LocalizedStrings.NRoundsLeft, remainingRounds);
	}

	private string GetClockString(int remainingSeconds)
	{
		int num = remainingSeconds / 60;
		int num2 = remainingSeconds % 60;
		string text = ((num < 10) ? ("0" + num) : num.ToString());
		string text2 = ((num2 < 10) ? ("0" + num2) : num2.ToString());
		return text + ":" + text2;
	}
}
