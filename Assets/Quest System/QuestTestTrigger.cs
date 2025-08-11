using UnityEngine;
using System.Collections;

public class QuestTestTrigger : MonoBehaviour
{
    public QuestSO testQuest;

    private void Start()
    {
        StartCoroutine(CheckIntroCompletionRoutine());
    }

    private IEnumerator CheckIntroCompletionRoutine()
    {
        while (PlayerPrefs.GetInt("IntroCompleted", 0) == 0)
        {
            yield return new WaitForSeconds(1f);
        }
        yield return new WaitForSeconds(0.5f);
        QuestManager.Instance?.AddQuest(testQuest);
        Debug.Log("Intro completed, quest given.");
    }
}
