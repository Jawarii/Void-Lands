using UnityEngine;
using UnityEngine.SceneManagement;

public class EnsureBootstrapLoaded : MonoBehaviour
{
    [SerializeField] private string bootstrapSceneName = "BootstrapScene";
#if UNITY_EDITOR
    private void Awake()
    {
        bootstrapSceneName = "BootstrapScene";
        if (!SceneManager.GetSceneByName(bootstrapSceneName).isLoaded)
        {
            Debug.Log($"Editor detected Bootstrap not loaded. Loading {bootstrapSceneName} additively.");
            SceneManager.LoadScene(bootstrapSceneName, LoadSceneMode.Additive);

        }
    }
#endif
}
