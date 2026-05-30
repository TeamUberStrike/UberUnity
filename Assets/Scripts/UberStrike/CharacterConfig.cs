using System;
using UberStrike.Core.Models;
using UberStrike.Core.Types;
using UnityEngine;

public class CharacterConfig : MonoBehaviour, IShootable
{
	private Transform _transform;

	private DamageInfo _lastShotInfo;

	private float _isVulnerableAfter;

	[SerializeField]
	private bool _isLocalPlayer;

	[SerializeField]
	private PlayerDamageEffect _damageFeedback;

	[SerializeField]
	private CharacterTrigger _aimTrigger;

	[SerializeField]
	private PlayerDropPickupItem _playerDropWeapon;

	public ICharacterState State { get; private set; }

	public Avatar Avatar { get; private set; }

	public bool IsLocal
	{
		get
		{
			return _isLocalPlayer;
		}
	}

	public Transform Transform
	{
		get
		{
			return _transform;
		}
	}

	private bool IsMe
	{
		get
		{
			return State.Player.Cmid == PlayerDataManager.Cmid;
		}
	}

	public bool IsVulnerable
	{
		get
		{
			return _isVulnerableAfter < Time.time;
		}
	}

	public bool IsDead { get; private set; }

	public float TimeLastGrounded { get; private set; }

	public WeaponSimulator WeaponSimulator { get; private set; }

	public SoundSimulator SoundSimulator { get; private set; }

	public TeamID Team
	{
		get
		{
			return (State != null) ? State.Player.TeamID : TeamID.NONE;
		}
	}

	public CharacterTrigger AimTrigger
	{
		get
		{
			return _aimTrigger;
		}
	}

	public float WalkingSoundSpeed
	{
		get
		{
			return 0.3157895f;
		}
	}

	public float DiveSoundSpeed
	{
		get
		{
			return 1.6f;
		}
	}

	public float SwimSoundSpeed
	{
		get
		{
			return 1.2f;
		}
	}

	private void Awake()
	{
		WeaponSimulator = new WeaponSimulator(this);
		SoundSimulator = new SoundSimulator(this);
		_transform = base.transform;
	}

	private void Update()
	{
		if (State != null && !IsDead && _transform != null)
		{
			_transform.localPosition = State.Position;
			_transform.localRotation = State.HorizontalRotation;
			WeaponSimulator.Update();
			SoundSimulator.Update();
		}
	}

	private void OnDestroy()
	{
		if (Avatar != null)
		{
			Avatar.OnDecoratorChanged -= OnDecoratorUpdated;
		}
	}

	public void OnJump()
	{
		State.MovementState |= MoveStates.Grounded | MoveStates.Jumping;
		if ((bool)Avatar.Decorator)
		{
			Avatar.Decorator.PlayJumpSound();
			if ((bool)Avatar.Decorator.AnimationController)
			{
				Avatar.Decorator.AnimationController.Jump();
			}
			RaycastHit hitInfo;
			if (Physics.Raycast(_transform.position, Vector3.down, out hitInfo, 3f, UberstrikeLayerMasks.ProtectionMask))
			{
				ParticleEffectController.ShowJumpEffect(hitInfo.point, hitInfo.normal);
			}
		}
	}

	public void Initialize(ICharacterState state, Avatar avatar)
	{
		State = state;
		State.OnDeltaUpdate += OnDeltaUpdate;
		_transform.position = State.Position;
		if (!State.Player.IsAlive)
		{
			Debug.Log("Initialize as dead player at " + State.Position);
		}
		base.gameObject.name = string.Format("Player{0}_{1}", State.Player.Cmid, State.Player.PlayerName);
		SetAvatar(avatar);
		WeaponSimulator.UpdateWeapons(State.Player.CurrentWeaponSlot, State.Player.Weapons);
	}

	public void Reset()
	{
		IsDead = false;
		Avatar.CleanupRagdoll();
		WeaponSimulator.UpdateWeaponSlot(State.Player.CurrentWeaponSlot, !_isLocalPlayer);
		Update();
		_isVulnerableAfter = Time.time + 2f;
	}

	private void OnDeltaUpdate(GameActorInfoDelta update)
	{
		foreach (GameActorInfoDelta.Keys key in update.Changes.Keys)
		{
			switch (key)
			{
			case GameActorInfoDelta.Keys.Weapons:
				WeaponSimulator.UpdateWeapons(State.Player.CurrentWeaponSlot, State.Player.Weapons);
				WeaponSimulator.UpdateWeaponSlot(State.Player.CurrentWeaponSlot, !IsLocal);
				if (IsLocal && !Singleton<WeaponController>.Instance.CheckWeapons(GameState.Current.PlayerData.Player.Weapons))
				{
					GameState.Current.Player.InitializeWeapons();
				}
				break;
			case GameActorInfoDelta.Keys.CurrentWeaponSlot:
				WeaponSimulator.UpdateWeaponSlot(State.Player.CurrentWeaponSlot, true);
				break;
			case GameActorInfoDelta.Keys.Gear:
				if (!IsLocal)
				{
					Avatar.Loadout.UpdateGearSlots(State.Player.Gear);
				}
				break;
			case GameActorInfoDelta.Keys.Health:
				Avatar.Decorator.HudInformation.SetHealthBarValue((float)State.Player.Health / 100f);
				break;
			}
		}
	}

	public void ApplyDamage(DamageInfo damageInfo)
	{
		if (damageInfo.Damage <= 0)
		{
			return;
		}
		_lastShotInfo = damageInfo;
		if (State.Player.IsAlive)
		{
			if (damageInfo.IsExplosion)
			{
				GameState.Current.Actions.ExplosionHitDamage(State.Player.Cmid, (ushort)damageInfo.Damage, damageInfo.Force, damageInfo.SlotId, damageInfo.Distance);
			}
			else
			{
				GameState.Current.Actions.DirectHitDamage(State.Player.Cmid, (ushort)damageInfo.Damage, damageInfo.BodyPart, damageInfo.Force, damageInfo.SlotId, damageInfo.Bullets);
			}
			PlayDamageSound();
			if (!IsLocal && (State.Player.TeamID == TeamID.NONE || State.Player.TeamID != GameState.Current.PlayerData.Player.TeamID))
			{
				GameState.Current.PlayerData.AppliedDamage.Value = damageInfo;
				ShowDamageFeedback(damageInfo);
			}
		}
	}

	public virtual void ApplyForce(Vector3 position, Vector3 force)
	{
		if (IsLocal)
		{
			GameState.Current.Player.MoveController.ApplyForce(force, CharacterMoveController.ForceType.Additive);
		}
		else
		{
			GameState.Current.Actions.PlayerHitFeeback(State.Player.Cmid, force);
		}
	}

	private void SetAvatar(Avatar avatar)
	{
		if (Avatar != null)
		{
			Avatar.OnDecoratorChanged -= OnDecoratorUpdated;
		}
		Avatar = avatar;
		Avatar.OnDecoratorChanged += OnDecoratorUpdated;
		OnDecoratorUpdated();
	}

	private void OnDecoratorUpdated()
	{
		if (!Avatar.Decorator)
		{
			return;
		}
		try
		{
			Avatar.Decorator.GetComponent<Renderer>().receiveShadows = false;
			Avatar.Decorator.GetComponent<Renderer>().castShadows = true;
			Avatar.Decorator.transform.parent = _transform;
			Avatar.Decorator.SetPosition(new Vector3(0f, -1.05f, 0f), Quaternion.identity);
			Avatar.Decorator.HudInformation.SetCharacterInfo(State.Player);
			Avatar.Decorator.HudInformation.SetHealthBarValue((float)State.Player.Health / 100f);
			Avatar.Decorator.CurrentFootStep = ((!(GameState.Current.Map != null)) ? FootStepSoundType.Rock : GameState.Current.Map.DefaultFootStep);
			CharacterHitArea[] hitAreas = Avatar.Decorator.HitAreas;
			foreach (CharacterHitArea characterHitArea in hitAreas)
			{
				if ((bool)characterHitArea)
				{
					characterHitArea.Shootable = this;
				}
			}
			Color skinColor = ((!_isLocalPlayer) ? State.Player.SkinColor : PlayerDataManager.SkinColor);
			Avatar.Decorator.Configuration.SetSkinColor(skinColor);
			WeaponSimulator.UpdateWeaponSlot(State.Player.CurrentWeaponSlot, !_isLocalPlayer);
			if ((bool)Avatar.Decorator.AnimationController)
			{
				Avatar.Decorator.AnimationController.SetCharacter(State);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	internal void Destroy()
	{
		try
		{
			Singleton<ProjectileManager>.Instance.RemoveAllProjectilesFromPlayer(State.Player.PlayerId);
			Singleton<QuickItemSfxController>.Instance.DestroytSfxFromPlayer(State.Player.PlayerId);
			if (State != null)
			{
				State.OnDeltaUpdate -= OnDeltaUpdate;
			}
			Avatar.CleanupRagdoll();
			if (Avatar.Decorator != null && IsLocal)
			{
				Avatar.Decorator.transform.parent = null;
			}
			if (base.gameObject != null)
			{
				AvatarBuilder.Destroy(base.gameObject);
			}
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
	}

	private void PlayDamageSound()
	{
		if (IsLocal)
		{
			if (State.Player.ArmorPoints > 0)
			{
				AutoMonoBehaviour<SfxManager>.Instance.PlayInGameAudioClip(GameAudio.LocalPlayerHitArmorRemaining);
			}
			else if (State.Player.Health < 25)
			{
				AutoMonoBehaviour<SfxManager>.Instance.PlayInGameAudioClip(GameAudio.LocalPlayerHitNoArmorLowHealth);
			}
			else
			{
				AutoMonoBehaviour<SfxManager>.Instance.PlayInGameAudioClip(GameAudio.LocalPlayerHitNoArmor);
			}
		}
	}

	private void ShowDamageFeedback(DamageInfo shot)
	{
		PlayerDamageEffect playerDamageEffect = UnityEngine.Object.Instantiate(_damageFeedback, shot.Hitpoint, (!(shot.Force.magnitude > 0f)) ? Quaternion.identity : Quaternion.LookRotation(shot.Force)) as PlayerDamageEffect;
		if ((bool)playerDamageEffect)
		{
			playerDamageEffect.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
			playerDamageEffect.Show(shot);
		}
	}

	internal void SetDead(Vector3 direction, BodyPart bodyPart = BodyPart.Body, int target = 0, UberstrikeItemClass itemClass = UberstrikeItemClass.WeaponMachinegun)
	{
		IsDead = true;
		if ((bool)_transform)
		{
			_transform.position = State.Position;
		}
		Singleton<QuickItemSfxController>.Instance.DestroytSfxFromPlayer(State.Player.PlayerId);
		if ((bool)Avatar.Decorator)
		{
			Avatar.Decorator.HudInformation.Hide();
			Avatar.Decorator.PlayDieSound();
		}
		if (!_isLocalPlayer)
		{
			Avatar.HideWeapons();
		}
		DamageInfo damageInfo = new DamageInfo(direction, bodyPart);
		damageInfo.WeaponClass = itemClass;
		if (PlayerDataManager.Cmid == target && (itemClass == UberstrikeItemClass.WeaponCannon || itemClass == UberstrikeItemClass.WeaponLauncher))
		{
			damageInfo.Force = direction.normalized;
			damageInfo.Damage = ((_lastShotInfo == null) ? Convert.ToInt16(100) : _lastShotInfo.Damage);
		}
		Avatar.SpawnRagdoll(damageInfo);
	}
}
