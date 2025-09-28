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

    private void Start()
    {
        // Hook into InputField events for proper mobile keyboard handling
        if (field != null)
        {
            field.onEndEdit.AddListener(OnChatInputEndEdit);
        }
    }

    private void OnChatInputEndEdit(string text)
    {
        // Only handle this for mobile platforms - desktop uses PlayerInput Return key handling
        if (Application.isMobilePlatform && chatActive)
        {
            // Find PlayerManager to handle the chat toggle
            GameObject player = GameObject.Find("/Player");
            if (player != null)
            {
                PlayerManager playerManager = player.GetComponent<PlayerManager>();
                if (playerManager != null)
                {
                    // Check if text was entered (Ready button) or empty (Cancel button)
                    if (!string.IsNullOrEmpty(text.Trim()))
                    {
                        // Ready button pressed with text - close chat first, then send message
                        // This ensures the message is processed when chatActive is false
                        playerManager.ToggleChat(); // Close chat first
                        ChatEditEnd(); // Then send message
                    }
                    else
                    {
                        // Cancel button or empty text - just close chat
                        field.text = "";
                        playerManager.ToggleChat();
                    }
                }
            }
        }
        // Desktop behavior is handled by PlayerInput Return key detection
    }

    internal void ToggleChat(bool visible)
    {
        chatPanel.SetActive(true);
        chatActive = visible;

        field.gameObject.SetActive(visible);
        if (visible)
        {
            field.ActivateInputField();
            field.Select();
        }
        scroll.value = 1f;
    }

    public void AddChatMessage(string message, bool isSelf)
    {
        Debug.Log($"AddChatMessage called: message='{message}', isSelf={isSelf}, chatActive={chatActive}");
        
        chatPanel.SetActive(true);
        field.gameObject.SetActive(chatActive);

        Text chatItemClone = Instantiate(chatItemPrefab);
        chatItemClone.text = message;
        chatItemClone.transform.SetParent(chatContent, false);
        
        // Disable any layout groups that might interfere
        var layoutGroup = chatContent.GetComponent<UnityEngine.UI.VerticalLayoutGroup>();
        if (layoutGroup != null) layoutGroup.enabled = false;
        
        var contentSizeFitter = chatContent.GetComponent<UnityEngine.UI.ContentSizeFitter>();
        if (contentSizeFitter != null) contentSizeFitter.enabled = false;
        
        // Fix positioning - ensure message is positioned correctly within content bounds
        RectTransform msgRect = chatItemClone.GetComponent<RectTransform>();
        msgRect.anchorMin = new Vector2(0, 1);
        msgRect.anchorMax = new Vector2(1, 1);
        msgRect.pivot = new Vector2(0.5f, 1);
        
        // Position messages stacked from top, within visible bounds
        int messageIndex = chatContent.childCount - 1;
        float yPosition = -25f * messageIndex; // 25 pixels per message
        msgRect.anchoredPosition = new Vector2(0, yPosition);
        msgRect.sizeDelta = new Vector2(-10, 20); // Full width minus padding, 20px height
        
        Debug.Log($"Applied positioning: messageIndex={messageIndex}, yPosition={yPosition}");
        
        // Force layout refresh
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(chatContent.GetComponent<RectTransform>());
        
        Debug.Log($"Message instantiated and added. ChatPanel active: {chatPanel.activeInHierarchy}");
        Debug.Log($"ChatContent active: {chatContent.gameObject.activeInHierarchy}, ChatContent child count: {chatContent.childCount}");
        Debug.Log($"Message clone active: {chatItemClone.gameObject.activeInHierarchy}, Message text: '{chatItemClone.text}'");
        
        // Check UI positioning and sizes
        RectTransform chatPanelRect = chatPanel.GetComponent<RectTransform>();
        RectTransform chatContentRect = chatContent.GetComponent<RectTransform>();
        RectTransform messageRect = chatItemClone.GetComponent<RectTransform>();
        
        Debug.Log($"ChatPanel size: {chatPanelRect.rect.size}, position: {chatPanelRect.anchoredPosition}");
        Debug.Log($"ChatContent size: {chatContentRect.rect.size}, position: {chatContentRect.anchoredPosition}");
        Debug.Log($"Message size: {messageRect.rect.size}, position: {messageRect.anchoredPosition}");

        StartCoroutine(Scroll());
        
        if(hideChat!=null) StopCoroutine(hideChat);
        if (chatActive&&!isSelf) return;
        hideChat = StartCoroutine(HideChat());      
    }

    IEnumerator Scroll()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        Debug.Log($"Setting scroll.value to 1f. Current value was: {scroll.value}");
        scroll.value = 1f;
        
        // Check scroll rect setup
        ScrollRect scrollRect = scroll.GetComponentInParent<ScrollRect>();
        if (scrollRect != null)
        {
            Debug.Log($"ScrollRect found. Content: {scrollRect.content?.name}, Viewport: {scrollRect.viewport?.name}");
            Debug.Log($"ScrollRect normalizedPosition: {scrollRect.normalizedPosition}");
        }
        else
        {
            Debug.Log("No ScrollRect component found!");
        }
    }

    IEnumerator HideChat()
    {       
        Debug.Log("HideChat coroutine started, waiting 4 seconds...");
        yield return new WaitForSeconds(4f);
        Debug.Log($"HideChat timer expired. chatActive={chatActive}, will hide chat: {!chatActive}");
        if (!chatActive) chatPanel.SetActive(false);
        scroll.value = 1f;
    }



    public void ChatEditEnd()
    {
        if(field.text.Length>0&& field.text.Length < 101) GameObject.Find("/Network Client").SendMessage("SendChatMessage", field.text);
        field.text = "";       
        scroll.value = 1f;
    }

}
