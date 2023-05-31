using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Transform transportTargetPos;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (transportTargetPos)
            {
                other.transform.position = transportTargetPos.position;
                //GetComponent<AudioSource>().Play();
            }
        }

    }
}
