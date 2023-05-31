using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyBuilder : MonoBehaviour
{
    public Transform ragdollTemplate;

    GlobalResources globalResources;

    // appereances prefab
    private GameObject appereances;
    private GameObject lutzDefaultHat;

    //skeleton transforms
    internal Transform headJoint;
    internal Transform rightHandJoint;
    public GameObject charachter;
    private Transform charachterMeshes;

    List<GameObject> whiteList = new List<GameObject>();

    public void BuildAppereances(GameObject holo, GameObject head, GameObject face, GameObject upperBody, GameObject gloves, GameObject lowerBody, GameObject boots)
    {
        globalResources = GameObject.Find("/Global Resources").GetComponent<GlobalResources>();
        appereances = globalResources.appereancesPrefab;
        lutzDefaultHat = globalResources.lutzDefaultHatPrefab;

        // Clear charachter
        foreach (Transform child in charachter.transform)
            Destroy(child.gameObject);

        // assign to avatar
        if (holo)
        {
            // load holo
            GameObject holoClone = Instantiate(holo.GetComponent<Appereance>().physicalItem, charachter.transform.position, charachter.transform.rotation);
            holoClone.transform.parent = charachter.transform;
            Destroy(holoClone.GetComponent<Animator>());

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
            }

            BindPhysics();
        }
        else
        {
            // load appereances prefab
            appereances = globalResources.appereancesPrefab;
            GameObject appClone = Instantiate(appereances, charachter.transform.position, charachter.transform.rotation);
            appClone.transform.parent = charachter.transform;
            Destroy(appClone.GetComponent<Animator>());

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

            else whiteList.Add(GetPartFromSet("head", "lutz"));

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
            else
            {
                BindToHead(lutzDefaultHat);
                whiteList.Add(GetPartFromSet("head", "lutz"));
            }

            //gloves
            if (gloves)
            {
                whiteList.Add(GetPartFromSet("gloves", gloves.GetComponent<Appereance>().multipartSetName));
            }
            else whiteList.Add(GetPartFromSet("gloves", "lutz"));

            // upperbody
            if (upperBody)
            {
                whiteList.Add(GetPartFromSet("upperbody", upperBody.GetComponent<Appereance>().multipartSetName));
            }
            else whiteList.Add(GetPartFromSet("upperbody", "lutz"));

            // lowerbody
            if (lowerBody)
            {
                whiteList.Add(GetPartFromSet("lowerbody", lowerBody.GetComponent<Appereance>().multipartSetName));
            }
            else whiteList.Add(GetPartFromSet("lowerbody", "lutz"));

            //boots
            if (boots)
            {
                whiteList.Add(GetPartFromSet("boots", boots.GetComponent<Appereance>().multipartSetName));
            }
            else whiteList.Add(GetPartFromSet("boots", "lutz"));

            PurgeAppereances();
            BindPhysics();
            
        }
    }

    // Returns gameobject from set in appereances
    private GameObject GetPartFromSet(string partName, string setName)
    {
        for (int i = 0; i < charachterMeshes.childCount; i++)
        {
            if (charachterMeshes.GetChild(i).name == setName)
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
        for (int i = 0; i < charachterMeshes.childCount; i++)
        {
            for (int j = 0; j < charachterMeshes.GetChild(i).childCount; j++)
            {
                if (!whiteList.Contains(charachterMeshes.GetChild(i).GetChild(j).gameObject)) Destroy(charachterMeshes.GetChild(i).GetChild(j).gameObject);
            }
        }

        whiteList.Clear();
    }

    private void BindToHead(GameObject prefab)
    {
        GameObject fixedClone = Instantiate(prefab);
        fixedClone.transform.parent = headJoint;
        fixedClone.transform.localRotation = Quaternion.Euler(-106f, 90f, 0f);
        fixedClone.transform.localPosition = Vector3.zero;
        whiteList.Add(fixedClone);
    }

    GameObject GetObj(string childObj)
    {
        Transform[] allChildren = charachter.GetComponentsInChildren<Transform>();
        foreach (Transform t in allChildren)
        {
            if (t.name == childObj)
            {
                return t.gameObject;
            }
        }

        return null;
    }

    void BindPhysics()
    {
        if (GetComponent<DeadPlayer>() != null) GetComponent<DeadPlayer>().SetCamTarget(headJoint.parent.parent.parent);

        Rigidbody[] rigidBodies = ragdollTemplate.gameObject.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody r in rigidBodies)
        {
            string name = r.gameObject.name;
            //GetObj(name).AddComponent<Rigidbody>(r);
            GetObj(name).AddComponent<Rigidbody>();
        }
        
        CharacterJoint[] joints = ragdollTemplate.gameObject.GetComponentsInChildren<CharacterJoint>();
        foreach (CharacterJoint c in joints)
        {
            string name = c.gameObject.name;
            CharacterJoint j = GetObj(name).AddComponent<CharacterJoint>(c);
            j.connectedBody = j.transform.parent.GetComponentInParent<Rigidbody>();
        }

        CapsuleCollider[] colliders = ragdollTemplate.gameObject.GetComponentsInChildren<CapsuleCollider>();
        foreach (CapsuleCollider c in colliders)
        {
            string name = c.gameObject.name;
            GetObj(name).AddComponent<CapsuleCollider>(c);
        }

        SphereCollider[] scolliders = ragdollTemplate.gameObject.GetComponentsInChildren<SphereCollider>();
        foreach (SphereCollider c in scolliders)
        {
            string name = c.gameObject.name;
            GetObj(name).AddComponent<SphereCollider>(c);
        }

        BoxCollider[] bcolliders = ragdollTemplate.gameObject.GetComponentsInChildren<BoxCollider>();
        foreach (BoxCollider c in bcolliders)
        {
            string name = c.gameObject.name;
            GetObj(name).AddComponent<BoxCollider>(c);
        }


        if (GetComponent<DeadPlayer>() != null) GetComponent<DeadPlayer>().Launch();
    }
}

