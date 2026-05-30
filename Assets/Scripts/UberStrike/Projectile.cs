using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public abstract class Projectile : MonoBehaviour, IProjectile
{
	public const int DefaultTimeout = 30;

	[SerializeField]
	private Collider _trigger;

	[SerializeField]
	private Collider _collider;

	[SerializeField]
	private bool _showHeatwave;

	[SerializeField]
	private GameObject _explosionEffect;

	private Rigidbody _rigidbody;

	protected AudioSource _source;

	private float _positionSign;

	private Transform _transform;

	protected AudioClip _explosionSound;

	public ParticleConfigurationType ExplosionEffect { get; set; }

	public Rigidbody Rigidbody
	{
		get
		{
			return _rigidbody;
		}
	}

	public ProjectileDetonator Detonator { get; set; }

	public bool IsProjectileExploded { get; protected set; }

	public float TimeOut { get; set; }

	public int ID { get; set; }

	protected int CollisionMask
	{
		get
		{
			if ((bool)base.gameObject && base.gameObject.layer == 24)
			{
				return UberstrikeLayerMasks.RemoteRocketMask;
			}
			return UberstrikeLayerMasks.LocalRocketMask;
		}
	}

	protected virtual void Awake()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_source = GetComponent<AudioSource>();
		if (_collider == null && _trigger == null)
		{
			Debug.LogError("The Projectile " + base.gameObject.name + " has not assigned Collider or Trigger! Check your Inspector settings.");
		}
		if ((bool)_collider && _collider.isTrigger)
		{
			Debug.LogError("The Projectile " + base.gameObject.name + " has a Collider attached that is configured as Trigger! Check your Inspector settings.");
		}
		if ((bool)_trigger && !_trigger.isTrigger)
		{
			Debug.LogError("The Projectile " + base.gameObject.name + " has a Trigger attached that is configured as Collider! Check your Inspector settings.");
		}
		_transform = base.transform;
		_positionSign = Mathf.Sign(_transform.position.y);
	}

	protected virtual void Start()
	{
		if (GameState.Current.Map != null && GameState.Current.Map.HasWaterPlane)
		{
			_positionSign = Mathf.Sign(_transform.position.y - GameState.Current.Map.WaterPlaneHeight);
		}
		StartCoroutine(StartTimeout());
	}

	public void MoveInDirection(Vector3 direction)
	{
		Rigidbody.isKinematic = false;
		Rigidbody.linearVelocity = direction;
	}

	protected virtual IEnumerator StartTimeout()
	{
		yield return new WaitForSeconds((!(TimeOut > 0f)) ? 30f : TimeOut);
		Singleton<ProjectileManager>.Instance.RemoveProjectile(ID);
	}

	protected abstract void OnTriggerEnter(Collider c);

	protected abstract void OnCollisionEnter(Collision c);

	protected virtual void Update()
	{
		if (GameState.Current.Map != null && GameState.Current.Map.HasWaterPlane && _positionSign != Mathf.Sign(_transform.position.y - GameState.Current.Map.WaterPlaneHeight))
		{
			_positionSign = Mathf.Sign(_transform.position.y - GameState.Current.Map.WaterPlaneHeight);
			ParticleEffectController.ProjectileWaterRipplesEffect(ExplosionEffect, _transform.position);
		}
	}

	protected void Explode(Vector3 point, Vector3 normal, string tag)
	{
		Destroy();
		if (Detonator != null)
		{
			Detonator.Explode(point);
		}
		Singleton<ExplosionManager>.Instance.PlayExplosionSound(point, _explosionSound);
		Singleton<ExplosionManager>.Instance.ShowExplosionEffect(point, normal, tag, ExplosionEffect);
		if (_showHeatwave)
		{
			ParticleEffectController.ShowHeatwaveEffect(base.transform.position);
		}
		if ((bool)_explosionEffect)
		{
			UnityEngine.Object.Instantiate(_explosionEffect, point, Quaternion.LookRotation(normal));
		}
	}

	public void Destroy()
	{
		if (!IsProjectileExploded)
		{
			IsProjectileExploded = true;
			base.gameObject.SetActive(false);
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public void SetExplosionSound(AudioClip clip)
	{
		_explosionSound = clip;
	}

	protected void PlayBounceSound(Vector3 position)
	{
		AudioClip clip = GameAudio.LauncherBounce1;
		int num = UnityEngine.Random.Range(0, 2);
		if (num > 0)
		{
			clip = GameAudio.LauncherBounce2;
		}
		AutoMonoBehaviour<SfxManager>.Instance.Play3dAudioClip(clip, position, 1f);
	}

	public Vector3 Explode()
	{
		Vector3 vector = Vector3.zero;
		try
		{
			RaycastHit hitInfo;
			if (Physics.Raycast(base.transform.position - base.transform.forward, base.transform.forward, out hitInfo, 2f, CollisionMask))
			{
				vector = hitInfo.point - base.transform.forward * 0.01f;
				Explode(vector, hitInfo.normal, TagUtil.GetTag(hitInfo.collider));
			}
			else
			{
				vector = base.transform.position;
				Explode(vector, -base.transform.forward, string.Empty);
			}
		}
		catch (Exception message)
		{
			Debug.LogWarning(message);
		}
		return vector;
	}
}
