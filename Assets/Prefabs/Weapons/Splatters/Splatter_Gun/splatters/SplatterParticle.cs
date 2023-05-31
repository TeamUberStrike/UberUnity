using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplatterParticle : MonoBehaviour
{
    private bool impact = false;
    public float flySpeed = 33f;
    private AudioSource audioSource;
    public AudioClip impactSound;
    public Transform explosionEffect;

    public float fullDmgRadius = 1f;
    public float mediumDmgRadius = 2f;
    public float smallDmgRadius = 3f;

    internal float fullDmg = 30f;
    internal float mediumDmg = 20f;
    internal float littleDmg = 5f;

    public float reduceDmgToSelfBy = 3f;

    internal string weaponOfOrigin = "Unknown weapon";
    internal bool isLocal = true;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(SelfDestruct(5f));
    }

    void FixedUpdate()
    {
        if (!impact)
        {
            transform.Translate(transform.forward * flySpeed * Time.deltaTime, Space.World);

            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit, 4f) && hit.transform.gameObject.tag != "Player")
            {
                transform.position = hit.point;
                Detonate();
            }

        }
    }

    public void Detonate()
    {
        impact = true;
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(impactSound, 1f);
        GetComponent<Renderer>().enabled = false;

        if(isLocal)Explosion();
        Transform clone = Instantiate(explosionEffect, transform.position, transform.rotation);
        clone.transform.parent = transform;
    }

    IEnumerator SelfDestruct(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    private void Explosion()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, smallDmgRadius);
        int i = 0;
        List<int> enemiesHit = new List<int>();

        while (i < hitColliders.Length)
        {
            // Enemy hit
            if (hitColliders[i].tag == "Enemy")
            {
                NetworkPlayer manager = hitColliders[i].transform.root.gameObject.GetComponent<NetworkPlayer>();
                if (!enemiesHit.Contains(manager.networkId)) // Prevent combos 
                {
                    float dealedDamage = GetDmg(Vector3.Distance(hitColliders[i].transform.position, transform.position));
                    int criticalCode = 0; // splatter has no critical dmg
                    manager.TakeDamage(dealedDamage, criticalCode, transform.position, weaponOfOrigin);
                    enemiesHit.Add(manager.networkId);

                    // Show damage text
                    GameObject.Find("/Player").GetComponent<PlayerUI>().ShowDealedDamage(dealedDamage, hitColliders[i].transform.position);
                }
            }

            // Self hit
            if (hitColliders[i].tag == "Player")
            {
                
                // Send damage
                if (GameObject.Find("/Player"))
                    GameObject.Find("/Player").GetComponent<PlayerManager>().TakeDamage(
                    Mathf.Floor(GetDmg(Vector3.Distance(hitColliders[i].transform.position, transform.position)) / reduceDmgToSelfBy),
                    transform.position,
                    -1, -1);
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
}
