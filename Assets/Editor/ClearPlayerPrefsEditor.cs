using UnityEditor;
using UnityEngine;

public static class ClearPlayerPrefsEditor
{
    [MenuItem("Tools/Clear PlayerPrefs")]
    public static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs have been cleared.");
    }
}
