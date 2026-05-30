using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFeedback : MonoBehaviour
{
	public enum FeedbackType
	{
		Land = 0,
		Damage = 1,
		Weapon = 2
	}

	[Serializable]
	public class Feedback
	{
		public FeedbackType Type;

		public float Peak;

		public float TimeToPeak;

		public float TimeToEnd;

		public float MaxNoise;

		public float MaxAngle;

		private Vector3 _dir;

		public Feedback(Feedback f)
		{
			_dir = Vector3.zero;
			Type = f.Type;
			Peak = f.Peak;
			TimeToPeak = f.TimeToPeak;
			TimeToEnd = f.TimeToEnd;
		}

		public void SetDirection(Vector3 dir)
		{
			_dir = dir;
		}

		public Vector3 GetDirection()
		{
			return _dir;
		}
	}

	public bool DEBUG = true;

	private static CameraFeedback _instance;

	public Feedback[] Feedbacks;

	private Feedback _currentFeedback;

	private Transform _transformCache;

	private Quaternion _tmpRotation;

	private Vector3 _rotationAxis;

	private float _timer;

	private float _angle;

	private float _testAngle;

	public static CameraFeedback Instance
	{
		get
		{
			return _instance;
		}
	}

	private void Awake()
	{
		_instance = this;
		_transformCache = base.transform;
		_currentFeedback = null;
		_tmpRotation = Quaternion.identity;
	}

	private void Update()
	{
		if (_currentFeedback == null)
		{
			if (_transformCache.localPosition.sqrMagnitude > 0.001f)
			{
				_transformCache.localPosition = Vector3.Lerp(_transformCache.localPosition, Vector3.zero, Time.deltaTime);
				_transformCache.localRotation = Quaternion.Lerp(_transformCache.localRotation, Quaternion.identity, Time.deltaTime);
			}
			return;
		}
		Vector3 direction = _currentFeedback.GetDirection();
		float peak = _currentFeedback.Peak;
		float num = UnityEngine.Random.Range(0f - _currentFeedback.MaxNoise, _currentFeedback.MaxNoise);
		if (_timer < _currentFeedback.TimeToEnd + _currentFeedback.TimeToPeak)
		{
			if (_timer < _currentFeedback.TimeToPeak)
			{
				peak *= Mathf.Sin(_timer * (float)Math.PI * 0.5f / _currentFeedback.TimeToPeak);
				num = Mathf.Lerp(num, 0f, _timer / _currentFeedback.TimeToPeak);
				_angle = Mathf.Lerp(0f, _currentFeedback.MaxAngle, _timer / _currentFeedback.TimeToPeak);
			}
			else
			{
				float t = (_timer - _currentFeedback.TimeToPeak) / _currentFeedback.TimeToEnd;
				peak = Mathf.Lerp(peak, 0f, t);
				_angle = Mathf.Lerp(_angle, 0f, t);
				num = 0f;
			}
			_timer += Time.deltaTime;
			_transformCache.localPosition = peak * direction + _transformCache.right * num + _transformCache.up * num;
			_tmpRotation = Quaternion.AngleAxis(_angle, _rotationAxis);
			_testAngle = _angle;
		}
		else
		{
			_timer = 0f;
			_tmpRotation = Quaternion.identity;
			_currentFeedback = null;
		}
	}

	private void DoApplyFeedback()
	{
		GUI.Label(new Rect(10f, 50f, 300f, 20f), "Camera local position = " + _transformCache.localPosition);
		GUI.Label(new Rect(10f, 60f, 300f, 20f), "Camera world position = " + _transformCache.position);
		GUI.Label(new Rect(10f, 70f, 300f, 20f), "Rotation Axis = " + _rotationAxis);
		GUI.Label(new Rect(10f, 80f, 300f, 20f), "Rotation Angle = " + _testAngle);
		if (GUI.Button(new Rect(10f, 100f, 60f, 25f), "Land"))
		{
			onPlayerLand(new PlayerLandEvent());
		}
		if (GUI.Button(new Rect(80f, 100f, 60f, 25f), "Damage"))
		{
			onDamage(new GetDamageEvent(Vector3.back));
		}
		if (GUI.Button(new Rect(150f, 100f, 60f, 25f), "Weapon"))
		{
			onWeaponShoot(new WeaponShootEvent(Vector3.back, Feedbacks[2].MaxNoise, Feedbacks[2].MaxAngle));
		}
		Vector3[] array = new Vector3[3]
		{
			new Vector3(-0.8f, -0.3f, 0.6f),
			new Vector3(-0.8f, -0.1f, 0.6f),
			new Vector3(0.5f, -0.7f, 0.5f)
		};
		for (int i = 0; i < array.Length; i++)
		{
			if (GUI.Button(new Rect(10f, 125 + 25 * i, 100f, 25f), "Damage " + i))
			{
				onDamage(new GetDamageEvent(array[i]));
			}
		}
	}

	private void OnDrawGizmos()
	{
		if (!_transformCache)
		{
			_transformCache = base.transform;
		}
		Gizmos.color = Color.green;
		Gizmos.DrawRay(_transformCache.position, Feedbacks[1].GetDirection());
	}

	private void onPlayerLand(PlayerLandEvent ev)
	{
		ApplyFeedback(FeedbackType.Land, Vector3.down, Vector3.right);
	}

	private void onDamage(GetDamageEvent ev)
	{
		ApplyFeedback(FeedbackType.Damage, ev.Force, Vector3.zero);
	}

	private void onWeaponShoot(WeaponShootEvent ev)
	{
		Feedbacks[2].Peak = 1f;
		Feedbacks[2].MaxNoise = ev.Noise;
		Feedbacks[2].MaxAngle = ev.Angle;
		ApplyFeedback(FeedbackType.Weapon, ev.Force, Vector3.left);
	}

	public void ApplyFeedback(FeedbackType t, Vector3 dir, Vector3 rotAxis)
	{
		_timer = 0f;
		_currentFeedback = Feedbacks[(int)t];
		_currentFeedback.SetDirection(dir);
		_rotationAxis = ((!(rotAxis == Vector3.zero)) ? rotAxis : _transformCache.InverseTransformDirection(Vector3.Cross(Vector3.up, dir)));
	}

	public void ApplyFeedback(Vector3 dir, float noise, float angle)
	{
		Feedbacks[2].Peak = 1f;
		Feedbacks[2].MaxNoise = noise;
		Feedbacks[2].MaxAngle = angle;
		ApplyFeedback(FeedbackType.Weapon, dir, Vector3.left);
	}

	public Quaternion GetFeedbackRoation()
	{
		return _tmpRotation;
	}
}
