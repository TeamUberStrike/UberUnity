using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon, IWeapon
{
    public Vector2 spread = new Vector2(0.05f, 0.05f);
    public float shotCount = 6f;
    public float maxRange = 40f;
    public AudioClip[] firingSounds;
    public Transform muzzleIndex;

    
    private bool oneSound = true;

    public Transform shellIndex;
    public Vector3 shellSpeed;
    public GameObject optionalArrow;

    public override void PlayFireSound()
    {
        if (firingSounds.Length > 0)
            audioSource.PlayOneShot(firingSounds[Random.Range(0, firingSounds.Length - 1)]);
        else
            audioSource.PlayOneShot(fireSound);

        //custom narrator
        if (transform.root.name == "Player")
        {
            GameObject g = GameObject.Find("/Custom Narrator");
            if (g) g.SendMessage("PrimaryFire", GetType().ToString());
        }
    }

    // Get hit with range
    public override void FiringActions()
    {
        PlayFireSound();
        // if shell
        if (shellIndex) SpitShell(shellIndex, shellSpeed, 2);

        FireMuzzle();


        List<RaycastHit> hitData = GetRaycastHitObject(maxRange, spread, shotCount);
        // If hit
        if (hitData.Count > 0)
        {

            foreach (RaycastHit hit in hitData)
            {
                BulletTrail(muzzleIndex, hit.point, oneSound&& hit.distance < maxRange);
                if (hit.distance > maxRange) continue;

                if (hit.transform.tag == "Untagged")
                {
                    // SHOTGUN HAS THIS UNIQUE BLOCK
                    hitmark = GetImpact(hit.transform);
                    GameObject cloneHit = Instantiate(hitmark, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                    if(optionalArrow) Instantiate(optionalArrow, hit.point, transform.rotation);

                    //sound
                    if(oneSound) cloneHit.GetComponent<AudioSource>().PlayOneShot(GetImpactSound(hit.transform));
                    oneSound = false;

                    Destroy(cloneHit, 3f);
                    // SHOTGUN HAS THIS UNIQUE BLOCK
                }

                // Hit enemy
                else if (hit.transform.root.tag == "Enemy")
                {
                    float dealedDamage = damage;
                    int criticalCode = 0;

                    if (hit.transform.name == "Head") criticalCode = 1;
                    else if (hit.transform.name == "Nuts") criticalCode = 2;
                    if (criticalCode > 0) dealedDamage += criticalDmgBonus;

                    hit.transform.root.gameObject.GetComponent<NetworkPlayer>().
                    TakeDamage(dealedDamage, criticalCode, transform.position, weaponName);

                    CustomNarrator("HitEnemy");

                    // Show hitmark
                    Transform cloneh = Instantiate(hitmarkEnemy, hit.point, transform.parent.parent.rotation);
                    Destroy(cloneh.gameObject, 0.1f);

                    // Show damage text
                    transform.root.gameObject.GetComponent<PlayerUI>().ShowDealedDamage(dealedDamage, hit.point);

                    // arrow
                    if (optionalArrow)
                    {
                        GameObject arrow = Instantiate(optionalArrow, hit.point, transform.rotation);
                        arrow.transform.parent = hit.transform;
                    }
                }
            }
           
        } oneSound = true;
    }

    public override void FireMuzzle()
    {
        //muzzle
        if (muzzleIndex.position == Vector3.zero) return;
        Transform clone = Instantiate(muzzle, muzzleIndex.transform.position, muzzleIndex.transform.rotation);
        clone.parent = muzzleIndex;
        Destroy(clone.gameObject, 0.5f);

        //layer
        if (transform.root.gameObject.GetComponent<NetworkPlayer>() != null)
            SetLayerRecursively(clone.gameObject, 0);
    }

    internal override void NetworkFire(RaycastHit target)
    {
        List<RaycastHit> hitData = GetRaycastHitObject(muzzleIndex, target.point, spread, shotCount);

        if (hitData.Count > 0)
        {
            foreach (RaycastHit hit in hitData)
            {
                BulletTrail(muzzleIndex, hit.point, true);

                if (hit.transform.tag == "Untagged")
                {
                    hitmark = GetImpact(hit.transform);
                    GameObject cloneHit = Instantiate(hitmark, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                    if(oneSound) cloneHit.GetComponent<AudioSource>().PlayOneShot(GetImpactSound(hit.transform)); // sound
                    oneSound = false;
                    Destroy(cloneHit, 3f);
                }
            }
        }
        oneSound = true;
    }
}
