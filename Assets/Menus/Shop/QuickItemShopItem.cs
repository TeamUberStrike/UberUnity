using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickItemShopItem : ShopItem, IShopItem
{
    private Sprite thumb;
    private string itemName;

    internal override string GetName()
    {
        return prefab.GetComponent<QuickItem>().itemName;
    }

    internal override Sprite GetThumb()
    {
        return prefab.GetComponent<QuickItem>().thumbnail;
    }

    // quickitems are not created dynamically, 
    // so instead of InvokeItem we use this
    void Awake()
    {
        // find shopkeeper
        shopKeeper = GetComponentInParent(typeof(ShopKeeper)) as ShopKeeper;

        // set thumb
        transform.GetChild(0).gameObject.GetComponent<Image>().sprite = GetThumb();

        // set name
        transform.GetChild(1).gameObject.GetComponent<Text>().text = GetName();

    }

}
