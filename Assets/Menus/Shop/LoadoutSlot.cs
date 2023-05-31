using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutSlot : MonoBehaviour
{
    private ShopKeeper shopKeeper;

    internal GameObject currentPrefab;
    internal bool mouseIn = false;

    public string[] allowedItemTypes;
    public string preferenceKey = "unknown";

    void Start()
    {
        StartCoroutine(LoadPrefs());
    }

    // Wait until shopkeeper is ready
    // Load saved loadout from playerPrefs
    private IEnumerator LoadPrefs()
    {
        yield return new WaitUntil(() => shopKeeper!=null);
        if (PlayerPrefs.HasKey(preferenceKey))
        {
            if (PlayerPrefs.GetString(preferenceKey) != null && PlayerPrefs.GetString(preferenceKey) != "null")
            {
                currentPrefab = Resources.Load(PlayerPrefs.GetString(preferenceKey), typeof(GameObject)) as GameObject;
                SetText();
                SetImage();
            }
        }
    }

    public void PointerIn(bool isIn)
    {
        mouseIn = isIn;
        if (isIn)
        {
            shopKeeper.mouseOnSlot = gameObject;
        }
        else
        {
            shopKeeper.mouseOnSlot = null;
        }
    }

    public bool TypeRequest(string type)
    {
        foreach(string i in allowedItemTypes)
        {
            if (i == type)
            {
                return true;
            }
        }
        return false;
    }

    public void SetShopKeeper(ShopKeeper keeper)
    {
        shopKeeper = keeper;
    }

    public void SetPrefab(GameObject prefab)
    {
        currentPrefab = prefab;
        SetText();
        SetImage();
        SavePreference();

        // for clothes
        if (currentPrefab.GetComponent<Appereance>()!=null)
        {
            shopKeeper.UpdateAvatar();
        }
    }

    private void SetText()
    {
        string t = shopKeeper.GetNameFromPrefab(currentPrefab);
        transform.GetChild(1).gameObject.GetComponent<Text>().text = t;

    }

    private void SetImage()
    {
        Sprite s = shopKeeper.GetSpriteFromPrefab(currentPrefab);
        transform.GetChild(2).gameObject.GetComponent<Image>().sprite = s;
    }

    private void SavePreference()
    {
        PlayerPrefs.SetString(preferenceKey, currentPrefab.name);
    }

    // drag out
    public void BeginDrag()
    {
        if (currentPrefab != null)
        {
            shopKeeper.Drag(currentPrefab, shopKeeper.GetSpriteFromPrefab(currentPrefab), true);
            transform.GetChild(2).gameObject.GetComponent<Image>().sprite = shopKeeper.emptySlot;
            transform.GetChild(1).gameObject.GetComponent<Text>().text = "Empty slot";
        }
    }

    public void EndDrag()
    {
        if(currentPrefab != null)
        {
            shopKeeper.EndDrag();
            if (!mouseIn) SetEmpty();
            else SetPrefab(currentPrefab);
        }
    }

    private void SetEmpty()
    {
        PlayerPrefs.SetString(preferenceKey, "null");
        if (currentPrefab.GetComponent<Appereance>() != null) shopKeeper.UpdateAvatar();
        currentPrefab = null;      
    }

    public string GetSavedType()
    {
        if (PlayerPrefs.HasKey(preferenceKey))
        {
            if (PlayerPrefs.GetString(preferenceKey) != null && PlayerPrefs.GetString(preferenceKey) != "null")
            {
                GameObject saved = Resources.Load(PlayerPrefs.GetString(preferenceKey), typeof(GameObject)) as GameObject;
                return shopKeeper.GetTypeFromPrefab(saved);

            }
        }

        return "null";
    }


}
