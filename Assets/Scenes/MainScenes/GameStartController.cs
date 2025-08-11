using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartController : MonoBehaviour
{
    public SceneFader sceneFader; // Reference to the SceneFader script
    private AudioSource audioSource;
    public AudioClip clickClip;

    public void StartGame()
    {
        sceneFader = GameObject.Find("BlackOutImage").GetComponent<SceneFader>();
        // Play the click sound
        PlayClickSound();
        // Start the fade-out process and load the scene after a delay
        
        if (PlayerPrefs.GetInt("IntroCompleted", 0) == 1)
        {
            StartCoroutine(FadeOutAndLoad("Level-1"));
        }
        else
        {
            StartCoroutine(FadeOutAndLoad("IntroScene"));
        }
    }

    private System.Collections.IEnumerator FadeOutAndLoad(string sceneName)
    {
        // Start fading out
        yield return StartCoroutine(sceneFader.FadeOut());

        // Wait for 1 second
        yield return new WaitForSeconds(1.0f);

        // Load the specified scene
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
        sceneFader = GameObject.Find("BlackOutImage").GetComponent<SceneFader>();
        // Play the click sound
        PlayClickSound();
        // Start fade out and quit after a delay
        StartCoroutine(FadeOutAndQuit());
    }
    public void LeaveToStartMenu()
    {
        sceneFader = GameObject.Find("BlackOutImage").GetComponent<SceneFader>();
        // Play the click sound
        PlayClickSound();
        // Start the fade-out process and load the scene after a delay
        StartCoroutine(FadeOutAndLoad("MainScreen"));
    }

    public void Resume()
    {
        // Play the click sound
        PlayClickSound();
        GetComponentInParent<Canvas>().enabled = false;
    }

    private System.Collections.IEnumerator FadeOutAndQuit()
    {
        // Start fading out
        yield return StartCoroutine(sceneFader.FadeOut());

        // Wait for 1 second
        yield return new WaitForSeconds(1.0f);

        // Quit the application
        Application.Quit();

        // Log a message for debugging in the editor
#if UNITY_EDITOR
        Debug.Log("Game quit (Application.Quit called).");
#endif
    }
    public void PlayClickSound()
    {
        audioSource = GameObject.Find("MenusSoundSource").GetComponent<AudioSource>();
        audioSource.PlayOneShot(clickClip);
    }
}
