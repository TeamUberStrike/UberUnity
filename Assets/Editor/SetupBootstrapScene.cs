using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SetupBootstrapScene
{
    [MenuItem("UberStrike/Create Bootstrap Scene")]
    public static void CreateBootstrapScene()
    {
        // Create a new empty scene
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // 1. Main Camera
        var cameraGO = new GameObject("Main Camera");
        cameraGO.tag = "MainCamera";
        var cam = cameraGO.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;
        cameraGO.AddComponent<AudioListener>();

        // 2. GlobalSceneLoader — the bootstrap entry point
        var loaderGO = new GameObject("GlobalSceneLoader");
        loaderGO.AddComponent<GlobalSceneLoader>();

        // 3. Save the scene
        string scenePath = "Assets/Scenes/GlobalScene.unity";
        // Ensure directory exists
        if (!System.IO.Directory.Exists("Assets/Scenes"))
            System.IO.Directory.CreateDirectory("Assets/Scenes");

        EditorSceneManager.SaveScene(scene, scenePath);
        Debug.Log("Bootstrap scene created at: " + scenePath);

        // 4. Update Build Settings — put GlobalScene first
        UpdateBuildSettings(scenePath);

        Debug.Log("Build settings updated. GlobalScene is now Scene 0.");
        Debug.Log("Press Play to test authentication with HaZard's server!");
    }

    static void UpdateBuildSettings(string bootstrapScenePath)
    {
        var scenes = new List<EditorBuildSettingsScene>();

        // Scene 0: Bootstrap (GlobalScene)
        scenes.Add(new EditorBuildSettingsScene(bootstrapScenePath, true));

        // Scene 1: MainMenu
        scenes.Add(new EditorBuildSettingsScene("Assets/Menus/MainMenu.unity", true));

        // Scene 2: Intro (optional, can skip to MainMenu)
        scenes.Add(new EditorBuildSettingsScene("Assets/Menus/Intro.unity", true));

        // Add all map scenes
        string[] mapScenes = new string[]
        {
            "Assets/Maps/Apex_Twin/Apex_Twin.unity",
            "Assets/Maps/Catalyst/Catalyst.unity",
            "Assets/Maps/Cuberspace/Cuberspace.unity",
            "Assets/Maps/Danger_Zone/Danger_Zone.unity",
            "Assets/Maps/Fort_Winter/Fort_Winter.unity",
            "Assets/Maps/Gideons_Tower_II/Gideons_Tower_II.unity",
            "Assets/Maps/Lost_Paradise_II/Lost_Paradise_2.unity",
            "Assets/Maps/Monkey_Island/Monkey_Island.unity",
            "Assets/Maps/Research_Hub/Research_Hub.unity",
            "Assets/Maps/Sky_Garden/Sky_Garden.unity",
            "Assets/Maps/Space_City/Space_City.unity",
            "Assets/Maps/Spaceport_Alpha/Spaceport_Alpha.unity",
            "Assets/Maps/SuperPRISM_Reactor/SuperPRISM_Reactor.unity",
            "Assets/Maps/Temple_Of_The_Raven/Temple_Of_The_Raven.unity",
            "Assets/Maps/The_Hangar/The_Hangar.unity",
            "Assets/Maps/The_Warehouse/The_Warehouse.unity",
        };

        foreach (var mapScene in mapScenes)
        {
            if (System.IO.File.Exists(mapScene))
                scenes.Add(new EditorBuildSettingsScene(mapScene, true));
        }

        EditorBuildSettings.scenes = scenes.ToArray();
    }
}
