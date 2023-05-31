using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupWeapon : MonoBehaviour
{
    public GameObject weaponPrefab;
    public float respawnTime = 40f;
    public bool oneTimeOnly = false;
    Rigidbody rb;
    private bool available = true;

    void Start()
    {
        GameObject clone = Instantiate(weaponPrefab);
        clone.transform.parent = transform;

        Vector3 objCenter = Vector3.zero;
        if (clone.GetComponent<Renderer>()!=null) objCenter = clone.GetComponent<Renderer>().bounds.center;

        clone.transform.localPosition = new Vector3(0f,0f,-objCenter.z);
        rb = GetComponent<Rigidbody>();

        Destroy(clone.GetComponent<Animator>());
        SetLayerRecursively(clone, 0);
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (null == obj)
        {
            return;
        }

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (null == child)
            {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (available)
        {
            if (collision.gameObject.tag == "Player")
            {
                foreach (Transform child in transform) child.gameObject.SetActive(false);
                available = false;
                StartCoroutine(Respawn());
                collision.gameObject.SendMessage("WeaponPickup", weaponPrefab.name + "");
                rb.isKinematic = true;
            }
            else if (collision.gameObject.tag == "Enemy")
            {
                foreach (Transform child in transform) child.gameObject.SetActive(false);
                available = false;
                StartCoroutine(Respawn());
                rb.isKinematic = true;
            }

        }
    }

    IEnumerator Respawn()
    {
        if (oneTimeOnly) Destroy(gameObject);
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(respawnTime);
        available = true;
        GetComponent<Collider>().enabled = true;
        rb.isKinematic = false;

        foreach (Transform child in transform) child.gameObject.SetActive(true);
    }

}
