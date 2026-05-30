using System;
using System.Collections;
using UnityEngine;

public class HUDArmorBar : MonoBehaviour
{
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
	private float PulseSpeed = 20f;

	[SerializeField]
	private float PulseScale = 7f;

	private float oldValue = -1f;

	private float baseWidth;

	private float baseScale;

	private readonly float NORMAL_MAX = 100f;

	private void Start()
	{
		baseWidth = bgr.transform.localScale.x;
		baseScale = text.transform.localScale.x;
		GameState.Current.PlayerData.ArmorPoints.AddEventAndFire(OnArmorPoints, this);
	}

	public void OnArmorPoints(int value)
	{
		StopAllCoroutines();
		if ((float)value != oldValue)
		{
			StartCoroutine(PulseCrt((float)value >= oldValue));
		}
	}

	private void Update()
	{
		int value = GameState.Current.PlayerData.ArmorPoints.Value;
		if ((float)value != oldValue)
		{
			oldValue = Mathf.MoveTowards(oldValue, value, Time.deltaTime * animateSpeed);
			bgr.transform.localScale = bgr.transform.localScale.SetX(Mathf.Max(NORMAL_MAX, oldValue) / NORMAL_MAX * baseWidth);
			bar.transform.localScale = bgr.transform.localScale.SetX(oldValue / NORMAL_MAX * baseWidth);
			text.text = Mathf.FloorToInt(oldValue).ToString();
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
