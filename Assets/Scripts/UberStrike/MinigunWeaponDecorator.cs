using UnityEngine;

public class MinigunWeaponDecorator : BaseWeaponDecorator
{
	[SerializeField]
	private AudioClip _duringShootSound;

	[SerializeField]
	private AudioClip _warmUpSound;

	[SerializeField]
	private AudioClip _warmDownSound;

	private AudioSource _warmUpAudioSource;

	private AudioSource _warmDownAudioSource;

	private AudioSource _duringShootAudioSource;

	private WeaponHeadAnimation _headAnim;

	private float _maxWarmUpTime;

	private float _maxWarmDownTime;

	public float MaxWarmUpTime
	{
		get
		{
			return _maxWarmUpTime;
		}
	}

	public float MaxWarmDownTime
	{
		get
		{
			return _maxWarmDownTime;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if (_warmUpSound == null)
		{
			throw new MissingReferenceException("MinigunWeaponDecorator - _warmUpSound is NULL");
		}
		if (_warmDownSound == null)
		{
			throw new MissingReferenceException("MinigunWeaponDecorator - _warmDownSound is NULL");
		}
		InitAudioSource();
		_headAnim = GetComponentInChildren<WeaponHeadAnimation>();
	}

	private void InitAudioSource()
	{
		if ((bool)_duringShootSound)
		{
			_duringShootAudioSource = base.gameObject.AddComponent<AudioSource>();
			if ((bool)_duringShootAudioSource)
			{
				_duringShootAudioSource.loop = true;
				_duringShootAudioSource.priority = 0;
				_duringShootAudioSource.playOnAwake = false;
				_duringShootAudioSource.clip = _duringShootSound;
			}
		}
		_warmUpAudioSource = base.gameObject.AddComponent<AudioSource>();
		if ((bool)_warmUpAudioSource)
		{
			_warmUpAudioSource.priority = 0;
			_warmUpAudioSource.playOnAwake = false;
			_maxWarmUpTime = _warmUpSound.length;
			_warmUpAudioSource.clip = _warmUpSound;
		}
		if ((bool)_warmDownSound)
		{
			_warmDownAudioSource = base.gameObject.AddComponent<AudioSource>();
			if ((bool)_warmDownAudioSource)
			{
				_warmDownAudioSource.priority = 0;
				_warmDownAudioSource.playOnAwake = false;
				_maxWarmDownTime = _warmDownSound.length;
				_warmDownAudioSource.clip = _warmDownSound;
			}
		}
	}

	public override void ShowShootEffect(RaycastHit[] hits)
	{
		base.ShowShootEffect(hits);
	}

	public void PlayWindUpSound(float time)
	{
		if ((bool)_warmDownAudioSource)
		{
			_warmDownAudioSource.Stop();
		}
		if ((bool)_warmUpAudioSource)
		{
			_warmUpAudioSource.time = time;
			_warmUpAudioSource.Play();
		}
	}

	public void PlayWindDownSound(float time)
	{
		if ((bool)_duringShootAudioSource)
		{
			_duringShootAudioSource.Stop();
		}
		if ((bool)_warmUpAudioSource)
		{
			_warmUpAudioSource.Stop();
		}
		if ((bool)_warmDownAudioSource)
		{
			_warmDownAudioSource.time = time;
			_warmDownAudioSource.Play();
		}
	}

	public void PlayDuringSound()
	{
		if (!_duringShootAudioSource.isPlaying)
		{
			_duringShootAudioSource.Play();
		}
	}

	public override void StopSound()
	{
		base.StopSound();
		_duringShootAudioSource.Stop();
	}

	public void SpinWeaponHead()
	{
		if ((bool)_headAnim)
		{
			_headAnim.OnShoot();
		}
	}
}
