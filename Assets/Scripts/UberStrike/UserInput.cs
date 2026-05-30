using UberStrike.Core.Models;
using UnityEngine;

public static class UserInput
{
	public static float ZoomSpeed;

	public static Vector2 TouchLookSensitivity;

	public static Vector2 Mouse;

	public static Vector3 VerticalDirection;

	public static Vector3 HorizontalDirection;

	public static bool IsWalking
	{
		get
		{
			return (GameState.Current.PlayerData.KeyState & KeyState.Walking) != KeyState.Still && (GameState.Current.PlayerData.KeyState ^ KeyState.Horizontal) != KeyState.Still && (GameState.Current.PlayerData.KeyState ^ KeyState.Vertical) != KeyState.Still;
		}
	}

	public static bool IsMouseLooking
	{
		get
		{
			return AutoMonoBehaviour<InputManager>.Instance.RawValue(GameInputKey.HorizontalLook) != 0f || AutoMonoBehaviour<InputManager>.Instance.RawValue(GameInputKey.VerticalLook) != 0f;
		}
	}

	public static bool IsMovingVertically
	{
		get
		{
			return (GameState.Current.PlayerData.KeyState & (KeyState.Jump | KeyState.Crouch)) != 0;
		}
	}

	public static bool IsMovingUp
	{
		get
		{
			return (GameState.Current.PlayerData.KeyState & KeyState.Jump) != 0;
		}
	}

	public static bool IsMovingDown
	{
		get
		{
			return (GameState.Current.PlayerData.KeyState & KeyState.Crouch) != 0;
		}
	}

	public static Quaternion Rotation
	{
		get
		{
			return Quaternion.AngleAxis(Mouse.x, Vector3.up) * Quaternion.AngleAxis(Mouse.y, Vector3.left);
		}
	}

	public static Quaternion HorizontalRotation
	{
		get
		{
			return Quaternion.AngleAxis(Mouse.x, Vector3.up);
		}
	}

	public static Quaternion VerticalRotation
	{
		get
		{
			return Quaternion.AngleAxis(Mouse.y, Vector3.left);
		}
	}

	static UserInput()
	{
		ZoomSpeed = 1f;
		TouchLookSensitivity = new Vector2(1f, 0.5f);
		Reset();
	}

	public static void Reset()
	{
		Mouse = new Vector2(0f, 0f);
		VerticalDirection = Vector3.zero;
		HorizontalDirection = Vector3.zero;
	}

	public static void UpdateDirections()
	{
		ResetDirection();
		if ((GameState.Current.PlayerData.KeyState & KeyState.Left) != KeyState.Still)
		{
			HorizontalDirection.x -= 127f;
		}
		if ((GameState.Current.PlayerData.KeyState & KeyState.Right) != KeyState.Still)
		{
			HorizontalDirection.x += 127f;
		}
		if ((GameState.Current.PlayerData.KeyState & KeyState.Forward) != KeyState.Still)
		{
			HorizontalDirection.z += 127f;
		}
		if ((GameState.Current.PlayerData.KeyState & KeyState.Backward) != KeyState.Still)
		{
			HorizontalDirection.z -= 127f;
		}
		if ((GameState.Current.PlayerData.KeyState & KeyState.Jump) != KeyState.Still)
		{
			VerticalDirection.y += 127f;
		}
		if ((GameState.Current.PlayerData.KeyState & KeyState.Crouch) != KeyState.Still)
		{
			VerticalDirection.y -= 127f;
		}
		HorizontalDirection.Normalize();
		VerticalDirection.Normalize();
	}

	public static void ResetDirection()
	{
		HorizontalDirection = Vector3.zero;
		VerticalDirection = Vector3.zero;
	}

	public static KeyState GetkeyState(GameInputKey slot)
	{
		switch (slot)
		{
		case GameInputKey.Forward:
			return KeyState.Forward;
		case GameInputKey.Backward:
			return KeyState.Backward;
		case GameInputKey.Right:
			return KeyState.Right;
		case GameInputKey.Left:
			return KeyState.Left;
		case GameInputKey.Crouch:
			return KeyState.Crouch;
		case GameInputKey.Jump:
			return KeyState.Jump;
		default:
			return KeyState.Still;
		}
	}

	public static void SetRotation(float hAngle = 0f, float vAngle = 0f)
	{
		Mouse = new Vector2(hAngle, 0f - vAngle);
		UpdateMouse();
		UpdateDirections();
	}

	public static void UpdateMouse()
	{
		if (Camera.main != null)
		{
			float num = Mathf.Pow(Camera.main.fieldOfView / ApplicationDataManager.ApplicationOptions.CameraFovMax, 1.1f);
			Mouse.x += AutoMonoBehaviour<InputManager>.Instance.RawValue(GameInputKey.HorizontalLook) * ApplicationDataManager.ApplicationOptions.InputXMouseSensitivity * num;
			Mouse.x = ClampAngle(Mouse.x, -360f, 360f);
			int num2 = ((!ApplicationDataManager.ApplicationOptions.InputInvertMouse) ? 1 : (-1));
			Mouse.y += AutoMonoBehaviour<InputManager>.Instance.RawValue(GameInputKey.VerticalLook) * ApplicationDataManager.ApplicationOptions.InputXMouseSensitivity * (float)num2 * num;
			Mouse.y = ClampAngle(Mouse.y, -88f, 88f);
		}
	}

	public static bool IsPressed(KeyState k)
	{
		return (GameState.Current.PlayerData.KeyState & k) != 0;
	}

	private static float ClampAngle(float angle, float min, float max)
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
