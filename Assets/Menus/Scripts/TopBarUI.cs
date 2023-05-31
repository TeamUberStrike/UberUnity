using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TopBarUI : MonoBehaviour
{
    public GameObject optionsCanvas;

    public GameObject audioButton;
    public Sprite audioMuted;
    public Sprite audioNormal;

    public GameObject playMenu;
    public GameObject profileMenu;

    private int lastNavButton = 0;
    private int lastMainGategory = 0;
    private int lastLoadoutTab = 0;

    public GameObject[] mainGategories;
    public GameObject[] loadoutTabs;

    // Options
    public Toggle motionBlur;
    public Slider fieldOfView;
    public Dropdown quality;
    public Slider sensitivy;
    public Slider volume;

    // Sounds
    internal AudioSource audioSource;
    public AudioClip clickReady;
    public AudioClip clickUnReady;
    public AudioClip openPanel;
    public AudioClip leaveServer;
    public AudioClip joinServer;

    // username
    public GameObject topBarUsername;
    public GameObject usernamePopup;
    public Text usernameInput;
    public GameObject invalidNameMessage;

    private bool inTheMenus = true;
    public GameObject[] hideWhenNotMenus;
    public GameObject[] hideWhenMenus;
    public GameObject spawnActions;

    public Transform respawnCanvas;


    void Start()
    {
        // Skybox
        if(GameObject.Find("/Environment"))
        if(GameObject.Find("/Environment").GetComponent<Environment>().skybox!=null)RenderSettings.skybox = GameObject.Find("/Environment").GetComponent<Environment>().skybox;


        //audio
        audioSource = GetComponent<AudioSource>();

        //set default states
        inTheMenus = SceneManager.GetActiveScene().name == "MainMenu";
        if (!inTheMenus)
        {
            NavigationBar(2);
            HideMenuOnlyItems();
        }
        else HideGameOnlyItems();
        

        // Volume
        if (PlayerPrefs.GetInt("mute") == 1) audioButton.GetComponent<Image>().sprite = audioMuted;

        LoadOptions();
        UpdateVolume();

        // player name
        // ask for name if not exist
        if (PlayerPrefs.HasKey("username")) UpdateUsername();
        
        else
        {
            // set default settings
            NameChange();
            ApplyOptions(false);
        }

    }

    public void SubmitUsername()
    {
        string input = usernameInput.text;

        // validate lenght
        // if not ok
        if (input.Length < 2 || input.Length > 21)
        {
            audioSource.PlayOneShot(clickUnReady);
            invalidNameMessage.SetActive(true);
        }
        // if ok
        else
        {
            PlayerPrefs.SetString("username", input);
            usernamePopup.SetActive(false);
            audioSource.PlayOneShot(joinServer);
            UpdateUsername();

            
            Application.Quit(); //DEBUG////////////////////////////////////////////////////////////////////////////////////////////////////
        }   
    }

    public void ToggleOptions()
    {
        optionsCanvas.SetActive(!optionsCanvas.activeInHierarchy);
        if (optionsCanvas.activeInHierarchy) LoadOptions();
        audioSource.PlayOneShot(clickReady);
    }

    public void ToggleFullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
        audioSource.PlayOneShot(clickReady);
    }

    public void ToggleMute()
    {
        // Do switch
        // 1 mute
        // -1 not muted
        if (PlayerPrefs.HasKey("mute")) PlayerPrefs.SetInt("mute", -1*PlayerPrefs.GetInt("mute"));
        else PlayerPrefs.SetInt("mute",1);

        // Change icon and mute camera
        if (PlayerPrefs.GetInt("mute")==1) audioButton.GetComponent<Image>().sprite = audioMuted;       
        else audioButton.GetComponent<Image>().sprite = audioNormal;

        UpdateVolume();
    }

    public void ApplyOptions(bool toggle)
    {
        // Motion blur
        if (motionBlur.isOn) PlayerPrefs.SetInt("motion_blur", 1);
        else PlayerPrefs.SetInt("motion_blur", -1);

        // Quality
        PlayerPrefs.SetInt("quality", quality.value);
        QualitySettings.SetQualityLevel(quality.value, true);

        // Volume
        PlayerPrefs.SetFloat("volume",volume.value);
        UpdateVolume();

        // FOV
        PlayerPrefs.SetFloat("fov", fieldOfView.value);

        // Sensitivy
        PlayerPrefs.SetFloat("sensitivy", sensitivy.value);

        if (toggle)
        {
            ToggleOptions();
            audioSource.PlayOneShot(leaveServer);
        }      
    }

    private void LoadOptions()
    {
        // motion blur
        if (PlayerPrefs.HasKey("motion_blur"))
        motionBlur.isOn = PlayerPrefs.GetInt("motion_blur") > 0;

        // quality
        if (PlayerPrefs.HasKey("quality"))
            quality.value = PlayerPrefs.GetInt("quality");

        // volume
        if (PlayerPrefs.HasKey("volume"))
            volume.value = PlayerPrefs.GetFloat("volume");

        // FOV
        if (PlayerPrefs.HasKey("fov"))
            fieldOfView.value = PlayerPrefs.GetFloat("fov");

        // sensitivy
        if (PlayerPrefs.HasKey("sensitivy"))
            sensitivy.value = PlayerPrefs.GetFloat("sensitivy");
    }

    public void UpdateVolume()
    {
        PlayerPrefs.SetFloat("volume", volume.value);
        float v = 0.6f;
        if (PlayerPrefs.HasKey("mute"))
        {
            if (PlayerPrefs.GetInt("mute") == 1)
            {
                v = 0f;
            }
            else
            {
                v = PlayerPrefs.GetFloat("volume")/100f;
            }
        }

        AudioListener.volume = v;
    }

    public void NavigationBar(int index)
    {
        if (index != lastNavButton)
        {
            lastNavButton = index;
            audioSource.PlayOneShot(openPanel);

            switch (index)
            {
                case 0:
                    // HOME
                    profileMenu.SetActive(false);
                    playMenu.SetActive(false);
                    ShowSideBar(false);
                    break;
                case 1:
                    // PLAY
                    playMenu.SetActive(true);
                    ShowSideBar(false);
                    break;
                case 2:
                    // SHOP
                    profileMenu.SetActive(false);
                    playMenu.SetActive(false);
                    ShowSideBar(true);
                    break;
                case 3:
                    // PROFILE
                    profileMenu.SetActive(true);
                    playMenu.SetActive(false);
                    ShowSideBar(true);
                    break;
                case 4:
                    // OPTIONS
                    ToggleOptions();
                    break;
                case 5:
                    // QUIT
                    Application.Quit();
                    break;
                default:
                    break;
            }
        }
        else if(gameObject.activeInHierarchy) audioSource.PlayOneShot(clickReady);
    }

    public void ShowSideBar(bool status)
    {
        transform.parent.gameObject.GetComponent<Animator>().SetBool("toggle",status);
    }

    public void MainGategories(int index)
    {
        if (index != lastMainGategory)
        {
            lastMainGategory = index;

            foreach(GameObject gategory in mainGategories)
            {
                gategory.SetActive(false);
            }

            mainGategories[index].SetActive(true);
        }

        audioSource.PlayOneShot(clickReady);       
    }

    public void LoadoutTabs(int index)
    {
        if (index != lastLoadoutTab)
        {
            lastLoadoutTab= index;

            foreach (GameObject tab in loadoutTabs)
            {
                tab.SetActive(false);
            }

            loadoutTabs[index].SetActive(true);
        }

        audioSource.PlayOneShot(clickReady);      
    }

    public void NameChange()
    {
        usernamePopup.SetActive(true);
        invalidNameMessage.SetActive(false);
    }

    public void CancelNameChange()
    {
        if (PlayerPrefs.HasKey("username")) usernamePopup.SetActive(false);
        else Application.Quit();
    }

    private void UpdateUsername()
    {
        string newName = PlayerPrefs.GetString("username");
        topBarUsername.GetComponent<Text>().text = newName;
        if (GameObject.Find("/Player Avatar")) GameObject.Find("/Player Avatar").SendMessage("UpdateNameTag");
    }

    // clears all local data
    public void ClearLocalData()
    {
        PlayerPrefs.DeleteAll();
        Application.Quit();
    }

    public void SpawnPlayerInGame()
    {
        if (inTheMenus|| respawnCanvas.GetChild(0).gameObject.GetComponent<RespawnManager>().currentTime>0) return;

        spawnActions.SetActive(false);

        // hide player avatar
        if(GameObject.Find("/Player Avatar")) GameObject.Find("/Player Avatar").SetActive(false);
        respawnCanvas.GetChild(0).gameObject.SetActive(false);

        // create player
        GameObject.Find("/Network Client").GetComponent<Client>().LocalPlayerSpawn();

        HideMenuSystemForGame();
    }

    private void HideMenuOnlyItems()
    {
        foreach(GameObject o in hideWhenNotMenus) o.SetActive(false);     
    }   

    private void HideGameOnlyItems()
    {
        foreach (GameObject o in hideWhenMenus) o.SetActive(false);
    }

    public void EditLoadoutInGame()
    {
        gameObject.SetActive(true);
        NavigationBar(2);
        respawnCanvas.GetChild(0).gameObject.SetActive(false);
        //if(GameObject.Find("/Dead Player")) GameObject.Find("/Dead Player").GetComponentInChildren<Camera>().rect = new Rect(-0.33f, 0f, 1f, 1f);
    }
    
    public void ResumeGame()
    {
        GameObject.Find("/Player").SendMessage("ContinueGame");
        HideMenuSystemForGame();

    }

    void HideMenuSystemForGame()
    {
        // hide menu system     
        NavigationBar(0);
        for (int i = 0; i < transform.root.childCount; i++)
            if (transform.root.GetChild(i) != respawnCanvas
                && transform.root.GetChild(i).name != "Pause UI"
                && transform.root.GetChild(i).name != "Game Log UI")
                transform.root.GetChild(i).gameObject.SetActive(false);
    }

    public void LeaveServer()
    {
        GameObject.Find("/Network Client").GetComponent<Client>().Quit();
        SceneManager.LoadScene("MainMenu");
    }

    public void KillYourself()
    {
        if (GameObject.Find("/Player")) GameObject.Find("/Player").SendMessage("KillYourself");
    }
}
