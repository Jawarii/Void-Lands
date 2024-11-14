using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ReplaceChildInClickLoot : EditorWindow
{
    public GameObject prefabToAdd; // Predetermined prefab to add as a child

    [MenuItem("Tools/Replace Child in ClickLoot Objects")]
    public static void ShowWindow()
    {
        GetWindow<ReplaceChildInClickLoot>("Replace Child in ClickLoot Objects");
    }

    private void OnGUI()
    {
        GUILayout.Label("Replace Child Objects", EditorStyles.boldLabel);
        prefabToAdd = (GameObject)EditorGUILayout.ObjectField("Prefab to Add", prefabToAdd, typeof(GameObject), false);

        if (GUILayout.Button("Search and Replace"))
        {
            SearchAndReplaceChildren();
        }
    }

    private void SearchAndReplaceChildren()
    {
        if (prefabToAdd == null)
        {
            Debug.LogError("Please assign a prefab to add.");
            return;
        }

        // Find all prefabs in the project
        string[] guids = AssetDatabase.FindAssets("t:Prefab");
        List<GameObject> objectsWithClickLoot = new List<GameObject>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (obj != null && obj.GetComponent<ClickLootBehaviour>() != null)
            {
                objectsWithClickLoot.Add(obj);
            }
        }

        foreach (GameObject obj in objectsWithClickLoot)
        {
            // Open the prefab for editing
            GameObject prefabInstance = PrefabUtility.LoadPrefabContents(AssetDatabase.GetAssetPath(obj));

            // Remove the first child
            if (prefabInstance.transform.childCount > 0)
            {
                Transform firstChild = prefabInstance.transform.GetChild(0);
                DestroyImmediate(firstChild.gameObject);
            }

            // Add the new child (the predetermined prefab)
            GameObject newChild = PrefabUtility.InstantiatePrefab(prefabToAdd) as GameObject;
            newChild.transform.SetParent(prefabInstance.transform);

            // Reset RectTransform if applicable
            RectTransform rectTransform = newChild.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = Vector2.zero;
                rectTransform.sizeDelta = Vector2.zero;
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                rectTransform.localRotation = Quaternion.identity;

                // Scale the canvas so that its scale divided by the parent's scale equals 1
                Vector3 parentScale = prefabInstance.transform.localScale;
                rectTransform.localScale = new Vector3(
                    (1f / parentScale.x) * 0.9f,
                    (1f / parentScale.y) * 0.9f,
                    (1f / parentScale.z) * 0.9f
                );
            }

            // Save the prefab changes
            PrefabUtility.SaveAsPrefabAsset(prefabInstance, AssetDatabase.GetAssetPath(obj));
            PrefabUtility.UnloadPrefabContents(prefabInstance);
        }

        Debug.Log("Child replacement, RectTransform reset, and scaling adjustment completed.");
    }
}
