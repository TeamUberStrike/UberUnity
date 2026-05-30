using System.Collections;
using UnityEngine;

internal class RobotPiecesLogic : MonoBehaviour
{
	[SerializeField]
	private AudioClip[] _robotScrapsDestructionAudios;

	[SerializeField]
	private GameObject _robotPieces;

	public void ExplodeRobot(GameObject robotObject, int lifeTimeMilliSeconds)
	{
		if (_robotPieces != null)
		{
			Rigidbody[] componentsInChildren = _robotPieces.GetComponentsInChildren<Rigidbody>();
			foreach (Rigidbody rigidbody in componentsInChildren)
			{
				rigidbody.AddExplosionForce(5f, base.transform.position, 2f, 0f, ForceMode.Impulse);
			}
		}
		if (_robotScrapsDestructionAudios != null && _robotScrapsDestructionAudios.Length > 0)
		{
			AudioClip audioClip = _robotScrapsDestructionAudios[Random.Range(0, _robotScrapsDestructionAudios.Length)];
			if ((bool)audioClip)
			{
				base.GetComponent<AudioSource>().clip = audioClip;
				base.GetComponent<AudioSource>().Play();
			}
		}
		StartCoroutine(DestroyRobotPieces(robotObject, lifeTimeMilliSeconds));
	}

	public void PlayRobotScrapsDestructionAudio()
	{
		if (_robotScrapsDestructionAudios != null && _robotScrapsDestructionAudios.Length > 0)
		{
			AudioClip audioClip = _robotScrapsDestructionAudios[Random.Range(0, _robotScrapsDestructionAudios.Length)];
			if ((bool)audioClip)
			{
				base.GetComponent<AudioSource>().clip = audioClip;
				base.GetComponent<AudioSource>().Play();
			}
		}
	}

	private IEnumerator DestroyRobotPieces(GameObject robotObject, int lifeTimeMilliSeconds)
	{
		yield return new WaitForSeconds(lifeTimeMilliSeconds / 1000);
		PlayRobotScrapsDestructionAudio();
		yield return new WaitForSeconds(base.GetComponent<AudioSource>().clip.length);
		Object.Destroy(robotObject);
	}
}
