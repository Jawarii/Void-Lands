using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuestUIManager : MonoBehaviour
{
    public static QuestUIManager Instance { get; private set; }

    public Transform questListContainer;
    public GameObject questEntryPrefab;
    public GameObject objectiveEntryPrefab;
    public GameObject questPanel;
    public Canvas questCanvas;
    public GameObject notificationPanel;

    [Header("Reward UI")]
    public GameObject rewardPanel;
    public Transform rewardListContainer;
    public GameObject rewardEntryPrefab;

    [Header("Reward Icons")]
    public Sprite goldIcon;
    public Sprite expIcon;

    [SerializeField] private AudioClip objectiveCompleteSFX;
    [SerializeField] private AudioClip rewardClaimedSFX;
    [SerializeField] private AudioClip questAcquiredSFX;
    private AudioSource audioSource;

    private List<GameObject> activeEntries = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        StartCoroutine(DelayedSubscription());

        audioSource = GetComponent<AudioSource>();
    }

    private IEnumerator DelayedSubscription()
    {
        yield return null;
        QuestManager.Instance.OnQuestAcquired += HandleQuestAcquired;
        QuestManager.Instance.OnQuestProgressUpdated += HandleProgressUpdate;
        QuestManager.Instance.OnObjectiveCompleted += HandleObjectiveCompleted;
        QuestManager.Instance.OnQuestRewarded += HandleQuestRewarded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;

            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.OnQuestAcquired -= HandleQuestAcquired;
                QuestManager.Instance.OnQuestProgressUpdated -= HandleProgressUpdate;
                QuestManager.Instance.OnObjectiveCompleted -= HandleObjectiveCompleted;
                QuestManager.Instance.OnQuestRewarded -= HandleQuestRewarded;
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (questPanel == null)
            questPanel = GameObject.Find("QuestPanel");

        if (questListContainer == null)
            questListContainer = GameObject.Find("QuestListContainer")?.transform;

        RefreshUI();
    }

    private void HandleQuestAcquired(string questName)
    {
        RefreshUI();
        PlayQuestAcquiredSound();
        DisplayNotification(false, questName);
    }

    private void HandleProgressUpdate(string questName)
    {
        RefreshUI();

        foreach (var entry in activeEntries)
        {
            var titleText = entry.transform.Find("QuestTitle").GetComponent<TextMeshProUGUI>();
            if (titleText != null && titleText.text == questName)
            {
                var pulse = entry.GetComponentInChildren<ObjectivePulse>();
                if (pulse != null)
                    pulse.PlayPulse();
            }
        }
    }

    private void HandleObjectiveCompleted(string questName)
    {
        RefreshUI();

        foreach (var entry in activeEntries)
        {
            var titleText = entry.transform.Find("QuestTitle").GetComponent<TextMeshProUGUI>();
            if (titleText != null && titleText.text == questName)
            {
                var pulse = entry.GetComponentInChildren<ObjectivePulse>();
                if (pulse != null)
                    pulse.PlayPulse();

                var checkmark = entry.transform.Find("ObjectivesContainer/ObjectiveEntry/StatusIcon/CompleteIcon");
                if (checkmark != null)
                {
                    Animator animator = checkmark.GetComponent<Animator>();
                    if (animator != null)
                    {
                        animator.Play("CheckmarkPop", -1, 0f);
                    }
                }
            }
        }
        PlayObjectiveCompleteSound();
    }

    private void HandleQuestRewarded(string questName, int gold, int exp, Dictionary<ItemInfo, int> items)
    {
        RefreshUI();
        StartCoroutine(ShowRewardPopup(gold, exp, items));
        PlayRewardClaimedSound();
        DisplayNotification(true, questName);
    }

    public void RefreshUI()
    {
        foreach (var entry in activeEntries)
            Destroy(entry);

        activeEntries.Clear();

        if (QuestManager.Instance == null || questListContainer == null)
            return;

        foreach (var quest in QuestManager.Instance.activeQuests)
        {
            if (quest.isRewarded) continue;

            GameObject questEntry = Instantiate(questEntryPrefab, questListContainer);
            activeEntries.Add(questEntry);

            TextMeshProUGUI titleText = questEntry.transform.Find("QuestTitle").GetComponent<TextMeshProUGUI>();
            titleText.text = quest.questData.questName;

            Transform objectivesContainer = questEntry.transform.Find("ObjectivesContainer");

            GameObject objectiveEntry = Instantiate(objectiveEntryPrefab, objectivesContainer);
            string objectiveDescription;
            if (quest.questData.questType == QuestType.Talk)
            {
                objectiveDescription = quest.questData.questDescription;
            }
            else
            {
                objectiveDescription = $"{quest.currentProgress}/{quest.questData.targetAmount} {quest.questData.targetTag}";
            }

            TextMeshProUGUI objectiveText = objectiveEntry.transform.Find("ObjectiveText").GetComponent<TextMeshProUGUI>();
            objectiveText.text = objectiveDescription;

            Transform statusIcon = objectiveEntry.transform.Find("StatusIcon");
            Transform checkmark = statusIcon.Find("CompleteIcon");
            bool isCompleted = quest.isComplete;
            checkmark.gameObject.SetActive(isCompleted);
        }
    }

    private IEnumerator ShowRewardPopup(int gold, int exp, Dictionary<ItemInfo, int> items)
    {
        yield return new WaitForSeconds(0.1f);
        rewardPanel.SetActive(true);

        if (gold > 0)
        {
            CreateRewardEntry("Gold", $"x{gold}", goldIcon, true);
        }
        yield return new WaitForSeconds(0.1f);
        if (exp > 0)
        {
            CreateRewardEntry($"+{exp:N0} EXP", "", expIcon, false);
        }
        yield return new WaitForSeconds(0.1f);
        if (items != null && items.Count > 0)
        {
            foreach (var pair in items)
            {
                ItemInfo item = pair.Key;
                int amount = pair.Value;

                Sprite icon = item.itemUISprite != null ? item.itemUISprite : item.GetComponent<SpriteRenderer>()?.sprite;
                string displayAmount = amount > 1 ? $"x{amount}" : "";
                CreateRewardEntry(item.itemName, displayAmount, icon, amount > 1, item.textColor);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    private void CreateRewardEntry(string name, string amount, Sprite icon, bool showAmount, Color? overrideColor = null)
    {
        GameObject entry = Instantiate(rewardEntryPrefab, rewardListContainer);
        entry.transform.SetSiblingIndex(0);

        var nameText = entry.transform.Find("RewardName").GetComponent<TextMeshProUGUI>();
        nameText.text = name;
        if (overrideColor.HasValue)
            nameText.color = overrideColor.Value;

        entry.transform.Find("RewardAmount").gameObject.SetActive(showAmount);
        if (showAmount)
            entry.transform.Find("RewardAmount").GetComponent<TextMeshProUGUI>().text = amount;

        var iconImage = entry.transform.Find("RewardIcon").GetComponent<Image>();
        if (icon != null)
        {
            iconImage.sprite = icon;
            iconImage.enabled = true;
        }
        else
        {
            iconImage.enabled = false;
        }

        var behaviour = entry.GetComponent<RewardEntryBehaviour>();
        if (behaviour != null)
            behaviour.BeginFadeAndDestroy(5f);
    }

    public void ToggleQuestPanel()
    {
        questPanel.SetActive(!questPanel.activeSelf);
    }

    private void PlayObjectiveCompleteSound()
    {
        if (objectiveCompleteSFX != null && audioSource != null)
        {
            audioSource.volume = 1f;
            audioSource.PlayOneShot(objectiveCompleteSFX);
        }
    }

    private void PlayRewardClaimedSound()
    {
        if (rewardClaimedSFX != null && audioSource != null)
        {
            audioSource.volume = 0.75f;
            audioSource.PlayOneShot(rewardClaimedSFX);
        }
    }

    private void PlayQuestAcquiredSound()
    {
        if (questAcquiredSFX != null && audioSource != null)
        {
            audioSource.volume = 0.5f;
            audioSource.PlayOneShot(questAcquiredSFX);
        }
    }

    public void DisplayNotification(bool isReward, string questName)
    {
        StartCoroutine(DisplayNotificationRoutine(isReward, questName));
    }

    private IEnumerator DisplayNotificationRoutine(bool isReward, string questName)
    {
        yield return null;
        notificationPanel.SetActive(true);

        CanvasGroup canvasGroup = notificationPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = notificationPanel.AddComponent<CanvasGroup>();

        TMP_Text notifyText = notificationPanel.GetComponentInChildren<TMP_Text>();
        notifyText.text = isReward ? $"Quest Completed: {questName}" : $"Quest Acquired: {questName}";

        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 1f, 0.15f)); // Fast fade in
        yield return new WaitForSeconds(4);
        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 0f, 0.75f)); // Slow fade out

        notifyText.text = "";
        notificationPanel.SetActive(false);
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }
}
