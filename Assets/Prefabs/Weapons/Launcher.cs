using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : Weapon, IWeapon
{
    public Rigidbody grenade;
    public float launchSpeed = 12f;
    float playerStartSpeed = 0f;
    public AudioClip[] firingSounds;

    private bool hasDetonateProjectile = false;
    private List<LauncherGrenade> waitingToBeDetonated = new List<LauncherGrenade>();
    private bool isLocal = false;
    public GameObject optional_muzzle;
    public Transform optional_muzzle_index;

    public override void PlayFireSound()
    {
        if(firingSounds.Length>0)
        audioSource.PlayOneShot(firingSounds[Random.Range(0, firingSounds.Length - 1)]);
        else
            audioSource.PlayOneShot(fireSound);
    }

    void Update()
    {
        if (hasDetonateProjectile&&inHand)
        {
            if (Input.GetButtonUp("Fire2")) {
                if (GameObject.Find("/Network Client")) GameObject.Find("/Network Client").SendMessage("DetonateFinalWord");
                DetonateFinalWord();
            }
        }
    }

    internal void DetonateFinalWord()
    {
        foreach (LauncherGrenade lg in waitingToBeDetonated)
        {
            if (lg)
                lg.Detonate();
        }
        waitingToBeDetonated.Clear();
    }

    public override void FiringActions()
    {
        PlayFireSound();

        isLocal = (transform.root.gameObject.GetComponent<NetworkPlayer>() == null);

        //demolisher is treated in differend medhod
        if (GetName().Trim().ToLower().Contains("demolisher")) { Demolisher(); return; }

        //final word
        if (GetName().Trim().ToLower().Contains("final")) hasDetonateProjectile = true;


        if (optional_muzzle && optional_muzzle_index)
        {
            // Muzzle (optional)
            GameObject clonez = Instantiate(optional_muzzle, optional_muzzle_index.position, optional_muzzle_index.rotation);
            clonez.transform.parent = optional_muzzle_index;
            Destroy(clonez.gameObject, 1.2f);
        }

        Transform cam = transform.parent.parent;
        playerStartSpeed =transform.root.gameObject.GetComponent<Rigidbody>().velocity.y;
        Rigidbody clone = Instantiate(grenade, cam.position+cam.TransformDirection(Vector3.forward), cam.rotation);
        clone.velocity = cam.TransformDirection(Vector3.forward * (launchSpeed+Mathf.Abs(playerStartSpeed)));

        // Set grenade properties
        LauncherGrenade particle=clone.GetComponent<LauncherGrenade>();
        waitingToBeDetonated.Add(particle);
        particle.weaponOfOrigin = weaponName;
        particle.fullDmg = damage;
        particle.isLocal = (transform.root.gameObject.GetComponent<NetworkPlayer>() == null);
        particle.mediumDmg = Mathf.Ceil(damage / 3);
        particle.littleDmg = Mathf.Ceil(damage / 4);


        //if instant impact
        List<RaycastHit> hitData = GetRaycastHitObject(5.5f, Vector2.zero, 1f);

        // If hit
        if (hitData.Count > 0)
        {
            foreach (RaycastHit hit in hitData)
            {
                if (hit.distance > 5.5f) continue;
                if (hit.transform.tag == "Untagged" || hit.transform.tag == "Enemy")
                {
                    particle.Detonate();
                }

            }
        }
    }

    void Demolisher()
    {
        //-2 extra ammo
        if(isLocal)SetAmmo(-2);

        float t = 0.2f;
        StartCoroutine(FireDemolisher(transform.GetChild(0).GetChild(0), 0f));
        StartCoroutine(FireDemolisher(transform.GetChild(0).GetChild(1), t));
        StartCoroutine(FireDemolisher(transform.GetChild(0).GetChild(2), t*2));
    }

    IEnumerator FireDemolisher(Transform pos, float delay)
    {
        yield return new WaitForSeconds(delay);
      
        if(isLocal) playerStartSpeed = transform.root.gameObject.GetComponent<Rigidbody>().velocity.y; 
        Rigidbody cloned = Instantiate(grenade, pos.position, pos.rotation);
        if(isLocal)cloned.velocity = pos.TransformDirection(Vector3.forward * (launchSpeed + Mathf.Abs(playerStartSpeed)));
        else cloned.velocity = GetComponentInParent<NetworkPlayer>().raycastPivot.TransformDirection(Vector3.forward * (launchSpeed + Mathf.Abs(playerStartSpeed)));
        

        // Set grenade properties
        LauncherGrenade particle = cloned.GetComponent<LauncherGrenade>();
        particle.weaponOfOrigin = weaponName;
        particle.isLocal = isLocal;
        particle.fullDmg = damage;
        particle.mediumDmg = Mathf.Ceil(damage / 3);
        particle.littleDmg = Mathf.Ceil(damage / 4);

        if (!isLocal) yield break;
        //if instant impact
        List<RaycastHit> hitData = GetRaycastHitObject(1f, Vector2.zero, 1f);

        // If hit
        if (hitData.Count > 0)
        {
            foreach (RaycastHit hit in hitData)
            {
                if (hit.distance > 1f) continue;
                if (hit.transform.tag == "Untagged" || hit.transform.tag == "Enemy")
                {
                    particle.Detonate();

                    //self dmg
                    if (GameObject.Find("/Player"))
                        GameObject.Find("/Player").GetComponent<PlayerManager>().TakeDamage(
                        20f,
                        transform.position,
                        -1, -1);
                }

            }
        }
    }

    internal override void NetworkFire(RaycastHit target)
    {
        //demolisher is treated in differend medhod
        if (GetName().Trim().ToLower().Contains("demolisher")) { Demolisher(); return; }

        //final word
        if (GetName().Trim().ToLower().Contains("final")) hasDetonateProjectile = true;


        if (optional_muzzle && optional_muzzle_index)
        {
            // Muzzle (optional)
            GameObject clonez = Instantiate(optional_muzzle, optional_muzzle_index.position, optional_muzzle_index.rotation);
            clonez.transform.parent = optional_muzzle_index;
            SetLayerRecursively(clonez, 0);
            Destroy(clonez.gameObject, 1.2f);
        }

        Transform cam = GetComponentInParent<NetworkPlayer>().raycastPivot;
        playerStartSpeed = transform.root.gameObject.GetComponent<Rigidbody>().velocity.y;
        Rigidbody clone = Instantiate(grenade, optional_muzzle_index.position, cam.rotation);
        clone.velocity = cam.TransformDirection(Vector3.forward * (launchSpeed + Mathf.Abs(playerStartSpeed)));

        // Set grenade properties
        LauncherGrenade particle = clone.GetComponent<LauncherGrenade>();
        waitingToBeDetonated.Add(particle);
        particle.weaponOfOrigin = weaponName;
        particle.fullDmg = damage;
        particle.isLocal = (transform.root.gameObject.GetComponent<NetworkPlayer>() == null);
        particle.mediumDmg = Mathf.Ceil(damage / 3);
        particle.littleDmg = Mathf.Ceil(damage / 4);
    }
}
