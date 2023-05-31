using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RespawnManager : MonoBehaviour
{
    public Text coutdown;
    public Button button;

    public int waitTime = 5;
    internal int currentTime = 0;

    public Text deathCauseMsg;


    void OnEnable()
    {
        button.gameObject.SetActive(false);
        coutdown.gameObject.SetActive(true);
        currentTime = waitTime;
        InvokeRepeating("Timer", 0, 1);
    }

    void Timer()
    {
        currentTime--;
        coutdown.text = "Respawn: " + currentTime;
        if (currentTime <= 0) EndCountdown();
        
    }

    void EndCountdown()
    {
        coutdown.gameObject.SetActive(false);
        button.gameObject.SetActive(true);
        CancelInvoke();
    }

    void SetDeathCause(string cause)
    {
        deathCauseMsg.text = cause;
    }
}
