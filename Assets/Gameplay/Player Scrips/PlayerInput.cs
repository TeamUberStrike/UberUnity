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
        shootAction = actions.FindAction("Attack");
        shootAction.Enable();
        aimAction = actions.FindAction("Aim");
        aimAction.Enable();
        pauseGameAction = actions.FindAction("PauseGame");
        pauseGameAction.Enable();

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

        // Get mouse scroll wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        playerMotor.MouseScroll(scroll);

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

    // Hide/show cursor
    public void ToggleCursor(bool hidden)
    {
        Cursor.visible = !hidden;
        if (hidden) Cursor.lockState = CursorLockMode.Locked;
        else Cursor.lockState = CursorLockMode.None;  
    }

}
