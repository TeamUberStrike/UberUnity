using UnityEngine;

public class HUDMultitouchController : MonoBehaviour
{
	[SerializeField]
	private UIEventReceiver upButton;

	[SerializeField]
	private UIEventReceiver downButton;

	[SerializeField]
	private UIEventReceiver rightButton;

	[SerializeField]
	private UIEventReceiver leftButton;

	[SerializeField]
	private UIEventReceiver jumpButton;

	private bool moveFwd;

	private bool moveBack;

	private bool moveRight;

	private bool moveLeft;

	private Vector2 lastDirection;

	private Vector2 MoveInteriaRolloff = new Vector2(24f, 20f);

	public bool Moving { get; private set; }

	private void Start()
	{
		upButton.OnPressed = delegate(bool el)
		{
			moveFwd = el;
		};
		downButton.OnPressed = delegate(bool el)
		{
			moveBack = el;
		};
		rightButton.OnPressed = delegate(bool el)
		{
			moveRight = el;
		};
		leftButton.OnPressed = delegate(bool el)
		{
			moveLeft = el;
		};
		jumpButton.OnPressed = delegate(bool el)
		{
			TouchInput.WishJump = el;
		};
	}

	private void LateUpdate()
	{
		Moving = moveFwd || moveBack || moveLeft || moveRight;
		Vector2 zero = Vector2.zero;
		if (moveLeft)
		{
			zero += new Vector2(-1f, 0f);
		}
		if (moveRight)
		{
			zero += new Vector2(1f, 0f);
		}
		if (moveFwd)
		{
			zero += new Vector2(0f, 1f);
		}
		if (moveBack)
		{
			zero += new Vector2(0f, -1f);
		}
		if (zero.y == 0f)
		{
			zero.y = Mathf.Lerp(lastDirection.y, zero.y, Time.deltaTime * MoveInteriaRolloff.y);
		}
		if (zero.x == 0f)
		{
			zero.x = Mathf.Lerp(lastDirection.x, zero.x, Time.deltaTime * MoveInteriaRolloff.x);
		}
		lastDirection = TouchInput.WishDirection;
		TouchInput.WishDirection = zero;
	}
}
