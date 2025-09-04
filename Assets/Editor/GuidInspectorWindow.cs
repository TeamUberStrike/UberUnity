using UnityEditor;
using UnityEngine;

public class GuidInspectorWindow : EditorWindow
{
    private string guidInput = "";

    [MenuItem("Tools/Guid Inspector")]
    public static void ShowWindow()
    {
        GetWindow<GuidInspectorWindow>("GUID Inspector");
    }

    private void OnGUI()
    {
        GUILayout.Label("Enter a Unity GUID", EditorStyles.boldLabel);
        guidInput = EditorGUILayout.TextField("GUID:", guidInput);

        if (GUILayout.Button("Find Asset"))
        {
            if (string.IsNullOrEmpty(guidInput))
            {
                Debug.LogWarning("Please enter a GUID.");
                return;
            }

            string path = AssetDatabase.GUIDToAssetPath(guidInput);
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning(string.Format("GUID {0} not found in Assets/ or Packages/. Might be a built-in Unity resource.", guidInput));
            }
            else
            {
                Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);
                if (obj != null)
                {
                    Debug.Log(string.Format("GUID {0} points to: {1} ({2})", guidInput, path, obj.GetType().Name));
                    EditorGUIUtility.PingObject(obj); // highlight asset in Project window
                }
                else
                {
                    Debug.LogWarning(string.Format("Could not load object at {0}. Possibly an internal reference.", path));
                }
            }
        }
    }
}
