using System;
using UberStrike.Core.Types;
using UnityEngine;

public class ExplosiveGrenadeQuickItem : QuickItem, IProjectile, IGrenadeProjectile
{
	private class FlyingState : IState
	{
		private ExplosiveGrenadeQuickItem behaviour;

		private float _timeOut;

		public FlyingState(ExplosiveGrenadeQuickItem behaviour)
		{
			this.behaviour = behaviour;
		}

		public void OnEnter()
		{
			_timeOut = Time.time + (float)behaviour._config.LifeTime;
			ExplosiveGrenadeQuickItem explosiveGrenadeQuickItem = behaviour;
			explosiveGrenadeQuickItem.OnCollisionEnterEvent = (Action<Collision>)Delegate.Combine(explosiveGrenadeQuickItem.OnCollisionEnterEvent, new Action<Collision>(OnCollisionEnterEvent));
			if (!behaviour._config.IsSticky)
			{
				ExplosiveGrenadeQuickItem explosiveGrenadeQuickItem2 = behaviour;
				explosiveGrenadeQuickItem2.OnTriggerEnterEvent = (Action<Collider>)Delegate.Combine(explosiveGrenadeQuickItem2.OnTriggerEnterEvent, new Action<Collider>(OnTriggerEnterEvent));
			}
			GameObject gameObject = behaviour.gameObject;
			if (!gameObject || !GameState.Current.Avatar.Decorator || !gameObject.GetComponent<Collider>())
			{
				return;
			}
			Collider collider = gameObject.GetComponent<Collider>();
			CharacterHitArea[] hitAreas = GameState.Current.Avatar.Decorator.HitAreas;
			foreach (CharacterHitArea characterHitArea in hitAreas)
			{
				if (gameObject.activeSelf && characterHitArea.gameObject.activeSelf)
				{
					Physics.IgnoreCollision(collider, characterHitArea.GetComponent<Collider>());
				}
			}
		}

		public void OnExit()
		{
			ExplosiveGrenadeQuickItem explosiveGrenadeQuickItem = behaviour;
			explosiveGrenadeQuickItem.OnCollisionEnterEvent = (Action<Collision>)Delegate.Remove(explosiveGrenadeQuickItem.OnCollisionEnterEvent, new Action<Collision>(OnCollisionEnterEvent));
			if (!behaviour._config.IsSticky)
			{
				ExplosiveGrenadeQuickItem explosiveGrenadeQuickItem2 = behaviour;
				explosiveGrenadeQuickItem2.OnTriggerEnterEvent = (Action<Collider>)Delegate.Remove(explosiveGrenadeQuickItem2.OnTriggerEnterEvent, new Action<Collider>(OnTriggerEnterEvent));
			}
		}

		public void OnUpdate()
		{
			if (_timeOut < Time.time)
			{
				behaviour.machine.PopState();
				Singleton<ProjectileManager>.Instance.RemoveProjectile(behaviour.ID);
			}
		}

		public void OnResume()
		{
		}

		private void OnCollisionEnterEvent(Collision c)
		{
			if (LayerUtil.IsLayerInMask(UberstrikeLayerMasks.GrenadeCollisionMask, c.gameObject.layer))
			{
				behaviour.machine.PopState();
				Singleton<ProjectileManager>.Instance.RemoveProjectile(behaviour.ID);
				GameState.Current.Actions.RemoveProjectile(behaviour.ID, true);
			}
			else if (behaviour._config.IsSticky)
			{
				if (c.contacts.Length > 0)
				{
					behaviour.transform.position = c.contacts[0].point + c.contacts[0].normal * behaviour.GetComponent<Collider>().bounds.extents.sqrMagnitude;
				}
				behaviour.machine.PopState();
				behaviour.machine.PushState(2);
			}
			PlayBounceSound(c.transform.position);
		}

		private void OnTriggerEnterEvent(Collider c)
		{
			if (LayerUtil.IsLayerInMask(UberstrikeLayerMasks.GrenadeCollisionMask, c.gameObject.layer))
			{
				behaviour.machine.PopState();
				Singleton<ProjectileManager>.Instance.RemoveProjectile(behaviour.ID);
				GameState.Current.Actions.RemoveProjectile(behaviour.ID, true);
			}
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
	}

	private class DeployedState : IState
	{
		private ExplosiveGrenadeQuickItem behaviour;

		private float _timeOut;

		public DeployedState(ExplosiveGrenadeQuickItem behaviour)
		{
			this.behaviour = behaviour;
		}

		public void OnEnter()
		{
			_timeOut = Time.time + (float)behaviour._config.LifeTime;
			ExplosiveGrenadeQuickItem explosiveGrenadeQuickItem = behaviour;
			explosiveGrenadeQuickItem.OnTriggerEnterEvent = (Action<Collider>)Delegate.Combine(explosiveGrenadeQuickItem.OnTriggerEnterEvent, new Action<Collider>(OnTriggerEnterEvent));
			if ((bool)behaviour.GetComponent<Rigidbody>())
			{
				behaviour.GetComponent<Rigidbody>().isKinematic = true;
			}
			if ((bool)behaviour.GetComponent<Collider>())
			{
				UnityEngine.Object.Destroy(behaviour.GetComponent<Collider>());
			}
			behaviour.gameObject.layer = 2;
			if ((bool)behaviour.DeployedEffect)
			{
				behaviour.DeployedEffect.Play();
			}
		}

		public void OnResume()
		{
		}

		public void OnExit()
		{
			ExplosiveGrenadeQuickItem explosiveGrenadeQuickItem = behaviour;
			explosiveGrenadeQuickItem.OnTriggerEnterEvent = (Action<Collider>)Delegate.Remove(explosiveGrenadeQuickItem.OnTriggerEnterEvent, new Action<Collider>(OnTriggerEnterEvent));
		}

		private void OnTriggerEnterEvent(Collider c)
		{
			if (LayerUtil.IsLayerInMask(UberstrikeLayerMasks.GrenadeCollisionMask, c.gameObject.layer))
			{
				behaviour.machine.PopState();
				Singleton<ProjectileManager>.Instance.RemoveProjectile(behaviour.ID);
				GameState.Current.Actions.RemoveProjectile(behaviour.ID, true);
			}
		}

		public void OnUpdate()
		{
			if (_timeOut < Time.time)
			{
				behaviour.machine.PopState();
				Singleton<ProjectileManager>.Instance.RemoveProjectile(behaviour.ID);
			}
		}
	}

	[SerializeField]
	private Renderer _renderer;

	[SerializeField]
	private ParticleSystem _smoke;

	[SerializeField]
	private ParticleSystem _deployedEffect;

	[SerializeField]
	private AudioClip _explosionSound;

	[SerializeField]
	private GameObject _explosionSfx;

	[SerializeField]
	private ExplosiveGrenadeConfiguration _config;

	private StateMachine machine = new StateMachine();

	private bool _isDestroyed;

	public ParticleSystem Smoke
	{
		get
		{
			return _smoke;
		}
	}

	public ParticleSystem DeployedEffect
	{
		get
		{
			return _deployedEffect;
		}
	}

	public Renderer Renderer
	{
		get
		{
			return _renderer;
		}
	}

	public override QuickItemConfiguration Configuration
	{
		get
		{
			return _config;
		}
		set
		{
			_config = (ExplosiveGrenadeConfiguration)value;
		}
	}

	public int ID { get; set; }

	public Vector3 Position
	{
		get
		{
			return (!base.transform) ? Vector3.zero : base.transform.position;
		}
		private set
		{
			if ((bool)base.transform)
			{
				base.transform.position = value;
			}
		}
	}

	public Vector3 Velocity
	{
		get
		{
			return (!base.GetComponent<Rigidbody>()) ? Vector3.zero : base.GetComponent<Rigidbody>().linearVelocity;
		}
		private set
		{
			if ((bool)base.GetComponent<Rigidbody>())
			{
				base.GetComponent<Rigidbody>().linearVelocity = value;
			}
		}
	}

	private event Action<Collider> OnTriggerEnterEvent;

	private event Action<Collision> OnCollisionEnterEvent;

	public event Action<IGrenadeProjectile> OnProjectileExploded;

	public event Action<IGrenadeProjectile> OnProjectileEmitted;

	protected override void OnActivated()
	{
		Vector3 vector = GameState.Current.PlayerData.ShootingPoint + GameState.Current.Player.EyePosition;
		Vector3 position = vector + GameState.Current.PlayerData.ShootingDirection * 2f;
		Vector3 velocity = GameState.Current.PlayerData.ShootingDirection * _config.Speed;
		float distance = 2f;
		RaycastHit hitInfo;
		if (Physics.Raycast(vector, GameState.Current.PlayerData.ShootingDirection * 2f, out hitInfo, distance, UberstrikeLayerMasks.LocalRocketMask))
		{
			ExplosiveGrenadeQuickItem explosiveGrenadeQuickItem = Throw(hitInfo.point, Vector3.zero) as ExplosiveGrenadeQuickItem;
			explosiveGrenadeQuickItem.machine.PopAllStates();
			explosiveGrenadeQuickItem.OnProjectileExploded = (Action<IGrenadeProjectile>)Delegate.Combine(explosiveGrenadeQuickItem.OnProjectileExploded, (Action<IGrenadeProjectile>)delegate(IGrenadeProjectile p)
			{
				ProjectileDetonator.Explode(p.Position, p.ID, _config.Damage, Vector3.up, _config.SplashRadius, 5, 5, Configuration.ID, UberstrikeItemClass.WeaponLauncher, DamageEffectType.None, 0f);
			});
			Singleton<ProjectileManager>.Instance.RemoveProjectile(explosiveGrenadeQuickItem.ID);
			GameState.Current.Actions.RemoveProjectile(explosiveGrenadeQuickItem.ID, true);
		}
		else
		{
			IGrenadeProjectile grenadeProjectile = Throw(position, velocity);
			grenadeProjectile.OnProjectileExploded += delegate(IGrenadeProjectile p)
			{
				ProjectileDetonator.Explode(p.Position, p.ID, _config.Damage, Vector3.up, _config.SplashRadius, 5, 5, Configuration.ID, UberstrikeItemClass.WeaponLauncher, DamageEffectType.None, 0f);
			};
		}
	}

	public IGrenadeProjectile Throw(Vector3 position, Vector3 velocity)
	{
		ExplosiveGrenadeQuickItem explosiveGrenadeQuickItem = UnityEngine.Object.Instantiate(this) as ExplosiveGrenadeQuickItem;
		if ((bool)explosiveGrenadeQuickItem)
		{
			explosiveGrenadeQuickItem.gameObject.SetActive(true);
			for (int i = 0; i < explosiveGrenadeQuickItem.transform.childCount; i++)
			{
				explosiveGrenadeQuickItem.transform.GetChild(i).gameObject.SetActive(true);
			}
			explosiveGrenadeQuickItem.Position = position;
			explosiveGrenadeQuickItem.Velocity = velocity;
			explosiveGrenadeQuickItem.GetComponent<Collider>().material.bounciness = _config.Bounciness;
			explosiveGrenadeQuickItem.machine.RegisterState(1, new FlyingState(explosiveGrenadeQuickItem));
			explosiveGrenadeQuickItem.machine.RegisterState(2, new DeployedState(explosiveGrenadeQuickItem));
			explosiveGrenadeQuickItem.machine.PushState(1);
		}
		if (this.OnProjectileEmitted != null)
		{
			this.OnProjectileEmitted(explosiveGrenadeQuickItem);
		}
		return explosiveGrenadeQuickItem;
	}

	public void SetLayer(UberstrikeLayer layer)
	{
		LayerUtil.SetLayerRecursively(base.transform, layer);
	}

	private void Update()
	{
		machine.Update();
	}

	private void OnGUI()
	{
		if (base.Behaviour.IsCoolingDown && base.Behaviour.FocusTimeRemaining > 0f)
		{
			float num = Mathf.Clamp((float)Screen.height * 0.03f, 10f, 40f);
			float num2 = num * 10f;
			GUI.Label(new Rect(((float)Screen.width - num2) * 0.5f, Screen.height / 2 + 20, num2, num), "Charging Grenade", BlueStonez.label_interparkbold_16pt);
			GUITools.DrawWarmupBar(new Rect(((float)Screen.width - num2) * 0.5f, Screen.height / 2 + 50, num2, num), base.Behaviour.FocusTimeTotal - base.Behaviour.FocusTimeRemaining, base.Behaviour.FocusTimeTotal);
		}
	}

	private void OnTriggerEnter(Collider c)
	{
		if (this.OnTriggerEnterEvent != null)
		{
			this.OnTriggerEnterEvent(c);
		}
	}

	private void OnCollisionEnter(Collision c)
	{
		if (this.OnCollisionEnterEvent != null)
		{
			this.OnCollisionEnterEvent(c);
		}
	}

	public Vector3 Explode()
	{
		Vector3 result = Vector3.zero;
		try
		{
			if (_explosionSound != null)
			{
				AutoMonoBehaviour<SfxManager>.Instance.Play3dAudioClip(_explosionSound, base.transform.position, 1f);
			}
			if ((bool)_explosionSfx)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(_explosionSfx) as GameObject;
				if ((bool)gameObject)
				{
					gameObject.transform.position = base.transform.position;
					SelfDestroy selfDestroy = gameObject.AddComponent<SelfDestroy>();
					if ((bool)selfDestroy)
					{
						selfDestroy.SetDelay(2f);
					}
				}
			}
			else
			{
				ParticleEffectController.ShowExplosionEffect(ParticleConfigurationType.LauncherDefault, SurfaceEffectType.None, base.transform.position, Vector3.up);
			}
			if (this.OnProjectileExploded != null)
			{
				this.OnProjectileExploded(this);
			}
			result = base.transform.position;
			Destroy();
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		return result;
	}

	public void Destroy()
	{
		if (!_isDestroyed)
		{
			_isDestroyed = true;
			base.gameObject.SetActive(false);
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
