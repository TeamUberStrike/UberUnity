using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RavenDoorUnit : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" || other.tag == "Enemy")
        {
            animator.SetBool("isOpen",true);
        }
        
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" || other.tag == "Enemy")
        {
            animator.SetBool("isOpen", false);
        }

    }
}
