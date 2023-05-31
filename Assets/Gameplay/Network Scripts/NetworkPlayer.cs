using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkPlayer : MonoBehaviour
{
    public int networkId;
    public string networkName = "client";
    internal NetworkAvatar networkAvatar;

    private Client client;
    public Transform nameTag;
    public PickupWeapon drop;
    private GlobalResources globalResources;
    private GameObject enemyRagdoll;

    private PlayerData latestPlayerData;

    public Transform groundRayOrigin;
    public Transform raycastPivot;
    public Transform weaponHand;

    string currentGroundMaterial = "";
    float distanceToGround = 0f;
    bool inGround = false;
    bool swimming = false;
    bool crouch = false;

    PlayerAudio playerAudio;

    void Start()
    {
        // avatar
        networkAvatar = GetComponent<NetworkAvatar>();

        // nametag name
        nameTag.GetChild(0).gameObject.GetComponent<Text>().text = networkName;

        // get client
        client = GameObject.Find("/Network Client").GetComponent<Client>();

        // get global resources
        globalResources = GameObject.Find("/Global Resources").GetComponent<GlobalResources>();

        // log message
        client.log.LogMessage(networkName, " joined the game", "", false, false);

        // set obj link
        ((PlayerData)client.playerDatas[networkId]).playerObject = gameObject;

        //ragdoll
        enemyRagdoll = globalResources.enemyRagdoll;

        //audio
        playerAudio = GetComponent<PlayerAudio>();
    }

    void Update()
    {
        // Disconnect player
        if (latestPlayerData!=null&&latestPlayerData.destroy){
            client.log.LogMessage(networkName, " left the game", "", false, false);
            Destroy(gameObject);
            return;}

        //Die
        if (latestPlayerData != null && latestPlayerData.die) Die();

        //////////////////////////////////////////////////////
        // Get latest data ///////////////////////////////////
        latestPlayerData = client.GetLatestDatas(networkId);//
        //////////////////////////////////////////////////////
        
        // appereances & weapons
        if (latestPlayerData.appereancesChanged) LoadAppereances();
        if (latestPlayerData.weaponChanged) EquipWeapon();

        // fire
        if (latestPlayerData.pendingPrimaryFire) FireWeapon();
        if (latestPlayerData.pendingFinalWord) FinalWord();

        // death
        if (latestPlayerData.position.y!=-666) ((PlayerData)client.playerDatas[networkId]).isAlive = true;
        else ((PlayerData)client.playerDatas[networkId]).isAlive = false;

        // position
        transform.position = Vector3.Lerp(transform.position, latestPlayerData.position, 0.3f);

        // walk sound loop
        if (!swimming && inGround && Vector3.Distance(latestPlayerData.position, transform.position) > 0.05) playerAudio.PlayWalk(currentGroundMaterial);

        // rotation Y
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(latestPlayerData.rotation),0.3f);

        // rotation X
        raycastPivot.localRotation = Quaternion.Euler(latestPlayerData.subRotation);

        // nameTag
        nameTag.LookAt(2 * nameTag.position - client.playerCamera.position);
        nameTag.gameObject.SetActive(client.isAlive);

        // ground
        inGround = getDistanceToGround()>0.1f&&distanceToGround<0.2f;

        // crouch
        crouch = latestPlayerData.crouch;
    }

    public void TakeDamage(float amount, int criticalDmgCode, Vector3 position, string sourceWeapon)
    {
        Debug.Log("Dealed " + amount + " damage with " + sourceWeapon + " from " + position + ". critical code: " + criticalDmgCode);
        client.DamageDealedTo(networkId, amount, criticalDmgCode, position);
    }

    void Die()
    {
        ((PlayerData)client.playerDatas[networkId]).die = false;

        // drop weapon
        GameObject wPrefab = globalResources.weapons[((PlayerData)client.playerDatas[networkId]).weapon];
        PickupWeapon w = Instantiate(drop, latestPlayerData.position+(Vector3.up*2), Quaternion.identity);
        w.weaponPrefab = wPrefab;
        Destroy(w.gameObject, 8f);

        // corpse
        GameObject holo = (latestPlayerData.holo <= 0) ? null : globalResources.appereances[latestPlayerData.holo];
        GameObject head = (latestPlayerData.head <= 0) ? null : globalResources.appereances[latestPlayerData.head];
        GameObject face = (latestPlayerData.face <= 0) ? null : globalResources.appereances[latestPlayerData.face];
        GameObject upperbody = (latestPlayerData.upperbody <= 0) ? null : globalResources.appereances[latestPlayerData.upperbody];
        GameObject gloves = (latestPlayerData.gloves <= 0) ? null : globalResources.appereances[latestPlayerData.gloves];
        GameObject lowerbody = (latestPlayerData.lowerbody <= 0) ? null : globalResources.appereances[latestPlayerData.lowerbody];
        GameObject boots = (latestPlayerData.boots <= 0) ? null : globalResources.appereances[latestPlayerData.boots];

        GameObject cc = Instantiate(enemyRagdoll, latestPlayerData.position, Quaternion.Euler(latestPlayerData.rotation));
        cc.GetComponent<BodyBuilder>().BuildAppereances(holo, head, face, upperbody, gloves, lowerbody, boots);
        Destroy(cc,8f);

        //force
        if (((PlayerData)client.playerDatas[networkId]).lastKiller > 0)
        {
            Vector3 pos;
            if (client.GetLatestDatas(((PlayerData)client.playerDatas[networkId]).lastKiller) != null) pos = client.GetLatestDatas(((PlayerData)client.playerDatas[networkId]).lastKiller).position;
            else if (GameObject.Find("/Player")) pos = GameObject.Find("/Player").transform.position;
            else pos = transform.position;

                Rigidbody[] rbs = cc.GetComponentsInChildren<Rigidbody>();
                foreach (Rigidbody r in rbs) r.AddForce((transform.position - pos).normalized*6, ForceMode.Impulse);
            
        }  
    }

    void LoadAppereances()
    {
        ((PlayerData)client.playerDatas[networkId]).appereancesChanged = false;

        // get appereances
        GameObject holo = (latestPlayerData.holo<=0) ? null : globalResources.appereances[latestPlayerData.holo];
        GameObject head = (latestPlayerData.head <= 0) ? null : globalResources.appereances[latestPlayerData.head];
        GameObject face = (latestPlayerData.face <= 0) ? null : globalResources.appereances[latestPlayerData.face];
        GameObject upperbody = (latestPlayerData.upperbody <= 0) ? null : globalResources.appereances[latestPlayerData.upperbody];
        GameObject gloves = (latestPlayerData.gloves <= 0) ? null : globalResources.appereances[latestPlayerData.gloves];
        GameObject lowerbody = (latestPlayerData.lowerbody <= 0) ? null : globalResources.appereances[latestPlayerData.lowerbody];
        GameObject boots = (latestPlayerData.boots <= 0) ? null : globalResources.appereances[latestPlayerData.boots];

        // debug
        /*
        Debug.Log("Client '" + networkName + "' appereances received: \n\t" +
                ((holo) ? holo.name : "null") + ", \n\t" +
                ((head) ? head.name : "null") + ", \n\t" +
                ((face) ? face.name : "null") + ", \n\t" +
                ((upperbody) ? upperbody.name : "null") + ", \n\t" +
                ((lowerbody) ? lowerbody.name : "null") + ", \n\t" +
                ((gloves) ? gloves.name : "null") + ", \n\t" +               
                ((boots) ? boots.name:"null"));
        */

        // build avatar
        networkAvatar.BuildAppereances(holo, head, face, upperbody, gloves, lowerbody, boots);
    }

    void EquipWeapon()
    {
        ((PlayerData)client.playerDatas[networkId]).weaponChanged = false;

        // clear all
        for (int i = 0; i < weaponHand.childCount; i++)
            Destroy(weaponHand.GetChild(i).gameObject);

        // add current
        GameObject newWeapon = Instantiate(FixedAudio(globalResources.weapons[((PlayerData)client.playerDatas[networkId]).weapon]), weaponHand.position,weaponHand.rotation);
        Destroy(newWeapon.GetComponent<Animator>());
        newWeapon.transform.parent = weaponHand;
        newWeapon.transform.localRotation = Quaternion.Euler(0f, 90f, 90f);

        newWeapon.GetComponent<Weapon>().SetLayerRecursively(newWeapon, 0);
    }

    void FireWeapon()
    {
        //get hit point
        RaycastHit t = GetFireHitPoint();       

        ((PlayerData)client.playerDatas[networkId]).pendingPrimaryFire = false;

        // sound
        if(weaponHand.childCount>0)
        weaponHand.GetChild(0).gameObject.SendMessage("PlayFireSound");

        //muzzle
        if (weaponHand.childCount==0) return;
        Weapon w = weaponHand.GetChild(0).gameObject.GetComponent<Weapon>();
        w.FireMuzzle();
        if (t.point != Vector3.zero) w.NetworkFire(t);

    }

    //makes audiosource work with 3d distances
    internal GameObject FixedAudio(GameObject g)
    {
        if (g.GetComponent<AudioSource>() != null)
        {
            AudioSource a = g.GetComponent<AudioSource>();
            a.playOnAwake = false;
            a.spatialBlend = 1;
            a.maxDistance = 45;
            a.volume = 0.7f;
            a.minDistance = 0;
            a.rolloffMode = AudioRolloffMode.Linear;
        }

        return g;
    }

    RaycastHit GetFireHitPoint()
    {
        RaycastHit hit;
        if (Physics.Raycast(raycastPivot.position, raycastPivot.forward, out hit))
        {
            return hit;
        }

        return hit;
    }

    float getDistanceToGround()
    {
        // Get closest ground around player
        float closest = -1111;
        
            RaycastHit hit;

            Ray downRay = new Ray(groundRayOrigin.position, -Vector3.up);
            Debug.DrawRay(transform.position, -Vector3.up, Color.green); // draw rays

            if (Physics.Raycast(downRay, out hit))
            {
                if (closest == -1111) { closest = hit.distance; }
                else if (hit.distance < closest) { closest = hit.distance; }

                //material
                if (hit.transform.gameObject.GetComponent<Terrain>()) currentGroundMaterial = "grass";
                else if (hit.transform.gameObject.GetComponent<Renderer>()) currentGroundMaterial = hit.transform.gameObject.GetComponent<Renderer>().material.name.Trim().ToLower();
            }
        
        distanceToGround = closest;
        return distanceToGround;
    }

    //detonate final word/kiss grenades
    internal void FinalWord()
    {
        ((PlayerData)client.playerDatas[networkId]).pendingFinalWord = false;
        if (weaponHand.childCount > 0)
            if (weaponHand.GetChild(0).gameObject.GetComponent<Weapon>())
                if (weaponHand.GetChild(0).gameObject.name.ToLower().Contains("final"))
                    weaponHand.GetChild(0).gameObject.SendMessage("DetonateFinalWord");
    }
}
