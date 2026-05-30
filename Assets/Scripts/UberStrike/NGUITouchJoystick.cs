using System;
using UnityEngine;

public class NGUITouchJoystick : MonoBehaviour
{
	[SerializeField]
	private GameObject backgroundContainer;

	[SerializeField]
	private UISprite movingStick;

	[SerializeField]
	private Vector2 joystickLimits = new Vector2(128f, 128f);

	[SerializeField]
	private Rect touchBoundary = new Rect(0f, 0f, Screen.width, Screen.height);

	public Action<Vector2> OnJoystickMoved;

	public Action OnJoystickStopped;

	private Rect boundary = default(Rect);

	private Rect joystickBoundary = default(Rect);

	private TouchFinger finger = new TouchFinger();

	private Vector2 joystickPosition = Vector2.zero;

	public Rect TouchBoundary
	{
		set
		{
			touchBoundary = value;
			boundary = touchBoundary;
		}
	}

	private void Awake()
	{
		boundary = touchBoundary;
	}

	private void Update()
	{
		Touch[] touches = Input.touches;
		for (int i = 0; i < touches.Length; i++)
		{
			Touch touch = touches[i];
			if (touch.phase == TouchPhase.Began && boundary.ContainsTouch(touch.position) && finger.FingerId == -1)
			{
				finger = new TouchFinger
				{
					StartPos = new Vector2(touch.position.x, touch.position.y),
					StartTouchTime = Time.time,
					FingerId = touch.fingerId
				};
				joystickBoundary = new Rect(touch.position.x - joystickLimits.x / 2f, touch.position.y - joystickLimits.y / 2f, joystickLimits.x, joystickLimits.y);
				Vector3 vector = UICamera.currentCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, UICamera.currentCamera.nearClipPlane));
				vector = backgroundContainer.transform.parent.InverseTransformPoint(new Vector3(vector.x, vector.y, 0f));
				backgroundContainer.transform.localPosition = vector;
				movingStick.transform.localPosition = vector;
				ShowJoystick(true);
			}
			else
			{
				if (finger.FingerId != touch.fingerId)
				{
					continue;
				}
				if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
				{
					joystickPosition.x = Mathf.Clamp(touch.position.x, joystickBoundary.x, joystickBoundary.x + joystickBoundary.width);
					joystickPosition.y = Mathf.Clamp(touch.position.y, joystickBoundary.y, joystickBoundary.y + joystickBoundary.height);
					Vector3 position = new Vector3(joystickPosition.x, joystickPosition.y, 0f);
					position = UICamera.currentCamera.ScreenToWorldPoint(position);
					movingStick.transform.localPosition = backgroundContainer.transform.parent.InverseTransformPoint(position);
					Vector2 zero = Vector2.zero;
					zero.x = (joystickPosition.x - finger.StartPos.x) * 2f / joystickBoundary.width;
					zero.y = (joystickPosition.y - finger.StartPos.y) * 2f / joystickBoundary.height;
					zero *= ApplicationDataManager.ApplicationOptions.TouchMoveSensitivity;
					if (touch.phase == TouchPhase.Moved && OnJoystickMoved != null)
					{
						OnJoystickMoved(zero);
					}
				}
				else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
				{
					ShowJoystick(false);
					boundary = touchBoundary;
					if (OnJoystickStopped != null)
					{
						OnJoystickStopped();
					}
					finger.Reset();
				}
			}
		}
	}

	private void ShowJoystick(bool show)
	{
		movingStick.enabled = show;
		NGUITools.SetActiveChildren(backgroundContainer, show);
		NGUITools.SetActiveChildren(movingStick.gameObject, show);
	}
}
