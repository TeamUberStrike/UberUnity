using System.Collections.Generic;
using UnityEngine;

public class SfxManager : AutoMonoBehaviour<SfxManager>
{
	private class AudioPool
	{
		private Queue<AudioSource> audioPool;

		public AudioPool(int size = 10)
		{
			Transform transform = new GameObject("AudioPool").transform;
			Object.DontDestroyOnLoad(transform);
			audioPool = new Queue<AudioSource>();
			for (int i = 0; i < size; i++)
			{
				AudioSource component = new GameObject("AudioSource", typeof(AudioSource)).GetComponent<AudioSource>();
				component.gameObject.transform.parent = transform;
				component.playOnAwake = false;
				component.minDistance = 0f;
				component.maxDistance = 80f;
				component.rolloffMode = AudioRolloffMode.Custom;
				component.loop = false;
				audioPool.Enqueue(component);
			}
		}

		public void PlayClipAtPoint(AudioClip clip, Vector3 position, float volume)
		{
			if (clip != null)
			{
				AudioSource audioSource = audioPool.Dequeue();
				audioSource.transform.position = position;
				audioSource.clip = clip;
				audioSource.volume = volume;
				audioSource.Play();
				audioPool.Enqueue(audioSource);
			}
		}
	}

	private AudioPool audioPool;

	private AudioSource gameAudioSource;

	private AudioSource uiAudioSource;

	private AudioSource uiAudioSourceLooped;

	private AudioClip[] _footStepDirt;

	private AudioClip[] _footStepGrass;

	private AudioClip[] _footStepMetal;

	private AudioClip[] _footStepHeavyMetal;

	private AudioClip[] _footStepRock;

	private AudioClip[] _footStepSand;

	private AudioClip[] _footStepWater;

	private AudioClip[] _footStepWood;

	private AudioClip[] _swimAboveWater;

	private AudioClip[] _swimUnderWater;

	private AudioClip[] _footStepSnow;

	private AudioClip[] _footStepGlass;

	private Dictionary<string, AudioClip[]> _surfaceImpactSoundMap;

	public static float EffectsAudioVolume
	{
		get
		{
			return ApplicationDataManager.ApplicationOptions.AudioEffectsVolume;
		}
	}

	public static float MusicAudioVolume
	{
		get
		{
			return ApplicationDataManager.ApplicationOptions.AudioMusicVolume;
		}
	}

	public static float MasterAudioVolume
	{
		get
		{
			return ApplicationDataManager.ApplicationOptions.AudioMasterVolume;
		}
	}

	private void Awake()
	{
		audioPool = new AudioPool();
		gameAudioSource = base.gameObject.AddComponent<AudioSource>();
		gameAudioSource.loop = false;
		gameAudioSource.playOnAwake = false;
		gameAudioSource.rolloffMode = AudioRolloffMode.Linear;
		gameAudioSource.priority = 100;
		uiAudioSource = base.gameObject.AddComponent<AudioSource>();
		uiAudioSource.loop = false;
		uiAudioSource.playOnAwake = false;
		uiAudioSource.rolloffMode = AudioRolloffMode.Linear;
		uiAudioSource.priority = 100;
		uiAudioSourceLooped = base.gameObject.AddComponent<AudioSource>();
		uiAudioSourceLooped.loop = true;
		uiAudioSourceLooped.playOnAwake = false;
		uiAudioSourceLooped.rolloffMode = AudioRolloffMode.Linear;
		_footStepDirt = new AudioClip[4]
		{
			GameAudio.FootStepDirt1,
			GameAudio.FootStepDirt2,
			GameAudio.FootStepDirt3,
			GameAudio.FootStepDirt4
		};
		_footStepGrass = new AudioClip[4]
		{
			GameAudio.FootStepGrass1,
			GameAudio.FootStepGrass2,
			GameAudio.FootStepGrass3,
			GameAudio.FootStepGrass4
		};
		_footStepMetal = new AudioClip[4]
		{
			GameAudio.FootStepMetal1,
			GameAudio.FootStepMetal2,
			GameAudio.FootStepMetal3,
			GameAudio.FootStepMetal4
		};
		_footStepHeavyMetal = new AudioClip[4]
		{
			GameAudio.FootStepHeavyMetal1,
			GameAudio.FootStepHeavyMetal2,
			GameAudio.FootStepHeavyMetal3,
			GameAudio.FootStepHeavyMetal4
		};
		_footStepRock = new AudioClip[4]
		{
			GameAudio.FootStepRock1,
			GameAudio.FootStepRock2,
			GameAudio.FootStepRock3,
			GameAudio.FootStepRock4
		};
		_footStepSand = new AudioClip[4]
		{
			GameAudio.FootStepSand1,
			GameAudio.FootStepSand2,
			GameAudio.FootStepSand3,
			GameAudio.FootStepSand4
		};
		_footStepWater = new AudioClip[3]
		{
			GameAudio.FootStepWater1,
			GameAudio.FootStepWater2,
			GameAudio.FootStepWater3
		};
		_footStepWood = new AudioClip[4]
		{
			GameAudio.FootStepWood1,
			GameAudio.FootStepWood2,
			GameAudio.FootStepWood3,
			GameAudio.FootStepWood4
		};
		_swimAboveWater = new AudioClip[4]
		{
			GameAudio.SwimAboveWater1,
			GameAudio.SwimAboveWater2,
			GameAudio.SwimAboveWater3,
			GameAudio.SwimAboveWater4
		};
		_swimUnderWater = new AudioClip[1] { GameAudio.SwimUnderWater };
		_footStepSnow = new AudioClip[4]
		{
			GameAudio.FootStepSnow1,
			GameAudio.FootStepSnow2,
			GameAudio.FootStepSnow3,
			GameAudio.FootStepSnow4
		};
		_footStepGlass = new AudioClip[4]
		{
			GameAudio.FootStepGlass1,
			GameAudio.FootStepGlass2,
			GameAudio.FootStepGlass3,
			GameAudio.FootStepGlass4
		};
		AudioClip[] value = new AudioClip[4]
		{
			GameAudio.ImpactCement1,
			GameAudio.ImpactCement2,
			GameAudio.ImpactCement3,
			GameAudio.ImpactCement4
		};
		AudioClip[] value2 = new AudioClip[5]
		{
			GameAudio.ImpactGlass1,
			GameAudio.ImpactGlass2,
			GameAudio.ImpactGlass3,
			GameAudio.ImpactGlass4,
			GameAudio.ImpactGlass5
		};
		AudioClip[] value3 = new AudioClip[4]
		{
			GameAudio.ImpactGrass1,
			GameAudio.ImpactGrass2,
			GameAudio.ImpactGrass3,
			GameAudio.ImpactGrass4
		};
		AudioClip[] value4 = new AudioClip[5]
		{
			GameAudio.ImpactMetal1,
			GameAudio.ImpactMetal2,
			GameAudio.ImpactMetal3,
			GameAudio.ImpactMetal4,
			GameAudio.ImpactMetal5
		};
		AudioClip[] value5 = new AudioClip[5]
		{
			GameAudio.ImpactSand1,
			GameAudio.ImpactSand2,
			GameAudio.ImpactSand3,
			GameAudio.ImpactSand4,
			GameAudio.ImpactSand5
		};
		AudioClip[] value6 = new AudioClip[5]
		{
			GameAudio.ImpactStone1,
			GameAudio.ImpactStone2,
			GameAudio.ImpactStone3,
			GameAudio.ImpactStone4,
			GameAudio.ImpactStone5
		};
		AudioClip[] value7 = new AudioClip[5]
		{
			GameAudio.ImpactWater1,
			GameAudio.ImpactWater2,
			GameAudio.ImpactWater3,
			GameAudio.ImpactWater4,
			GameAudio.ImpactWater5
		};
		AudioClip[] value8 = new AudioClip[5]
		{
			GameAudio.ImpactWood1,
			GameAudio.ImpactWood2,
			GameAudio.ImpactWood3,
			GameAudio.ImpactWood4,
			GameAudio.ImpactWood5
		};
		_surfaceImpactSoundMap = new Dictionary<string, AudioClip[]>
		{
			{ "Wood", value8 },
			{ "SolidWood", value8 },
			{ "Stone", value6 },
			{ "Metal", value4 },
			{ "Sand", value5 },
			{ "Grass", value3 },
			{ "Glass", value2 },
			{ "Cement", value },
			{ "Water", value7 }
		};
	}

	public void PlayInGameAudioClip(AudioClip audioClip, ulong delay = 0)
	{
		if (audioClip != null)
		{
			AutoMonoBehaviour<SfxManager>.Instance.gameAudioSource.PlayOneShot(audioClip);
		}
	}

	public void Play2dAudioClip(SoundEffect sound)
	{
		Play2dAudioClip(sound.Clip, 0uL, sound.Volume, sound.Pitch);
	}

	public void Play2dAudioClip(AudioClip audioClip, ulong delay = 0, float volume = 1f, float pitch = 1f)
	{
		if (!(audioClip == null))
		{
			if (delay != 0)
			{
				AutoMonoBehaviour<SfxManager>.Instance.uiAudioSource.clip = audioClip;
				AutoMonoBehaviour<SfxManager>.Instance.uiAudioSource.PlayDelayed(delay);
			}
			else
			{
				AutoMonoBehaviour<SfxManager>.Instance.uiAudioSource.PlayOneShot(audioClip);
			}
			ApplicationOptions applicationOptions = ApplicationDataManager.ApplicationOptions;
			float num = ((!applicationOptions.AudioEnabled) ? 0f : (applicationOptions.AudioEffectsVolume * applicationOptions.AudioMasterVolume));
			AutoMonoBehaviour<SfxManager>.Instance.uiAudioSource.volume = num * volume;
			AutoMonoBehaviour<SfxManager>.Instance.uiAudioSource.pitch = pitch;
		}
	}

	public void PlayLoopedAudioClip(SoundEffect sound)
	{
		PlayLoopedAudioClip(sound.Clip, sound.Volume, sound.Pitch);
	}

	public void PlayLoopedAudioClip(AudioClip audioClip, float volume = 1f, float pitch = 1f)
	{
		if (!(audioClip == null))
		{
			ApplicationOptions applicationOptions = ApplicationDataManager.ApplicationOptions;
			float num = ((!applicationOptions.AudioEnabled) ? 0f : applicationOptions.AudioEffectsVolume);
			uiAudioSourceLooped.volume = num * Mathf.Clamp01(volume);
			uiAudioSourceLooped.pitch = Mathf.Clamp(pitch, -3f, 3f);
			if (!(uiAudioSourceLooped.clip == audioClip) || !uiAudioSourceLooped.isPlaying)
			{
				uiAudioSourceLooped.clip = audioClip;
				uiAudioSourceLooped.Play();
			}
		}
	}

	public void StopLoopedAudioClip()
	{
		uiAudioSourceLooped.Stop();
	}

	public void Play3dAudioClip(AudioClip clip, Vector3 position, float volume = 1f)
	{
		ApplicationOptions applicationOptions = ApplicationDataManager.ApplicationOptions;
		float num = ((!applicationOptions.AudioEnabled) ? 0f : (applicationOptions.AudioEffectsVolume * applicationOptions.AudioMasterVolume));
		volume *= num;
		audioPool.PlayClipAtPoint(clip, position, volume);
	}

	public AudioClip GetFootStepAudioClip(FootStepSoundType footStep)
	{
		AudioClip[] array = null;
		switch (footStep)
		{
		case FootStepSoundType.Grass:
			array = _footStepGrass;
			break;
		case FootStepSoundType.Metal:
			array = _footStepMetal;
			break;
		case FootStepSoundType.HeavyMetal:
			array = _footStepHeavyMetal;
			break;
		case FootStepSoundType.Rock:
			array = _footStepRock;
			break;
		case FootStepSoundType.Sand:
			array = _footStepSand;
			break;
		case FootStepSoundType.Water:
			array = _footStepWater;
			break;
		case FootStepSoundType.Wood:
			array = _footStepWood;
			break;
		case FootStepSoundType.Swim:
			array = _swimAboveWater;
			break;
		case FootStepSoundType.Dive:
			array = _swimUnderWater;
			break;
		case FootStepSoundType.Snow:
			array = _footStepSnow;
			break;
		case FootStepSoundType.Glass:
			array = _footStepGlass;
			break;
		default:
			array = _footStepDirt;
			break;
		}
		if (array.Length > 1)
		{
			return array[Random.Range(0, array.Length)];
		}
		return array[0];
	}

	public void PlayImpactSound(string surfaceType, Vector3 position)
	{
		AudioClip[] value = null;
		if (_surfaceImpactSoundMap.TryGetValue(surfaceType, out value))
		{
			Play3dAudioClip(value[Random.Range(0, value.Length)], position, 1f);
		}
	}

	public void EnableAudio(bool enabled)
	{
		AudioListener.volume = ((!enabled) ? 0f : ApplicationDataManager.ApplicationOptions.AudioMasterVolume);
	}

	public void UpdateMasterVolume()
	{
		if (ApplicationDataManager.ApplicationOptions.AudioEnabled)
		{
			AudioListener.volume = ApplicationDataManager.ApplicationOptions.AudioMasterVolume;
		}
	}

	public void UpdateMusicVolume()
	{
		if (ApplicationDataManager.ApplicationOptions.AudioEnabled)
		{
			AutoMonoBehaviour<BackgroundMusicPlayer>.Instance.Volume = ApplicationDataManager.ApplicationOptions.AudioMusicVolume;
			if (GameState.Current.Map != null)
			{
				GameState.Current.Map.UpdateVolumes(ApplicationDataManager.ApplicationOptions.AudioMusicVolume);
			}
		}
	}

	public void UpdateEffectsVolume()
	{
		ApplicationOptions applicationOptions = ApplicationDataManager.ApplicationOptions;
		float volume = ((!applicationOptions.AudioEnabled) ? 0f : applicationOptions.AudioEffectsVolume);
		AutoMonoBehaviour<SfxManager>.Instance.uiAudioSource.volume = volume;
		AutoMonoBehaviour<SfxManager>.Instance.gameAudioSource.volume = volume;
		AutoMonoBehaviour<SfxManager>.Instance.uiAudioSourceLooped.volume = volume;
	}
}
