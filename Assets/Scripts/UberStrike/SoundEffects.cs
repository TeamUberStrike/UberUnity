using UnityEngine;

public class SoundEffects : MonoBehaviour
{
	public SoundEffectTunable HealthNoise_0_25;

	public SoundEffectTunable HealthHeartbeat_0_25;

	public SoundEffectTunable Health_100_200_Increase;

	public static SoundEffects Instance;

	private void Awake()
	{
		Instance = this;
		Object.DontDestroyOnLoad(Instance.gameObject);
	}
}
