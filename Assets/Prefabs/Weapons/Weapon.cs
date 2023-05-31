using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    string GetName();
    float GetMaxZoom();
    void BringInHand(bool inHand);
    void SetAmmo(float add);
    bool GetUsesHardScope();
}

public class Weapon : MonoBehaviour
{
    // Weapon info
    public string weaponName = "?? Weapon";
    public Sprite thumbnail;
    public int tier = 1;

    internal GameObject hitmark;
    internal Transform hitmarkEnemy;

    // Ammo
    public float baseAmmo = 10f;
    public float maxAmmo = 20f;
    internal float ammo = 0;
    public bool unlimitedAmmo = false;

    // Fire
    internal bool canFire = true;
    public bool rapidFire = false;
    public float fireRate = 1.5f;
    internal bool fireReleased = true;
    internal Transform muzzle;

    // Audio
    public AudioClip fireSound;
    public AudioClip activeSound;
    internal AudioSource audioSource;
    internal Animator animator;

    // Damage
    public float damage = 10f;
    public float criticalDmgBonus = 10f;

    internal bool inHand = false;
    private GameObject networkClient;
    internal GlobalResources globalResources;
    internal Transform bulletTrail;

    public Vector3 pivotPrefix;

    public void SetAmmo(float add)
    {
        ammo += add;
        if (ammo > maxAmmo) ammo = maxAmmo;

        // Update UI
        object[] tempStorage = new object[4];
        tempStorage[0] = ammo;
        tempStorage[1] = unlimitedAmmo;
        transform.root.gameObject.SendMessage("UpdateAmmoCounter", tempStorage);
    }
   
    public float GetAmmo()
    {
        return ammo;
    }

    public string GetName()
    {
        return weaponName;
    }

    public virtual bool GetUsesHardScope()
    {
        return false;
    }

    public virtual float GetMaxZoom()
    {
        return 42.2f;
    }

    public void BringInHand(bool inHand)
    {
        this.inHand = inHand;

        // init if not set
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // in hand
        animator.SetBool("inHand", inHand);
        if (inHand)
        {
            audioSource.PlayOneShot(activeSound);
            SetAmmo(transform.root.gameObject.GetComponent<PlayerManager>().GetCollectedAmmoFor(this.GetType().ToString()));

            //custom narrator
            if (transform.root.name == "Player")
            {
                GameObject g = GameObject.Find("/Custom Narrator");
                if (g) g.SendMessage("WeaponActivate", GetType().ToString());
            }
        }
    }

    void PrimaryFire(bool fired)
    {
        if (!fired) { fireReleased = true; }
        if (canFire && rapidFire && !fireReleased || canFire && fireReleased && fired)
        {
            if (ammo > 0 || unlimitedAmmo)
            {
                if(networkClient!=null && networkClient.activeInHierarchy) networkClient.SendMessage("LocalPlayerFiredWeapon");

                animator.SetTrigger("shoot");
                ammo--;
                canFire = false;
                fireReleased = false;
                StartCoroutine(LimitFireRate());

                FiringActions();
                transform.root.gameObject.SendMessage("CrosshairShoot");
                SetAmmo(0);

                //custom narrator rapid fire
                if (transform.root.name == "Player")
                {
                    if (fireRate < 0.3 && rapidFire)
                    {
                        GameObject g = GameObject.Find("/Custom Narrator");
                        if (g) g.SendMessage("RapidFire");
                    }
                }
            }
            else
            {
                transform.root.gameObject.SendMessage("OutOfAmmoSound");
                CustomNarrator("OutOfAmmo");
            }
        }
    }

    public virtual void FiringActions()
    {
        Debug.Log("virtual fire action");
    }

    IEnumerator LimitFireRate()
    {
        yield return new WaitForSeconds(fireRate);
        canFire = true;

        //rapid
        if (Input.GetButton("Fire1") && inHand&& transform.root.gameObject.GetComponent<PlayerInput>().isActiveAndEnabled) PrimaryFire(true);
    }

    
    // Init
    void Start()
    {
        try
        {
            networkClient = GameObject.Find("/Network Client");
        }
        catch { }

        globalResources = GameObject.Find("/Global Resources").GetComponent<GlobalResources>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        ammo = baseAmmo;
        hitmarkEnemy = globalResources.hitmarkEnemy;
        muzzle = globalResources.defaultMuzzle;
        bulletTrail = globalResources.bulletTrail;

        if (pivotPrefix != Vector3.zero) transform.localPosition = pivotPrefix;
    }

    public virtual void PlayFireSound()
    {
        if(audioSource&&fireSound)audioSource.PlayOneShot(fireSound);
    }

    //for local
    public List<RaycastHit> GetRaycastHitObject(float maxRange, Vector2 spread, float shotCount)
    {
        Transform playerCamera = transform.parent.parent;
        List<RaycastHit> hits = new List<RaycastHit>();

        RaycastHit hit; 
        Camera cam = playerCamera.gameObject.GetComponent<Camera>();
        Vector3 rayOrigin = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));

        do
        {
            Vector3 spreadVector = GetSpread(spread);
            if (Physics.Raycast(rayOrigin, playerCamera.forward + spreadVector, out hit))
            {
                hits.Add(hit);
            }
                
            shotCount--;
        } while (shotCount > 0);
        
        return hits;
    }

    //for network
    public List<RaycastHit> GetRaycastHitObject(Transform origin, Vector3 target, Vector2 spread, float shotCount)
    {
        List<RaycastHit> hits = new List<RaycastHit>();

        RaycastHit hit;
        do
        {
            Vector3 spreadVector = GetSpread(spread);
            if (Physics.Raycast(origin.position, (target-origin.position).normalized+spreadVector, out hit))
            {
                hits.Add(hit);
            }

            shotCount--;
        } while (shotCount > 0);

        return hits;
    }

    public GameObject GetImpact(Transform t)
    {
        if (!t.gameObject.GetComponent<Renderer>()) return globalResources.stoneBulletEffect; ;
        string materialName = t.gameObject.GetComponent<Renderer>().material.name.Trim().ToLower();
        GameObject result = globalResources.stoneBulletEffect; // default

        //metal, rock, sand, wood
        if (materialName.Contains("metal")) result = globalResources.metalBulletEffect;
        else if (materialName.Contains("sand") || materialName.Contains("grass") || materialName.Contains("dirt")) result = globalResources.sandBulletEffect;
        else if (materialName.Contains("wood")) result = globalResources.woodBulletEffect;

        return result;
    }

    public AudioClip GetImpactSound(Transform t)
    {
        if(t.gameObject.GetComponent<Renderer>()==null) return globalResources.GetBulletHitSound("cement");
        string materialName = t.gameObject.GetComponent<Renderer>().material.name.Trim().ToLower();
        return globalResources.GetBulletHitSound(materialName);
    }

    /*SHELL TYPES
     * 0 = machine
     * 1 = handgun
     * 2 = shotgun
     * 3 = sniper
     * */
    public void SpitShell(Transform shellIndex, Vector3 shellSpeed, int shellType)
    {
        // instantiate
        GameObject shellClone = Instantiate(globalResources.GetShell(shellType),shellIndex.position, 
            shellIndex.rotation * Quaternion.Euler(new Vector3(Random.Range(-25f, 25f), Random.Range(-25f, 25f), Random.Range(-25f, 25f)))); // randomize rotation

        // erase after time
        Destroy(shellClone,0.3f);

        // randomize position
        if ( shellSpeed == Vector3.zero) shellSpeed = new Vector3(-9f * Random.Range(1f, 1.2f), 4f * Random.Range(1f, 2f), 2f * Random.Range(1f, 2f));
        else shellSpeed = new Vector3(shellSpeed.x * Random.Range(1f, 1.2f), shellSpeed.y * Random.Range(1f, 2f), shellSpeed.z * Random.Range(1f, 2f));

        // add force
        shellClone.GetComponent<Rigidbody>().AddRelativeForce(shellSpeed, ForceMode.Impulse);
    }
    public virtual void FireMuzzle()
    {
        // Override when needed
    }

    public void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (null == obj) return;
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (null == child) continue;           
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    internal Vector3 GetSpread(Vector2 spread)
    {
        return new Vector3(Random.Range(-spread.x, spread.x), Random.Range(-spread.y, spread.y), Random.Range(-spread.x, spread.x));
    }

    // same with sound option
    public void BulletTrail(Transform pos, Vector3 targetPoint, bool hasSound)
    {
        Transform clonet = Instantiate(bulletTrail, pos.position, pos.rotation);
        clonet.LookAt(targetPoint);
        clonet.gameObject.GetComponent<BulletTrail>().sound = hasSound;
        Destroy(clonet.gameObject, 1f);       
    }

    //custom narrator
    internal void CustomNarrator(string e)
    {
        if (transform.root.name != "Player") return;
        GameObject g = GameObject.Find("/Custom Narrator");
        if (g) g.SendMessage(e);
    }

    internal virtual void NetworkFire(RaycastHit targetPos)
    {
        // Override when needed
    }
}
