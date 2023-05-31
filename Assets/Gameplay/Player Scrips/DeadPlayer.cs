using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadPlayer : MonoBehaviour
{
    public new Transform camera;
    public Transform ragDoll;
    BodyBuilder bodyBuilder;
    private Transform cameraTarget;
    AudioSource audioSource;
    public AudioClip[] dyingSounds;
    public Transform weapon;
    private Transform killer;
    Vector3 force;

    void Start()
    {
        gameObject.name = "Dead Player";
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(dyingSounds[Random.Range(0,dyingSounds.Length-1)],0.7f);
        StartCoroutine(SwapCameraTarget());
        cameraTarget = ragDoll;
        bodyBuilder = GetComponent<BodyBuilder>();
        SetAppereances();
    }

    void SetAppereances()
    {
        GameObject holo = Resources.Load(PlayerPrefs.GetString("equipped_holo"), typeof(GameObject)) as GameObject;
        GameObject head = Resources.Load(PlayerPrefs.GetString("equipped_head"), typeof(GameObject)) as GameObject;
        GameObject face = Resources.Load(PlayerPrefs.GetString("equipped_face"), typeof(GameObject)) as GameObject;
        GameObject gloves = Resources.Load(PlayerPrefs.GetString("equipped_gloves"), typeof(GameObject)) as GameObject;
        GameObject upperBody = Resources.Load(PlayerPrefs.GetString("equipped_upperbody"), typeof(GameObject)) as GameObject;
        GameObject lowerBody = Resources.Load(PlayerPrefs.GetString("equipped_lowerbody"), typeof(GameObject)) as GameObject;
        GameObject boots = Resources.Load(PlayerPrefs.GetString("equipped_boots"), typeof(GameObject)) as GameObject;

        bodyBuilder.BuildAppereances(holo, head, face, upperBody, gloves, lowerBody, boots);
    }

    void SetWeapon(int weaponId)
    {
        GameObject wPrefab = GameObject.Find("/Global Resources").GetComponent<GlobalResources>().weapons[weaponId];
        GameObject w = Instantiate(wPrefab, weapon.position, weapon.rotation);
        Destroy(w.GetComponent<Animator>());
        Destroy(w.GetComponent<Weapon>());
        w.transform.parent = weapon;
    }
    
    void Update()
    {
        if (cameraTarget == null) return;
        if (cameraTarget.position.y>-600f) { 
            Quaternion targetRotation = Quaternion.LookRotation(cameraTarget.position - camera.position);
            camera.rotation = Quaternion.Slerp(camera.rotation, targetRotation, 5 * Time.deltaTime);
        }
    }
    
    void SetKiller(GameObject killer)
    {
        this.killer = killer.transform;
    }

    void StartVelocity(Vector3 v)
    {
        force = v;
    }

    internal void Launch()
    {
        Rigidbody[] rbs = gameObject.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody r in rbs)
        r.AddForce(force, ForceMode.Impulse);
    }

    IEnumerator SwapCameraTarget()
    {
        yield return new WaitForSeconds(3f);
        if(killer) cameraTarget = killer;
    }

    internal void SetCamTarget(Transform t)
    {
        cameraTarget = t;
    }
}
