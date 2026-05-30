using System;
using System.Collections;
using UnityEngine;

public class HUDHealthBar : MonoBehaviour
{
	[SerializeField]
	private UIPanel panel;

	[SerializeField]
	private UISprite bar;

	[SerializeField]
	private UISprite bgr;

	[SerializeField]
	private UISprite icon;

	[SerializeField]
	private UILabel text;

	[SerializeField]
	private float animateSpeed = 200f;

	[SerializeField]
	private float criticalBlinkingSpeed = 6.5f;

	[SerializeField]
	private float PulseSpeed = 20f;

	[SerializeField]
	private float PulseScale = 7f;

	private float oldValue = -1f;

	private float baseWidth;

	private float baseScale;

	private Color normalBarColor;

	private int criticalHealthLastTime;

	private readonly float CRITICAL_VALUE = 25f;

	private readonly float NORMAL_MAX = 100f;

	private void Start()
	{
		baseWidth = bgr.transform.localScale.x;
		baseScale = text.transform.localScale.x;
		normalBarColor = bar.color;
		GameState.Current.PlayerData.Health.AddEventAndFire(OnHealthPoints, this);
	}

	private void OnEnable()
	{
		GameState.Current.PlayerData.Health.Fire();
	}

	public void OnHealthPoints(int value, int oldValue)
	{
		if ((GameStateId)GameData.Instance.GameState != GameStateId.None && oldValue > 0 && oldValue <= 100 && value > 100)
		{
			AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(SoundEffects.Instance.Health_100_200_Increase.Interpolate(value, 100f, 200f));
		}
		StopAllCoroutines();
		if (value != oldValue)
		{
			StartCoroutine(PulseCrt(value >= oldValue));
		}
		StartCoroutine(AnimateCrt(value));
	}

	private IEnumerator AnimateCrt(int value)
	{
		panel.alpha = 1f;
		while ((float)value != oldValue)
		{
			oldValue = Mathf.MoveTowards(oldValue, value, Time.deltaTime * animateSpeed);
			bgr.transform.localScale = bgr.transform.localScale.SetX(Mathf.Max(NORMAL_MAX, oldValue) / NORMAL_MAX * baseWidth);
			bar.transform.localScale = bgr.transform.localScale.SetX(oldValue / NORMAL_MAX * baseWidth);
			bar.color = ((!(oldValue > CRITICAL_VALUE)) ? new Color(1f, 0.23529412f, 16f / 85f) : normalBarColor);
			text.text = Mathf.FloorToInt(oldValue).ToString();
			if (oldValue == 0f || oldValue > CRITICAL_VALUE)
			{
				AutoMonoBehaviour<SfxManager>.Instance.StopLoopedAudioClip();
			}
			yield return 0;
		}
		if (!(oldValue <= CRITICAL_VALUE) || !(oldValue > 0f))
		{
			yield break;
		}
		AutoMonoBehaviour<SfxManager>.Instance.PlayLoopedAudioClip(SoundEffects.Instance.HealthNoise_0_25.Interpolate(oldValue, 0f, CRITICAL_VALUE));
		while (true)
		{
			float time = Time.time * criticalBlinkingSpeed;
			panel.alpha = Mathf.Clamp01(Mathf.Sin(time) + 1f);
			if (time % ((float)Math.PI * 2f) >= (float)Math.PI && criticalHealthLastTime != (int)(time / ((float)Math.PI * 2f)))
			{
				criticalHealthLastTime = (int)(time / ((float)Math.PI * 2f));
				AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(SoundEffects.Instance.HealthHeartbeat_0_25.Interpolate(oldValue, 0f, CRITICAL_VALUE));
			}
			yield return 0;
		}
	}

	private IEnumerator PulseCrt(bool up)
	{
		float time = 0f;
		while (true)
		{
			time = Mathf.Min(time + Time.deltaTime * PulseSpeed, (float)Math.PI);
			float pulse = Mathf.Sin(time) * PulseScale;
			text.transform.localScale = (Vector3.one * (baseScale + pulse * (float)(up ? 1 : (-1)))).SetZ(1f);
			if (time >= (float)Math.PI)
			{
				break;
			}
			yield return 0;
		}
	}
}
