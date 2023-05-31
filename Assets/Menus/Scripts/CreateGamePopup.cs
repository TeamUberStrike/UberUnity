using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreateGamePopup : MonoBehaviour
{
    // values
    int gameMode;
    int map;
    /*string gameName;
    string password;
    int maxPlayers;
    int timeLimit;
    int maxKills;*/

    // UI
    public Image mapImage;
    public Text nameLenght;
    public Text passLenght;
    public Text gameNameText; // = playername
    public InputField pass;
    public Slider maxPlayersSlider;
    public Slider timeLimitSlider;
    public Slider maxKillsSlider;
    public Text maxPlayersText;
    public Text timeLimitText;
    public Text maxKillsText;

    public Button[] gameModeButtons;
    public Button[] mapButtons;
    public GameObject container;

    public Sprite defaultMapSprite;
    public Sprite selectedMapSprite;

    public Sprite defaultModeSprite;
    public Sprite selectedModeSprite;

    //maps
    public Scene[] maps;
    public Sprite[] mapSprites;

    public GameObject loadingDialog;
    public Text loadStatus;
    public Slider loadProgress;

    void Awake()
    {
        SetMap(0);
        SetGamemode(0);
        gameNameText.text = PlayerPrefs.GetString("username");
        nameLenght.text = "("+PlayerPrefs.GetString("username").Length+"/21)";
    }

    public void CreateGame()
    {
        if (gameMode == 2)
        {
            container.SetActive(false);
            StartCoroutine(LoadAsync(map+2));
        }
    }

    public void CancelGame()
    {
        container.SetActive(false);
    }

    public void SetMap(int i)
    {
        foreach (Button b in mapButtons)
        {
            b.GetComponent<Image>().sprite = defaultMapSprite;
            b.gameObject.GetComponentInChildren<Text>().color = new Color(179f, 179f, 179f, 0.9f);
        }

        mapButtons[i].GetComponent<Image>().sprite = selectedMapSprite;
        mapButtons[i].gameObject.GetComponentInChildren<Text>().color = new Color(0.66f, 1f, 1f);

        map = i;
        print("map selected: " + map);
        mapImage.sprite = mapSprites[i];
    }

    public void SetGamemode(int i)
    {
        foreach (Button b in gameModeButtons)
        {
            b.GetComponent<Image>().sprite = defaultModeSprite;
            b.gameObject.GetComponentInChildren<Text>().color = new Color(179f, 179f, 179f, 0.5f);
        }

        gameModeButtons[i].GetComponent<Image>().sprite = selectedModeSprite;
        gameModeButtons[i].gameObject.GetComponentInChildren<Text>().color = new Color(0.66f, 1f, 1f);
        gameMode = i;
        print("mode selected: " + gameMode);
    }

    public void UpdateMaxKillsSlider()
    {
        maxKillsText.text = maxKillsSlider.value + "";
    }

    public void UpdateMaxPlayersSlider()
    {
        maxPlayersText.text = maxPlayersSlider.value + "";
    }
    public void UpdateTimeSlider()
    {
        timeLimitText.text = timeLimitSlider.value + "";
    }

    public void ShowContainer()
    {
        container.SetActive(true);
    }

    public void UpdatePass()
    {
        passLenght.text = "(" + pass.text.Length + "/8)";
    }

    IEnumerator LoadAsync(int i)
    {
        loadingDialog.SetActive(true);
        loadStatus.text = "Loading";
        yield return new WaitForSeconds(0.2f);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(i);

        while (!asyncLoad.isDone)
        {
            loadProgress.value = asyncLoad.progress;
            yield return null;
        }
    }

}
