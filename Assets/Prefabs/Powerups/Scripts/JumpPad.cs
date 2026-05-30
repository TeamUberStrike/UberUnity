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

    [Tooltip("Multiplier applied to forceDirection before sending to PlayerMotor. " +
             "Sends final velocity directly — no extra modifier applied by PlayerMotor.")]
    public float forceMultiplier = 10f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Vector3 finalVelocity;
            if (isAccel)
                finalVelocity = (transform.forward * accelMultiply + new Vector3(0f, forceDirection.y, 0f)) * forceMultiplier;
            else
                finalVelocity = forceDirection * forceMultiplier;

            // PowerUp receives final velocity — sets it directly
            other.gameObject.SendMessage("PowerUp", finalVelocity);
            if (audioSource != null) audioSource.Play(0);
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
