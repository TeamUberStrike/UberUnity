using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handgun : Weapon, IWeapon
{
    public Vector2 spread = new Vector2(0.02f, 0.02f);
    public float maxRange = 50f;

    public Transform muzzleIndex;
    public bool isSupressed = false;
    public Transform shellIndex;
    public Vector3 shellSpeed;
    public GameObject optionalArrow;
    public AudioClip[] firingSounds;

    public override void PlayFireSound()
    {
        if (firingSounds.Length > 0) audioSource.PlayOneShot(firingSounds[Random.Range(0, firingSounds.Length - 1)]);
        else audioSource.PlayOneShot(fireSound);
    }

    public override void FiringActions()
    {
        PlayFireSound();

        if(!isSupressed) FireMuzzle();

        // if shell
        if (shellIndex) SpitShell(shellIndex, shellSpeed, 1);

        // Get hit with range
        List<RaycastHit> hitData = GetRaycastHitObject(maxRange, spread, 1f);

        // If hit
        if (hitData.Count > 0)
        {
            foreach (RaycastHit hit in hitData)
            {
                BulletTrail(muzzleIndex, hit.point, true);
                if (hit.distance > maxRange) return;
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
                    else if (hit.transform.name == "Nuts") criticalCode = 2;
                    if (criticalCode > 0) dealedDamage += criticalDmgBonus;

                    hit.transform.root.gameObject.GetComponent<NetworkPlayer>().
                    TakeDamage(dealedDamage, criticalCode, transform.position, weaponName);

                    // Show hitmark
                    Transform cloneh = Instantiate(hitmarkEnemy, hit.point, transform.parent.parent.rotation);
                    Destroy(cloneh.gameObject, 0.1f);

                    // Show damage text
                    transform.root.gameObject.GetComponent<PlayerUI>().ShowDealedDamage(dealedDamage, hit.point);

                    CustomNarrator("HitEnemy");

                    // arrow
                    if (optionalArrow)
                    {
                        GameObject arrow = Instantiate(optionalArrow, hit.point, transform.rotation);
                        arrow.transform.parent = hit.transform;
                    }
                }
            }
        }

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
        List<RaycastHit> hitData = GetRaycastHitObject(muzzleIndex, target.point, spread, 1f);

        if (hitData.Count > 0)
        {
            foreach (RaycastHit hit in hitData)
            {
                BulletTrail(muzzleIndex, hit.point, true);

                if (hit.transform.tag == "Untagged")
                {
                    hitmark = GetImpact(hit.transform);
                    GameObject cloneHit = Instantiate(hitmark, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                    cloneHit.GetComponent<AudioSource>().PlayOneShot(GetImpactSound(hit.transform)); // sound
                    Destroy(cloneHit, 3f);
                }
            }
        }
    }
}
