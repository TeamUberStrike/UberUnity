using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AppereanceType
{
    Holo, Head, Face, Gloves, UpperBody, LowerBody, Boots
}

public class Appereance : MonoBehaviour
{
    public string appereanceName = "Unnamed appereance";
    public Sprite thumbnail;
    public AppereanceType appType = AppereanceType.Head;
    public float armorGain = 0f;
    public string multipartSetName;
    public GameObject physicalItem; // for helmets etc..
    public bool dontHideDefaultFace = false;

    internal string GetAppType()
    {
        return appType.ToString();
    }
}
