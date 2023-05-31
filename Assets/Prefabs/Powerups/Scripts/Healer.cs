using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{
    Transform player;
    bool active = false;
    Animator animator;
    float energy = 10f;
    public ParticleSystem s;
    AudioSource a;
    public AudioClip healSound;

    void Start()
    {
        player = GameObject.Find("/Player").transform;
        animator = GetComponentInChildren<Animator>();
        a = GetComponent<AudioSource>();
        InvokeRepeating("Heal",2f,2f);
    }

    void Update()
    {
        if (player == null) { Die(); return; }
        if (!active) return;

        //distance
        float dist = Vector3.Distance(transform.position,player.position);
        if (dist > 6f|| GetComponent<Collider>().enabled) return;

        Quaternion targetRotation = Quaternion.LookRotation(player.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5 * Time.deltaTime);

        if (dist < 1.6f) return;
        transform.position = Vector3.Lerp(transform.position, player.position, Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (active) return;
        if (collision.gameObject.tag != "Player") Activate();
    }

    void Activate()
    {
        active = true;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().enabled = false;
        animator.SetTrigger("activate");
    }

    void Die()
    {
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Collider>().enabled = true;
        animator.SetTrigger("die");
        //Destroy(gameObject);
    }

    void Heal()
    {
        energy--;

        if (energy <= 0)
        {
            Die();
            return;
        }

        //distance
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > 4f) return;

        s.Play();
        a.PlayOneShot(healSound,1f);
        player.gameObject.SendMessage("UpdateHealth", 10f);
        player.gameObject.SendMessage("ShowPickupName", "Heal");
        
    }
}
