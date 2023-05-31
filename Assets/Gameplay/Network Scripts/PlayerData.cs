using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    // Identity
    public int id;
    public string name = "not set";
    public GameObject playerObject; 

    // Transform
    public Vector3 position = Vector3.zero;
    public Vector3 rotation = Vector3.zero;
    public Vector3 subRotation = Vector3.zero;

    //crouch
    public bool crouch = false;

    public bool destroy = false;
    public bool die = false;
    public bool isAlive = false;

    // stats
    public float kills = 0;
    public float deaths = 0;

    // Current weapon ID
    public bool weaponChanged = false;
    public int weapon = 1;

    // appereances
    public bool appereancesChanged = false;
    public int holo = -1;
    public int head = -1;
    public int face = -1;
    public int gloves = -1;
    public int upperbody = -1;
    public int lowerbody = -1;
    public int boots = -1;

    public int lastKiller = 0;

    // Weapon shoot
    public bool pendingPrimaryFire = false;

    public bool pendingFinalWord = false;

    public PlayerData(string name)
    {
        this.name = name;
    }
}