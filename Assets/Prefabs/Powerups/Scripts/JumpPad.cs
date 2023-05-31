using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class JumpPad : MonoBehaviour
{
    [SerializeField] private Vector3 forceDirection = Vector3.zero;

    AudioSource audioSource;
    public bool isAccel = false;
    public float accelMultiply = 5f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (isAccel) other.gameObject.SendMessage("PowerUp",(transform.forward*accelMultiply)+new Vector3(0f,forceDirection.y,0f));
            else other.gameObject.SendMessage("PowerUp", forceDirection);
            audioSource.Play(0);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            other.gameObject.SendMessage("SetCanUsePowerUp", true);
        }
    }
}
