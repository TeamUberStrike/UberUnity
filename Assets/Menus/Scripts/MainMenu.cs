using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    AudioSource radio;
    public GameObject fadeIn;
    public AudioClip[] tracks;

    void Start()
    {
        radio = GetComponent<AudioSource>();
        StartCoroutine(FadeIn());
        PlayMusic();

    }

    void PlayMusic()
    {
        int randClip = Random.Range(0, tracks.Length);
        radio.clip = tracks[randClip];
        radio.Play();
        StartCoroutine(Replay());
    }

    IEnumerator Replay()
    {
        yield return new WaitUntil(() => radio.isPlaying == false);
        PlayMusic();
    }

    IEnumerator FadeIn()
    {
        fadeIn.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        fadeIn.SetActive(false);
    }

    
}
