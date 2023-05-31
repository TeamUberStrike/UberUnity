using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponShopItem : ShopItem, IShopItem
{
    internal int tier;
    private string weaponClass;

    internal override string GetName()
    {
        return prefab.GetComponent<Weapon>().weaponName;
    }

    internal override Sprite GetThumb()
    {
        return prefab.GetComponent<Weapon>().thumbnail;
    }

    internal override void InvokeItem(){

        // Get tier
        tier = prefab.GetComponent<Weapon>().tier;

        // Get weapon class
        weaponClass = GetWeaponClass();

        // Set description
        transform.GetChild(2).gameObject.GetComponent<Text>().text = "Tier " + tier + " " + weaponClass;

    }

    private string GetWeaponClass()
    {
        string raw = prefab.GetComponent<Weapon>().GetType().ToString();

        // Todo: switch name prefix

        return raw;
    }


}
