using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public int weaponIndex = 0;
    int weaponsLenght = 3;
    bool canSwitch = true;

    private string[] loadoutKeys = new string[]
    {
        "equipped_primary_weapon",
        "equipped_melee_weapon",
        "equipped_tertiary_weapon",
        "equipped_secondary_weapon"       
    };

    internal GameObject currentWeapon;
    private PlayerUI playerUI;

    private GameObject networkClient;
    private GlobalResources globalResources;

    // Init 
    void Start()
    {
        try {
            networkClient = GameObject.Find("/Network Client");
            globalResources = GameObject.Find("/Global Resources").GetComponent<GlobalResources>();
        }
        catch{Debug.LogWarning("Playerhand did not find network objects");}


        // Get weapons
        foreach (string key in loadoutKeys)
        {
            if (PlayerPrefs.HasKey(key))
            {
                if (PlayerPrefs.GetString(key) != null && PlayerPrefs.GetString(key) != "null")
                {
                    //GameObject weapon = AssetDatabase.LoadAssetAtPath(PlayerPrefs.GetString(key), (typeof(GameObject))) as GameObject;
                    GameObject weapon = Resources.Load(PlayerPrefs.GetString(key), typeof(GameObject)) as GameObject;
                    GameObject clone = Instantiate(weapon, transform.position, transform.rotation);
                    clone.transform.SetParent(transform);
                    
                }
            }
        }

        // set lenght
        weaponsLenght = transform.childCount-1;

        // if there is no weapon get defaults
        if (weaponsLenght < 0)
        {
            AddWeapon("SplatBat", "MachineGun", false);
            // set lenght
            //weaponsLenght = transform.childCount - 1;
        }

        playerUI = transform.root.gameObject.GetComponent<PlayerUI>();

        SetWeaponIndex(0);
    }

    void SetWeaponIndex(float scroll)
    {
        if (canSwitch)
        {
            StartCoroutine(SetCanSwitch());

            // Hide last weapon
            SetPreviousWeapon(weaponIndex);

            // Get direction
            if (scroll > 0)
            {
                weaponIndex++;
            }
            else if (scroll < 0)
            {
                weaponIndex--;
            }

            // Update index and show current weapon
            weaponIndex = Limit(weaponIndex);
            SetCurrentWeapon();

            // UI update
            playerUI.UpdateWeaponCarousel(GetWeaponAt(weaponIndex + 1).GetName(), GetWeaponAt(weaponIndex).GetName(), GetWeaponAt(weaponIndex - 1).GetName());
            playerUI.SetCrosshair(GetWeaponAt(weaponIndex).GetType().ToString());
        }
    }

    // Limit weapon switch rate
    IEnumerator SetCanSwitch()
    {
        canSwitch = false;
        yield return new WaitForSeconds(0.3f); // Limit
        canSwitch = true;
    }

    void SetCurrentWeapon()
    {
        currentWeapon = transform.GetChild(weaponIndex).gameObject;
        GetWeaponAt(weaponIndex).BringInHand(true);
        NetworkWeaponChanged();
    }

    void SetPreviousWeapon(int index)
    {
        GetWeaponAt(index).BringInHand(false);
    }

    private int Limit(int weaponIndex)
    {
        // Make loop
        if (weaponIndex < 0)
        {
            return weaponsLenght;
        }
        else if (weaponIndex > weaponsLenght)
        {
            return 0;
        }

        return weaponIndex;
    }

    public IWeapon GetWeaponAt(int position)
    {
        position = Limit(position);
        Weapon weaponClass = transform.GetChild(position).gameObject.GetComponent<Weapon>();

        if (weaponClass is IWeapon)
        {
            IWeapon weapon = (IWeapon)weaponClass;
            return weapon;
        }

        return null;
    }
    
    // This is almost identical to setWeaponIndex()
    public void JumpToIndex(int position)
    {
        if (canSwitch)
        {
            StartCoroutine(SetCanSwitch());

            // Hide last weapon
            SetPreviousWeapon(weaponIndex);


            // Update index and show current weapon
            weaponIndex = Limit(position-1);
            SetCurrentWeapon();

            // UI update
            playerUI.UpdateWeaponCarousel(GetWeaponAt(weaponIndex + 1).GetName(), GetWeaponAt(weaponIndex).GetName(), GetWeaponAt(weaponIndex - 1).GetName());
            playerUI.SetCrosshair(GetWeaponAt(weaponIndex).GetType().ToString());
        }
    }

    // Add weapons to hand
    internal void AddWeapon(string weaponName, string weaponName2, bool fromPickUp)
    {
        GameObject cloneq = Instantiate(Resources.Load(weaponName, typeof(GameObject)) as GameObject, transform.position, transform.rotation);
        cloneq.transform.SetParent(transform);

        if (weaponName2!=null)
        {
            cloneq = Instantiate(Resources.Load(weaponName2, typeof(GameObject)) as GameObject, transform.position, transform.rotation);
            cloneq.transform.SetParent(transform);
        }

        // set lenght
        weaponsLenght = transform.childCount - 1;
        


        if (fromPickUp)
        {
            JumpToIndex(-1);
            StartCoroutine(PickUpWeaponTimeLimit(weaponsLenght));
        }
    }

    // remove weapon after time
    private IEnumerator PickUpWeaponTimeLimit(int pos)
    {
        yield return new WaitForSeconds(20f); // wait
        canSwitch = true;
        while (pos > weaponsLenght) pos--; // if there is multiple weapons picked up
        if(weaponIndex == pos) JumpToIndex(1);

        //GetWeaponAt(pos).BringInHand(false);
        for(int i = 0; i < transform.childCount; i++) GetWeaponAt(i).BringInHand(false);



        weaponsLenght--;
        Destroy(transform.GetChild(pos).gameObject);

        SetCurrentWeapon();

        // UI update
        playerUI.UpdateWeaponCarousel(GetWeaponAt(weaponIndex + 1).GetName(), GetWeaponAt(weaponIndex).GetName(), GetWeaponAt(weaponIndex - 1).GetName());
        playerUI.SetCrosshair(GetWeaponAt(weaponIndex).GetType().ToString());
    }

    private void NetworkWeaponChanged()
    {
        if (networkClient != null&&networkClient.activeInHierarchy)
        {
            networkClient.SendMessage("LocalPlayerChangedToWeapon", globalResources.GetWeaponId(GetWeaponAt(weaponIndex).GetName()));
        }      
    }
}
