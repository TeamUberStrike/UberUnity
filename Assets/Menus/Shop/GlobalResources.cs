using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GlobalResources : MonoBehaviour
{
    // weapons and appereances
    public List<GameObject> weapons = new List<GameObject>();
    public List<GameObject> appereances = new List<GameObject>();

    // appereances prefab
    public GameObject appereancesPrefab;
    public GameObject lutzDefaultHatPrefab;

    // hitmark
    public Transform hitmarkEnemy;
    public Transform bulletTrail;

    public Transform defaultMuzzle;
    public GameObject enemyRagdoll;

    //bullethit sounds
    public AudioClip[] bulletHitSounds;
    private Hashtable hits = new Hashtable();

    public GameObject woodBulletEffect;
    public GameObject stoneBulletEffect;
    public GameObject sandBulletEffect;
    public GameObject metalBulletEffect;

    public GameObject[] shells;


    // for paperbag laitela -mask like SFX
    public CustomNarrator laitelaNarrator;
    public CustomNarrator taalasmaaNarrator;

    public AudioClip GetBulletHitSound(string materialType)
    {
        string[] gTypes = {"wood", "water", "stone", "sand", "metal", "grass", "glass", "cement" };
        string foundType = "cement"; // default is cement

        // find type
        foreach (string s in gTypes) if (materialType.Contains(s)) { foundType = s; break; }

        // get clip count
        int i = 1; 
        while (hits.Contains(foundType + i)) i++; i--;

        int randomIndex = Random.Range(1, i);
        if (hits.Contains(foundType + randomIndex)) return (AudioClip)hits[foundType + randomIndex];
        else Debug.LogWarning("hit sound index out of bounds");

        return null;
    }

    // init
    void Awake()
    {
        //DontDestroyOnLoad(this.gameObject);
        LoadResources();

        // build bullethit map
        foreach (AudioClip a in bulletHitSounds)
        {
            string s = a.name.Trim().ToLower().Replace("impact", "").Replace("-sharedassets0.assets", "") ;
            s = s.Substring(0,s.Length-4);
            hits.Add(s, a);
        }
    }

    void LoadResources()
    {
        GameObject[] resources = Resources.LoadAll("/", typeof(GameObject)).Cast<GameObject>().ToArray();

        foreach (GameObject g in resources)
        {
            if (g.GetComponent<Weapon>() != null) weapons.Add(g);          
            else if (g.GetComponent<Appereance>() != null) appereances.Add(g);           

        }
    }

    public int GetWeaponId(string weaponName)
    {
        for(int i = 0; i < weapons.Count; i++)
        {
            if (weapons[i].GetComponent<Weapon>().GetName() == weaponName) return i;
        }

        //default case
        return 1;
    }

    public GameObject GetShell(int type)
    {
        return shells[type];
    }
}
