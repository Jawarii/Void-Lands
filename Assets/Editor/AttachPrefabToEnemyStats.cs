using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class AttachPrefabToEnemyStats : EditorWindow
{
    public GameObject prefabToAttach;

    [MenuItem("Tools/Attach Prefab to EnemyStats")]
    public static void ShowWindow()
    {
        GetWindow<AttachPrefabToEnemyStats>("Attach Prefab to EnemyStats");
    }

    private void OnGUI()
    {
        GUILayout.Label("Attach Prefab to All Objects with EnemyStats", EditorStyles.boldLabel);
        prefabToAttach = (GameObject)EditorGUILayout.ObjectField("Prefab to Attach", prefabToAttach, typeof(GameObject), false);

        if (GUILayout.Button("Attach Prefab"))
        {
            if (prefabToAttach == null)
            {
                Debug.LogError("Prefab not assigned. Please assign a prefab.");
                return;
            }

            AttachPrefabToAll();
        }
    }

    private void AttachPrefabToAll()
    {
        string[] guids = AssetDatabase.FindAssets("t:Prefab");
        int count = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefabAsset != null && prefabAsset.GetComponent<EnemyStats>() != null)
            {
                // Open the prefab in Prefab Mode
                GameObject prefabInstance = PrefabUtility.LoadPrefabContents(path);

                if (prefabInstance != null)
                {
                    // Check if the prefab already has the child
                    Transform existingChild = prefabInstance.transform.Find(prefabToAttach.name);
                    if (existingChild == null)
                    {
                        // Instantiate and attach the prefab as a child
                        GameObject newChild = Instantiate(prefabToAttach, prefabInstance.transform);
                        newChild.name = prefabToAttach.name; // Optional: rename the child
                        count++;
                    }

                    // Save changes to the prefab and unload it
                    PrefabUtility.SaveAsPrefabAsset(prefabInstance, path);
                    PrefabUtility.UnloadPrefabContents(prefabInstance);
                }
            }
        }

        Debug.Log($"Attached prefab to {count} prefabs with EnemyStats.");
    }
}
