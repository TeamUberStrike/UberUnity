using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : Weapon, IWeapon
{
    public Transform projectile;
    public GameObject optional_muzzle;
    public Transform optional_muzzle_index;

    public float launchDelay = 0f;

    public override void FiringActions()
    {
        PlayFireSound();
        if (optional_muzzle&& optional_muzzle_index)
        {
            // Muzzle (optional)
            GameObject clonez = Instantiate(optional_muzzle, optional_muzzle_index.position, optional_muzzle_index.rotation);
            clonez.transform.parent = optional_muzzle_index;
            SetLayerRecursively(clonez, 9);
            Destroy(clonez.gameObject, 3f);
        }

        StartCoroutine(Launch());       
    }

    IEnumerator Launch()
    {
        yield return new WaitForSeconds(launchDelay);

        if (inHand)
        {
            // Create bullet
            Transform cam = transform.parent.parent;
            Transform clone = Instantiate(projectile, cam.position + (-cam.forward / 3), cam.rotation);

            // Set bullet properties
            CannonBullet particle = clone.GetComponent<CannonBullet>();
            particle.weaponOfOrigin = weaponName;
            particle.isLocal = (transform.root.gameObject.GetComponent<NetworkPlayer>() == null);
            particle.fullDmg = damage;
            particle.mediumDmg = Mathf.Ceil(damage / 3);
            particle.littleDmg = Mathf.Ceil(damage / 4);
        }
    }

    internal override void NetworkFire(RaycastHit target)
    {
        if (optional_muzzle && optional_muzzle_index)
        {
            // Muzzle (optional)
            GameObject clonez = Instantiate(optional_muzzle, optional_muzzle_index.position, optional_muzzle_index.rotation);
            clonez.transform.parent = optional_muzzle_index;
            SetLayerRecursively(clonez, 0);
            Destroy(clonez.gameObject, 3f);
        }
        StartCoroutine(NetworkLaunch());
    }

    IEnumerator NetworkLaunch()
    {
        yield return new WaitForSeconds(launchDelay);
      
        // Create bullet
        Transform cam = GetComponentInParent<NetworkPlayer>().raycastPivot;

        Transform clone = Instantiate(projectile, optional_muzzle_index.position, cam.rotation);

        // Set bullet properties
        CannonBullet particle = clone.GetComponent<CannonBullet>();
        particle.weaponOfOrigin = weaponName;
        particle.isLocal = (transform.root.gameObject.GetComponent<NetworkPlayer>() == null);
        particle.fullDmg = damage;
        particle.mediumDmg = Mathf.Ceil(damage / 3);
        particle.littleDmg = Mathf.Ceil(damage / 4);       
    }
}
