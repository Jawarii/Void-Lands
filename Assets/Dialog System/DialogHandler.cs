using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogHandler : MonoBehaviour
{
    public DialogDataSO dialogData;
    public Canvas dialogCanvas;
    public TextMeshProUGUI dialogText;

    public float typewriterSpeed = 0.04f;

    [Header("Typewriter Sound")]
    public AudioClip typeSoundClip;
    public float typeSoundCooldown = 0.06f;

    private AudioSource audioSource;
    private float lastSoundTime;

    private int currentLine = 0;
    private bool isTyping = false;
    private bool skipTyping = false;
    private bool allowInput = false;

    private Coroutine typewriterCoroutine;

    public GameObject acquireQuestIcon;
    public GameObject inProgressQuestIcon;
    public GameObject completeQuestIcon;

    private void Start()
    {
        dialogCanvas.enabled = false;
        dialogText.text = "";
        audioSource = GetComponent<AudioSource>();
    }
    public void SetQuestIcons()
    {
        if (dialogData == null)
            return;
        switch (dialogData.dialogType)
        {
            case DialogType.AcquireQuest:
                ToggleQuestIcons(true, false, false);
                break;
            case DialogType.InProgressQuest:
                ToggleQuestIcons(false, true, false);
                break;
            case DialogType.CompleteQuest:
                ToggleQuestIcons(false, false, true);
                break;
            default:
                ToggleQuestIcons(false, false, false);
                break;
        }
    }
    private void ToggleQuestIcons(bool acquire, bool inProgress, bool complete)
    {
        acquireQuestIcon.SetActive(acquire);
        inProgressQuestIcon.SetActive(inProgress);
        completeQuestIcon.SetActive(complete);
    }
    public void StartDialog()
    {
        dialogCanvas.enabled = true;
        currentLine = 0;

        if (typewriterCoroutine != null)
            StopCoroutine(typewriterCoroutine);

        typewriterCoroutine = StartCoroutine(TypeLine(dialogData.dialogLines[currentLine]));

        allowInput = false;
        StartCoroutine(EnableInputNextFrame());
    }

    private IEnumerator EnableInputNextFrame()
    {
        yield return null; // Wait one frame to avoid initial input trigger
        allowInput = true;
    }

    private void Update()
    {
        SetQuestIcons();
        if (!dialogCanvas.enabled || !allowInput) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (isTyping)
            {
                skipTyping = true;
            }
            else
            {
                currentLine++;

                if (currentLine < dialogData.dialogLines.Length)
                {
                    if (typewriterCoroutine != null)
                        StopCoroutine(typewriterCoroutine);

                    typewriterCoroutine = StartCoroutine(TypeLine(dialogData.dialogLines[currentLine]));
                }
                else
                {
                    EndDialog();
                }
            }
        }
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogText.text = "";

        foreach (char c in line)
        {
            if (skipTyping)
            {
                dialogText.text = line;
                break;
            }

            dialogText.text += c;

            if (typeSoundClip != null && audioSource != null)
            {
                if (Time.time - lastSoundTime >= typeSoundCooldown)
                {
                    audioSource.PlayOneShot(typeSoundClip);
                    lastSoundTime = Time.time;
                }
            }

            yield return new WaitForSeconds(typewriterSpeed);
        }

        isTyping = false;
        skipTyping = false;
        typewriterCoroutine = null;
    }

    public void EndDialog()
    {
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }

        isTyping = false;
        skipTyping = false;
        allowInput = false;

        if (audioSource != null)
            audioSource.Stop();

        if (dialogData != null)
        {
            QuestManager qm = FindObjectOfType<QuestManager>();

            switch (dialogData.dialogType)
            {
                case DialogType.AcquireQuest:
                    if (dialogData.relatedQuest != null)
                        qm.AddQuest(dialogData.relatedQuest);
                    break;

                case DialogType.CompleteQuest:
                    if (dialogData.relatedQuest != null)
                        qm.CompleteQuest(dialogData.relatedQuest.questName);
                    break;

                case DialogType.InProgressQuest:
                    if (dialogData.relatedQuest != null && dialogData.relatedQuest.questType == QuestType.Talk)
                    {
                        qm.MarkObjectiveCompleteForTalkQuest(dialogData.relatedQuest.questName);
                        GetComponentInChildren<InteractableNPCBehaviour>()?.ForceNPCInteraction(); // Force Next Dialog - if quest is Talk quest
                        return;
                    }
                    else break;
                // Return instead of break to not disable canvas in this situation
                case DialogType.None:
                default:
                    break;
            }
        }

        dialogCanvas.enabled = false;
        dialogText.text = "";
        GetComponentInChildren<InteractableNPCBehaviour>()?.OnDialogEnded();
    }
    public void InterruptDialog()
    {
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }

        isTyping = false;
        skipTyping = false;
        allowInput = false;

        if (audioSource != null)
            audioSource.Stop();

        dialogCanvas.enabled = false;
        dialogText.text = "";

        GetComponentInChildren<InteractableNPCBehaviour>()?.OnDialogEnded();
    }
}
