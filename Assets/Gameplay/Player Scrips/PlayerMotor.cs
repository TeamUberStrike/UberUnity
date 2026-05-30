using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private Rigidbody rigidBody;
    public PhysicsMaterial pMaterial;
    internal Vector3 movement;
    private GameObject hand;
    private PlayerManager playerManager;
    private PlayerUI playerUI;
    private PlayerAudio playerAudio;

    private bool mapHasWater;
    private float groundLimit = 1.29f; // This value needs to changed if we modify player collider | original: 1.29f
    private float distanceToGround = 0f;

    private float playerRotationX = 0f;
    private float playerRotationY = 0f;
    private float lastRotation = 0.0f;

    internal bool quickitemsActive = false;
    internal bool secondaryQuickitemsActive = false;
    private string currentGroundMaterial="";

    bool canUsePowerUp = true;
    private bool aiming = false;
    private bool climbing = false;
    private bool swim = false;

    // Manual vertical velocity — decoupled from Rigidbody to match CharacterController behavior
    private float verticalVelocity = 0f;

    // Accumulated mouse input between frames for smooth camera
    private float accumulatedMouseX = 0f;
    private float accumulatedMouseY = 0f;

    public ParticleSystem distortion;

    private float moveSpeed = 0f;
    private bool sideways = false;

    public float groundMoveSpeed = 7.6f;  // Authentic: CharacterMoveController.PlayerWalkSpeed = 7.6
    internal float rotationSpeed = 75f;
    internal float originalRotationSpeed = 75f;
    [Header("Movement — authentic UberStrike 4.7.1 (CharacterMoveController) constants")]
    public float jumpForce = 15f;     // PlayerJumpSpeed = 15
    public float uberGravity = 50f;   // EnviromentSettings.Gravity = 50
    public float groundAccel = 15f;   // GroundAcceleration
    public float airAccel = 3f;       // AirAcceleration — low value enables strafe-jump / bunny-hop
    public float groundFriction = 8f; // GroundFriction
    public float stopSpeed = 8f;      // StopSpeed
    public float maxHorizontalSpeed = 22.8f; // ClampHorizontally (3x walk = StrafeJumpMultiplier)
    private Vector3 horizVel = Vector3.zero;  // persistent horizontal velocity (carries momentum)
    [Header("Pad Settings")]
    public float maxPadVerticalVel = 45f;  // Cap to prevent ceiling clipping on enclosed maps
    private Rigidbody primaryQuickItem;
    private Rigidbody secondaryQuickItem;
    public Transform playerCamera;

    private int airTime = 0;
    internal Collider capsule;
    private bool pendingHandEffect = false;
    private bool canHandEffect = true;
    private Vector3 handVelocity = Vector3.zero;
    private float handVelocityY = 0f;
    private float handPositionY = -0.359f;
    private Camera handCamera;
    private float handTime = 11f;
    private Animator handAnimator;
    private float rbDrag;
    private bool canJump = true;
    internal bool jumpHeld = false;  // Set by PlayerInput every frame (continuous state)
    private bool crouching = false;
    private Animator cameraAnimator;

    private float groundCheckRadius = 0.48f;
    private Vector3[] groundRayChecks = new Vector3[9];

    // Runs before first frame
    void Start()
    {
        // Enforce physics values on every spawn — prevents prefab serialized values
        // from overriding tuned values on death/respawn. Remove once prefab is updated.
        groundMoveSpeed = 7.6f;  // authentic PlayerWalkSpeed
        jumpForce = 15f;         // authentic PlayerJumpSpeed
        uberGravity = 50f;       // authentic EnviromentSettings.Gravity
        maxPadVerticalVel = 45f;

        capsule = GetComponent<Collider>();

        // Init array
        groundRayChecks[0] = new Vector3(groundCheckRadius,0f,0f);
        groundRayChecks[1] = new Vector3(0f, 0f, groundCheckRadius);
        groundRayChecks[2] = new Vector3(-groundCheckRadius, 0f, 0f);
        groundRayChecks[3] = new Vector3(0f, 0f, -groundCheckRadius);
        groundRayChecks[4] = new Vector3(-groundCheckRadius / 2, 0f, groundCheckRadius / 2);
        groundRayChecks[5] = new Vector3(groundCheckRadius / 2, 0f, -groundCheckRadius / 2);
        groundRayChecks[6] = new Vector3(-groundCheckRadius / 2, 0f, -groundCheckRadius / 2);
        groundRayChecks[7] = new Vector3(0f, 0f, 0f);

        // Init
        playerAudio = GetComponent<PlayerAudio>();
        playerManager = GetComponent<PlayerManager>();
        playerUI = GetComponent<PlayerUI>();
        rigidBody = GetComponent<Rigidbody>();
        playerCamera = transform.Find("Player Camera");
        cameraAnimator = playerCamera.gameObject.GetComponent<Animator>();
        hand = playerCamera.GetChild(0).gameObject;
        handCamera = playerCamera.Find("Camera Mask").gameObject.GetComponent<Camera>();

        Vector3 rot = playerCamera.localRotation.eulerAngles;
        lastRotation = rot.x;
        handAnimator = handCamera.GetComponent<Animator>();

        // Ensure Rigidbody interpolation is on for smooth visual movement
        rigidBody.interpolation = RigidbodyInterpolation.Interpolate;

        // Disable Unity's built-in gravity — we apply it manually to match
        // original UberStrike's CharacterController (gravity=50, not Unity's 9.81)
        rigidBody.useGravity = false;
        // Prevent fast-moving player from clipping through geometry
        rigidBody.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Water
        var envObj = GameObject.Find("/Environment");
        mapHasWater = envObj != null && envObj.GetComponent<Environment>() != null && envObj.GetComponent<Environment>().mapHasWater;
        rbDrag = rigidBody.linearDamping;

        UpdateOptions();
        UpdateQuickItems();
    }

    // activate quickitems if equipped
    private void UpdateQuickItems()
    {
        if (PlayerPrefs.HasKey("equipped_primary_quickitem"))
        {
            quickitemsActive = PlayerPrefs.GetString("equipped_primary_quickitem") != "null";
            playerUI.quickItemsActive = quickitemsActive;
            playerUI.primaryContainer.SetActive(quickitemsActive);

            if (quickitemsActive)
            {
                primaryQuickItem = (Resources.Load(PlayerPrefs.GetString("equipped_primary_quickitem"), typeof(GameObject)) as GameObject).GetComponent<Rigidbody>();
                playerUI.SetIcon(0, primaryQuickItem.gameObject);
            }
        }
        if (PlayerPrefs.HasKey("equipped_secondary_quickitem"))
        {
            secondaryQuickitemsActive = PlayerPrefs.GetString("equipped_secondary_quickitem") != "null";
            playerUI.secondaryQuickitemsActive = secondaryQuickitemsActive;
            playerUI.secondaryContainer.SetActive(secondaryQuickitemsActive);

            if (secondaryQuickitemsActive)
            {
                secondaryQuickItem = (Resources.Load(PlayerPrefs.GetString("equipped_secondary_quickitem"), typeof(GameObject)) as GameObject).GetComponent<Rigidbody>();
                playerUI.SetIcon(1, secondaryQuickItem.gameObject);
            }
        }
    }

    // updates game settings
    public void UpdateOptions()
    {
        //// Motion blur
        //if (PlayerPrefs.HasKey("motion_blur"))
        //    if (PlayerPrefs.GetInt("motion_blur") == -1)
        //        playerCamera.gameObject.GetComponent<Kino.Motion>().enabled = false;
        
        // Volume
        float v = 0.6f;
        if (PlayerPrefs.HasKey("mute"))
        {
            if (PlayerPrefs.GetInt("mute") == 1) v = 0f;            
            else v = PlayerPrefs.GetFloat("volume") / 100f;          
        }

        AudioListener.volume = v;

        // fov
        if (PlayerPrefs.HasKey("fov")) {
            float fov;            
                fov = PlayerPrefs.GetFloat("fov");
                playerCamera.gameObject.GetComponent<Camera>().fieldOfView = fov;
                playerUI.zoomMax = fov;
                playerUI.currentZoom = fov;           
        }

        // sensitivity
        rotationSpeed = 60.82f;
        if (PlayerPrefs.HasKey("sensitivy"))         
            rotationSpeed = 60.82f * PlayerPrefs.GetFloat("sensitivy");
        originalRotationSpeed = rotationSpeed;
    }

    // Runs every frame
    void Update()
    {
        getDistanceToGround();
        if (distanceToGround > 15) airTime++;

        if (transform.position.y < -100 || transform.position.y > 500) playerManager.Die(-1, -1);

        if (mapHasWater)
        {
            swim = rigidBody.position.y < 0;
            // useGravity is always false — manual gravity handles swim vs air
            if (swim) rigidBody.linearDamping = 1f;
            else rigidBody.linearDamping = rbDrag;

            // water distortion effect
            if (rigidBody.position.y > -0.8f && rigidBody.position.y < -0.5f)
            {
                ParticleSystem.MainModule main = distortion.main;
                main.loop = true;
                if (!distortion.isPlaying) distortion.Play();
            }
            else
            {
                ParticleSystem.MainModule main = distortion.main;
                main.loop = false;
            }
                       
        }
    }

    bool GetTransformDirectionCollision()
    {
        RaycastHit hit;

        Ray targetRay = new Ray(transform.position + new Vector3(0f,-0.3f,0f), transform.TransformDirection(movement));
        Debug.DrawRay(transform.position + new Vector3(0f, -0.4f, 0f), transform.TransformDirection(movement).normalized);

        if (Physics.Raycast(targetRay, out hit, 0.7f))
        {
            if (hit.transform.tag == "Untagged") return true;
        }
        return false;
    }

    // Do all movement in FixedUpdate method
    void FixedUpdate()
    {
        // Refresh ground distance at physics rate
        getDistanceToGround();
        float dt = Time.fixedDeltaTime;

        bool isGrounded = distanceToGround >= 0 && distanceToGround < groundLimit;

        // --- Jump check every physics frame (matches original CheckJump) ---
        // Original: checks if jump key is HELD + canJump + grounded → jumps instantly.
        // This enables bunny hopping: press space mid-air, land, instant jump next frame.
        if (isGrounded && jumpHeld && canJump && !crouching && !climbing && !swim)
        {
            canJump = false;
            verticalVelocity = jumpForce;
            pendingHandEffect = true;
        }

        // --- Vertical velocity (manual, like CharacterController) ---
        // Apply gravity: _currentVelocity[1] -= Gravity * dt (original CharacterMoveController)
        if (!isGrounded && !climbing)
        {
            if (swim)
                verticalVelocity -= uberGravity * 0.1f * dt;
            else
                verticalVelocity -= uberGravity * dt;
        }
        else if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = 0f;  // Stop falling when grounded
        }
        // Clamp vertical velocity — original: Mathf.Clamp(_currentVelocity[1], -150, 150)
        verticalVelocity = Mathf.Clamp(verticalVelocity, -150f, 150f);

        // Swim up with spacebar
        if (swim && Input.GetKey(KeyCode.Space))
            verticalVelocity += uberGravity * 0.3f * dt;

        // Ladder climbing — W=up, S=down. Use raw vertical input directly.
        // Don't use walk velocity (it pushes INTO ladder, collision cancels movement).
        if (climbing)
        {
            float climbInput = Input.GetAxisRaw("Vertical");  // W=1, S=-1
            if (Mathf.Abs(climbInput) > 0.1f)
                verticalVelocity = climbInput * moveSpeed * 1.5f;
            else
                verticalVelocity = 0f;  // Hang on ladder when not pressing W/S
            // Gentle push toward ladder to maintain OnCollisionStay contact
            padPush = transform.forward * 1.5f;
        }

        //physics material
        if (isGrounded && !GetTransformDirectionCollision()) capsule.material = null;
        else if (climbing) capsule.material = null;
        else capsule.material = pMaterial;

        // Player movement
        SetMoveSpeed();

        // --- Authentic 4.7.1 horizontal movement: Quake accel + friction (CharacterMoveController) ---
        // horizVel carries momentum across frames. Ground friction decelerates you; the low
        // air-acceleration (3) with NO air friction is exactly what builds speed when strafe-jumping.
        Vector3 wishDir = transform.TransformDirection(movement);
        wishDir.y = 0f;
        if (wishDir.sqrMagnitude > 1f) wishDir.Normalize();

        if (isGrounded && !climbing) ApplyFriction(dt);
        ApplyAcceleration(wishDir, moveSpeed, isGrounded ? groundAccel : airAccel, dt);

        // Clamp horizontal speed (original ClampHorizontally = 22.8)
        if (horizVel.sqrMagnitude > maxHorizontalSpeed * maxHorizontalSpeed)
            horizVel = horizVel.normalized * maxHorizontalSpeed;

        // --- Apply: Quake horizontal + manual vertical + external push (pads / explosions / ladder) ---
        // linearVelocity is set fresh each frame; the physics engine resolves wall/ceiling collisions.
        rigidBody.linearVelocity = new Vector3(horizVel.x, verticalVelocity, horizVel.z) + padPush;

        // Decay external push only (pads/explosions/ladder). Walk deceleration is ApplyFriction above.
        if (isGrounded)
            padPush = Vector3.Lerp(padPush, Vector3.zero, 8f * dt);
        else
            padPush = Vector3.Lerp(padPush, Vector3.zero, 1.5f * dt);

        // Player body rotation (uses accumulated mouse input)
        Quaternion deltaRotation = Quaternion.Euler(new Vector3(0f, accumulatedMouseX, 0f) * rotationSpeed);
        rigidBody.MoveRotation(rigidBody.rotation * deltaRotation);
        accumulatedMouseX = 0f;

        // Walk anim
        if (movement.magnitude > 0.3f)
        {
            if (!handAnimator.enabled) handAnimator.Play("defaultHand");

            // Enable animator
            handAnimator.enabled = true;
            handAnimator.SetBool("walk", true);
        }
        else
        {
            // Disable animator
            handAnimator.enabled = false;
            handAnimator.SetBool("walk", false);

            float rectW = 1f;
            handCamera.rect = new Rect(handCamera.rect.x + -1f * playerRotationY * 0.001f, 0f, rectW, rectW);
            handCamera.rect = new Rect(Mathf.SmoothDamp(handCamera.rect.x, 0f, ref handVelocityY, handTime * Time.deltaTime), 0f, rectW, rectW);
            if (handCamera.rect.x > -0.001f && handCamera.rect.x < 0.001f) handCamera.rect = new Rect(0f,0f,1f,1f);
        }

        // Hand Y
        if (pendingHandEffect && canHandEffect)
        {
            handPositionY = -0.6f;
            pendingHandEffect = false;
            canHandEffect = false;
            StartCoroutine(LimitHandEffect());
        }

        hand.transform.localPosition = new Vector3(0.263f, hand.transform.localPosition.y + -1f * playerRotationX * 0.001f,0.573f);
        hand.transform.localPosition = Vector3.SmoothDamp(hand.transform.localPosition, new Vector3(0.263f, handPositionY, 0.573f), ref handVelocity, handTime * Time.deltaTime);
    }

    // --- Authentic Quake-style helpers (ported from CharacterMoveController 4.7.1) ---

    // Ground friction: bleed off speed toward zero. No friction in air → momentum for bunny-hop.
    private void ApplyFriction(float dt)
    {
        float speed = horizVel.magnitude;
        if (speed < 0.0001f) { horizVel = Vector3.zero; return; }
        float control = Mathf.Max(stopSpeed, speed);
        float drop = control * groundFriction * dt;
        float newSpeed = Mathf.Max(speed - drop, 0f) / speed;
        horizVel *= newSpeed;
    }

    // Accelerate toward wishDir up to wishSpeed along that axis (classic Quake ground/air accel).
    private void ApplyAcceleration(Vector3 wishDir, float wishSpeed, float accel, float dt)
    {
        float current = Vector3.Dot(horizVel, wishDir);
        float add = wishSpeed - current;
        if (add <= 0f) return;
        float accelSpeed = accel * wishSpeed * dt;
        if (accelSpeed > add) accelSpeed = add;
        horizVel += wishDir * accelSpeed;
    }

    // Camera rotation in LateUpdate for smooth mouse look (runs every render frame, not fixed timestep)
    void LateUpdate()
    {
        // Player camera vertical rotation
        float clampAngle = 89f;
        lastRotation += accumulatedMouseY * rotationSpeed;
        lastRotation = Mathf.Clamp(lastRotation, -clampAngle, clampAngle);
        playerCamera.localRotation = Quaternion.Euler(lastRotation, 0f, 0.0f);
        accumulatedMouseY = 0f;
    }

    private void SetMoveSpeed()
    {
        moveSpeed = groundMoveSpeed;
        if (movement.z != 0 && movement.x != 0) sideways = true;     
        else sideways = false;
        
        if (aiming)
        {
            // Slow down when aim — original UberStrike: PLAYER_ZOOM_SLOWDOWN = 1.8f
            if(hand.GetComponent<PlayerHand>().currentWeapon.GetComponent<Sniper>() != null
                || hand.GetComponent<PlayerHand>().currentWeapon.GetComponent<Machinegun>() != null)
            {
                moveSpeed = groundMoveSpeed - 1.8f;
            }
        }

        if (crouching) moveSpeed = groundMoveSpeed * 0.23f;  // Original UberStrike: PLAYER_DUCK_SCALE
        if (swim) moveSpeed = groundMoveSpeed * 0.4f;  // Original UberStrike: PLAYER_SWIM_SCALE
    }

    internal void MouseScroll(float scroll)
    {
        if (aiming && scroll != 0)
        {
            // Sniper zoom
            if (hand.GetComponent<PlayerHand>().currentWeapon.GetComponent<Sniper>() != null) playerUI.Zoom(scroll);
        }
        else if(scroll != 0) hand.SendMessage("SetWeaponIndex", scroll);       
    }

    // Jump is now handled in FixedUpdate via jumpHeld state (continuous check).
    // This method kept for any direct callers but FixedUpdate is the primary path.
    internal void Jump()
    {
        Crouch(false);
    }

    // Called from PlayerInput when jump key is released
    internal void JumpReleased()
    {
        canJump = true;
    }

    internal void Shoot(bool hold)
    {
        if (hold) hand.GetComponent<PlayerHand>().currentWeapon.SendMessage("PrimaryFire", true);
        else hand.GetComponent<PlayerHand>().currentWeapon.SendMessage("PrimaryFire", false);
    }

    internal void Aim(bool aiming)
    {
        this.aiming = aiming;

        if (aiming)
        {
            // Sniper aim start
            if (hand.GetComponent<PlayerHand>().currentWeapon.GetComponent<Sniper>() != null)
            {
                playerUI.ToggleSniperScope(true);
            }

            // Machinegun aim start
            if (hand.GetComponent<PlayerHand>().currentWeapon.GetComponent<Machinegun>() != null)
            {
                playerUI.ToggleCrosshair(true, playerUI.crossMachinegun);
                hand.GetComponent<PlayerHand>().currentWeapon.GetComponent<Animator>().SetBool("ironSight", true);
            }
        }
        else
        {
            // Sniper aim end
            if (hand.GetComponent<PlayerHand>().currentWeapon.GetComponent<Sniper>() != null)
            {
                playerUI.ToggleSniperScope(false);
                rotationSpeed = originalRotationSpeed;
            }

            // Machinegun aim start
            if (hand.GetComponent<PlayerHand>().currentWeapon.GetComponent<Machinegun>() != null)
            {
                playerUI.ToggleCrosshair(false, playerUI.crossMachinegun);
                hand.GetComponent<PlayerHand>().currentWeapon.GetComponent<Animator>().SetBool("ironSight", false);
            }
        }
    }

    internal void MouseLook(float x, float y)
    {
        playerRotationY = x;
        playerRotationX = y;
        // Accumulate for physics (FixedUpdate) and camera (LateUpdate)
        accumulatedMouseX += x * Time.deltaTime;
        accumulatedMouseY += y * Time.deltaTime;
    }

    internal void Move(float x, float z)
    {
        if (distanceToGround < groundLimit)
            movement = Vector3.ClampMagnitude(new Vector3(z, 0f, x), 1f);
        else
            // Air control: allow direction changes in air but slightly damped
            movement = Vector3.Lerp(movement, Vector3.ClampMagnitude(new Vector3(z, 0f, x), 1f), 6f * Time.deltaTime);
    }

    // Spring grenade
    internal void UseItem(int itemId)
    {
        if (itemId == 0)
        {
            // Check if quickitems available
            if (playerManager.primaryItems > 0 && playerManager.canUsePrimaryItem && quickitemsActive)
            {
                playerManager.UseItem(itemId);

                Rigidbody clone = Instantiate(primaryQuickItem, playerCamera.position + playerCamera.TransformDirection(Vector3.forward), playerCamera.rotation);
                clone.linearVelocity = playerCamera.TransformDirection(Vector3.forward * (17f + Mathf.Abs(rigidBody.linearVelocity.y)));
            }
        }
        else if (itemId == 1)
        {
            // Check if quickitems available
            if (playerManager.secondaryItems > 0 && playerManager.canUseSecondaryItem && secondaryQuickitemsActive)
            {
                playerManager.UseItem(itemId);

                Rigidbody clone = Instantiate(secondaryQuickItem, playerCamera.position + playerCamera.TransformDirection(Vector3.forward), playerCamera.rotation);
                clone.linearVelocity = playerCamera.TransformDirection(Vector3.forward * (17f + Mathf.Abs(rigidBody.linearVelocity.y)));
            }
        }
         
    }

    // Temporary horizontal push from jump pads (decays over time)
    private Vector3 padPush = Vector3.zero;

    /// <summary>
    /// Called by ForceField/JumpPad. Accepts FINAL velocity vector (already scaled).
    /// ForceField applies * 0.035 before calling. JumpPad applies its own multiplier.
    /// Replaces velocity entirely (original ForceType.Exclusive behavior).
    /// </summary>
    public void PowerUp(Vector3 finalVelocity)
    {
        if (canUsePowerUp)
        {
            // Cap vertical — maxPadVerticalVel prevents extreme ceiling launches
            verticalVelocity = Mathf.Clamp(finalVelocity.y, -maxPadVerticalVel, maxPadVerticalVel);
            // Horizontal: let physics engine handle collision (no artificial cap needed)
            padPush = new Vector3(finalVelocity.x, 0f, finalVelocity.z);
            canUsePowerUp = false;
        }
    }

    public void SetCanUsePowerUp(bool state)
    {
        canUsePowerUp = state;
    }

    /// <summary>
    /// Apply explosion/rocket force to player. Additive — adds to current velocity
    /// instead of replacing it (unlike PowerUp which is Exclusive).
    /// Use this for rocket jumps, splash damage knockback, spring grenades, etc.
    /// Original: ForceType.Additive → _currentVelocity = Scale(vel, (1, 0.5, 1)) + force * 0.035
    /// </summary>
    public void ApplyExplosionForce(Vector3 force)
    {
        // Original additive: halves existing vertical velocity, adds scaled force
        verticalVelocity = verticalVelocity * 0.5f + force.y;
        // Always apply horizontal — ground friction in padPush decay handles sliding.
        // Rocket jumping at your feet while grounded needs the horizontal push to work.
        padPush += new Vector3(force.x, 0f, force.z);
    }

    void getDistanceToGround()
    {
        // Get closest ground around player
        float closest = -1111;
        foreach (Vector3 vector in groundRayChecks)
        {
            RaycastHit hit;
            
            Ray downRay = new Ray(transform.position + vector, -Vector3.up);
            Debug.DrawRay(transform.position+vector, -Vector3.up, Color.green); // draw rays

            if (Physics.Raycast(downRay, out hit))
            {  
                if (closest == -1111) { closest = hit.distance;}
                else if (hit.distance < closest) { closest = hit.distance; }

                //material
                if (vector == Vector3.zero && hit.transform.gameObject.GetComponent<Terrain>()) currentGroundMaterial = "grass";
                else if (vector == Vector3.zero && hit.transform.gameObject.GetComponent<Renderer>()) currentGroundMaterial = hit.transform.gameObject.GetComponent<Renderer>().material.name.Trim().ToLower();
            }     
        }
        distanceToGround = closest;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Do hand effect when hit floor
        if(collision.gameObject.tag == "Untagged" && distanceToGround < groundLimit && !aiming)
        {
            //sound
            playerAudio.PlayLanding(currentGroundMaterial);

            // sound grunt
            if (airTime>105)
            {
                airTime = 0;
                playerAudio.Play(playerAudio.landing);
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Ladder") climbing = true;

        // walk sound loop
        if(movement!=Vector3.zero&&!climbing&&!swim && distanceToGround < groundLimit) playerAudio.PlayWalk(currentGroundMaterial);
    }
    
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ladder") climbing = false;
    }

    IEnumerator LimitHandEffect()
    {
        // Do hand effect
        yield return new WaitForSeconds(0.06f);
        handPositionY = -0.359f;
        yield return new WaitForSeconds(0.4f);
        pendingHandEffect = false;
        canHandEffect = true;
    }

    internal void SetWeapon(int position)
    {
        hand.GetComponent<PlayerHand>().JumpToIndex(position);
    }

    internal void Crouch(bool crouch)
    {
        // Original UberStrike: can crouch while moving (duck walk at 23% speed)
        // Only prevent crouch when climbing, swimming, or in air
        if (climbing || swim) crouch = false;
        bool isGrounded = distanceToGround >= 0 && distanceToGround < groundLimit;
        if (!isGrounded && crouch) crouch = false;  // Can't start crouching mid-air
        if (crouch == crouching) return;
        cameraAnimator.SetBool("down", crouch);
        crouching = crouch;
        GameObject networkClient = GameObject.Find("/Network Client");
        if (networkClient != null) networkClient.SendMessage("LocalPlayerCrouch", crouching);
    }
}
