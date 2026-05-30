using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFeedbackManager : MonoBehaviour
{
	private class WeaponBobManager
	{
		public struct BobData
		{
			private float _xAmplitude;

			private float _yAmplitude;

			private float _frequency;

			public float XAmplitude
			{
				get
				{
					return _xAmplitude;
				}
			}

			public float YAmplitude
			{
				get
				{
					return _yAmplitude;
				}
			}

			public float Frequency
			{
				get
				{
					return _frequency;
				}
			}

			public BobData(float xamp, float yamp, float freq)
			{
				_xAmplitude = xamp;
				_yAmplitude = yamp;
				_frequency = freq;
			}
		}

		private readonly Dictionary<LevelCamera.BobMode, BobData> _bobData;

		private LevelCamera.BobMode _bobMode;

		private BobData _data;

		public BobData Data
		{
			get
			{
				return _data;
			}
		}

		public LevelCamera.BobMode Mode
		{
			get
			{
				return _bobMode;
			}
			set
			{
				if (_bobMode != value)
				{
					_bobMode = value;
					_data = _bobData[value];
				}
			}
		}

		public WeaponBobManager()
		{
			_bobData = new Dictionary<LevelCamera.BobMode, BobData>();
			foreach (int value in Enum.GetValues(typeof(LevelCamera.BobMode)))
			{
				switch ((LevelCamera.BobMode)value)
				{
				case LevelCamera.BobMode.Walk:
					_bobData[(LevelCamera.BobMode)value] = new BobData(0.5f, 3f, 6f);
					break;
				case LevelCamera.BobMode.Run:
					_bobData[(LevelCamera.BobMode)value] = new BobData(1f, 3f, 8f);
					break;
				case LevelCamera.BobMode.Crouch:
					_bobData[(LevelCamera.BobMode)value] = new BobData(0.5f, 3f, 12f);
					break;
				default:
					_bobData[(LevelCamera.BobMode)value] = new BobData(0f, 0f, 0f);
					break;
				}
			}
			_data = _bobData[LevelCamera.BobMode.Idle];
		}
	}

	public enum WeaponMode
	{
		Primary = 0,
		Second = 1,
		PutDown = 2
	}

	private abstract class WeaponState
	{
		protected bool _isRunning;

		protected float _time;

		private BaseWeaponLogic _weapon;

		private BaseWeaponDecorator _decorator;

		protected Vector3 _pivotOffset;

		protected float _currentRotation;

		protected float _transitionTime;

		protected Vector3 _targetPosition;

		protected Quaternion _targetRotation;

		public Vector3 PivotVector
		{
			get
			{
				return _pivotOffset + ((!Instance._isIronSight) ? Decorator.DefaultPosition : Decorator.IronSightPosition);
			}
		}

		public bool IsRunning
		{
			get
			{
				return _isRunning;
			}
		}

		public bool IsValid
		{
			get
			{
				return _weapon != null && _decorator != null;
			}
		}

		public BaseWeaponDecorator Decorator
		{
			get
			{
				return _decorator;
			}
		}

		public BaseWeaponLogic Weapon
		{
			get
			{
				return _weapon;
			}
		}

		public Vector3 TargetPosition
		{
			get
			{
				return _targetPosition;
			}
		}

		public Quaternion TargetRotation
		{
			get
			{
				return _targetRotation;
			}
		}

		protected WeaponState(BaseWeaponLogic weapon, BaseWeaponDecorator decorator)
		{
			_time = 0f;
			_weapon = weapon;
			_decorator = decorator;
			_isRunning = _weapon != null;
		}

		public abstract void Update();

		public abstract void Finish();

		public void Reset()
		{
			_pivotOffset = new Vector3(0f, 0f, 0.2f);
		}

		public virtual bool CanTransit(WeaponMode mode)
		{
			return Instance.CurrentWeaponMode != mode;
		}
	}

	private class PickUpState : WeaponState
	{
		private bool _isFiring;

		public PickUpState(BaseWeaponLogic weapon, BaseWeaponDecorator decorator)
			: base(weapon, decorator)
		{
			_transitionTime = Mathf.Max(Instance.WeaponAnimation.PickUpDuration, weapon.Config.SwitchDelayMilliSeconds / 1000);
			if (decorator.IsMelee)
			{
				_currentRotation = -90f;
				if ((bool)base.Decorator)
				{
					base.Decorator.CurrentRotation = Quaternion.Euler(0f, 0f, _currentRotation);
					base.Decorator.CurrentPosition = decorator.DefaultPosition;
					base.Decorator.IsEnabled = true;
				}
			}
			else
			{
				_currentRotation = Instance.WeaponAnimation.PutDownAngles;
				_pivotOffset = -Instance._pivotPoint.localPosition;
				if ((bool)base.Decorator)
				{
					base.Decorator.CurrentRotation = Quaternion.Euler(Instance.WeaponAnimation.PutDownAngles, 0f, 0f);
					base.Decorator.CurrentPosition = Quaternion.AngleAxis(_currentRotation, Vector3.right) * base.PivotVector;
					base.Decorator.IsEnabled = true;
				}
			}
			LevelCamera.ResetZoom();
		}

		public override void Update()
		{
			if (!base.IsValid)
			{
				return;
			}
			if (base.IsRunning)
			{
				if (_time <= _transitionTime)
				{
					_currentRotation = Mathf.Lerp(_currentRotation, Instance.WeaponAnimation.PickUpAngles, _time / _transitionTime);
					if (base.Decorator.IsMelee)
					{
						_targetPosition = base.Decorator.DefaultPosition;
						_targetRotation = Quaternion.Euler(0f, 0f, _currentRotation);
					}
					else
					{
						_targetPosition = Quaternion.AngleAxis(_currentRotation, Vector3.right) * base.PivotVector;
						_targetRotation = Quaternion.Euler(_currentRotation + base.Decorator.DefaultAngles.x, base.Decorator.DefaultAngles.y, base.Decorator.DefaultAngles.z);
					}
					if (!Instance._isIronSight)
					{
						base.Decorator.CurrentPosition = _targetPosition;
						base.Decorator.CurrentRotation = _targetRotation;
					}
					_time += Time.deltaTime;
				}
				if (_time > _transitionTime * 0.25f)
				{
					base.Weapon.IsWeaponActive = true;
				}
				if (_time > _transitionTime)
				{
					Finish();
				}
			}
			if (!(_time > _transitionTime * 0.25f))
			{
				return;
			}
			if (Instance._isIronSight)
			{
				_pivotOffset = Vector3.Lerp(_pivotOffset, Vector2.zero, Time.deltaTime * 20f);
				if (base.Decorator.CurrentPosition == base.Decorator.IronSightPosition)
				{
					Instance._isIronSightPosDone = true;
				}
				else
				{
					Instance._isIronSightPosDone = false;
				}
			}
			else
			{
				_pivotOffset = Vector3.Lerp(_pivotOffset, new Vector3(0f, 0f, 0.2f), Time.deltaTime * 10f);
			}
			if (Instance._fire.time < Instance._fire.Duration)
			{
				if (!base.IsRunning)
				{
					if (!Instance._isIronSight && _pivotOffset == new Vector3(0f, 0f, 0.2f))
					{
						Instance._fire.HandleFeedback();
						base.Decorator.CurrentPosition = _targetPosition + Instance._fire.PositionOffset;
						base.Decorator.CurrentRotation = _targetRotation * Instance._fire.RotationOffset;
					}
					else
					{
						base.Decorator.CurrentPosition = base.PivotVector + Instance._dip.PositionOffset;
						base.Decorator.CurrentRotation = _targetRotation * Instance._dip.RotationOffset;
					}
					_isFiring = true;
				}
				return;
			}
			if (_isFiring)
			{
				_isFiring = false;
				Instance._time = 0f;
				Instance._angleX = 0f;
				Instance._angleY = 0f;
			}
			Quaternion identity = Quaternion.identity;
			identity = ((!Instance._isIronSight || !(Instance._dip.PositionOffset == Vector3.zero)) ? Instance.CalculateBobDip() : Quaternion.identity);
			if (!base.Decorator.IsMelee)
			{
				base.Decorator.CurrentPosition = identity * base.PivotVector + Instance._dip.PositionOffset;
				base.Decorator.CurrentRotation = _targetRotation * Instance._dip.RotationOffset * identity;
			}
			else
			{
				base.Decorator.CurrentRotation = _targetRotation * Instance._dip.RotationOffset * identity;
			}
		}

		public override void Finish()
		{
			if (_isRunning)
			{
				_isRunning = false;
				if (base.Weapon != null)
				{
					base.Weapon.IsWeaponActive = true;
					Instance._currentWeaponMode = WeaponMode.Primary;
				}
				if (base.Decorator.IsMelee)
				{
					_targetRotation = Quaternion.Euler(0f, 0f, Instance.WeaponAnimation.PickUpAngles);
					_targetPosition = base.Decorator.DefaultPosition;
				}
				else
				{
					_targetRotation = Quaternion.Euler(Instance.WeaponAnimation.PickUpAngles + base.Decorator.DefaultAngles.x, base.Decorator.DefaultAngles.y, base.Decorator.DefaultAngles.z);
					_targetPosition = Quaternion.AngleAxis(Instance.WeaponAnimation.PickUpAngles, Vector3.right) * base.PivotVector;
				}
			}
		}

		public override string ToString()
		{
			return "Pick Up State";
		}
	}

	private class PutDownState : WeaponState
	{
		private bool _destroy;

		public PutDownState(BaseWeaponLogic weapon, BaseWeaponDecorator decorator, bool destroy = false)
			: base(weapon, decorator)
		{
			_destroy = destroy;
			_currentRotation = decorator.CurrentRotation.eulerAngles.x;
			if (_currentRotation > 300f)
			{
				_currentRotation = 360f - _currentRotation;
			}
			if (!decorator.IsMelee)
			{
				_pivotOffset = -Instance._pivotPoint.localPosition;
			}
			_transitionTime = Instance.WeaponAnimation.PutDownDuration;
			if (base.Weapon != null)
			{
				base.Weapon.IsWeaponActive = false;
			}
		}

		public override void Update()
		{
			if (base.IsRunning && base.IsValid && !(_time > _transitionTime))
			{
				if (base.Decorator.IsMelee)
				{
					_currentRotation = Mathf.Lerp(_currentRotation, -90f, _time / _transitionTime);
					_targetPosition = base.Decorator.DefaultPosition;
					_targetRotation = Quaternion.Euler(0f, 0f, _currentRotation);
				}
				else
				{
					_currentRotation = Mathf.Lerp(_currentRotation, Instance.WeaponAnimation.PutDownAngles, _time / _transitionTime);
					_targetPosition = Quaternion.AngleAxis(_currentRotation, Vector3.right) * base.PivotVector;
					_targetRotation = Quaternion.Euler(_currentRotation, 0f, 0f);
				}
				base.Decorator.CurrentPosition = _targetPosition;
				base.Decorator.CurrentRotation = _targetRotation;
				_time += Time.deltaTime;
				if (_time > _transitionTime)
				{
					Finish();
				}
			}
		}

		public override void Finish()
		{
			if (!_isRunning)
			{
				return;
			}
			_isRunning = false;
			if ((bool)base.Decorator)
			{
				base.Decorator.IsEnabled = false;
				base.Decorator.CurrentPosition = base.Decorator.DefaultPosition;
				base.Decorator.CurrentRotation = _targetRotation;
				if (_destroy)
				{
					UnityEngine.Object.Destroy(base.Decorator.gameObject);
				}
			}
		}

		public override string ToString()
		{
			return "Put down";
		}
	}

	[Serializable]
	public class FeedbackData
	{
		public float timeToPeak;

		public float timeToEnd;

		public float noise;

		public float angle;

		public float strength;

		public float recoilTime;
	}

	protected struct Feedback
	{
		public float time;

		public float noise;

		public float angle;

		public float timeToPeak;

		public float timeToEnd;

		public float strength;

		public float recoilTime;

		public Vector3 direction;

		public Vector3 rotationAxis;

		private float _maxAngle;

		private float _angle;

		private Vector3 _positionOffset;

		private Quaternion _rotationOffset;

		public float DebugAngle
		{
			get
			{
				return _angle;
			}
		}

		public float Duration
		{
			get
			{
				return timeToPeak + timeToEnd;
			}
		}

		public Vector3 PositionOffset
		{
			get
			{
				return _positionOffset;
			}
		}

		public Quaternion RotationOffset
		{
			get
			{
				return _rotationOffset;
			}
		}

		public void HandleFeedback()
		{
			float num = 0f;
			float num2 = UnityEngine.Random.Range(0f - noise, noise);
			_maxAngle = Mathf.Lerp(_maxAngle, angle, Time.deltaTime * 10f);
			if (time < Duration)
			{
				time += Time.deltaTime;
				if (time < Duration)
				{
					if (time < timeToPeak)
					{
						num = strength * Mathf.Sin(time * (float)Math.PI * 0.5f / timeToPeak);
						noise = Mathf.Lerp(noise, 0f, time / timeToPeak);
						_angle = Mathf.Lerp(0f, _maxAngle, Mathf.Pow(time / timeToPeak, 2f));
					}
					else
					{
						float t = (time - timeToPeak) / timeToEnd;
						num = strength * Mathf.Cos((time - timeToPeak) * (float)Math.PI * 0.5f / timeToEnd);
						_angle = Mathf.Lerp(_maxAngle, 0f, t);
						if (time != 0f)
						{
							num2 = 0f;
						}
					}
					if ((bool)Singleton<WeaponController>.Instance.CurrentWeapon)
					{
						_positionOffset = num * direction + Singleton<WeaponController>.Instance.CurrentWeapon.transform.right * num2 + Singleton<WeaponController>.Instance.CurrentWeapon.transform.up * num2;
						_rotationOffset = Quaternion.AngleAxis(_angle, rotationAxis);
					}
				}
				else
				{
					_angle = 0f;
					_positionOffset = Vector3.zero;
					_rotationOffset = Quaternion.identity;
				}
			}
			else
			{
				time = 0f;
				_angle = 0f;
				_positionOffset = Vector3.zero;
				_rotationOffset = Quaternion.identity;
			}
		}

		public void Reset()
		{
			time = 0f;
			timeToEnd = 0f;
			timeToPeak = -1f;
			angle = 0f;
			direction = Vector3.zero;
			_angle = 0f;
			_positionOffset = Vector3.zero;
			_rotationOffset = Quaternion.identity;
		}
	}

	[Serializable]
	public class WeaponAnimData
	{
		public float PutDownAngles = 30f;

		public float PutDownDuration;

		public float PickUpAngles;

		public float PickUpDuration;
	}

	private WeaponMode _currentWeaponMode;

	private WeaponState _pickupWeaponState;

	private WeaponState _putDownWeaponState;

	private WeaponBobManager _bobManager;

	public FeedbackData WeaponDip;

	public FeedbackData WeaponFire;

	protected Feedback _fire;

	protected Feedback _dip;

	private bool _needLerp;

	public WeaponAnimData WeaponAnimation;

	private float _angleY;

	private float _angleX;

	private float _time;

	private float _sign;

	[SerializeField]
	private Transform _pivotPoint;

	private bool _isIronSight;

	private bool _isIronSightPosDone;

	public static WeaponFeedbackManager Instance { get; private set; }

	public static bool IsBobbing
	{
		get
		{
			return (bool)Instance && Instance._bobManager.Mode != LevelCamera.BobMode.None;
		}
	}

	public bool IsIronSighted
	{
		get
		{
			return _isIronSight;
		}
		private set
		{
			_isIronSight = value;
			GameState.Current.PlayerData.IsIronSighted.Value = value;
		}
	}

	public WeaponMode CurrentWeaponMode
	{
		get
		{
			return _currentWeaponMode;
		}
		private set
		{
			_currentWeaponMode = value;
		}
	}

	public bool _isWeaponInIronSightPosition
	{
		get
		{
			return _isIronSight && _isIronSightPosDone;
		}
	}

	private void Awake()
	{
		Instance = this;
		_bobManager = new WeaponBobManager();
	}

	private void OnEnable()
	{
		_dip.Reset();
		_fire.Reset();
		CurrentWeaponMode = WeaponMode.PutDown;
	}

	private void Update()
	{
		if (_putDownWeaponState != null)
		{
			_putDownWeaponState.Update();
		}
		if (_pickupWeaponState != null)
		{
			_pickupWeaponState.Update();
		}
	}

	private Quaternion CalculateBobDip()
	{
		if (_dip.time <= _dip.Duration)
		{
			_dip.HandleFeedback();
		}
		else if (_needLerp)
		{
			_angleX = Mathf.Lerp(_angleX, 0f, Time.deltaTime * 9f);
			_angleY = Mathf.Lerp(_angleY, 0f, Time.deltaTime * 9f);
			if (_angleX < 0.01f && _angleY < 0.01f)
			{
				_time = 0f;
				_needLerp = false;
			}
		}
		else
		{
			float num = Mathf.Sin(_bobManager.Data.Frequency * _time);
			_angleX = Mathf.Abs(_bobManager.Data.XAmplitude * num);
			_angleY = _bobManager.Data.YAmplitude * num * _sign;
			_time += Time.deltaTime;
		}
		return Quaternion.Euler(_angleX, _angleY, 0f);
	}

	public static void SetBobMode(LevelCamera.BobMode mode)
	{
		if ((bool)Instance && Instance._bobManager.Mode != mode)
		{
			Instance._bobManager.Mode = mode;
			if (mode == LevelCamera.BobMode.Run)
			{
				Instance._needLerp = false;
				Instance._sign = ((!AutoMonoBehaviour<InputManager>.Instance.IsDown(GameInputKey.Right)) ? 1 : (-1));
				Instance._time = Mathf.Asin(Instance._angleX / Instance._bobManager.Data.XAmplitude) / Instance._bobManager.Data.Frequency;
			}
			else
			{
				Instance._needLerp = true;
			}
		}
	}

	public void LandingDip()
	{
		if ((!(_fire.time > 0f) || !(_fire.time < _fire.Duration)) && CurrentWeaponMode != WeaponMode.PutDown)
		{
			_dip.time = 0f;
			_dip.angle = WeaponDip.angle;
			_dip.noise = WeaponDip.noise;
			_dip.strength = WeaponDip.strength;
			_dip.timeToPeak = WeaponDip.timeToPeak;
			_dip.timeToEnd = WeaponDip.timeToEnd;
			_dip.direction = Vector3.down;
			_dip.rotationAxis = Vector3.right;
		}
	}

	public void Fire()
	{
		if (CurrentWeaponMode != WeaponMode.PutDown)
		{
			_fire.noise = WeaponFire.noise;
			_fire.strength = WeaponFire.strength;
			_fire.timeToPeak = WeaponFire.timeToPeak;
			_fire.timeToEnd = WeaponFire.timeToEnd;
			_fire.direction = Vector3.back;
			_fire.rotationAxis = Vector3.left;
			_fire.recoilTime = WeaponFire.recoilTime;
			if (_dip.time < _dip.Duration)
			{
				_dip.Reset();
			}
			if (_fire.time > _fire.recoilTime && _fire.time < _fire.Duration)
			{
				_fire.time = WeaponFire.timeToPeak / 3f;
				_fire.angle = WeaponFire.angle / 3f;
			}
			else if (_fire.time >= _fire.Duration)
			{
				_fire.time = 0f;
				_fire.angle = WeaponFire.angle;
			}
		}
	}

	public void PutDown(bool destroy = false)
	{
		if (_pickupWeaponState != null && _pickupWeaponState.IsValid)
		{
			PutDownWeapon(_pickupWeaponState.Weapon, _pickupWeaponState.Decorator, destroy);
			_pickupWeaponState = null;
		}
	}

	public void PickUp(WeaponSlot slot)
	{
		if (_pickupWeaponState != null && _pickupWeaponState.IsValid)
		{
			if (_pickupWeaponState.Weapon == slot.Logic)
			{
				return;
			}
			PutDownWeapon(_pickupWeaponState.Weapon, _pickupWeaponState.Decorator);
		}
		else if (_pickupWeaponState == null && _putDownWeaponState != null && _putDownWeaponState.Weapon == slot.Logic)
		{
			_putDownWeaponState.Finish();
		}
		_pickupWeaponState = new PickUpState(slot.Logic, slot.Decorator);
		WeaponFire.recoilTime = WeaponConfigurationHelper.GetRateOfFire(slot.View);
		WeaponFire.strength = WeaponConfigurationHelper.GetRecoilMovement(slot.View);
		WeaponFire.angle = WeaponConfigurationHelper.GetRecoilKickback(slot.View);
	}

	public void BeginIronSight()
	{
		if (!_isIronSight)
		{
			IsIronSighted = true;
		}
	}

	public void EndIronSight()
	{
		IsIronSighted = false;
	}

	public void ResetIronSight()
	{
		IsIronSighted = false;
		if (_pickupWeaponState != null)
		{
			_pickupWeaponState.Reset();
		}
		if (_putDownWeaponState != null)
		{
			_putDownWeaponState.Reset();
		}
	}

	private void PutDownWeapon(BaseWeaponLogic weapon, BaseWeaponDecorator decorator, bool destroy = false)
	{
		if (_putDownWeaponState != null)
		{
			_putDownWeaponState.Finish();
		}
		_putDownWeaponState = new PutDownState(weapon, decorator, destroy);
	}

	public void SetFireFeedback(FeedbackData data)
	{
		WeaponFire.angle = data.angle;
		WeaponFire.noise = data.noise;
		WeaponFire.strength = data.strength;
		WeaponFire.timeToEnd = data.timeToEnd;
		WeaponFire.timeToPeak = data.timeToPeak;
		WeaponFire.recoilTime = data.recoilTime;
	}
}
