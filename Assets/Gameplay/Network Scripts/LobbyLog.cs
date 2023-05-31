using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyLog : MonoBehaviour
{
    // game log
    public GameObject logItemPrefab;
    public Color outlineBlue;
    public Color outlineRed;
    public Color outlineBlack;

    // chat
    public InputField field;
    public GameObject chatPanel;
    internal bool chatActive;
    public Text chatItemPrefab;
    public Transform chatContent;
    Coroutine hideChat;
    public Scrollbar scroll;

    public void LogMessage(string entrySubject, string entryEvent, string entryObject, bool friendlySubject, bool friendlyObject)
    {
        // instantiate
        GameObject logItemClone = Instantiate(logItemPrefab);
        logItemClone.transform.SetParent(transform, false);
        Destroy(logItemClone, 4.5f);

        // get parts
        GameObject sbj = logItemClone.transform.GetChild(0).gameObject; // subject
        GameObject vnt = logItemClone.transform.GetChild(1).gameObject; // event
        GameObject obj = logItemClone.transform.GetChild(2).gameObject; // object

        //set texts
        sbj.GetComponent<Text>().text = entrySubject;
        vnt.GetComponent<Text>().text = entryEvent;
        obj.GetComponent<Text>().text = entryObject;

        //set colors
        //if (!friendlySubject) sbj.GetComponent<Outline>().effectColor = outlineRed;
        //else if (friendlySubject == null) sbj.GetComponent<Outline>().effectColor = outlineBlack;
        //if (friendlyObject) obj.GetComponent<Outline>().effectColor = outlineBlue;
        //else if (friendlyObject == null) obj.GetComponent<Outline>().effectColor = outlineBlack;
    }

    internal void ToggleChat(bool visible)
    {
        chatPanel.SetActive(true);
        chatActive = visible;

        field.gameObject.SetActive(visible);
        field.ActivateInputField();
        field.Select();
        scroll.value = 0f;
    }

    public void AddChatMessage(string message, bool isSelf)
    {
        chatPanel.SetActive(true);
        field.gameObject.SetActive(chatActive);

        Text chatItemClone = Instantiate(chatItemPrefab);
        chatItemClone.text = message;
        chatItemClone.transform.SetParent(chatContent, false);

        StartCoroutine(Scroll());
        
        if(hideChat!=null) StopCoroutine(hideChat);
        if (chatActive&&!isSelf) return;
        hideChat = StartCoroutine(HideChat());      
    }

    IEnumerator Scroll()
    {
        yield return new WaitForSeconds(0.02f);
        scroll.value = 0f;
    }

    IEnumerator HideChat()
    {       
        yield return new WaitForSeconds(4f);
        if (!chatActive) chatPanel.SetActive(false);
        scroll.value = 0f;
    }

    public void ChatEditEnd()
    {
        if(field.text.Length>0&& field.text.Length < 101) GameObject.Find("/Network Client").SendMessage("SendChatMessage", field.text);
        field.text = "";       
        scroll.value = 0f;
    }

}
