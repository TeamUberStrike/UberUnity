using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerUI : MonoBehaviour
{
    GameObject statsCanvas;
    public GameObject playerCanvas;
    public GameObject sniperScope;
    public GameObject weaponCamera;
    public GameObject crosshair;

    internal PlayerMotor playerMotor;

    public GameObject primaryCount;
    public GameObject primaryIcon;
    public GameObject primaryContainer;
    public GameObject primaryLoader;

    public GameObject secondaryCount;
    public GameObject secondaryIcon;
    public GameObject secondaryContainer;
    public GameObject secondaryLoader;

    public GameObject healthCount;
    public GameObject armorCount;
    public GameObject ammoCount;

    public GameObject weaponTop;
    public GameObject weaponMiddle;
    public GameObject weaponBottom;
    public Animator carouselAnimator;

    // Crosshairs
    private GameObject currentCrosshair;
    public GameObject crossSniper;
    public GameObject crossMelee;
    public GameObject crossShotgun;
    public GameObject crossCannon;
    public GameObject crossMachinegun;

    public GameObject healthCriticalMask;
    public GameObject pickupInfo;
    private float pickupInfoCombo = 0f;
    private string lastPickup = "";
    PlayerAudio playerAudio;
    internal PlayerHand playerHand;

    private float scopeSens = 60f;
    internal bool quickItemsActive = false;
    internal bool secondaryQuickitemsActive = false;
    internal GameObject pauseScreen;

    private Camera playerCamera;   
    internal float zoomMax = 59f; // Default FOV
    private float zoomMin = 8.2f; // Max zoom
    internal float currentZoom;
    private float zoomSensitivy = 29f;
    private float lastZoomDirection = -1f;

    private bool scopeIn = false;
    private bool statsIn = false;

    public Transform dmgText;
    public RectTransform damagePointer;

    public Text killName;

    // Init
    void Start()
    {
        playerMotor = GetComponent<PlayerMotor>();
        playerAudio = GetComponent<PlayerAudio>();
        playerCamera = transform.GetChild(0).GetComponent<Camera>();
        playerHand = transform.GetChild(0).Find("hand").gameObject.GetComponent<PlayerHand>();

        if (SceneManager.GetActiveScene().name == "MainMenu") return;
        pauseScreen = GameObject.Find("/MenuSystem/Pause UI").transform.GetChild(0).gameObject;
        statsCanvas = GameObject.Find("/Network Client/Lobby Stats UI");
    }

    public void ToggleStats(bool show)
    {
        if (show)
        {
            if (SceneManager.GetActiveScene().name == "MainMenu") return;
            statsCanvas.transform.GetChild(0).gameObject.SetActive(true);
            statsIn = true;

            if (!scopeIn)
            {
                playerCanvas.SetActive(false);
            }     
        }
        else
        {
            statsCanvas.transform.GetChild(0).gameObject.SetActive(false);
            playerCanvas.SetActive(true);
            statsIn = false;
        }
    }

    public void ToggleSniperScope(bool show)
    {
        // Dont show crosshair for snipers that say so
        if (playerHand.GetWeaponAt(playerHand.weaponIndex).GetUsesHardScope())
        {
            sniperScope.SetActive(show);
        }

        weaponCamera.SetActive(!show);

        if(!show)
        {
            playerCamera.fieldOfView = zoomMax;
            currentZoom = zoomMax;
            lastZoomDirection = -1;
            playerAudio.Play(playerAudio.scopeOut);
            scopeIn = false;
        }
        else
        {
            currentZoom = playerHand.GetWeaponAt(playerHand.weaponIndex).GetMaxZoom() + (playerHand.GetWeaponAt(playerHand.weaponIndex).GetMaxZoom() / 2);
            playerCamera.fieldOfView = currentZoom;

            playerAudio.Play(playerAudio.scopeIn);
            scopeIn = true;

            // sensitivy
            playerMotor.rotationSpeed = playerMotor.originalRotationSpeed * (currentZoom / scopeSens);
        }

        if (statsIn)
        {
            // Make stats always on top
            playerCanvas.SetActive(true);
            statsCanvas.SetActive(false);
            statsCanvas.SetActive(true);
        }
    }

    internal void SetIcon(int itemId, GameObject item)
    {
        ((itemId == 0) ? primaryIcon : secondaryIcon).GetComponent<Image>().sprite = item.GetComponent<QuickItem>().icon;      
    }

    public void Zoom(float input)
    {
        currentZoom += (input * -1) * zoomSensitivy;
        zoomMin = playerHand.GetWeaponAt(playerHand.weaponIndex).GetMaxZoom();
        currentZoom = Mathf.Clamp(currentZoom, zoomMin, zoomMax);
        playerCamera.fieldOfView = currentZoom;

        // sensitivy
        playerMotor.rotationSpeed = playerMotor.originalRotationSpeed * (currentZoom/scopeSens);

        // Sound
        if (input > 0){input = 1;}else{input = -1;}
        if (input != lastZoomDirection) {
            if (input > 0)
            {
                playerAudio.Play(playerAudio.zoomIn);
            }
            else
            {
                playerAudio.Play(playerAudio.zoomOut);
            }

            lastZoomDirection = input;
        }
    }

    // Hide and show crosshairs with this method
    internal void ToggleCrosshair(bool hide, GameObject crossHairObj)
    {
        float opacity = 0f;
        if (!hide) { opacity = 0.4f; }

        Image[] images = crossHairObj.GetComponentsInChildren<Image>();
        foreach (Image img in images)
        {
            Color c = img.color;
            c.a = opacity;
            img.color = c;
        }
    }

    public void SetCrosshair(string weaponClass)
    {
        currentCrosshair = crosshair.transform.Find(weaponClass).gameObject;

        // Hide all
        Image[] images = crosshair.GetComponentsInChildren<Image>();
        foreach(Image img in images){
            Color c = img.color;
            c.a = 0f;
            img.color = c;
        }

        // Dont show crosshair for snipers that say so
        if(weaponClass == "Sniper")           
        {
            playerHand = transform.GetChild(0).Find("hand").gameObject.GetComponent<PlayerHand>(); // Get hand if missing
            if (playerHand.GetWeaponAt(playerHand.weaponIndex).GetUsesHardScope()) return;     
        }

        // Show what we want     
        images = currentCrosshair.transform.GetComponentsInChildren<Image>();
        foreach (Image img in images)
        {
            Color c = img.color;
            c.a = 0.4f;
            img.color = c;
        }
    }

    internal void UpdateItemCount(int itemId, float value, bool decrease)
    {
        if (itemId == 0)
        {
            primaryCount.GetComponent<Text>().text = "" + value;

            // Set color
            Image img = primaryContainer.GetComponent<Image>();
            Image imgIcon = primaryIcon.GetComponent<Image>();
            Color c = img.color;

            if (decrease)
            {
                c.a = 0.3f;
                primaryLoader.GetComponent<Animation>().Play();
            }
            else if (value > 0)
            {
                c.a = 1f;
                playerAudio = GetComponent<PlayerAudio>();
                try { playerAudio.Play(playerAudio.springRefresh); }
                catch { }
            }

            img.color = c;
            imgIcon.color = c;
        }
        else if (itemId == 1)
        {
            secondaryCount.GetComponent<Text>().text = "" + value;

            // Set color
            Image img = secondaryContainer.GetComponent<Image>();
            Image imgIcon = secondaryIcon.GetComponent<Image>();
            Color c = img.color;

            if (decrease)
            {
                c.a = 0.3f;
                secondaryLoader.GetComponent<Animation>().Play();
            }
            else if (value > 0)
            {
                c.a = 1f;
                playerAudio = GetComponent<PlayerAudio>();
                try { playerAudio.Play(playerAudio.springRefresh); }
                catch { }
            }

            img.color = c;
            imgIcon.color = c;
        }
    }

    internal void UpdateHealthCount(float value)
    {
        healthCount.GetComponent<Text>().text = "" + value;
        healthCount.GetComponent<Animation>().Play();
    }

    internal void UpdateArmorCount(float value)
    {
        armorCount.GetComponent<Text>().text = "" + value;
        armorCount.GetComponent<Animation>().Play();
    }

    internal void UpdateWeaponCarousel(string top, string middle, string bottom)
    {
        weaponTop.GetComponent<Text>().text = top;
        weaponMiddle.GetComponent<Text>().text = middle;
        weaponBottom.GetComponent<Text>().text = bottom;
        carouselAnimator.Play("default");
    }

    public void ShowPickupName(string name)
    {
        pickupInfo.GetComponent<Animation>().Play();
        if (lastPickup == name)
        {
            pickupInfoCombo++;
            
            if(pickupInfoCombo > 0f)
            {
                pickupInfo.GetComponent<Text>().text = name + " x" + (pickupInfoCombo+1);
            }
            else
            {
                pickupInfo.GetComponent<Text>().text = name;
            }
        }       
        else
        {
            pickupInfoCombo = 0;
            pickupInfo.GetComponent<Text>().text = name;
        }

        lastPickup = name;
    }

    void CrosshairShoot()
    {
        // Animate
        if (currentCrosshair.GetComponent<Animation>() != null)
        {
            Animation a = currentCrosshair.GetComponent<Animation>();
            a.Stop();
            a.Play();
        }
    }

    void UpdateAmmoCounter(object[] args)
    {
        if (!(bool)args[1])
        {
            ammoCount.GetComponent<Text>().text = "" + (float)args[0];
        }
        else
        {
            ammoCount.GetComponent<Text>().text = "∞";
        }     
    }

    internal void ToggleHealthCritical(bool show)
    {
        healthCriticalMask.SetActive(show);
    }

    public void ShowDealedDamage(float amount, Vector3 position)
    {
        Transform clone = Instantiate(dmgText, position-transform.right/2, Quaternion.identity);
        clone.gameObject.GetComponent<EnemyDamageText>().SetText(amount.ToString());
        Destroy(clone.gameObject, 2f);
    }

    internal void ShowDamagePointer(Vector3 position, float amount)
    {
        // Spawn new damage pointer
        RectTransform cloned = Instantiate(damagePointer);
        cloned.SetParent(playerCanvas.transform, false);
        Destroy(cloned.gameObject, 1f);

        // Damage direction
        Vector3 targetDir = position - transform.position;
        float angle = Vector3.Angle(targetDir, transform.forward);

        if (Vector3.Angle(transform.right, targetDir) > 90f)
            cloned.rotation = Quaternion.Euler(0, 0, angle);
        else
            cloned.rotation = Quaternion.Euler(0, 0, -angle);

        // Scale by damage amount
        cloned.sizeDelta = new Vector2(Mathf.Clamp(amount*2,0f,82f), 73.5f);
    }

    // Hide/Show HUD
    internal void ToggleHUD()
    {
        playerCanvas.GetComponent<Canvas>().enabled = !playerCanvas.GetComponent<Canvas>().enabled;
    }

    // Hide/Show pause screen
    internal void TogglePauseScreen(bool visible)
    {
        pauseScreen.SetActive(visible);
    }

    // Show killed player name
    public void ShowKillName(string killed)
    {
        killName.text = killed;
    }
}

