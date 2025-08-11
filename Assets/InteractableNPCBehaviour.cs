using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableNPCBehaviour : MonoBehaviour
{
    public Canvas interactableCanvas;
    public Canvas dialogCanvas;
    public DialogHandler dialogHandler;

    [Header("Dialog Conditions (Ordered by Priority)")]
    public List<DialogCondition> dialogConditions = new();

    [Header("Fallback if no condition is met")]
    public DialogDataSO defaultDialog;

    private bool reactivationDelay = false;
    private QuestManager questManager;

    [System.Serializable]
    public class DialogCondition
    {
        public string questName;
        public DialogDataSO offerQuestDialog;
        public DialogDataSO inProgressDialog;
        public DialogDataSO onCompleteDialog;
        public bool skipIfRewarded = true;
    }

    private void Start()
    {
        questManager = FindObjectOfType<QuestManager>();
    }

    public void InvokeNPCInteraction()
    {
        if (interactableCanvas != null && interactableCanvas.enabled)
        {
            UpdateCurrentDialog();
            dialogHandler.StartDialog();
            interactableCanvas.enabled = false;
        }
    }

    public void ForceNPCInteraction()
    {
        if (interactableCanvas != null)
        {
            UpdateCurrentDialog();
            dialogHandler.StartDialog();
            interactableCanvas.enabled = false;
        }
    }

    private void UpdateCurrentDialog()
    {
        foreach (var condition in dialogConditions)
        {
            if (string.IsNullOrEmpty(condition.questName))
                continue;

            bool hasQuest = questManager.HasQuest(condition.questName);
            bool isComplete = questManager.IsObjectiveCompleted(condition.questName);
            bool isRewarded = questManager.IsRewarded(condition.questName);

            if (isRewarded && condition.skipIfRewarded)
                continue;

            if (hasQuest)
            {
                if (!isComplete)
                {
                    dialogHandler.dialogData = condition.inProgressDialog;
                    return;
                }
                else
                {
                    dialogHandler.dialogData = condition.onCompleteDialog;
                    return;
                }
            }
            else
            {
                dialogHandler.dialogData = condition.offerQuestDialog;
                return;
            }
        }

        dialogHandler.dialogData = defaultDialog;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            TryEnableInteractCanvas();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            TryEnableInteractCanvas();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && interactableCanvas != null)
        {
            dialogHandler.InterruptDialog();
            interactableCanvas.enabled = false;
            interactableCanvas.GetComponentInChildren<InteractButtonAnimator>().interactableGo = null;
        }
    }

    private void TryEnableInteractCanvas()
    {
        if (reactivationDelay)
            return;

        if (interactableCanvas != null && dialogCanvas != null)
        {
            if (!dialogCanvas.enabled)
            {
                interactableCanvas.enabled = true;
                interactableCanvas.GetComponentInChildren<InteractButtonAnimator>().interactableGo = gameObject;
            }
            else
            {
                interactableCanvas.enabled = false;
            }
        }
    }

    public void OnDialogEnded(float delay = 0.0f)
    {
        return; // Bugged for now, dont use.
        //StartCoroutine(ReactivateCanvasAfterDelay(delay));
    }
    private void Update()
    {
        UpdateCurrentDialog();
    }
}
