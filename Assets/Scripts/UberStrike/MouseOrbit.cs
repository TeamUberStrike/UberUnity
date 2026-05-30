using System;
using UnityEngine;

public class MouseOrbit : MonoBehaviour
{
	private const float zoomSpeedFactor = 15f;

	private const float zoomTouchSpeedFactor = 0.001f;

	private const float flingSpeedFactor = 0.1f;

	private const float orbitSpeedFactor = 3f;

	private const float panSpeedFactor = 0.01f;

	[SerializeField]
	private Transform target;

	private Vector2 zoomMax = new Vector2(1.3f, 5f);

	private Vector2 panMax = new Vector2(-0.5f, 0.5f);

	private Vector2 panTotalMax = new Vector2(-1f, 1f);

	public Vector3 OrbitConfig;

	public float yPanningOffset;

	private float zoomDistance = 5f;

	public int MaxX;

	private Vector2 mouseAxisSpin;

	private Vector3 mousePos;

	private bool listenToMouseUp;

	private bool isMouseDragging;

	public static MouseOrbit Instance { get; private set; }

	public Vector3 OrbitOffset { get; set; }

	public static bool Disable { get; set; }

	private void Awake()
	{
		Instance = this;
		Disable = false;
		if (target == null)
		{
			throw new NullReferenceException("MouseOrbit.target not set");
		}
	}

	private void Start()
	{
		mouseAxisSpin = Vector2.zero;
		Vector3 eulerAngles = base.transform.eulerAngles;
		OrbitConfig.x = eulerAngles.y;
		OrbitConfig.y = eulerAngles.x;
		MaxX = Screen.width;
	}

	private void OnEnable()
	{
		zoomDistance = (OrbitConfig.z = Mathf.Clamp(Vector3.Distance(base.transform.position, target.position), zoomMax[0], zoomMax[1]));
		OrbitConfig.x = base.transform.rotation.eulerAngles.y;
		OrbitConfig.y = base.transform.rotation.eulerAngles.x;
	}

	private void LateUpdate()
	{
		if (!PopupSystem.IsAnyPopupOpen && !PanelManager.IsAnyPanelOpen && GUIUtility.hotControl == 0)
		{
			if (base.GetComponent<Camera>().pixelRect.Contains(Input.mousePosition) && Input.GetAxis("Mouse ScrollWheel") != 0f)
			{
				OrbitConfig.z = Mathf.Clamp(zoomDistance - Input.GetAxis("Mouse ScrollWheel") * 15f, zoomMax[0], zoomMax[1]);
			}
			if (Input.GetMouseButtonDown(0) && base.GetComponent<Camera>().pixelRect.Contains(Input.mousePosition))
			{
				mouseAxisSpin = Vector2.zero;
				listenToMouseUp = true;
				isMouseDragging = true;
			}
			if (Input.GetMouseButtonUp(0))
			{
				if (listenToMouseUp)
				{
					float num = Mathf.Clamp((Input.mousePosition - mousePos).magnitude, 0f, 3f);
					mouseAxisSpin = (Input.mousePosition - mousePos).normalized * num;
				}
				else
				{
					mouseAxisSpin = Vector2.zero;
				}
				listenToMouseUp = false;
			}
			mousePos = Input.mousePosition;
			if (isMouseDragging && Input.GetMouseButton(0))
			{
				OrbitConfig.x += Input.GetAxis("Mouse X") * 3f;
				yPanningOffset -= Input.GetAxis("Mouse Y") * 0.01f * ((!IsValueWithin(yPanningOffset, panMax[0], panMax[1])) ? 0.3f : 1f);
			}
			else if (mouseAxisSpin.magnitude > 0.010000001f)
			{
				mouseAxisSpin = Vector2.Lerp(mouseAxisSpin, Vector2.zero, Time.deltaTime * 2f);
				OrbitConfig.x += mouseAxisSpin.x * 0.1f;
				yPanningOffset -= mouseAxisSpin.y * 0.01f * 0.1f * ((!IsValueWithin(yPanningOffset, panMax[0], panMax[1])) ? 0.3f : 1f);
			}
			else
			{
				mouseAxisSpin = Vector2.zero;
			}
			if (!isMouseDragging || !Input.GetMouseButton(0))
			{
				yPanningOffset = Mathf.Lerp(yPanningOffset, Mathf.Clamp(yPanningOffset, panMax[0], panMax[1]), Time.deltaTime * 10f);
			}
			yPanningOffset = Mathf.Clamp(yPanningOffset, panTotalMax[0], panTotalMax[1]);
			zoomDistance = Mathf.Lerp(zoomDistance, Mathf.Clamp(OrbitConfig.z, zoomMax[0], zoomMax[1]), Time.deltaTime * 5f);
			Transform(base.transform);
		}
		else
		{
			listenToMouseUp = false;
			mouseAxisSpin = Vector2.zero;
		}
		if (isMouseDragging && !Input.GetMouseButton(0))
		{
			isMouseDragging = false;
		}
	}

	public void Transform(Transform transform)
	{
		Vector3 position;
		Quaternion rotation;
		Transform(out position, out rotation);
		transform.position = position;
		transform.rotation = rotation;
	}

	public void Transform(out Vector3 position, out Quaternion rotation2)
	{
		float num = 1f - Mathf.Clamp01(zoomDistance / zoomMax[1]);
		Quaternion quaternion = (rotation2 = Quaternion.Euler(OrbitConfig.y, OrbitConfig.x, 0f));
		position = target.position + quaternion * new Vector3(0f, 0f, 0f - zoomDistance) + quaternion * (OrbitOffset + new Vector3(0f, yPanningOffset * num, 0f)) * zoomDistance;
	}

	public void SetTarget(Transform t)
	{
		target = t;
	}

	private static bool IsValueWithin(float value, float min, float max)
	{
		return value >= min && value <= max;
	}

	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}
}
