using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : Weapon, IWeapon
{
    public bool hasLaserEffect = false;
    public GameObject laser;

    public float maxZoom = 12f;
    public bool usesHardScope = true;
    public GameObject optionalArrow;
    public Transform muzzleIndex;
    public bool isSupressed = false;
    public Transform shellIndex;
    public Vector3 shellSpeed;
    public AudioClip[] firingSounds;

    public override void PlayFireSound()
    {
        if (firingSounds.Length > 0)
            audioSource.PlayOneShot(firingSounds[Random.Range(0, firingSounds.Length - 1)]);
        else
            audioSource.PlayOneShot(fireSound);
    }

    public override void FiringActions()
    {
        PlayFireSound();
        // if shell
        if (shellIndex) SpitShell(shellIndex, shellSpeed, 3);

        if(!isSupressed) FireMuzzle();

        // Get hit with range
        List<RaycastHit> hitData = GetRaycastHitObject(2000f, Vector2.zero, 1f);

        // If hit
        if (hitData.Count > 0)
        {
            foreach(RaycastHit hit in hitData)
            {
                if(!hasLaserEffect) BulletTrail(muzzleIndex, hit.point, true);

                // Hit map
                if (hit.transform.tag == "Untagged")
                {
                    hitmark = GetImpact(hit.transform);
                    GameObject cloneHit = Instantiate(hitmark, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                    if (optionalArrow) Instantiate(optionalArrow, hit.point, transform.rotation);

                    cloneHit.GetComponent<AudioSource>().PlayOneShot(GetImpactSound(hit.transform)); // sound
                    Destroy(cloneHit, 3f);
                }

                // Hit enemy
                else if (hit.transform.root.tag == "Enemy")
                {
                    float dealedDamage = damage;
                    int criticalCode = 0;

                    if (hit.transform.name == "Head") criticalCode = 1;
                    else if(hit.transform.name == "Nuts") criticalCode = 2;
                    if (criticalCode > 0) dealedDamage += criticalDmgBonus;

                    hit.transform.root.gameObject.GetComponent<NetworkPlayer>().
                    TakeDamage(dealedDamage, criticalCode, transform.position, weaponName);

                    // Show damage text
                    transform.root.gameObject.GetComponent<PlayerUI>().ShowDealedDamage(dealedDamage, hit.point);

                    CustomNarrator("HitEnemy");

                    // Show hitmark
                    Transform cloneh = Instantiate(hitmarkEnemy, hit.point, transform.parent.parent.rotation);
                    Destroy(cloneh.gameObject, 0.1f);

                    // arrow
                    if (optionalArrow) {
                        GameObject arrow = Instantiate(optionalArrow, hit.point, transform.rotation);
                        arrow.transform.parent = hit.transform;
                    }
                    
                }
            }
        }
    }

    public override void FireMuzzle()
    {
        // laser
        if (hasLaserEffect)
        {
            //network
            if (transform.root.gameObject.GetComponent<NetworkPlayer>() == null)
            {
                GameObject clonel = Instantiate(laser, muzzleIndex.transform.position + muzzleIndex.transform.forward * 75f, muzzleIndex.transform.rotation);
                Destroy(clonel, 1.6f);
            }
            
            return;
        }

        // muzzle
        if (muzzleIndex.position == Vector3.zero) return;
        Transform clone = Instantiate(muzzle, muzzleIndex.transform.position, muzzleIndex.transform.rotation);
        clone.parent = muzzleIndex;
        Destroy(clone.gameObject, 0.5f);
       
        // for non-local players
        if (transform.root.gameObject.GetComponent<NetworkPlayer>() != null)
        {
            //layer
            SetLayerRecursively(clone.gameObject, 0); 
        }
    }

    public override bool GetUsesHardScope()
    {
        return usesHardScope;
    }

    public override float GetMaxZoom()
    {
        return maxZoom;
    }

    internal override void NetworkFire(RaycastHit hit)
    {
        if (!hasLaserEffect) BulletTrail(muzzleIndex, hit.point, true);
        else
        {
            Transform r = GetComponentInParent<NetworkPlayer>().raycastPivot;
            GameObject clonel = Instantiate(laser, r.position+ (-r.up/2) + r.forward * 75f, r.rotation);
            Destroy(clonel, 1.6f);
        }

        // Hit map
        if (hit.transform.tag == "Untagged")
        {
            hitmark = GetImpact(hit.transform);
            GameObject cloneHit = Instantiate(hitmark, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
            if (optionalArrow)
            {
                GameObject a=Instantiate(optionalArrow, hit.point, transform.rotation);
                Destroy(a, 30f);
            }

            cloneHit.GetComponent<AudioSource>().PlayOneShot(GetImpactSound(hit.transform)); // sound
            Destroy(cloneHit, 3f);
        }
    }
}
