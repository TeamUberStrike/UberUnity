using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyMine : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip detonateSound;
    private bool armed = true;
    public Transform explosionEffect;
    private Transform clone = null;

    //float fullDmgRadius = 2f;
    float mediumDmgRadius = 4f;
    float smallDmgRadius = 6f;

    public float fullDmg = 150f;
    public float mediumDmg = 75f;
    public float littleDmg = 33f;

    public float reduceDmgToSelfBy = 1f;
    public float selfJumpPower = 1f;
    internal bool isLocal = true;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!armed) return;

        // Hit ground
        if (collision.gameObject.tag != "Player")
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!armed) return;
        // Hit enemy
        if (other.gameObject.tag == "Enemy" && !IsBlocked(other)) Detonate();
    }

    public void Detonate()
    {
        GetComponent<Collider>().enabled = false;
        armed = false;
        GetComponent<Renderer>().enabled = false;
        if (GetComponent<TrailRenderer>() != null) GetComponent<TrailRenderer>().emitting = false;
        if (transform.childCount > 0) transform.GetChild(0).gameObject.SetActive(false);
        if (transform.childCount > 1) transform.GetChild(1).gameObject.SetActive(false);

        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(detonateSound);

        if (isLocal) Explosion();
        clone = Instantiate(explosionEffect, transform.position, transform.rotation);
    }

    private void Explosion()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, smallDmgRadius);
        int i = 0;
        List<int> enemiesHit = new List<int>();

        while (i < hitColliders.Length)
        {
            // Enemy hit
            if (hitColliders[i].tag == "Enemy" && !IsBlocked(hitColliders[i]))
            {
                NetworkPlayer manager = hitColliders[i].transform.root.gameObject.GetComponent<NetworkPlayer>();
                if (!enemiesHit.Contains(manager.networkId)) // Prevent combos 
                {
                    float dealedDamage = GetDmg(Vector3.Distance(hitColliders[i].transform.position, transform.position));
                    int criticalCode = 0; // grenade has no critical dmg
                    manager.TakeDamage(dealedDamage, criticalCode, transform.position, "stickymine");
                    enemiesHit.Add(manager.networkId);

                    // Show damage text
                    GameObject.Find("/Player").GetComponent<PlayerUI>().ShowDealedDamage(dealedDamage, hitColliders[i].transform.position);
                }
            }

            // Self hit
            if (hitColliders[i].tag == "Player" && !IsBlocked(hitColliders[i]))
            {
                // Send damage
                if (GameObject.Find("/Player"))
                    GameObject.Find("/Player").GetComponent<PlayerManager>().TakeDamage(
                    Mathf.Floor(GetDmg(Vector3.Distance(hitColliders[i].transform.position, transform.position)) / reduceDmgToSelfBy),
                    transform.position,
                    -1, -1);

                // Jump
                hitColliders[i].SendMessage("PowerUp", (hitColliders[i].transform.position - transform.position) / 2 * selfJumpPower);
                hitColliders[i].SendMessage("SetCanUsePowerUp", true);
            }

            i++;
        }

    }

    // Get amount of dmg by distance
    private float GetDmg(float distance)
    {
        if (distance < mediumDmgRadius) return fullDmg;
        if (distance < smallDmgRadius) return mediumDmg;
        return littleDmg;
    }

    private bool IsBlocked(Collider c)
    {
        RaycastHit hitInfo;
        if (Physics.Linecast(transform.position, c.transform.position, out hitInfo))
        {
            if (hitInfo.transform.tag == c.tag) return false;
        }

        return true;
    }
}
