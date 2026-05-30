using UnityEngine;

public class AvatarDecorator : MonoBehaviour
{
	[SerializeField]
	private CharacterHitArea[] _hitAreas;

	[SerializeField]
	private Transform _weaponAttachPoint;

	private float _nextFootStepTime;

	private AudioSource _audio;

	private Transform _transform;

	public Animation Animation { get; private set; }

	public Animator Animator { get; private set; }

	public AvatarAnimationController AnimationController { get; private set; }

	public FootStepSoundType CurrentFootStep { get; set; }

	public AvatarHudInformation HudInformation { get; private set; }

	public AvatarDecoratorConfig Configuration { get; private set; }

	public CharacterHitArea[] HitAreas
	{
		get
		{
			return _hitAreas;
		}
		set
		{
			_hitAreas = value;
		}
	}

	public Transform WeaponAttachPoint
	{
		get
		{
			return _weaponAttachPoint;
		}
		set
		{
			_weaponAttachPoint = value;
		}
	}

	private void Awake()
	{
		_transform = base.transform;
		_audio = GetComponent<AudioSource>();
		Animator = GetComponent<Animator>();
		AnimationController = GetComponent<AvatarAnimationController>();
		HudInformation = GetComponentInChildren<AvatarHudInformation>();
		Configuration = GetComponent<AvatarDecoratorConfig>();
	}

	public void SetLayers(UberstrikeLayer layer)
	{
		LayerUtil.SetLayerRecursively(base.transform, layer);
	}

	public Transform GetBone(BoneIndex bone)
	{
		return Configuration.GetBone(bone);
	}

	public void SetPosition(Vector3 position, Quaternion rotation)
	{
		base.transform.localPosition = position;
		base.transform.localRotation = rotation;
	}

	public void PlayFootSound(float walkingSpeed)
	{
		PlayFootSound(walkingSpeed, CurrentFootStep);
	}

	public void PlayJumpSound()
	{
		_nextFootStepTime = Time.time + 0.3f;
	}

	public void PlayFootSound(float walkingSpeed, FootStepSoundType sound)
	{
		if (_nextFootStepTime < Time.time)
		{
			_nextFootStepTime = Time.time + walkingSpeed;
			_audio.clip = AutoMonoBehaviour<SfxManager>.Instance.GetFootStepAudioClip(sound);
			AutoMonoBehaviour<SfxManager>.Instance.Play3dAudioClip(_audio.clip, _transform.position, 1f);
		}
	}

	public void PlayDieSound()
	{
		int num = Random.Range(0, 3);
		AudioClip clip = GameAudio.NormalKill1;
		switch (num)
		{
		case 0:
			clip = GameAudio.NormalKill1;
			break;
		case 1:
			clip = GameAudio.NormalKill2;
			break;
		case 3:
			clip = GameAudio.NormalKill3;
			break;
		}
		AutoMonoBehaviour<SfxManager>.Instance.Play3dAudioClip(clip, _transform.position, 1f);
	}
}
