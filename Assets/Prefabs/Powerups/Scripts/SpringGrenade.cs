using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringGrenade : MonoBehaviour
{
    [SerializeField] private float jumpForce = 2.5f;

    public Sprite thumb;
    public string itemName;

    private bool isArmed = false;
    //private Rigidbody rigidBody;

    // Init
    void Start()
    {
        //rigidBody = GetComponent<Rigidbody>();
        StartCoroutine(SelfDestruct(10f));
    }

    // Collision
    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && isArmed)
        {
            // Explode grenade
            Vector3 forceVector = new Vector3(0f, jumpForce, 0f);
            other.gameObject.SendMessage("PowerUp", forceVector);
            Destroy(gameObject);
            other.gameObject.SendMessage("SetCanUsePowerUp", true);

            // Sound
            PlayerAudio audio = other.gameObject.GetComponent<PlayerAudio>();
            audio.Play(audio.spring);
        }
        else if(other.tag != "Player" && other.tag != "Powerup")
        {
            isArmed = true;
        }
    }



    IEnumerator SelfDestruct(float delay)
    {
        yield return new WaitForSeconds(delay);
        if(transform.position.y<-100f) Destroy(gameObject);
        else StartCoroutine(SelfDestruct(10f));
    }
}
