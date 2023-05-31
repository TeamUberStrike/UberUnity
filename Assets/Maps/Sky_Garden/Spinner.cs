using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{

    public float speed = 1f;
    public Vector3 direction = Vector3.up;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(direction*speed*Time.deltaTime);
    }
}
