using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private Rigidbody rigidBody;
    public PhysicMaterial pMaterial;
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

    public ParticleSystem distortion;

    private float moveSpeed = 0f;
    private bool sideways = false;

    public float groundMoveSpeed = 6f;
    internal float rotationSpeed = 75f; //45
    internal float originalRotationSpeed = 75f;
    public float jumpForce = 33f;
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
    private bool crouching = false;
    private Animator cameraAnimator;

    private float groundCheckRadius = 0.48f;
    private Vector3[] groundRayChecks = new Vector3[9];

    // Runs before first frame
    void Start()
    {
        capsule = GetComponent<Collider>();

        // Init array
        groundRayChecks[0] = new Vector3(groundCheckRadius,0f,0f);
        groundRayChecks[1] = new Vector3(0f, 0f, groundCheckRadius);
        groundRayChecks[2] = new Vector3(-groundCheckRadius, 0f, 0f);
        groundRayChecks[3] = new Vector3(0f, 0f, -groundCheckRadius);
        groundRayChecks[4] = new Vector3(-groundCheckRadius / 2, 0f, groundCheckRadius / 2);
        groundRayChecks[5] = new Vector3(groundCheckRadius / 2, 0f, -groundCheckRadius / 2);
        groundRayChecks[6] = new Vector3(-groundCheckRadius / 2, 0f, -groundCheckRadius / 2);
        groundRayChecks[7] = new Vector3(groundCheckRadius / 2, 0f, groundCheckRadius / 2);
        groundRayChecks[7] = new Vector3(0f, 0f, 0f);

        // Init
        playerAudio = GetComponent<PlayerAudio>();
        cameraAnimator = playerCamera.gameObject.GetComponent<Animator>();
        playerManager = GetComponent<PlayerManager>();
        playerUI = GetComponent<PlayerUI>();
        rigidBody = GetComponent<Rigidbody>();
        playerCamera = transform.Find("Player Camera");
        hand = playerCamera.GetChild(0).gameObject;
        handCamera = playerCamera.Find("Camera Mask").gameObject.GetComponent<Camera>();

        Vector3 rot = playerCamera.localRotation.eulerAngles;
        lastRotation = rot.x;
        handAnimator = handCamera.GetComponent<Animator>();

        // Water
        mapHasWater = GameObject.Find("/Environment").GetComponent<Environment>().mapHasWater;
        rbDrag = rigidBody.drag;

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
        // Motion blur
        if (PlayerPrefs.HasKey("motion_blur"))
            if (PlayerPrefs.GetInt("motion_blur") == -1)
                playerCamera.gameObject.GetComponent<Kino.Motion>().enabled = false;
        
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
            rigidBody.useGravity = !swim;
            if (swim) rigidBody.drag = 1f;
            else rigidBody.drag = rbDrag;

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
        //physics material
        if (distanceToGround < groundLimit && !GetTransformDirectionCollision()) capsule.material = null;
        else 
        if(climbing) capsule.material = null;
        else capsule.material = pMaterial;

        // Player movement
        SetMoveSpeed();
        if (rigidBody.velocity.magnitude>0.3f|| movement.magnitude > 0.3f) Crouch(false);

        // If ladder collision
        if (climbing && movement!=Vector3.zero) rigidBody.MovePosition(rigidBody.position + transform.TransformDirection(Vector3.up) * moveSpeed*1.5f * Time.deltaTime);

        // If swim
        if (swim && GetComponent<PlayerInput>().isActiveAndEnabled&&distanceToGround>groundLimit&&!Input.GetKey(KeyCode.Space)) rigidBody.AddForce(-Vector3.up, ForceMode.Acceleration);
        else if(Input.GetKey(KeyCode.Space)) rigidBody.AddForce(Vector3.up, ForceMode.Acceleration);

        Vector3 useTheForce = movement;          
        // Jump speed gain
        if (sideways && distanceToGround > groundLimit 
            && !swim && !climbing && distanceToGround < groundLimit + 2
            && Mathf.Abs(rigidBody.velocity.z) < 6.6f && Mathf.Abs(rigidBody.velocity.x) < 6.6f)
        
            if(sideways) rigidBody.AddForce(transform.TransformDirection(useTheForce) * moveSpeed * 1.8f,ForceMode.Acceleration);
            //else rigidBody.AddForce(transform.TransformDirection(useTheForce) * moveSpeed * 5f);

        rigidBody.MovePosition(rigidBody.position + transform.TransformDirection(movement) * moveSpeed * Time.deltaTime);

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

            float rectW = 1f; //1.2f
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

        // Player rotation
        Quaternion deltaRotation = Quaternion.Euler(new Vector3(0f, playerRotationY, 0f) * rotationSpeed * Time.deltaTime);
        rigidBody.MoveRotation(rigidBody.rotation * deltaRotation);

        // Player camera rotation               
        float clampAngle = 89f;
        lastRotation += playerRotationX * rotationSpeed * Time.deltaTime;
        lastRotation = Mathf.Clamp(lastRotation, -clampAngle, clampAngle);
        playerCamera.localRotation = Quaternion.Euler(lastRotation, 0f, 0.0f);
    }

    private void SetMoveSpeed()
    {
        moveSpeed = groundMoveSpeed;
        if (movement.z != 0 && movement.x != 0) sideways = true;     
        else sideways = false;
        
        if (aiming)
        {
            // Slow down when aim
            if(hand.GetComponent<PlayerHand>().currentWeapon.GetComponent<Sniper>() != null
                || hand.GetComponent<PlayerHand>().currentWeapon.GetComponent<Machinegun>() != null)
            {
                moveSpeed = groundMoveSpeed / 2.5f;
            }           
        }

        if(swim) moveSpeed = groundMoveSpeed / 4f;
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

    internal void Jump()
    {
        Crouch(false);
        if (!canJump) return;
        else StartCoroutine(CanJumpAgain());

        if (distanceToGround < groundLimit+0.06 && distanceToGround > 0)
        {
            pendingHandEffect = true;
            rigidBody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    IEnumerator CanJumpAgain()
    {
        canJump = false;
        yield return new WaitForSeconds(0.3f);
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
    }

    internal void Move(float x, float z)
    {
        if (distanceToGround < groundLimit)
            movement = Vector3.ClampMagnitude(new Vector3(z, 0f, x), 0.8f);
        else
            movement = Vector3.Lerp(movement, Vector3.ClampMagnitude(new Vector3(z, 0f, x), 0.7f), 1.9f*Time.deltaTime);
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
                clone.velocity = playerCamera.TransformDirection(Vector3.forward * (17f + Mathf.Abs(rigidBody.velocity.y)));
            }
        }
        else if (itemId == 1)
        {
            // Check if quickitems available
            if (playerManager.secondaryItems > 0 && playerManager.canUseSecondaryItem && secondaryQuickitemsActive)
            {
                playerManager.UseItem(itemId);

                Rigidbody clone = Instantiate(secondaryQuickItem, playerCamera.position + playerCamera.TransformDirection(Vector3.forward), playerCamera.rotation);
                clone.velocity = playerCamera.TransformDirection(Vector3.forward * (17f + Mathf.Abs(rigidBody.velocity.y)));
            }
        }
         
    }

    public void PowerUp(Vector3 forceDirection)
    {
        if (canUsePowerUp)
        {
            rigidBody.AddForce(forceDirection * jumpForce, ForceMode.Impulse);
            canUsePowerUp = false;
        }
    }

    public void SetCanUsePowerUp(bool state)
    {
        canUsePowerUp = state;
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
        if (climbing || swim || rigidBody.velocity != Vector3.zero) crouch = false;
        if(crouch==crouching) return;
        cameraAnimator.SetBool("down",crouch);
        crouching = crouch;
        if (GameObject.Find("/Network Client")) GameObject.Find("/Network Client").SendMessage("LocalPlayerCrouch", crouching);
    }
}
