using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppereanceShopItem : ShopItem, IShopItem
{
    float armorGain;

    internal override Sprite GetThumb()
    {
        return prefab.GetComponent<Appereance>().thumbnail;
    }
    internal override string GetName()
    {
        return prefab.GetComponent<Appereance>().appereanceName;
    }

    internal override void InvokeItem()
    {
        // set armor gain
        armorGain = prefab.GetComponent<Appereance>().armorGain;
        if (armorGain>0f) transform.GetChild(2).gameObject.GetComponent<Text>().text = "+"+armorGain+" armor";
    }
}
