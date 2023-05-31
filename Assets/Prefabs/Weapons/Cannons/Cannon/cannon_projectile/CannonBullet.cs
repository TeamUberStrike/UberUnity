using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBullet : MonoBehaviour
{
    private bool impact = false;

    public float flySpeed = 29f;
    private AudioSource audioSource;
    public AudioClip impactSound;

    float mediumDmgRadius = 2f;
    float smallDmgRadius = 5f;

    internal float fullDmg = 30f;
    internal float mediumDmg = 20f;
    internal float littleDmg = 5f;

    public float selfJumpPower = 2f;
    public float reduceDmgToSelfBy = 3f;

    internal string weaponOfOrigin = "Unknown weapon";
    public bool shockWave = false;
    float playerStartSpeed = 0f;
    public Transform explosionEffect;
    private float hitRange = 4f;

    public bool isLocal = true;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(SelfDestruct(8f));
        if(GameObject.Find("/Player")) playerStartSpeed = GameObject.Find("/Player").GetComponent<Rigidbody>().velocity.y;
    }

    void FixedUpdate()
    {
        if (!impact)
        {
            transform.Translate(transform.forward * (flySpeed+ Mathf.Abs(playerStartSpeed)) * Time.deltaTime, Space.World);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit)&&(hit.transform.gameObject.tag=="Enemy"|| hit.transform.gameObject.tag == "Untagged"))
            {
                if (Mathf.Abs(hit.distance) < hitRange)
                {
                    impact = true;

                    transform.position = hit.point;

                    GetComponent<Renderer>().enabled = false;
                    if (GetComponent<Light>() != null) GetComponent<Light>().enabled = false;

                    for (int i = 0; i < transform.childCount; i++)
                    {
                        transform.GetChild(i).gameObject.GetComponent<Renderer>().enabled = false;
                    }

                    if(isLocal) Explosion();

                    Transform clone = Instantiate(explosionEffect, transform.position, transform.rotation);
                    clone.transform.parent = transform;
                    clone.GetChild(2).gameObject.SetActive(shockWave); // set shockwave                 
                    audioSource.loop = false;
                    audioSource.Stop();
                    audioSource.PlayOneShot(impactSound, 1f);
                }
            }
        }
    }

    IEnumerator SelfDestruct(float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.loop = false;
        yield return new WaitUntil(() => audioSource.isPlaying == false);
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
            if (hitColliders[i].tag == "Enemy" && !IsBlocked(hitColliders[i]))
            {
                NetworkPlayer manager = hitColliders[i].transform.root.gameObject.GetComponent<NetworkPlayer>();
                if (!enemiesHit.Contains(manager.networkId)) // Prevent combos
                {
                    float dealedDamage = GetDmg(Vector3.Distance(hitColliders[i].transform.position, transform.position));
                    int criticalCode = 0; // cannon has no critical dmg
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
                if(GameObject.Find("/Player"))
                GameObject.Find("/Player").GetComponent<PlayerManager>().TakeDamage(
                    Mathf.Floor(GetDmg(Vector3.Distance(hitColliders[i].transform.position, transform.position)) / reduceDmgToSelfBy),
                    transform.position,
                    -1, -1);

                // Jump
                float dist = Vector3.Distance(hitColliders[i].transform.position, transform.position);

                if(dist<2) hitColliders[i].SendMessage("PowerUp", Vector3.Normalize((hitColliders[i].transform.position - transform.position)) * selfJumpPower);

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
