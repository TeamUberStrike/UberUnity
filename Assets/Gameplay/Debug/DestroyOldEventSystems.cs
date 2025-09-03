using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemMonitor : MonoBehaviour
{
    void Update()
    {
        var systems = Object.FindObjectsOfType<EventSystem>();

        if (systems.Length > 1)
        {
            Debug.LogWarning($"[EventSystemMonitor] Found {systems.Length} EventSystems. Destroying all but the first.");
            for (int i = 1; i < systems.Length; i++)
            {
                Object.Destroy(systems[i].gameObject);
            }
        }
    }
}