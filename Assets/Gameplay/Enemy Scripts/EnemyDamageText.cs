using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDamageText : MonoBehaviour
{
    private Transform lookTarget;
    public Text textElem;

    // Start is called before the first frame update
    void Start()
    {
        lookTarget = GameObject.Find("/Player").transform;
        transform.LookAt(2 * transform.position - lookTarget.position);

        float scale = Vector3.Distance(lookTarget.position, transform.position) / 21f;
        transform.localScale = new Vector3(scale, scale, scale);
    }

    // Update is called once per frame
    void Update()
    {
        if(lookTarget)
        transform.LookAt(2*transform.position-lookTarget.position);
        transform.Translate(Vector3.up * 0.3f * Time.deltaTime);
        
    }

    public void SetText(string text)
    {
        textElem.text = text;
    }


}
