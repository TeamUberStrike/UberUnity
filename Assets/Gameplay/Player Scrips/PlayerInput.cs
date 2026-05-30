using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerInput : MonoBehaviour
{
    private PlayerMotor playerMotor;
    private PlayerUI playerUI;
    private PlayerManager playerManager;

    // Init. This method runs before first frame
    private void Start()
    {
        playerMotor = GetComponent<PlayerMotor>();
        playerUI = GetComponent<PlayerUI>();
        playerManager = GetComponent<PlayerManager>();
        ToggleCursor(true);
    }

    // This method reads player input every frame
    private void Update()
    {
        // Get WASD — use GetAxisRaw for instant response (no Unity smoothing)
        float x = Input.GetAxisRaw("Vertical");
        float z = Input.GetAxisRaw("Horizontal");
        playerMotor.Move(x, z);

        // Get mouse
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = -Input.GetAxisRaw("Mouse Y");
        playerMotor.MouseLook(mouseX, mouseY);

        // Get right mouse button
        if (Input.GetButtonDown("Fire2")) { playerMotor.Aim(true); }
        if (Input.GetButtonUp("Fire2")) { playerMotor.Aim(false); }

        // Get Left mouse button
        if (Input.GetButtonDown("Fire1")) { playerMotor.Shoot(true); }
        if (Input.GetButtonUp("Fire1")) { playerMotor.Shoot(false); }

        // Track jump key state continuously — original UberStrike checks held state
        // every physics frame, not just button-down events. This enables bunny hopping:
        // press space mid-air → lands → instant jump on next physics frame.
        playerMotor.jumpHeld = Input.GetButton("Jump");
        if (Input.GetButtonUp("Jump")) { playerMotor.JumpReleased(); }

        // Get mouse scroll wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        playerMotor.MouseScroll(scroll);

        // Get Q key
        // This Input is hardcoded. We should make input axis for this later
        if (Input.GetKeyDown(KeyCode.Q)) { playerMotor.UseItem(0); }
        // Get E key
        if (Input.GetKeyDown(KeyCode.E)) { playerMotor.UseItem(1); }

        // Weapon switch: 1=Melee, 2=Primary, 3=Secondary, 4=Tertiary
        // Hand hierarchy: child 0=Primary, child 1=Melee, child 2=Secondary, child 3=Tertiary
        if (Input.GetKeyDown(KeyCode.Alpha1)) { playerMotor.Aim(true); playerMotor.Aim(false); playerMotor.SetWeapon(2); }  // Melee = index 1
        if (Input.GetKeyDown(KeyCode.Alpha2)) { playerMotor.Aim(true); playerMotor.Aim(false); playerMotor.SetWeapon(1); }  // Primary = index 0
        if (Input.GetKeyDown(KeyCode.Alpha3)) { playerMotor.Aim(true); playerMotor.Aim(false); playerMotor.SetWeapon(3); }  // Secondary = index 2
        if (Input.GetKeyDown(KeyCode.Alpha4)) { playerMotor.Aim(true); playerMotor.Aim(false); playerMotor.SetWeapon(4); }  // Tertiary = index 3

        // Pause
        // This Input is hardcoded. We should make input axis for this later
        if (Input.GetKeyDown(KeyCode.Escape)) { playerManager.PauseGame(); }

        // Toggle HUD
        // This Input is hardcoded. We should make input axis for this later
        if (Input.GetKeyDown(KeyCode.P)) { playerUI.ToggleHUD(); }


        // Get shift key for crouch
        if (Input.GetKeyDown(KeyCode.LeftShift)) { playerMotor.Crouch(true); }
        if (Input.GetKeyUp(KeyCode.LeftShift)) { playerMotor.Crouch(false); }

    }

    // Hide/show cursor
    public void ToggleCursor(bool hidden)
    {
        Cursor.visible = !hidden;
        if (hidden) Cursor.lockState = CursorLockMode.Locked;
        else Cursor.lockState = CursorLockMode.None;  
    }

}
