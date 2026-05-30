using UnityEngine;

[RequireComponent(typeof(Camera))]
public class WeaponCamera : MonoBehaviour
{
	private const float RESET_VELOCITY = 5f;

	private const float LERP_DURATION = 0.1f;

	[SerializeField]
	private float _maxDisplacementDelta = 0.4f;

	[SerializeField]
	private float _maxDisplacement = 0.8f;

	private Transform _transform;

	public Vector2 _currentAngle = Vector2.zero;

	public bool IsEnabled
	{
		get
		{
			return base.GetComponent<Camera>().enabled;
		}
		set
		{
			base.GetComponent<Camera>().enabled = value;
		}
	}

	private void Awake()
	{
		_transform = base.transform;
	}

	public void SetCameraEnabled(bool enabled)
	{
		base.GetComponent<Camera>().enabled = enabled;
	}

	private void LateUpdate()
	{
		if ((bool)WeaponFeedbackManager.Instance)
		{
			if (WeaponFeedbackManager.Instance.IsIronSighted)
			{
				_currentAngle = Vector2.Lerp(_currentAngle, Vector2.zero, Time.deltaTime * 5f);
				MoveWeapon();
				return;
			}
			float value = AutoMonoBehaviour<InputManager>.Instance.GetValue(GameInputKey.HorizontalLook);
			float value2 = AutoMonoBehaviour<InputManager>.Instance.GetValue(GameInputKey.VerticalLook);
			AddDeltaAngle(value, value2);
			MoveWeapon();
			_currentAngle = Vector2.Lerp(_currentAngle, Vector2.zero, Time.deltaTime * 5f);
		}
	}

	private void AddDeltaAngle(float x, float y)
	{
		Vector2 vector = Vector2.ClampMagnitude(new Vector2(x, y), _maxDisplacementDelta);
		_currentAngle = Vector2.ClampMagnitude(_currentAngle + vector, _maxDisplacement);
	}

	private void MoveWeapon()
	{
		_transform.localRotation = Quaternion.AngleAxis(_currentAngle.x, Vector3.up) * Quaternion.AngleAxis(0f - _currentAngle.y, Vector3.right);
	}
}
