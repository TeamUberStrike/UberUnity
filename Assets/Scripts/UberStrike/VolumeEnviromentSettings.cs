using UnityEngine;

[RequireComponent(typeof(Collider))]
public class VolumeEnviromentSettings : MonoBehaviour
{
	public EnviromentSettings Settings;

	private void Awake()
	{
		base.GetComponent<Collider>().isTrigger = true;
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (!(collider.tag == "Player"))
		{
			return;
		}
		GameState.Current.Player.MoveController.SetEnviroment(Settings, base.GetComponent<Collider>().bounds);
		if (Settings.Type == EnviromentSettings.TYPE.WATER)
		{
			float y = GameState.Current.Player.MoveController.Velocity.y;
			if (y < -20f)
			{
				AutoMonoBehaviour<SfxManager>.Instance.Play3dAudioClip(GameAudio.BigSplash, collider.transform.position, 1f);
			}
			else if (y < -10f)
			{
				AutoMonoBehaviour<SfxManager>.Instance.Play3dAudioClip(GameAudio.MediumSplash, collider.transform.position, 1f);
			}
		}
	}

	private void OnTriggerExit(Collider c)
	{
		if (c.tag == "Player")
		{
			GameState.Current.Player.MoveController.ResetEnviroment();
		}
	}
}
