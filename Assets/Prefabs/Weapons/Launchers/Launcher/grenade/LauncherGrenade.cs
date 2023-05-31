using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherGrenade : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip[] impactSound;
    public AudioClip detonateSound;

    private bool armed = true;
    public float detonateTime = 3f;
    public Transform explosionEffect;
    private Transform clone = null;

    //float fullDmgRadius = 2f;
    float mediumDmgRadius = 4f;
    float smallDmgRadius = 6f;

    public float fullDmg = 30f;
    public float mediumDmg = 20f;
    public float littleDmg = 5f;

    public float reduceDmgToSelfBy = 3f;
    public float selfJumpPower = 0.6f;
    internal string weaponOfOrigin = "Unknown weapon";
    internal bool isLocal = true;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(SelfDestruct(detonateTime));
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!armed) return;

        // Hit ground
        if (collision.gameObject.tag != "Player")
        {
            if(impactSound.Length>0) audioSource.PlayOneShot(impactSound[Random.Range(0, impactSound.Length - 1)]);
        }

    }

    void OnTriggerEnter(Collider other)
    {
        // Hit enemy
        if (other.gameObject.tag == "Enemy") Detonate();
    }

    IEnumerator SelfDestruct(float delay)
    {
        yield return new WaitForSeconds(delay);
        if(armed) Detonate();
        yield return new WaitUntil(() => audioSource.isPlaying == false);
        yield return new WaitForSeconds(0.5f);
        Destroy(clone.gameObject);
        Destroy(gameObject);
    }

    public void Detonate()
    {
        GetComponent<Collider>().enabled = false;
        armed = false;
        GetComponent<Renderer>().enabled = false; 
        if(GetComponent<TrailRenderer>()!=null) GetComponent<TrailRenderer>().emitting = false;
        if (transform.childCount > 0) transform.GetChild(0).gameObject.SetActive(false);
        if (transform.childCount > 1) transform.GetChild(1).gameObject.SetActive(false);

        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(detonateSound);

        if(isLocal) Explosion();
        clone = Instantiate(explosionEffect,transform.position,transform.rotation);
        //clone.transform.parent = transform;
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
                    manager.TakeDamage(dealedDamage, criticalCode, transform.position, weaponOfOrigin);
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
                    -1,-1);
                
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
            if(hitInfo.transform.tag == c.tag) return false;
        }

        return true;
    }
}
