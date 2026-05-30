using UnityEngine;

public class Teleport : MonoBehaviour
{
	[SerializeField]
	private Transform _spawnPoint;

	[SerializeField]
	private AudioClip _sound;

	private AudioSource _audio;

	private void Awake()
	{
		_audio = GetComponent<AudioSource>();
	}

	private void OnTriggerEnter(Collider c)
	{
		if (c.tag == "Player")
		{
			if ((bool)_audio)
			{
				AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(_sound, 0uL, 1f, 1f);
			}
			GameState.Current.Player.SpawnPlayerAt(_spawnPoint.position, _spawnPoint.rotation);
		}
		else if (c.tag == "Prop")
		{
			c.transform.position = _spawnPoint.position;
		}
	}
}
