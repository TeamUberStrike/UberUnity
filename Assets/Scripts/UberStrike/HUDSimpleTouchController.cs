using UnityEngine;

public class HUDSimpleTouchController : MonoBehaviour
{
	[SerializeField]
	private UIEventReceiver jumpButton;

	[SerializeField]
	private UIEventReceiver fireButton;

	[SerializeField]
	private NGUITouchJoystick joystick;

	private void Start()
	{
		joystick.TouchBoundary = new Rect(0f, Screen.height / 2, new Rect(0f, 0f, (float)Screen.width * 0.4f, Screen.height).width, Screen.height / 2);
		joystick.OnJoystickMoved = delegate(Vector2 el)
		{
			TouchInput.WishDirection = el;
		};
		joystick.OnJoystickStopped = delegate
		{
			TouchInput.WishDirection = Vector2.zero;
		};
		jumpButton.OnPressed = delegate(bool el)
		{
			TouchInput.WishJump = el;
		};
		fireButton.OnPressed = delegate(bool el)
		{
			EventHandler.Global.Fire(new GlobalEvents.InputChanged(GameInputKey.PrimaryFire, el ? 1 : 0));
		};
	}
}
