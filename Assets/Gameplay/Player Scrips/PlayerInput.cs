using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using DigitalRubyShared;



public class PlayerInput : MonoBehaviour
{
    public RectTransform joystickArea; 
    public InputActionAsset actions;
    private PlayerMotor playerMotor;
    private PlayerUI playerUI;
    private PlayerManager playerManager;

    private InputAction jumpAction;

    private InputAction moveAction;

    private InputAction shootAction;

    private InputAction aimAction;

    private InputAction pauseGameAction;

    private InputAction previousAction;

    private InputAction nextAction;

    private InputAction mouseScrollAction;

    private InputAction lookAction;

    private PanGestureRecognizer panGesture;

    // Init. This method runs before first frame
    private void Start()
    {
        playerMotor = GetComponent<PlayerMotor>();
        playerUI = GetComponent<PlayerUI>();
        playerManager = GetComponent<PlayerManager>();
        jumpAction = actions.FindAction("Jump");
        jumpAction.Enable();
        moveAction = actions["Move"];
        moveAction.Enable();
        shootAction = actions.FindAction("Shoot");
        shootAction.Enable();
        aimAction = actions.FindAction("Aim");
        aimAction.Enable();
        pauseGameAction = actions.FindAction("PauseGame");
        pauseGameAction.Enable();
        previousAction = actions.FindAction("Previous");
        previousAction.Enable();
        nextAction = actions.FindAction("Next");
        nextAction.Enable();
        mouseScrollAction = actions.FindAction("MouseScroll");
        mouseScrollAction.Enable();
        lookAction = actions.FindAction("Look");
        lookAction.Enable();
        CreatePanGesture();

        ToggleCursor(true);
    }

    // This method reads player input every frame
    private void Update()
    {
        // Get WASD
        Vector2 input = moveAction.ReadValue<Vector2>();
        playerMotor.Move(input.y, input.x);

        if (Touchscreen.current == null) // pan gesture for touch devices
        {
            Vector2 lookInput = lookAction.ReadValue<Vector2>();
            float mouseX = lookInput.x * 0.1f;
            float mouseY = -lookInput.y * 0.1f;
            playerMotor.MouseLook(mouseX, mouseY);
        }

        // Get right mouse button
        if (aimAction != null)
        {
            playerMotor.Aim(aimAction.IsPressed());
        }

        // Get Left mouse button
        if (shootAction != null)
        {
            playerMotor.Shoot(shootAction.IsPressed());
        }

        // Get spacebar
        if (jumpAction != null && jumpAction.WasPressedThisFrame())
        {
            playerMotor.Jump();
        }

        if (previousAction != null && previousAction.WasPressedThisFrame())
        {
            playerMotor.MouseScroll(-1);
        }

        if (nextAction != null && nextAction.WasPressedThisFrame())
        {
            playerMotor.MouseScroll(1);
        }

        Vector2 scrollDelta = mouseScrollAction.ReadValue<Vector2>();
        playerMotor.MouseScroll(scrollDelta.y);

        // Get Q key
        // This Input is hardcoded. We should make input axis for this later
        if (Input.GetKeyDown(KeyCode.Q)) { playerMotor.UseItem(0); }
        // Get E key
        if (Input.GetKeyDown(KeyCode.E)) { playerMotor.UseItem(1); }

        // Weapon switch shorcuts
        if (Input.GetKeyDown(KeyCode.Alpha1)) { playerMotor.Aim(true); playerMotor.Aim(false); playerMotor.SetWeapon(1); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { playerMotor.Aim(true); playerMotor.Aim(false); playerMotor.SetWeapon(4); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { playerMotor.Aim(true); playerMotor.Aim(false); playerMotor.SetWeapon(3); }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { playerMotor.Aim(true); playerMotor.Aim(false); playerMotor.SetWeapon(2); }

        // Pause
        // This Input is hardcoded. We should make input axis for this later
        if (jumpAction != null && pauseGameAction.WasPressedThisFrame())
        {
            playerManager.PauseGame();
        }

        // Toggle HUD
        // This Input is hardcoded. We should make input axis for this later
        if (Input.GetKeyDown(KeyCode.P)) { playerUI.ToggleHUD(); }


        // Get ctrl key for crouch
        // This Input is hardcoded. We should make input axis for this later
        if (Input.GetKeyDown(KeyCode.LeftControl)) { playerMotor.Crouch(true); }
        if (Input.GetKeyUp(KeyCode.LeftControl)) { playerMotor.Crouch(false); }

    }
    private void CreatePanGesture()
    {
        panGesture = new PanGestureRecognizer();
        panGesture.MinimumNumberOfTouchesToTrack = 1;
        panGesture.StateUpdated += PanGestureCallback;
        FingersScript.Instance.AddGesture(panGesture);
    }

    private void PanGestureCallback(GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Executing)
        {
            DebugText("Panned, Location: {0}, {1}, Delta: {2}, {3}", gesture.FocusX, gesture.FocusY, gesture.DeltaX, gesture.DeltaY);
            float deltaX = Mathf.Clamp(gesture.DeltaX * 0.1f, -20f, 20f);
            float deltaY = Mathf.Clamp(gesture.DeltaY * 0.1f, -20f, 20f);
            playerMotor.MouseLook(deltaX, -deltaY);
        }
        if (gesture.State == GestureRecognizerState.Ended)
        {
            playerMotor.MouseLook(0f, 0f);
            DebugText("Pan ended");
        }
    }

    private void DebugText(string text, params object[] format)
    {
        //bottomLabel.text = string.Format(text, format);
        Debug.Log(string.Format(text, format));
    }

    // Hide/show cursor
    public void ToggleCursor(bool hidden)
    {
        Cursor.visible = !hidden;
        if (hidden) Cursor.lockState = CursorLockMode.Locked;
        else Cursor.lockState = CursorLockMode.None;
    }

}
