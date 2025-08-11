using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneIntroController : MonoBehaviour
{
    [SerializeField] private PlayableDirector playableDirector;
    [SerializeField] private float transitionDelay = 30f; // Adjust for your timing
    [SerializeField] private string level1SceneName = "Level-1"; // Replace with your scene name
    [SerializeField] private Image blackOutImage;

    private void Awake()
    {
        QuestUIManager.Instance.questCanvas.enabled = false;
    }
    private void Start()
    {
        StartCoroutine(StartIntroSequence());
    }

    private IEnumerator StartIntroSequence()
    {
        yield return null;
        playableDirector.time = 0;
        playableDirector.Evaluate();
        playableDirector.Play();

        yield return new WaitForSeconds(transitionDelay);
        blackOutImage.GetComponent<SceneFader>().StartFadeOut();
        yield return new WaitForSeconds(2f);
        TransitionToLevel1();
    }

    private void TransitionToLevel1()
    {
        SceneManager.LoadScene(level1SceneName);
    }
}
