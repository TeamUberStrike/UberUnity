using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splattergun : Weapon, IWeapon
{
    public Transform projectile;
    public Vector2 spread = new Vector2(0.05f, 0.05f);

    public Transform muzzleIndex;
    public AudioClip warmDownSound;


    public override void FiringActions()
    {
        PlayFireSound();
        /* // Muzzle 
        Transform cloneM = Instantiate(muzzle, muzzleIndex.transform.position, muzzleIndex.transform.rotation);
        cloneM.parent = muzzleIndex;
        Destroy(cloneM.gameObject, 0.6f);*/

        Vector3 spreadVector = new Vector3(Random.Range(-spread.x, spread.x), Random.Range(-spread.y, spread.y), 0f);
        //Transform cam = transform.parent.parent;

        Transform clone = Instantiate(projectile, muzzleIndex.transform.position, muzzleIndex.transform.rotation * Quaternion.Euler(spreadVector));

        // Set bullet properties
        SplatterParticle particle = clone.GetComponent<SplatterParticle>();
        particle.weaponOfOrigin = weaponName;
        particle.fullDmg = damage;
        particle.isLocal = (transform.root.gameObject.GetComponent<NetworkPlayer>() == null);
        particle.mediumDmg = Mathf.Ceil(damage/3);
        particle.littleDmg = Mathf.Ceil(damage / 4);

        //if instant impact
        List<RaycastHit> hitData = GetRaycastHitObject(2f, Vector2.zero, 1f);
        
        // If hit
        if (hitData.Count > 0)
        {
            foreach (RaycastHit hit in hitData)
            {
                if (hit.distance > 2.2f) continue;
                if (hit.transform.tag == "Untagged" || hit.transform.tag == "Enemy")
                {
                    particle.Detonate();
                }

            }
        }
    }

    void Update()
    {
        if (Input.GetButtonUp("Fire1")&&warmDownSound!=null&&inHand&&ammo>0) { GetComponent<AudioSource>().PlayOneShot(warmDownSound,0.9f); }
    }

    internal override void NetworkFire(RaycastHit target)
    {
        Vector3 spreadVector = new Vector3(Random.Range(-spread.x, spread.x), Random.Range(-spread.y, spread.y), 0f);
        //Transform cam = transform.parent.parent;

        Transform clone = Instantiate(projectile, muzzleIndex.transform.position, GetComponentInParent<NetworkPlayer>().raycastPivot.rotation * Quaternion.Euler(spreadVector));

        // Set bullet properties
        SplatterParticle particle = clone.GetComponent<SplatterParticle>();
        particle.weaponOfOrigin = weaponName;
        particle.fullDmg = damage;
        particle.isLocal = (transform.root.gameObject.GetComponent<NetworkPlayer>() == null);
        particle.mediumDmg = Mathf.Ceil(damage / 3);
        particle.littleDmg = Mathf.Ceil(damage / 4);
    }
}

