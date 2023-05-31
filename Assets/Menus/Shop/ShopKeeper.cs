using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ShopKeeper : MonoBehaviour
{
    // weapons
    public GameObject weaponsContainer;
    public GameObject[] weaponPrefabs;
    public GameObject weaponItemTemplate;

    // appereances
    public GameObject appereancesContainer;
    public GameObject[] appereancePrefabs;
    public GameObject appereanceItemTemplate;

    private GameObject prefabInDrag = null;
    internal Sprite spriteInDrag;
    public GameObject dragItem;

    public GameObject itemInfoPopup;
    private bool infoPopupOn = false;

    public GameObject[] loadoutSlots;
    public List<GameObject> availableSlots = new List<GameObject>();
    internal GameObject mouseOnSlot;

    private AudioSource audioSource;
    public AudioClip clickReady;
    public AudioClip equipSound;
    public AudioClip equipSoundQuickitem;
    public AudioClip equipSoundGear;

    public GameObject player;
    public Text totalArmor;

    public Sprite emptySlot;
    internal PlayerAvatar playerAvatar;

    void Start()
    {
        // get global resources
        GlobalResources r = GameObject.Find("/Global Resources").GetComponent<GlobalResources>();
        weaponPrefabs = r.weapons.ToArray();
        appereancePrefabs = r.appereances.ToArray();

        InitItemSlots();
        audioSource = transform.parent.gameObject.GetComponent<AudioSource>();
        playerAvatar = GameObject.Find("/Player Avatar").GetComponent<PlayerAvatar>();
        UpdateAvatar();
    }

    // show only appereances of one class
    public void FilterAppereances(string appereanceType)
    {
        clearAppereancesList();

        // reset scroll
        appereancesContainer.transform.parent.parent.GetChild(2).gameObject.GetComponent<Scrollbar>().value = 1;

        foreach (GameObject prefab in appereancePrefabs)
        {
            if (prefab.GetComponent<Appereance>().GetAppType() == appereanceType)
            {
                GameObject appItem = Instantiate(appereanceItemTemplate);
                appItem.transform.SetParent(appereancesContainer.transform, false);
                appItem.GetComponent<AppereanceShopItem>().Build(prefab, this);
            }
        }

        audioSource.PlayOneShot(clickReady);
    }

    // show only weapons of one class
    public void FilterWeapons(string weaponClass)
    {
        clearWeaponsList();

        // reset scroll
        weaponsContainer.transform.parent.parent.GetChild(2).gameObject.GetComponent<Scrollbar>().value = 1;

        foreach (GameObject prefab in weaponPrefabs)
        {
            if (prefab.GetComponent<Weapon>().GetType().ToString()==weaponClass)
            {
                GameObject weaponitem = Instantiate(weaponItemTemplate);
                weaponitem.transform.SetParent(weaponsContainer.transform, false);
                weaponitem.GetComponent<WeaponShopItem>().Build(prefab, this);
            }
        }

        audioSource.PlayOneShot(clickReady);
    }

    // show all weapons
    public void ShowAllWeapons()
    {
        clearWeaponsList();

        foreach (GameObject prefab in weaponPrefabs)
        {
            GameObject weaponitem = Instantiate(weaponItemTemplate);
            weaponitem.transform.SetParent(weaponsContainer.transform, false);
            weaponitem.GetComponent<WeaponShopItem>().Build(prefab, this);
        }
    }

    // show all weapons
    public void ShowAllAppereances()
    {
        clearAppereancesList();

        foreach (GameObject prefab in appereancePrefabs)
        {
            GameObject appItem = Instantiate(appereanceItemTemplate);
            appItem.transform.SetParent(appereancesContainer.transform, false);
            appItem.GetComponent<AppereanceShopItem>().Build(prefab, this);
        }
    }

    private void clearWeaponsList()
    {
        foreach (Transform child in weaponsContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    private void clearAppereancesList()
    {
        foreach (Transform child in appereancesContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    // Drag items
    public void Drag(GameObject prefab, Sprite sprite, bool slotToSlot)
    {
        // hide popup
        ItemInfoPopup(false, null);

        // show right tab for draggable item
        // for springs
        if (prefab.GetComponent<QuickItem>() != null)
        {
            transform.parent.gameObject.GetComponent<TopBarUI>().LoadoutTabs(2);
        }
        // for weapons
        else if (prefab.GetComponent<Weapon>() != null)
        {
            transform.parent.gameObject.GetComponent<TopBarUI>().LoadoutTabs(0);
        }
        // for clothes
        else if (prefab.GetComponent<Appereance>() != null)
        {
            transform.parent.gameObject.GetComponent<TopBarUI>().LoadoutTabs(1);
        }

        spriteInDrag = sprite;
        prefabInDrag = prefab;
        dragItem.SetActive(true);
        dragItem.GetComponent<Image>().sprite = sprite;

        // Check if type already equipped
        // Send type requests to slots
        string type = GetTypeFromPrefab(prefabInDrag);
        if (!slotToSlot&&TypeAlreadyEquipped(type)!=null)
        {
            availableSlots.Add(TypeAlreadyEquipped(type));
        }
        else
        {
            foreach (GameObject slot in loadoutSlots)
            {
                if (slot.GetComponent<LoadoutSlot>().TypeRequest(type))
                {
                    availableSlots.Add(slot);
                }
            }
        }
        
        DrawHighLights(true);
    }

    // Return slot gameObject if already equipped type
    private GameObject TypeAlreadyEquipped(string type)
    {
        foreach (GameObject slot in loadoutSlots)
        {
            if (slot.GetComponent<LoadoutSlot>().GetSavedType() == type)
            {
                return slot;
            }          
        }

        return null;
    }

    public void EndDrag()
    {
        if (mouseOnSlot && availableSlots.Contains(mouseOnSlot))
        {
            // Equip items
            if (prefabInDrag.GetComponent<QuickItem>() != null)
            {
                audioSource.PlayOneShot(equipSoundQuickitem);
            }
            // equip cloth
            else if (prefabInDrag.GetComponent<Appereance>()!= null)
            {
                audioSource.PlayOneShot(equipSoundGear);
            }
            // weapons
            else
            {
                audioSource.PlayOneShot(equipSound);
                //Show weapon and play animations
                TakeWeaponInHand(prefabInDrag);
            }
            mouseOnSlot.GetComponent<LoadoutSlot>().SetPrefab(prefabInDrag);         
        }

        dragItem.SetActive(false);
        prefabInDrag = null;
        
        // Clear highlights
        DrawHighLights(false);
        availableSlots.Clear();
      
    }

    void Update()
    {
        // dragItem follow cursor
        if (prefabInDrag)
        {
            dragItem.transform.position = Input.mousePosition;
        }

        else if (infoPopupOn)
        {
            itemInfoPopup.transform.position = Input.mousePosition;
        }
    }

    private void InitItemSlots()
    {
        foreach(GameObject slot in loadoutSlots)
        {
            slot.GetComponent<LoadoutSlot>().SetShopKeeper(this);
        }
    }

    private void DrawHighLights(bool visible)
    {
        foreach (GameObject slot in availableSlots)
        { 
            slot.transform.GetChild(2).GetChild(0).gameObject.SetActive(visible);
        }
    }

    public string GetNameFromPrefab(GameObject prefab)
    {
        // in case if weapon
        if (prefab.GetComponent<Weapon>() != null)
        {
            return prefab.GetComponent<Weapon>().weaponName;
        }

        // in case if spring
        else if (prefab.GetComponent<QuickItem>() != null)
        {
            return prefab.GetComponent<QuickItem>().itemName;
        }
        // in case of cloth
        else if (prefab.GetComponent<Appereance>() != null)
        {
            return prefab.GetComponent<Appereance>().appereanceName;
        }
        return "";
    }

    public string GetTypeFromPrefab(GameObject prefab)
    {
        // in case of weapon
        if (prefab.GetComponent<Weapon>() != null)
        {
            return prefab.GetComponent<Weapon>().GetType().ToString();
        }
        // in case of spring
        else if (prefab.GetComponent<QuickItem>() != null)
        {
            return prefab.GetComponent<QuickItem>().type;
        }
        // in case of cloth
        else if (prefab.GetComponent<Appereance>() != null)
        {
            return prefab.GetComponent<Appereance>().GetAppType();
        }

        return "";
    }

    public Sprite GetSpriteFromPrefab(GameObject prefab)
    {
        // in case if weapon
        if (prefab.GetComponent<Weapon>() != null)
        {
            return prefab.GetComponent<Weapon>().thumbnail;
        }

        // in case if spring
        else if (prefab.GetComponent<QuickItem>() != null)
        {
            return prefab.GetComponent<QuickItem>().thumbnail;
        }
        // in case of cloth
        else if (prefab.GetComponent<Appereance>() != null)
        {
            return prefab.GetComponent<Appereance>().thumbnail;
        }

        return null;
    }

    // Used by "new" -tab ads
    // Direct to category with parameters
    public void WeaponAdShorcut(string weaponCategory)
    {
        // go to weapons category
        transform.parent.gameObject.SendMessage("MainGategories",1);

        // filter to right category
        FilterWeapons(weaponCategory);
    }

    internal void ItemInfoPopup(bool show, ShopItem item)
    {
        // Show
        if (!prefabInDrag && show)
        {
            // thumb
            itemInfoPopup.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = item.GetThumb();

            // name
            itemInfoPopup.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = item.GetName();

            // for weapons
            if (item.prefab.GetComponent<Weapon>()!=null)
            {
                Weapon w = item.prefab.GetComponent<Weapon>();

                // tier and class
                itemInfoPopup.transform.GetChild(1).gameObject.GetComponent<Text>().text = "Tier " + w.tier + " " + w.GetType().ToString();

                //damage
                itemInfoPopup.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Slider>().value = w.damage;

                //rate of fire
                itemInfoPopup.transform.GetChild(2).GetChild(1).gameObject.GetComponent<Slider>().value = w.fireRate;

                //ammo
                GameObject ammoSlider = itemInfoPopup.transform.GetChild(2).GetChild(2).gameObject;
                if (w.unlimitedAmmo) ammoSlider.SetActive(false);
                else
                {
                    ammoSlider.SetActive(true);
                    ammoSlider.GetComponent<Slider>().value = w.baseAmmo;

                }

                // critical hit bonus
                string[] weaponsWithBonus = new string[] {"Sniper","Machinegun","Handgun","Shotgun"};
                Text criticalHitText = itemInfoPopup.transform.GetChild(2).GetChild(3).gameObject.GetComponent<Text>();
                criticalHitText.text = "";
                foreach (string s in weaponsWithBonus)
                {
                    if (w.GetType().ToString() == s)
                    {
                        criticalHitText.text = "+" + w.criticalDmgBonus + " Critical hit bonus";
                    }
                }

            }

            itemInfoPopup.SetActive(true);
        }
        else
        // Hide
        itemInfoPopup.SetActive(false);
        infoPopupOn = show;
    }

    internal void TakeWeaponInHand(GameObject prefab)
    {
        if (!playerAvatar.gameObject.activeInHierarchy) return;

        Transform weaponHand = playerAvatar.rightHandJoint;

        //empty hand
        foreach (Transform child in weaponHand)
        {
            GameObject.Destroy(child.gameObject);
        }

        // parent for weapon to fix rotation
        GameObject parentT = new GameObject();
        parentT.transform.parent = weaponHand;
        parentT.transform.localRotation = Quaternion.Euler(0f, 90f, 90f);
        parentT.transform.localPosition = Vector3.zero;

        //instantiate weapon
        GameObject cloneWeapon = Instantiate(prefab, parentT.transform.position, parentT.transform.rotation);
        cloneWeapon.transform.parent = parentT.transform;


        playerAvatar.EquipWeapon(cloneWeapon);
        Destroy(cloneWeapon.GetComponent<Animator>());
    }

    internal void UpdateAvatar()
    {
        playerAvatar.LoadAppereances();
        if (PlayerPrefs.HasKey("gained_armor")) totalArmor.text = "Total armor points: "+PlayerPrefs.GetFloat("gained_armor");
    }


    public void SpawnToShootingRange()
    {
        // if menus
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {           
            if (GameObject.Find("/MenuSystem"))
            {
                playerAvatar.gameObject.SetActive(false);
                GameObject clonePlayer = Instantiate(player);
                clonePlayer.name = "Player";
                GameObject.Find("/Environment").GetComponent<AudioSource>().volume = 0f;
                GameObject.Find("/MenuSystem").SetActive(false);
                
            }
            else return;

        }

    }
}
