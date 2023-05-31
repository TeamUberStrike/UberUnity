using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupAmmo : MonoBehaviour
{
    [SerializeField] private string pickupName = "Ammo Pickup";
    [SerializeField] private string targetWeaponClass = "";
    [SerializeField] private float amount = 10f;
    [SerializeField] private float respawnTime = 10f;

    private bool available = true;

    // Collision
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && available)
        {
            gameObject.GetComponent<Renderer>().enabled = false;
            available = false;
            StartCoroutine(Respawn());

            // Send to manager
            object[] data = new object[3];
            data[0] = amount;
            data[1] = pickupName;
            data[2] = targetWeaponClass;
            other.gameObject.SendMessage("PickupAmmo", data);          
        }
        if (other.tag == "Enemy")
        {
            gameObject.GetComponent<Renderer>().enabled = false;
            available = false;
            StartCoroutine(Respawn());
        }
    }

    IEnumerator Respawn()
    {
        GetComponent<BoxCollider>().enabled = false;
        yield return new WaitForSeconds(respawnTime);
        available = true;
        gameObject.GetComponent<Renderer>().enabled = true;
        GetComponent<BoxCollider>().enabled = true;
    }
}
