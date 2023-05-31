using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    private float deaths = 0f;
    private float health = 100f;
    private float armor = 0f;
    private const float baseHealth = 100f;
    private bool decreasing = false;
    private bool inTheMenus;

    internal float primaryItems = 0f;
    internal bool canUsePrimaryItem = true;

    internal float secondaryItems = 0f;
    internal bool canUseSecondaryItem = true;

    private Hashtable collectedAmmo = new Hashtable();

    internal bool gamePaused;

    PlayerUI playerUI;
    PlayerAudio playerAudio;
    PlayerHand playerHand;
    internal Client client;
    private GameObject respawnUI;
    public GameObject deathViewPrefab;

    private int killCombo = 0;
    Coroutine clearCombos;

    void Start()
    {
        inTheMenus = SceneManager.GetActiveScene().name == "MainMenu";
        if(!inTheMenus)respawnUI = GameObject.Find("/MenuSystem/Respawn UI").transform.GetChild(0).gameObject;

        playerUI = GetComponent<PlayerUI>();
        playerAudio = GetComponent<PlayerAudio>();
        if ((Resources.Load(PlayerPrefs.GetString("equipped_primary_quickitem"), typeof(GameObject)) as GameObject) != null)
        {
            float ff = (Resources.Load(PlayerPrefs.GetString("equipped_primary_quickitem"), typeof(GameObject)) as GameObject).GetComponent<QuickItem>().stockCount;
            playerUI.UpdateItemCount(0, ff, false);
            primaryItems = ff;
        }
        
        if ((Resources.Load(PlayerPrefs.GetString("equipped_secondary_quickitem"), typeof(GameObject)) as GameObject) != null)
        {
            float f = (Resources.Load(PlayerPrefs.GetString("equipped_secondary_quickitem"), typeof(GameObject)) as GameObject).GetComponent<QuickItem>().stockCount;
            playerUI.UpdateItemCount(1, f, false);
            secondaryItems = f;
        }
        
        playerHand = playerUI.playerHand;

        SetDefaultValues();

        // Skybox
        if(GameObject.Find("/Environment").GetComponent<Environment>().skybox!=null)RenderSettings.skybox = GameObject.Find("/Environment").GetComponent<Environment>().skybox;

        if (GameObject.Find("/Network Client"))
        {
            client = GameObject.Find("/Network Client").GetComponent<Client>();
            client.AssignLocalPlayer(transform);
        }

    }

    void SetDefaultValues()
    {
        health = 100;
        armor = 0f;
        if (PlayerPrefs.HasKey("gained_armor")) UpdateArmor( PlayerPrefs.GetFloat("gained_armor") );

        playerUI.UpdateHealthCount(health);
        playerUI.UpdateArmorCount(armor);
    }

    public void GotKill(int killedID, int criticalCode)
    {
        playerUI.ShowKillName("You killed "+ client.GetLatestDatas(killedID).name);

        killCombo++;
        if (killCombo > 6) killCombo = 6;
        if(clearCombos!=null) StopCoroutine(clearCombos);
        clearCombos=StartCoroutine(ClearCombos());

        playerAudio.Badges(killCombo);
        if (criticalCode == 3) playerAudio.MeleeBadge();
        else if (criticalCode == 2) playerAudio.SpecialBadge(false);
        else if (criticalCode == 1) playerAudio.SpecialBadge(true);

        //custom narrator
        CustomNarrator("GotKill");
    }

    //custom narrator
    void CustomNarrator(string e)
    {
        GameObject g = GameObject.Find("/Custom Narrator");
        if (g) g.SendMessage(e);
    }

    private IEnumerator ClearCombos()
    {
        yield return new WaitForSeconds(3.5f);
        playerUI.ShowKillName("");
        yield return new WaitForSeconds(11.5f);
        killCombo = 0;
    }

    // Runs when health is zero
    internal void Die(int dealerPlayerID, int criticalCode)
    {
        if (inTheMenus) { PauseGame(); return; }
        if (!client.isAlive) return;
        client.isAlive = false;
        deaths++;

        /* criticalCode
         * 1 = headshot
         * 2 = nutshot
         * 3 = smackdown
         */

        //cause messages
        client.LocalPlayerDied(dealerPlayerID, criticalCode); // to others
        string causeMsg = ""; // self
        if (dealerPlayerID < 0) causeMsg = "Congratulations, you killed yourself.";
        else if (criticalCode == 0) causeMsg = "Killed by " + client.GetLatestDatas(dealerPlayerID).name;
        else if (criticalCode == 1) causeMsg = "Headshot from " + client.GetLatestDatas(dealerPlayerID).name;
        else if (criticalCode == 2) causeMsg = "Nutshot from " + client.GetLatestDatas(dealerPlayerID).name;
        else if (criticalCode == 3) causeMsg = "Smackdown from " + client.GetLatestDatas(dealerPlayerID).name;

        playerUI.ShowKillName("");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        CustomNarrator("Die");

        // hide pause 
        GameObject.Find("/MenuSystem/Pause UI").transform.GetChild(0).gameObject.SetActive(false);

        client.AssignLocalPlayer(null);
        respawnUI.SetActive(true);
        respawnUI.SendMessage("SetDeathCause", causeMsg);

        GameObject death = Instantiate(deathViewPrefab, transform.position, transform.rotation);
        death.SendMessage("SetWeapon", client.localWeaponId);     
        if(dealerPlayerID>0) death.SendMessage("SetKiller", client.GetLatestDatas(dealerPlayerID).playerObject);
        death.SendMessage("StartVelocity",GetComponent<Rigidbody>().velocity);


        Destroy(gameObject);
    }

    // Use negative value for damage, positive for pickups
    void UpdateHealth(float difference)
    {
        // Sounds when 100 is crossed
        if (health >= 100 && health + difference < 100)
        {
            playerAudio.Play(playerAudio.healthDecrease);
        }
        if(health <= 100 && health + difference > 100)
        {
            playerAudio.Play(playerAudio.healthEnergize);
        }

        // Do update
        health += difference;

        // Limit
        if(health > 200)
        {
            health = 200;
        }
        else if (health < 0)
        {
            health = 0;
        }

        if(health > baseHealth && !decreasing)
        {
            decreasing = true;
            InvokeRepeating("DecreaseValuesOverLimits", 2.0f, 2.0f);
        }

        playerUI.UpdateHealthCount(health);
        playerUI.ToggleHealthCritical(health < 40);            
    }

    // Use negative value for damage, positive for pickups
    void UpdateArmor(float difference)
    {
        armor += difference;

        // Limit
        if (armor > 120)
        {
            armor = 120;
        }

        playerUI.UpdateArmorCount(armor);
    }

    public void TakeDamage(float amount, Vector3 position, int criticalCode, int dealerPlayerID)
    {
        playerUI.ShowDamagePointer(position, amount);

        // All damage is applied to armor first
        // After that whats left goes to health        
        armor -= amount;
        float passToHealth = 0f;
        if (armor < 0)
        {
            passToHealth = armor;
            armor = 0;    
        }
        UpdateArmor(0);
        UpdateHealth(passToHealth);

        // Sounds
        if (armor>0)
        {
            playerAudio.Play(playerAudio.selfDmgArmor);
        }
        else if (health<50)
        {
            playerAudio.Play(playerAudio.selfDmgLowHp);
        }
        else
        {
            playerAudio.Play(playerAudio.selfDmgNoArmor);
        }

        CustomNarrator("TakeDamage");

        // Die
        if (health <= 0)
        {
            Die(dealerPlayerID, criticalCode);
        }
    }

    // When armor or health is over 100,
    // the values are decreased every 2 seconds
    void DecreaseValuesOverLimits()
    {
        if (health > baseHealth)
        {
            UpdateHealth(-1);
        }
        else
        {
            decreasing = false;
            CancelInvoke("DecreaseValuesOverLimits");
        }
    }

    internal void UseItem(int itemId)
    {
        if (itemId == 0)
        {
            primaryItems--;
            playerUI.UpdateItemCount(itemId, primaryItems, true);
        }
        else if (itemId == 1)
        {
            secondaryItems--;
            playerUI.UpdateItemCount(itemId, secondaryItems, true);
        }

        StartCoroutine(ItemRefresh(itemId));
    }

    IEnumerator ItemRefresh(int itemId)
    {
        if (itemId == 0)
        {
            canUsePrimaryItem = false;
            yield return new WaitForSeconds(2f);
            playerUI.UpdateItemCount(itemId, primaryItems, false);
            canUsePrimaryItem = true;
        }
        else if (itemId == 1)
        {
            canUseSecondaryItem = false;
            yield return new WaitForSeconds(2f);
            playerUI.UpdateItemCount(itemId,secondaryItems, false);
            canUseSecondaryItem = true;
        }
    }

    void PickupAmmo(object[] data)
    {
        float amount = (float)data[0];
        string type = (string)data[1];
        string target = (string)data[2];

        if (collectedAmmo.Contains(target))
        {
            amount += (float)collectedAmmo[target];
        }
        collectedAmmo[target] = amount;

        // Effects
        playerUI.ShowPickupName(type);
        playerAudio.Play(playerAudio.ammoPickup);

        // Update current weapons ammo
        // Other weapons load their ammo when become active
        IWeapon w = playerHand.GetWeaponAt(playerHand.weaponIndex);
        w.SetAmmo(GetComponent<PlayerManager>().GetCollectedAmmoFor(w.GetType().ToString()));
    }

    public float GetCollectedAmmoFor(string weaponType)
    {
        if (collectedAmmo.Contains(weaponType))
        {
            float ammo = (float)collectedAmmo[weaponType];
            collectedAmmo[weaponType] = 0f;
            return ammo;
        }
        return 0;
    }

    internal void PauseGame()
    {
        GetComponent<PlayerInput>().enabled = false; // Stop inputs
        GetComponent<PlayerMotor>().Move(0f, 0f); // Stop player
        GetComponent<PlayerMotor>().MouseLook(0f, 0f);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;


        // If in shootingRange
        if (inTheMenus)
        {
            GameObject[] rootObjs = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach(GameObject g in rootObjs)
            {
                if (g.name == "Player Avatar" || g.name == "MenuSystem") g.SetActive(true);
                if(g.name == "Environment") g.GetComponent<AudioSource>().volume = 1f;
            }

            Destroy(gameObject);

            //shop tab
            GameObject.Find("/MenuSystem/Top Bar UI").SendMessage("NavigationBar", 3);
            GameObject.Find("/MenuSystem/Top Bar UI").SendMessage("NavigationBar", 2);
        }
        // Other maps
        else
        {
            playerUI.TogglePauseScreen(true);
            gamePaused = true;

            GameObject g = GameObject.Find("/MenuSystem").transform.GetChild(0).gameObject;          
            g.SetActive(true); 

        }

        if (GameObject.Find("/Player"))
            GameObject.Find("/Player").GetComponent<PlayerMotor>().movement = Vector3.zero;
    }

    // call after game is paused
    public void ContinueGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerUI.TogglePauseScreen(false);
        gamePaused = false;
        GetComponent<PlayerInput>().enabled = true;
        GetComponent<PlayerMotor>().UpdateOptions();
    }

    // Pass weapon pickup to hand
    public void WeaponPickup(string w)
    {
        playerHand = playerUI.playerHand;
        playerHand.AddWeapon(w, null, true);
        playerAudio.Play(playerAudio.weaponPickup);
    }

    void KillYourself()
    {
        Die(-1,0);
    }
}
