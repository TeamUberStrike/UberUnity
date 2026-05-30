using System;
using System.Collections;
using UnityEngine;

public class GUIPageBase : MonoBehaviour
{
	[SerializeField]
	public float dismissDuration = 0.2f;

	[SerializeField]
	public float bringInDuration = 0.8f;

	public IEnumerator AnimateAlpha(float to, float duration, params UIButton[] buttons)
	{
		yield return StartCoroutine(AnimateAlpha(to, duration, Array.ConvertAll(buttons, (UIButton el) => el.GetComponent<UIPanel>())));
	}

	public IEnumerator AnimateAlpha(float to, float duration, params UIEventReceiver[] buttons)
	{
		yield return StartCoroutine(AnimateAlpha(to, duration, Array.ConvertAll(buttons, (UIEventReceiver el) => el.GetComponent<UIPanel>())));
	}

	public IEnumerator AnimateAlpha(float to, float duration, params GameObject[] objects)
	{
		yield return StartCoroutine(AnimateAlpha(to, duration, Array.ConvertAll(objects, (GameObject el) => el.GetComponent<UIPanel>())));
	}

	public IEnumerator AnimateAlpha(float to, float duration, params UIPanel[] buttons)
	{
		float duration2 = default(float);
		float to2 = default(float);
		TweenAlpha[] tweens = Array.ConvertAll(buttons, (UIPanel uIPanel) => TweenAlpha.Begin(uIPanel.gameObject, duration2, to2));
		TweenAlpha[] array = tweens;
		foreach (TweenAlpha el in array)
		{
			while (el.enabled)
			{
				yield return 0;
			}
		}
	}

	public void Dismiss(Action onFinished)
	{
		StopAllCoroutines();
		StartCoroutine(DismissCrt(onFinished));
	}

	private IEnumerator DismissCrt(Action onFinished)
	{
		yield return StartCoroutine(OnDismiss());
		if (onFinished != null)
		{
			onFinished();
		}
	}

	protected virtual IEnumerator OnDismiss()
	{
		yield return 0;
	}

	public void BringIn()
	{
		StopAllCoroutines();
		StartCoroutine(OnBringIn());
	}

	protected virtual IEnumerator OnBringIn()
	{
		yield return 0;
	}
}
