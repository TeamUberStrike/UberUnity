using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : MonoBehaviour
{
    AudioSource audioSource;
    private float volume = 0.7f;

    public AudioClip[] narrator;
    public GameObject[] badges;
    public GameObject specialBadge;

    public AudioClip switchWeapon;
    public AudioClip zoomIn;
    public AudioClip zoomOut;
    public AudioClip scopeIn;
    public AudioClip scopeOut;
    public AudioClip outOfAmmo;
    public AudioClip ammoPickup;

    public AudioClip takenTheLead;
    public AudioClip lostTheLead;

    public AudioClip spring;
    public AudioClip springRefresh;

    public AudioClip healthEnergize;
    public AudioClip healthDecrease;
    public AudioClip selfDmgNoArmor;
    public AudioClip selfDmgLowHp;
    public AudioClip selfDmgArmor;

    public AudioClip weaponPickup;
    public AudioClip headshotBell;
    public AudioClip nutshotKong;

    public AudioClip landing;

    public AudioClip[] footSteps;
    private Hashtable steps = new Hashtable();
    Coroutine canWalk;
    bool canPlayWalk = true;
    string[] gTypes = { "dirt", "glass", "heavymetal", "metal", "grass", "rock", "sand", "snow", "water", "wood" };

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(narrator[0], volume);

        // build footStep map
        foreach(AudioClip a in footSteps)
        {
            steps.Add(a.name.Trim().ToLower(),a);
        }

        // Custom narrator
        if (GameObject.Find("/Custom Narrator")) Destroy(GameObject.Find("/Custom Narrator"));
        if (PlayerPrefs.HasKey("equipped_head"))
        {
            //laitela
            if (PlayerPrefs.GetString("equipped_head") == "Paperbag laitela")
            {                
                CustomNarrator c=Instantiate(GameObject.Find("/Global Resources").GetComponent<GlobalResources>().laitelaNarrator);
                c.gameObject.name = "Custom Narrator";
                
            }

            //taalasmaa
            if (PlayerPrefs.GetString("equipped_head") == "Paperbag taalasmaa")
            {
                CustomNarrator c = Instantiate(GameObject.Find("/Global Resources").GetComponent<GlobalResources>().taalasmaaNarrator);
                c.gameObject.name = "Custom Narrator";

            }
        }
    }

    // Call this to play sounds elsewhere
    public void Play(AudioClip clip)
    {
        if(audioSource.enabled)
        audioSource.PlayOneShot(clip, volume);
    }

    //Play out of ammo sound
    void OutOfAmmoSound()
    {
        Play(outOfAmmo);
    }

    IEnumerator CanWalk()
    {
        canPlayWalk = false;
        yield return new WaitForSeconds(0.5f);
        canPlayWalk = true;
    }

    // footsteps
    public void PlayWalk(string groundMaterial)
    {
        if (!canPlayWalk) return;        
        if(canWalk!=null) StopCoroutine(canWalk);
        canWalk = StartCoroutine(CanWalk());

        
        string foundType = "rock"; // default is rock

        // find type
        foreach (string s in gTypes) if (groundMaterial.Contains(s)) { foundType = s; break; }

        // get clip count
        int i = 1;
        while (steps.Contains(foundType + i)) i++;
        i--;

        int randomIndex = Random.Range(1, i);
        if (steps.Contains(foundType + randomIndex)) Play((AudioClip)steps[foundType + randomIndex]);
        else Debug.LogWarning("footstep sound index out of bounds");
    }

    // footsteps landing
    public void PlayLanding(string groundMaterial)
    {
        if (audioSource.isPlaying) return;
        if (canWalk != null) StopCoroutine(canWalk);
        canWalk = StartCoroutine(CanWalk());

        string foundType = "rock"; // default is rock

        // find type
        foreach (string s in gTypes) if (groundMaterial.Contains(s)) { foundType = s; break; }

        // get clip count
        int i = 1;
        while(steps.Contains(foundType + i)) i++;
        i--;

        int randomIndex = Random.Range(1,i); 
        if (steps.Contains(foundType+randomIndex)) Play((AudioClip)steps[foundType + randomIndex]);
        else Debug.LogWarning("footstep sound index out of bounds");
    }


    public void Badges(int combo)
    {
        switch (combo)
        {
            // double kill
            case 2: StartCoroutine(ShowBadge(badges[0], narrator[10], true));
                break;

            // triple kill
            case 3: StartCoroutine(ShowBadge(badges[1], narrator[9], true));
                break;

            // quad kill
            case 4: StartCoroutine(ShowBadge(badges[2], narrator[8], true));
                break;

            // mega kill
            case 5: StartCoroutine(ShowBadge(badges[3], narrator[7], true));
                break;

            // uber kill
            case 6: StartCoroutine(ShowBadge(badges[4], narrator[6], true));              
                break;
        }
    }

    public void MeleeBadge()
    {
        specialBadge.GetComponent<Text>().text = "Smackdown";
        StartCoroutine(ShowBadge(specialBadge, narrator[23], false));
    }

    public void SpecialBadge(bool headShot)
    {
        if (headShot)
        {
            Play(headshotBell);
            //
            specialBadge.GetComponent<Text>().text = "Headshot";
            StartCoroutine(ShowBadge(specialBadge, narrator[14], false));
        }
        else // nutshot
        {
            Play(nutshotKong);
            //
            specialBadge.GetComponent<Text>().text = "Nutshot";
            StartCoroutine(ShowBadge(specialBadge, narrator[21], false));
        }
    }
    
    // param bool interrupt -> whether to wait current badge to finish or override it
    IEnumerator ShowBadge(GameObject badge, AudioClip sound, bool interrupt)
    {
        if (interrupt)
        {
            //hide all
            for (int i = 0; i < badge.transform.parent.childCount; i++)
                badge.transform.parent.GetChild(i).gameObject.SetActive(false);

            //show
            badge.SetActive(true);
            audioSource.Stop();          
        }
        else
        {
            yield return new WaitUntil(() => !audioSource.isPlaying);
            badge.SetActive(true);
        }

        Play(sound);
        badge.GetComponent<Animation>().Play();
        yield return new WaitUntil(() => !audioSource.isPlaying);

        GetComponent<PlayerUI>().ShowKillName("");
    }

    void HasTheLead(bool hasIt)
    {
        if (hasIt) Play(takenTheLead);
        else Play(lostTheLead);
    }

}
