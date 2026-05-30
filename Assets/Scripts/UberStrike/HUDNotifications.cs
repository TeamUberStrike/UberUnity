using System.Collections;
using UberStrike.Core.Models;
using UberStrike.Core.Types;
using UnityEngine;

public class HUDNotifications : MonoBehaviour
{
	[SerializeField]
	private UIPanel panel;

	[SerializeField]
	private UILabel labelBig;

	[SerializeField]
	private UILabel labelSmall;

	[SerializeField]
	private float defaultFadeInSpeed = 20f;

	[SerializeField]
	private float defaultFadeOutSpeed = 5f;

	private float lastKillTime;

	private int killCounter;

	private void Start()
	{
		GameData.Instance.OnNotification.AddEvent(delegate(string el)
		{
			Show(el, string.Empty, 1f);
		}, this);
		GameData.Instance.OnNotificationFull.AddEvent(delegate(string el1, string el2, float duration)
		{
			Show(el1, el2, duration);
		}, this);
		GameData.Instance.GameState.AddEventAndFire(delegate(GameStateId el)
		{
			switch (el)
			{
			case GameStateId.PrepareNextRound:
				Show(GetGameModeName(), GetGameModeHint(), 0f);
				break;
			case GameStateId.MatchRunning:
				Show(string.Empty, "Fight!", 1f);
				break;
			default:
				Hide();
				break;
			}
		}, this);
		GameData.Instance.OnPlayerKilled.AddEvent(OnPlayerKilled, this);
		panel.alpha = 0f;
	}

	private void OnPlayerKilled(GameActorInfo shooter, GameActorInfo target, UberstrikeItemClass weapon, BodyPart bodyPart)
	{
		if (target == null || shooter == null || shooter.Cmid != PlayerDataManager.Cmid || target.Cmid == PlayerDataManager.Cmid)
		{
			return;
		}
		bool flag = Time.time < lastKillTime + 10f;
		killCounter = ((!flag) ? 1 : (killCounter + 1));
		lastKillTime = Time.time;
		if (weapon == UberstrikeItemClass.WeaponMelee)
		{
			AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.Smackdown, 0uL, 1f, 1f);
		}
		else
		{
			switch (bodyPart)
			{
			case BodyPart.Head:
				AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.HeadShot, 0uL, 1f, 1f);
				break;
			case BodyPart.Nuts:
				AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.GotNutshotKill, 0uL, 1f, 1f);
				AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.NutShot, 0uL, 1f, 1f);
				break;
			}
		}
		string textBig = string.Empty;
		if (killCounter == 2)
		{
			textBig = "DOUBLE KILL";
			AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.DoubleKill, 1000uL, 1f, 1f);
		}
		else if (killCounter == 3)
		{
			textBig = "TRIPLE KILL";
			AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.TripleKill, 1000uL, 1f, 1f);
		}
		else if (killCounter == 4)
		{
			textBig = "QUAD KILL";
			AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.QuadKill, 1000uL, 1f, 1f);
		}
		else if (killCounter == 5)
		{
			textBig = "MEGA KILL";
			AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.MegaKill, 1000uL, 1f, 1f);
		}
		else if (killCounter > 5)
		{
			textBig = "UBER KILL";
			if (killCounter == 6)
			{
				AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.UberKill, 1000uL, 1f, 1f);
			}
		}
		Show(textBig, "You killed " + target.PlayerName, 1f);
	}

	private string GetGameModeName()
	{
		switch (GameState.Current.GameMode)
		{
		case GameModeType.DeathMatch:
			return LocalizedStrings.DeathMatch;
		case GameModeType.TeamDeathMatch:
			return LocalizedStrings.TeamDeathMatch;
		case GameModeType.EliminationMode:
			return LocalizedStrings.TeamElimination;
		default:
			return string.Empty;
		}
	}

	private string GetGameModeHint()
	{
		if (GameState.Current.GameMode == GameModeType.DeathMatch)
		{
			return "Get as many kills as you can before the time runs out";
		}
		return "Get as many kills for your team as you can\nbefore the time runs out";
	}

	public void Hide(bool immediate = false)
	{
		StopAllCoroutines();
		if (immediate)
		{
			panel.alpha = 0f;
		}
		else
		{
			StartCoroutine(HideCrt(defaultFadeOutSpeed));
		}
	}

	public void Show(string textBig, string textSmall, float duration = 1f)
	{
		Show(textBig, textSmall, defaultFadeInSpeed, defaultFadeOutSpeed, duration);
	}

	public void Show(string textBig, string textSmall, float fadeInSpeed, float fadeOutSpeed, float duration = 1f)
	{
		StopAllCoroutines();
		StartCoroutine(ShowCrt(textBig, textSmall, fadeInSpeed, fadeOutSpeed, duration));
	}

	public IEnumerator ShowCrt(string textBig, string textSmall, float fadeInSpeed, float fadeOutSpeed, float duration)
	{
		if (labelBig.text != textBig || labelSmall.text != textSmall)
		{
			panel.alpha = 0f;
		}
		labelBig.text = textBig;
		labelSmall.text = textSmall;
		while (panel.alpha < 1f)
		{
			panel.alpha = Mathf.MoveTowards(panel.alpha, 1f, Time.deltaTime * fadeInSpeed);
			yield return 0;
		}
		if (duration > 0f)
		{
			yield return new WaitForSeconds(duration);
			yield return StartCoroutine(HideCrt(fadeOutSpeed));
		}
	}

	private IEnumerator HideCrt(float fadeOutSpeed)
	{
		while (panel.alpha > 0f)
		{
			panel.alpha = Mathf.MoveTowards(panel.alpha, 0f, Time.deltaTime * fadeOutSpeed);
			yield return 0;
		}
	}
}
