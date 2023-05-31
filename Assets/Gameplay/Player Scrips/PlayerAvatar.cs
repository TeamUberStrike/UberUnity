using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAvatar : MonoBehaviour
{
    //nametag
    public GameObject nameTag;
    Transform nameTagTargetCamera;

    // appereances prefab
    public GameObject appereances;
    public GameObject lutzDefaultHat;

    //skeleton transforms
    internal Transform headJoint;
    internal Transform rightHandJoint;
    public GameObject charachter;
    private Transform charachterMeshes;

    

    //whitelist = objects not to remove 
    List<GameObject> whiteList = new List<GameObject>();

    private Animator animator;

    void Start()
    {
        nameTagTargetCamera = GameObject.Find("/MenuSystem/Main Camera").transform;
        UpdateNameTag();
        LoadAppereances();
    }

    void Update()
    {
        nameTag.transform.LookAt(2 * nameTag.transform.position - nameTagTargetCamera.position);
    }

    void UpdateNameTag()
    {
        if (PlayerPrefs.HasKey("username"))
        {
            nameTag.GetComponentInChildren<Text>().text = PlayerPrefs.GetString("username");
        }
    }

    public void LoadAppereances()
    {
        //load
        GameObject holo = Resources.Load(PlayerPrefs.GetString("equipped_holo"), typeof(GameObject)) as GameObject;
        GameObject head = Resources.Load(PlayerPrefs.GetString("equipped_head"), typeof(GameObject)) as GameObject;
        GameObject face = Resources.Load(PlayerPrefs.GetString("equipped_face"), typeof(GameObject)) as GameObject;
        GameObject gloves = Resources.Load(PlayerPrefs.GetString("equipped_gloves"), typeof(GameObject)) as GameObject;
        GameObject upperBody = Resources.Load(PlayerPrefs.GetString("equipped_upperbody"), typeof(GameObject)) as GameObject;
        GameObject lowerBody = Resources.Load(PlayerPrefs.GetString("equipped_lowerbody"), typeof(GameObject)) as GameObject;
        GameObject boots = Resources.Load(PlayerPrefs.GetString("equipped_boots"), typeof(GameObject)) as GameObject;

        // Update armor
        float gainedArmor = 0f;
        if (upperBody) gainedArmor += upperBody.GetComponent<Appereance>().armorGain;
        if (lowerBody) gainedArmor += lowerBody.GetComponent<Appereance>().armorGain;
        PlayerPrefs.SetFloat("gained_armor",gainedArmor);

        // Clear charachter
        foreach(Transform child in charachter.transform)
        {
            Destroy(child.gameObject);
        }

        // assign to avatar
        if (holo)
        {
            // load holo
            GameObject holoClone = Instantiate(holo.GetComponent<Appereance>().physicalItem, charachter.transform.position, charachter.transform.rotation);
            holoClone.transform.parent = charachter.transform;
            animator = holoClone.GetComponent<Animator>();

            // find transforms in skeleton
            Transform[] allChildren = charachter.GetComponentsInChildren<Transform>();
            foreach (Transform t in allChildren)
            {
                if (t.name == "QuickRigCharacter_Head")
                {
                    headJoint = t;
                }
                else if (t.name == "QuickRigCharacter_RightHand")
                {
                    rightHandJoint = t;
                }
            }
        }
        else
        {
            // load appereances prefab
            GameObject appClone = Instantiate(appereances, charachter.transform.position, charachter.transform.rotation);
            appClone.transform.parent = charachter.transform;
            animator = appClone.GetComponent<Animator>();

            // find transforms in skeleton
            Transform[] allChildren = charachter.GetComponentsInChildren<Transform>();
            foreach (Transform t in allChildren)
            {
                if (t.name == "QuickRigCharachter_Mesh")
                {
                    charachterMeshes = t;
                }
                else if (t.name == "QuickRigCharacter_Head")
                {
                    headJoint = t;
                }
                else if (t.name == "QuickRigCharacter_RightHand")
                {
                    rightHandJoint = t;
                }              
            }
                              
            // face
            if (face)
            {
                if (face.GetComponent<Appereance>().dontHideDefaultFace)
                {
                    whiteList.Add(GetPartFromSet("head", "lutz"));
                }

                // if face is physical
                if (face.GetComponent<Appereance>().physicalItem != null)
                {
                    BindToHead(face.GetComponent<Appereance>().physicalItem);
                }

                else whiteList.Add(GetPartFromSet("face", face.GetComponent<Appereance>().multipartSetName));
            }

            else whiteList.Add(GetPartFromSet("head","lutz"));

            // head
            if (head)
            {
                // if head is physical
                if (head.GetComponent<Appereance>().physicalItem != null)
                {
                    BindToHead(head.GetComponent<Appereance>().physicalItem);
                }

                else whiteList.Add(GetPartFromSet("head", head.GetComponent<Appereance>().multipartSetName));

            }
            else {
                BindToHead(lutzDefaultHat);
                whiteList.Add(GetPartFromSet("head", "lutz"));
            }
               
            //gloves
            if (gloves)
            {
                whiteList.Add(GetPartFromSet("gloves", gloves.GetComponent<Appereance>().multipartSetName));
            }else whiteList.Add(GetPartFromSet("gloves", "lutz"));
            
            // upperbody
            if (upperBody)
            {
                whiteList.Add(GetPartFromSet("upperbody", upperBody.GetComponent<Appereance>().multipartSetName));
            }else whiteList.Add(GetPartFromSet("upperbody", "lutz"));
            
            // lowerbody
            if (lowerBody)
            {
                whiteList.Add(GetPartFromSet("lowerbody", lowerBody.GetComponent<Appereance>().multipartSetName));
            }else whiteList.Add(GetPartFromSet("lowerbody", "lutz"));
            
            //boots
            if (boots)
            {
                whiteList.Add(GetPartFromSet("boots", boots.GetComponent<Appereance>().multipartSetName));
            } else whiteList.Add(GetPartFromSet("boots", "lutz"));

            PurgeAppereances();
            animator.Play("idle");
        }

    }

    // Returns gameobject from set in appereances
    private GameObject GetPartFromSet(string partName,string setName)
    {
        for (int i = 0; i < charachterMeshes.childCount; i++)
        {
            if (charachterMeshes.GetChild(i).name==setName)
            for (int j = 0; j < charachterMeshes.GetChild(i).childCount; j++)
            {
                if (charachterMeshes.GetChild(i).GetChild(j).name.Contains(partName)) return charachterMeshes.GetChild(i).GetChild(j).gameObject;
            }
        }

        //prevent infinite loop
        if (setName == "lutz")
        {
            print("infinite loop was going to occure at " + setName + ", " + partName);
            return new GameObject();
        }
        
        //default lutz if no part was found
        return GetPartFromSet(partName, "lutz");
    }


    // Remove all parts that are not in whitelist
    private void PurgeAppereances()
    {      
        for(int i = 0; i < charachterMeshes.childCount; i++)
        {
            for (int j = 0; j < charachterMeshes.GetChild(i).childCount; j++)
            {
                if (!whiteList.Contains(charachterMeshes.GetChild(i).GetChild(j).gameObject)) Destroy(charachterMeshes.GetChild(i).GetChild(j).gameObject); //charachterMeshes.GetChild(i).GetChild(j).gameObject.SetActive(false);
            }
        }

        whiteList.Clear();
    }

    private void BindToHead(GameObject prefab)
    {
        GameObject fixedClone = Instantiate(prefab);
        fixedClone.transform.parent = headJoint;
        fixedClone.transform.localRotation = Quaternion.Euler(-106f,90f,0f);
        fixedClone.transform.localPosition = Vector3.zero;
        whiteList.Add(fixedClone);
    }

    internal void EquipWeapon(GameObject weapon)
    {
        //melee pose
        if (weapon.GetComponent<Melee>() != null || weapon.GetComponent<Handgun>() != null) animator.SetBool("isMelee", true);

        // default pose
        else animator.SetBool("isMelee", false);
        
        animator.Play("equip_pose");
    }
}
