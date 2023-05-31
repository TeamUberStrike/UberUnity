using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{
    public Material skybox;
    public bool mapHasWater = false;
    public float gravity = -16.99f;

    void Start()
    {
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("quality"), true);
        Physics.gravity = new Vector3(0f, gravity, 0f);
    }
}
