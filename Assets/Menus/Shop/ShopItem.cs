using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IShopItem
{
    
}

public class ShopItem : MonoBehaviour
{
    public GameObject prefab; 

    internal ShopKeeper shopKeeper;

    public void Build(GameObject prefab, ShopKeeper shopKeeper)
    {
        this.prefab = prefab;
        this.shopKeeper = shopKeeper;

        // set thumb
        transform.GetChild(0).gameObject.GetComponent<Image>().sprite = GetThumb();

        // set name
        transform.GetChild(1).gameObject.GetComponent<Text>().text = GetName();

        // other inits
        InvokeItem();
    }

    internal virtual string GetName()
    {
        return "Unknown item";
    }

    internal virtual Sprite GetThumb()
    {
        return null;
    }

    internal virtual void InvokeItem()
    {

    }

    public void BeginDrag()
    {
        ShowPopup(false);
        shopKeeper.Drag(prefab, GetThumb(), false);

    }

    public void EndDrag()
    {
        shopKeeper.EndDrag();

    }

    public void ShowPopup(bool show)
    {
        shopKeeper.ItemInfoPopup(show,this);
    }

    public virtual void MouseScroll()
    {
        float delta = Input.GetAxis("Mouse ScrollWheel");

        transform.parent.parent.parent.GetChild(2).gameObject.GetComponent<Scrollbar>().value += delta;
    }

}
