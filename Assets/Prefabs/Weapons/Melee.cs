using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : Weapon, IWeapon
{
    public AudioClip[] hitSounds;

    public override void FiringActions()
    {
        PlayFireSound();
        // Get hit with range

        /*// Get hit with range
        List<RaycastHit> hitData = GetRaycastHitObject(0.77f, Vector2.zero, 1f);

        // If hit
        if (hitData.Count > 0)
        {
            foreach (RaycastHit hit in hitData)
            {
                // Hit map
                if (hit.transform.tag == "Untagged")
                {
                    return; // do this to prevent melee going throught walls
                }
            }
        }
        */
        AttackHit();
    }

    private void AttackHit()
    {
        Transform playerCamera = transform.parent.parent;
        Collider[] hitColliders = Physics.OverlapSphere(playerCamera.position + (playerCamera.forward), 1.27f);

        foreach(Collider hitCollider in hitColliders)
        {
            // Enemy hit
            if (hitCollider.tag == "Enemy")
            {
                float dealedDamage = damage;
                int criticalCode = 3; // melee has no critical dmg BUT code 3 means smackdown

                hitCollider.transform.root.gameObject.GetComponent<NetworkPlayer>().
                TakeDamage(dealedDamage, criticalCode, transform.position, weaponName);

                audioSource.PlayOneShot(hitSounds[Random.Range(0, hitSounds.Length - 1)]);

                // Show damage text
                transform.root.gameObject.GetComponent<PlayerUI>().ShowDealedDamage(dealedDamage, hitCollider.transform.position);

                CustomNarrator("HitEnemy");

                return;

            }
        }

    }
}
