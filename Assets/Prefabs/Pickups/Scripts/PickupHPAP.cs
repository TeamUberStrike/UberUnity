using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PickupHPAP : MonoBehaviour
{
    [SerializeField] private string pickupName = "Pickup";
    [SerializeField] private float amount = 10f;
    [SerializeField] private float respawnTime = 10f;
    [SerializeField] private bool isAP = false; // True means AP pickup, False HP

    private bool available = true;
    AudioSource audioSource;
    new private ParticleSystem particleSystem;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        particleSystem = GetComponent<ParticleSystem>();
    }

    // Collision
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && available)
        {
            gameObject.GetComponentInChildren<Renderer>().enabled = false;
            available = false;
            audioSource.Play(0);        
            StartCoroutine(Respawn());

            if (isAP)
            {
                other.gameObject.SendMessage("UpdateArmor", amount);
            }
            else
            {
                other.gameObject.SendMessage("UpdateHealth", amount);              
            }
            other.gameObject.SendMessage("ShowPickupName", pickupName);
        }
        else if (other.tag == "Enemy" && available)
        {
            gameObject.GetComponentInChildren<Renderer>().enabled = false;
            available = false;
            StartCoroutine(Respawn());
        }
    }

    IEnumerator Respawn()
    {
        particleSystem.Play();
        GetComponent<CapsuleCollider>().enabled = false;
        yield return new WaitForSeconds(respawnTime);
        available = true;
        gameObject.GetComponentInChildren<Renderer>().enabled = true;
        particleSystem.Play();
        GetComponent<CapsuleCollider>().enabled = true;
    }
}
