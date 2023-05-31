using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ServerList : MonoBehaviour
{
    public string url = "minidesk.dy.fi/api/uberstrike/get-ip.php";
    Coroutine post;

    /* 0 img
     * 1 name
     * 2 map -mode
     * 3 playercount
     */
    public GameObject listItemPrefab;

    void OnEnable()
    {
        RefeshServerList();
    }

    public void RefeshServerList()
    {
        if (post != null) StopCoroutine(post);
        post = StartCoroutine(PostRequest());
    }

    IEnumerator PostRequest()
    {
        // form
        WWWForm form = new WWWForm();
        form.AddField("region", 0);

        // request
        UnityWebRequest www = UnityWebRequest.Post(url, form);
        www.chunkedTransfer = false;

        // do async
        yield return www.SendWebRequest();

        // receive
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogWarning("Serverlist: " + www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
        }
    }


}
