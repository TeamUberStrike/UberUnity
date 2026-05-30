using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Editor tool: Place spawn points, pickups, and death areas from extracted original data.
/// Add this to any GameObject in the scene, select the map, and click "Populate Map" in Inspector.
/// Data is embedded from the UnityPy extraction of the original UberStrike level files.
/// </summary>
public class MapPopulator : MonoBehaviour
{
    [Header("Map Selection")]
    public string mapName = "AqualabResearchHub";

    [Header("Prefab References (assign in Inspector)")]
    public GameObject spawnPointPrefab;
    public GameObject healthPickupPrefab;
    public GameObject armorPickupPrefab;
    public GameObject ammoPickupPrefab;
    public GameObject deathAreaPrefab;

    [Header("Options")]
    public bool placeSpawns = true;
    public bool placePickups = true;
    public bool placeDeathAreas = true;
    public bool clearExistingFirst = false;

    /// <summary>
    /// Call this from an Editor button or context menu to populate the current scene.
    /// </summary>
    [ContextMenu("Populate Map")]
    public void PopulateMap()
    {
        // The actual data would be loaded from the JSON files at runtime/editor time.
        // For now, this demonstrates the placement approach.
        // In practice, use: JsonUtility or Newtonsoft to load {MapName}_v2.json
        Debug.Log("[MapPopulator] To use: load " + mapName + "_v2.json from UberStrike_Extracted/");
        Debug.Log("[MapPopulator] Or use the MapPopulatorEditor window for automatic placement.");
    }
}

#if UNITY_EDITOR
/// <summary>
/// Editor window that reads the extracted JSON and places objects in the scene.
/// Window > UberStrike > Map Populator
/// </summary>
public class MapPopulatorEditor : EditorWindow
{
    private string jsonFolder = @"C:\Users\Shadow\Downloads\UberStrike_Extracted";
    private string selectedMap = "AqualabResearchHub";
    private bool placeSpawns = true;
    private bool placePickups = true;
    private bool placeDeathAreas = true;
    private Vector2 scrollPos;

    private static readonly string[] MAP_NAMES = {
        "ApexTwin", "AqualabResearchHub", "Catalyst", "CuberSpace", "CuberStrike",
        "FortWinter", "GhostIsland", "GideonsTower", "MonkeyIsland", "LostParadise2",
        "SkyGarden", "SuperPRISMReactor", "TempleOfTheRaven", "TheHangar", "TheWarehouse"
    };

    [MenuItem("Window/UberStrike/Map Populator")]
    public static void ShowWindow()
    {
        GetWindow<MapPopulatorEditor>("Map Populator");
    }

    void OnGUI()
    {
        GUILayout.Label("UberStrike Map Populator", EditorStyles.boldLabel);
        GUILayout.Space(5);

        jsonFolder = EditorGUILayout.TextField("JSON Folder", jsonFolder);

        GUILayout.Space(5);
        GUILayout.Label("Select Map:");
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(200));
        foreach (var map in MAP_NAMES)
        {
            if (GUILayout.Toggle(selectedMap == map, map, "Button"))
                selectedMap = map;
        }
        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);
        placeSpawns = EditorGUILayout.Toggle("Place Spawn Points", placeSpawns);
        placePickups = EditorGUILayout.Toggle("Place Pickups", placePickups);
        placeDeathAreas = EditorGUILayout.Toggle("Place Death Areas", placeDeathAreas);

        GUILayout.Space(10);
        if (GUILayout.Button("Populate Scene", GUILayout.Height(30)))
        {
            PopulateScene();
        }

        if (GUILayout.Button("Clear Populated Objects", GUILayout.Height(25)))
        {
            ClearPopulated();
        }
    }

    void PopulateScene()
    {
        string path = System.IO.Path.Combine(jsonFolder, selectedMap + "_v2.json");
        if (!System.IO.File.Exists(path))
        {
            EditorUtility.DisplayDialog("Error", "JSON file not found: " + path, "OK");
            return;
        }

        string json = System.IO.File.ReadAllText(path);
        var data = JsonUtility.FromJson<MapData>(json);

        // Create parent containers
        GameObject root = new GameObject("[Populated_" + selectedMap + "]");
        Undo.RegisterCreatedObjectUndo(root, "Populate Map");

        int count = 0;

        if (placeSpawns)
        {
            GameObject spawnRoot = new GameObject("SpawnPoints");
            spawnRoot.transform.SetParent(root.transform);

            count += PlaceSpawnGroup(data.spawns.DM, "DM", spawnRoot.transform);
            count += PlaceSpawnGroup(data.spawns.TDM_Red, "TDM_Red", spawnRoot.transform);
            count += PlaceSpawnGroup(data.spawns.TDM_Blue, "TDM_Blue", spawnRoot.transform);
            count += PlaceSpawnGroup(data.spawns.TE_Red, "TE_Red", spawnRoot.transform);
            count += PlaceSpawnGroup(data.spawns.TE_Blue, "TE_Blue", spawnRoot.transform);
        }

        if (placePickups)
        {
            GameObject pickupRoot = new GameObject("Pickups");
            pickupRoot.transform.SetParent(root.transform);

            foreach (var pickup in data.pickups)
            {
                GameObject go = new GameObject(pickup.type + "_" + pickup.name);
                go.transform.SetParent(pickupRoot.transform);
                go.transform.position = new Vector3(pickup.position.x, pickup.position.y, pickup.position.z);
                go.tag = "Powerup";
                count++;
            }
        }

        if (placeDeathAreas)
        {
            GameObject deathRoot = new GameObject("DeathAreas");
            deathRoot.transform.SetParent(root.transform);

            foreach (var da in data.death_areas)
            {
                GameObject go = new GameObject("DeathArea_" + da.name);
                go.transform.SetParent(deathRoot.transform);
                go.transform.position = new Vector3(da.position.x, da.position.y, da.position.z);
                // Add a large trigger collider
                BoxCollider col = go.AddComponent<BoxCollider>();
                col.isTrigger = true;
                col.size = new Vector3(100, 5, 100);
                count++;
            }
        }

        Debug.Log("[MapPopulator] Placed " + count + " objects for " + selectedMap);
    }

    int PlaceSpawnGroup(List<SpawnEntry> spawns, string groupName, Transform parent)
    {
        if (spawns == null || spawns.Count == 0) return 0;

        GameObject group = new GameObject(groupName);
        group.transform.SetParent(parent);

        foreach (var sp in spawns)
        {
            GameObject go = new GameObject(groupName + "_" + sp.name);
            go.transform.SetParent(group.transform);
            go.transform.position = new Vector3(sp.position.x, sp.position.y, sp.position.z);
            go.transform.rotation = new Quaternion(sp.rotation.x, sp.rotation.y, sp.rotation.z, sp.rotation.w);
        }

        return spawns.Count;
    }

    void ClearPopulated()
    {
        var populated = GameObject.FindObjectsByType<Transform>(FindObjectsSortMode.None);
        int removed = 0;
        foreach (var t in populated)
        {
            if (t != null && t.name.StartsWith("[Populated_"))
            {
                Undo.DestroyObjectImmediate(t.gameObject);
                removed++;
            }
        }
        Debug.Log("[MapPopulator] Cleared " + removed + " populated roots");
    }

    // JSON data classes
    [System.Serializable]
    public class MapData
    {
        public string map_name;
        public int level_index;
        public SpawnData spawns;
        public List<PickupEntry> pickups;
        public List<PositionEntry> death_areas;
        public List<PositionEntry> teleporters;
    }

    [System.Serializable]
    public class SpawnData
    {
        public List<SpawnEntry> DM;
        public List<SpawnEntry> TDM_Red;
        public List<SpawnEntry> TDM_Blue;
        public List<SpawnEntry> TE_Red;
        public List<SpawnEntry> TE_Blue;
    }

    [System.Serializable]
    public class SpawnEntry
    {
        public string name;
        public Vec3 position;
        public Vec4 rotation;
    }

    [System.Serializable]
    public class PickupEntry
    {
        public string name;
        public string type;
        public Vec3 position;
    }

    [System.Serializable]
    public class PositionEntry
    {
        public string name;
        public Vec3 position;
    }

    [System.Serializable]
    public class Vec3
    {
        public float x, y, z;
    }

    [System.Serializable]
    public class Vec4
    {
        public float x, y, z, w;
    }
}
#endif
