using System;
using System.Collections;
using UberStrike.Core.Models;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class LocalPlayer : MonoBehaviour
{
	private const float RundownThreshold = -300f;

	private const float GroundedThreshold = 0.5f;

	[SerializeField]
	private Transform _cameraTarget;

	[SerializeField]
	private Transform _weaponAttachPoint;

	[SerializeField]
	private WeaponCamera _weaponCamera;

	private bool _didPlayTargetSound;

	private float _damageFactor;

	private float _damageFactorDuration;

	private float _lastGrounded;

	public CharacterConfig Character { get; private set; }

	public float DamageFactor
	{
		get
		{
			return _damageFactor;
		}
		set
		{
			_damageFactor = Mathf.Clamp01(value);
			_damageFactorDuration = _damageFactor * 15f;
		}
	}

	public bool IsWalkingEnabled { get; set; }

	public bool EnableWeaponControl { get; set; }

	public CharacterMoveController MoveController { get; private set; }

	public WeaponCamera WeaponCamera
	{
		get
		{
			return _weaponCamera;
		}
	}

	public Transform WeaponAttachPoint
	{
		get
		{
			return _weaponAttachPoint;
		}
	}

	public Transform CameraTarget
	{
		get
		{
			return _cameraTarget;
		}
	}

	public Vector3 EyePosition
	{
		get
		{
			return new Vector3(0f, -0.2f, 0f);
		}
	}

	private void Awake()
	{
		MoveController = new CharacterMoveController(GetComponent<CharacterController>());
		MoveController.CharacterLanded += OnCharacterGrounded;
		IsWalkingEnabled = true;
	}

	private void OnEnable()
	{
		MoveController.Init();
		StartCoroutine(StartPlayerIdentification());
		StartCoroutine(StartUpdatePlayerPingTime(5));
	}

	private void OnDisable()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		AutoMonoBehaviour<InputManager>.Instance.IsInputEnabled = false;
	}

	private void Update()
	{
		_cameraTarget.localPosition = Vector3.Lerp(_cameraTarget.localPosition, GameState.Current.PlayerData.CurrentOffset, 10f * Time.deltaTime);
		if (_damageFactor != 0f)
		{
			if (_damageFactorDuration > 0f)
			{
				_damageFactorDuration -= Time.deltaTime;
			}
			if (_damageFactorDuration <= 0f || !GameState.Current.PlayerData.IsAlive)
			{
				_damageFactor = 0f;
				_damageFactorDuration = 0f;
			}
		}
		UpdateCameraBob();
		if (AutoMonoBehaviour<InputManager>.Instance.IsInputEnabled)
		{
			if (Cursor.lockState == CursorLockMode.Locked)
			{
				UserInput.UpdateMouse();
			}
			UserInput.UpdateDirections();
			if (Cursor.lockState == CursorLockMode.Locked)
			{
				UpdateRotation();
			}
		}
		else
		{
			UserInput.ResetDirection();
		}
	}

	private void LateUpdate()
	{
		Singleton<WeaponController>.Instance.LateUpdate();
	}

	private void UpdateRotation()
	{
		_cameraTarget.localRotation = UserInput.Rotation;
	}

	private IEnumerator StartPlayerIdentification()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.3f);
			if (GameState.Current.IsPlayerPaused)
			{
				continue;
			}
			Vector3 start = GameState.Current.PlayerData.ShootingPoint + GameState.Current.Player.EyePosition;
			Vector3 end = start + GameState.Current.PlayerData.ShootingDirection * 1000f;
			RaycastHit hit;
			if (Physics.Linecast(start, end, out hit, UberstrikeLayerMasks.IdentificationMask))
			{
				CharacterHitArea hitArea = hit.collider.GetComponent<CharacterHitArea>();
				if ((bool)hitArea && hitArea.Shootable != null && !hitArea.Shootable.IsLocal)
				{
					CharacterConfig character = (CharacterConfig)hitArea.Shootable;
					character.AimTrigger.HudInfo.Show();
					GameState.Current.PlayerData.FocusedPlayerTeam.Value = character.Team;
					if (!_didPlayTargetSound)
					{
						AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.FocusEnemy, 0uL, 1f, 1f);
						_didPlayTargetSound = true;
					}
				}
				else
				{
					GameState.Current.PlayerData.FocusedPlayerTeam.Value = TeamID.NONE;
					_didPlayTargetSound = false;
				}
			}
			else
			{
				GameState.Current.PlayerData.FocusedPlayerTeam.Value = TeamID.NONE;
				_didPlayTargetSound = false;
			}
		}
	}

	private IEnumerator StartUpdatePlayerPingTime(int sec)
	{
		while (true)
		{
			GameState.Current.PlayerData.SetPing(Singleton<GameStateController>.Instance.Client.Ping);
			yield return new WaitForSeconds(sec);
		}
	}

	private void OnCharacterGrounded(float velocity)
	{
		if (!GameState.Current.HasJoinedGame || !GameState.Current.IsInGame || WeaponFeedbackManager.IsBobbing || !(_lastGrounded + 0.5f < Time.time) || GameState.Current.PlayerData.Is(MoveStates.Diving))
		{
			return;
		}
		_lastGrounded = Time.time;
		if (Character != null && Character.Avatar != null && Character.Avatar.Decorator != null)
		{
			Character.Avatar.Decorator.PlayFootSound(Character.WalkingSoundSpeed);
			if (velocity < -20f)
			{
				LevelCamera.DoLandFeedback(true);
			}
			else
			{
				LevelCamera.DoLandFeedback(false);
			}
		}
	}

	private void UpdateCameraBob()
	{
		switch (GameState.Current.PlayerData.MovementState)
		{
		case MoveStates.Swimming:
			WeaponFeedbackManager.SetBobMode(LevelCamera.BobMode.Swim);
			break;
		case MoveStates.Grounded | MoveStates.Ducked:
			if (UserInput.IsWalking)
			{
				if (Singleton<WeaponController>.Instance.IsSecondaryAction)
				{
					WeaponFeedbackManager.SetBobMode(LevelCamera.BobMode.None);
				}
				else
				{
					WeaponFeedbackManager.SetBobMode(LevelCamera.BobMode.Crouch);
				}
			}
			else if (Singleton<WeaponController>.Instance.IsSecondaryAction)
			{
				WeaponFeedbackManager.SetBobMode(LevelCamera.BobMode.None);
			}
			else
			{
				WeaponFeedbackManager.SetBobMode(LevelCamera.BobMode.Idle);
			}
			break;
		case MoveStates.Flying:
			WeaponFeedbackManager.SetBobMode(LevelCamera.BobMode.Fly);
			break;
		case MoveStates.Grounded:
			if (UserInput.IsWalking)
			{
				if (Singleton<WeaponController>.Instance.IsSecondaryAction)
				{
					WeaponFeedbackManager.SetBobMode(LevelCamera.BobMode.None);
				}
				else
				{
					WeaponFeedbackManager.SetBobMode(LevelCamera.BobMode.Run);
				}
			}
			else if (Singleton<WeaponController>.Instance.IsSecondaryAction)
			{
				WeaponFeedbackManager.SetBobMode(LevelCamera.BobMode.None);
			}
			else if (!UserInput.IsWalking || MoveController.Velocity.y < -300f)
			{
				WeaponFeedbackManager.SetBobMode(LevelCamera.BobMode.Idle);
			}
			break;
		default:
			if (!UserInput.IsWalking || MoveController.Velocity.y < -300f)
			{
				WeaponFeedbackManager.SetBobMode(LevelCamera.BobMode.None);
			}
			break;
		}
	}

	public void InitializeWeapons()
	{
		Singleton<WeaponController>.Instance.InitializeAllWeapons(_weaponAttachPoint);
	}

	public void InitializePlayer()
	{
		try
		{
			InitializeWeapons();
			WeaponFeedbackManager.SetBobMode(LevelCamera.BobMode.None);
			LevelCamera.EnableLowPassFilter(false);
			UpdateRotation();
			MoveController.Start();
			MoveController.ResetDuckMode();
			Singleton<QuickItemController>.Instance.Reset();
			GameState.Current.PlayerData.InitializePlayer();
			DamageFactor = 0f;
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	public void SpawnPlayerAt(Vector3 pos, Quaternion rot)
	{
		try
		{
			base.transform.position = pos + Vector3.up;
			_cameraTarget.localRotation = rot;
			UserInput.SetRotation(rot.eulerAngles.y, 0f);
			LevelCamera.ResetFeedback();
			MoveController.ResetEnviroment();
			MoveController.Platform = null;
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	public void SetCurrentCharacterConfig(CharacterConfig character)
	{
		Character = character;
	}

	public void SetEnabled(bool enabled)
	{
		base.gameObject.SetActive(enabled);
	}
}
