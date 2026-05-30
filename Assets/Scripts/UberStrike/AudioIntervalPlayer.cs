using System.Collections;
using UnityEngine;

public class AudioIntervalPlayer : MonoBehaviour
{
	[SerializeField]
	private float waitTime = 30f;

	[SerializeField]
	private bool waitForClipLength;

	private IEnumerator Start()
	{
		base.GetComponent<AudioSource>().loop = false;
		while (true)
		{
			base.GetComponent<AudioSource>().Play();
			if (waitForClipLength && base.GetComponent<AudioSource>().clip != null)
			{
				yield return new WaitForSeconds(base.GetComponent<AudioSource>().clip.length);
			}
			yield return new WaitForSeconds(waitTime);
		}
	}
}
