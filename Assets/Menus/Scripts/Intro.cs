using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class Intro : MonoBehaviour
{
    private VideoPlayer video;

    void Start()
    {
        video = GetComponent<VideoPlayer>();
        video.Play();
        video.loopPointReached += CheckOver;
    }

    void CheckOver(UnityEngine.Video.VideoPlayer vP)
    {
        StartCoroutine(LoadMenu());
    }

    void Update()
    {
        if (Input.anyKey)
        {
            StartCoroutine(LoadMenu());
        }
    }

    IEnumerator LoadMenu()
    {
        video.enabled = false;
        yield return new WaitForSeconds(1.2f);       
        SceneManager.LoadScene("MainMenu"); 
    }
}
