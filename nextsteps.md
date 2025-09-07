in Player UI, add a button

Recommended Approach: Unity Input System + On-Screen Button
1. Define an Input Action
In your InputActions asset, create an action called Move (type: Value, Vector2).
Bind WASD/arrow keys for keyboard, and (optionally) a virtual joystick or button for touch.
2. Add an On-Screen Button
Install the Input System package.
In your Canvas, add an On-Screen Button (GameObject > UI > Input System > On-Screen Button).
In the On-Screen Button component, set the Control Path to <Keyboard>/w or to the action you want to trigger (e.g., /actions/Move).
3. Connect the Input System to Your Player Script
In your movement script, read the value from the Input System action (not directly from Input.GetAxis).
The Input System will automatically combine keyboard and on-screen button input.


using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public PlayerMotor playerMotor;
    public InputActionAsset actions;

    private InputAction moveAction;

    void Awake()
    {
        moveAction = actions.FindAction("Move");
    }

    void OnEnable() => moveAction.Enable();
    void OnDisable() => moveAction.Disable();

    void Update()
    {
        Vector2 move = moveAction.ReadValue<Vector2>();
        playerMotor.Move(move.y, move.x); // y = vertical, x = horizontal
    }
}