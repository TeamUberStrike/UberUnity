using System.Collections;
using UnityEngine;

public class AudioFader : MonoBehaviour
{
	public float PlayLength = 5f;

	public float SilentLength = 5f;

	public float FadeInLength = 1f;

	public float FadeOutLength = 1f;

	private void OnEnable()
	{
		StartCoroutine(PlayAudio());
	}

	private IEnumerator PlayAudio()
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
		}
	}
}
