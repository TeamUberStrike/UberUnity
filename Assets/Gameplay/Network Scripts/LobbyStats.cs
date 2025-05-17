using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyStats : MonoBehaviour
{
    Client client;
    Hashtable playerDatas;
    string localName;
    GlobalResources globalResources;

    // UI elements
    public Text gameNameText;
    public Text serverNameText;
    public Text modeText;
    public Text mapNameText;
    public Text playerCountText;

    public Transform statListLayout;
    public GameObject statListItemPrefab;
    
    void Start()
    {
        client = transform.parent.parent.gameObject.GetComponent<Client>();
        localName = client.localName;

        // default servername
        if (client.host == "192.168.1.158") serverNameText.text = "[FI] Debug";
        else serverNameText.text = client.host;

        globalResources = GameObject.Find("/Global Resources").GetComponent<GlobalResources>();
    }

    void OnEnable()
    {          
        //texts      
        mapNameText.text = "Map: "+SceneManager.GetActiveScene().name.Replace("_"," ");
        modeText.text = "Mode: "+"Deathmatch";
        gameNameText.text = "-";
    }

    void Update()
    {
        //get new datas
        playerDatas = client.playerDatas;

        //player count
        playerCountText.text = (playerDatas.Count+1)+" Players";

        //clear all
        for(int i = 0; i < statListLayout.childCount; i++) Destroy(statListLayout.GetChild(i).gameObject);
        
        //self
        GameObject sstatItemClone = Instantiate(statListItemPrefab);
        Transform t = sstatItemClone.transform;
        sstatItemClone.GetComponent<Image>().color = Color.white;
        t.GetChild(0).gameObject.GetComponent<Text>().text = localName; // name
        
        t.GetChild(3).gameObject.GetComponent<Text>().text = client.kills + ""; //kills
        t.GetChild(4).gameObject.GetComponent<Text>().text = client.deaths + ""; //deaths
        t.GetChild(5).gameObject.GetComponent<Text>().text = KDR(client.kills / client.deaths); //kdr 
        if(!client.isAlive) t.GetChild(6).gameObject.GetComponent<Image>().color = Color.white;
        else
        {
            t.GetChild(1).gameObject.SetActive(true);
            t.GetChild(2).gameObject.SetActive(true);
            t.GetChild(1).gameObject.GetComponent<Image>().sprite = globalResources.weapons[client.localWeaponId].GetComponent<Weapon>().thumbnail; // weapon img
            t.GetChild(2).gameObject.GetComponent<Text>().text = globalResources.weapons[client.localWeaponId].GetComponent<Weapon>().GetName(); // weapon name
        }

        t.SetParent(statListLayout, false);

        //others
        foreach (object o in playerDatas.Values)
        {
            PlayerData p = (PlayerData)o; 
            GameObject statItemClone = Instantiate(statListItemPrefab);
            Transform t2 = statItemClone.transform;

            statItemClone.transform.GetChild(0).gameObject.GetComponent<Text>().text = p.name; // name

            t2.GetChild(3).gameObject.GetComponent<Text>().text = p.kills+""; //kills
            t2.GetChild(4).gameObject.GetComponent<Text>().text = p.deaths+""; //deaths
            t2.GetChild(5).gameObject.GetComponent<Text>().text = KDR(p.kills / p.deaths); //kdr 
            if (!p.isAlive) t2.GetChild(6).gameObject.GetComponent<Image>().color = Color.white;
            else
            {
                t2.GetChild(1).gameObject.SetActive(true);
                t2.GetChild(2).gameObject.SetActive(true);
                t2.GetChild(1).gameObject.GetComponent<Image>().sprite = globalResources.weapons[p.weapon].GetComponent<Weapon>().thumbnail; // weapon img
                t2.GetChild(2).gameObject.GetComponent<Text>().text = globalResources.weapons[p.weapon].GetComponent<Weapon>().GetName(); // weapon name
            }

            t2.SetParent(statListLayout, false);          
        }       
    }

    private string KDR(float value)
    {
        if (float.IsNaN(value)) return "0";
        return (float)(Math.Round((double)value, 2))+"";
    }

}
