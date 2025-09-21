using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInput : MonoBehaviour
{
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

    private InputAction statisticsAction;

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
        statisticsAction = actions.FindAction("Statistics");
        statisticsAction.Enable();

        ToggleCursor(true);
    }

    // This method reads player input every frame
    private void Update()
    {
        // Get WASD
        Vector2 input = moveAction.ReadValue<Vector2>();
        playerMotor.Move(input.y, input.x);

        // Get mouse
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = -Input.GetAxisRaw("Mouse Y");
        playerMotor.MouseLook(mouseX, mouseY);

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

        if (jumpAction != null && pauseGameAction.WasPressedThisFrame())
        {
            playerManager.PauseGame();
        }

        if (playerUI != null)
        {
            playerUI.ToggleStats(statisticsAction.IsPressed());
        }

        // Toggle HUD
        // This Input is hardcoded. We should make input axis for this later
        if (Input.GetKeyDown(KeyCode.P)) { playerUI.ToggleHUD(); }


        // Get ctrl key for crouch
        // This Input is hardcoded. We should make input axis for this later
        if (Input.GetKeyDown(KeyCode.LeftControl)) { playerMotor.Crouch(true); }
        if (Input.GetKeyUp(KeyCode.LeftControl)) { playerMotor.Crouch(false); }

    }

    // Hide/show cursor
    public void ToggleCursor(bool hidden)
    {
        Cursor.visible = !hidden;
        if (hidden) Cursor.lockState = CursorLockMode.Locked;
        else Cursor.lockState = CursorLockMode.None;  
    }

}
