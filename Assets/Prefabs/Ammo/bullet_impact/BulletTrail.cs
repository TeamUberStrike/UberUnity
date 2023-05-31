using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    float hitRange = 21f;
    float flySpeed = 400f;
    public bool sound = false;
    public GameObject splashEffect;
    Vector3 startPos;

    public AudioClip[] waterHitSounds;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        transform.Translate(transform.forward * Time.deltaTime * flySpeed, Space.World);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, hitRange))
        {
            if (startPos.y > 0 && hit.point.y < 0 && GameObject.Find("/Environment").GetComponent<Environment>().mapHasWater) Splash(hit.point);
            Destroy(gameObject);
        }

    }       

    void Splash(Vector3 x)
    {
        Vector3 p = Calc(startPos,x);
        GameObject cloneHit = Instantiate(splashEffect, p, Quaternion.identity);
        if (sound) cloneHit.GetComponent<AudioSource>().PlayOneShot(waterHitSounds[Random.Range(0,waterHitSounds.Length - 1)]); // sound
        Destroy(cloneHit, 2f);
    }

    Vector3 Calc(Vector3 a, Vector3 b)
    {

        Vector3 direction = Vector3.Normalize(new Vector3(b.x - a.x, b.y - a.y, b.z - a.z));

        while (a.y > 0)
        {
            a += direction; // a.x += direction.x ...
        }

        a -= a.y * direction;

        return a;
    }
}
