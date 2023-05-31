using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oculus : MonoBehaviour
{
    GameObject player;
    public GameObject pointer;
    public AudioClip activateSound;
    float activations = 3f;
    bool first = false;

    AudioSource a;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("/Player");
        a = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) { Destroy(gameObject); }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Untagged")
        {
            if (first) return;
            first = true;
            InvokeRepeating("Activate", 2f, 2f);

        }
    }

    void Activate()
    {
        
        if (activations < 0) Destroy(gameObject);
        activations--;
        if(a.enabled)a.PlayOneShot(activateSound,1f);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 500f);
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
                    GameObject p = Instantiate(pointer,transform.position,transform.rotation);
                    p.transform.parent = transform;
                    p.transform.LookAt(hitColliders[i].transform);
                    Destroy(p, 0.1f);
                }
            }
            i++;
        }
    }
}
