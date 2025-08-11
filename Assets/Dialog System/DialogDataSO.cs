using UnityEngine;

public enum DialogType
{
    None,
    AcquireQuest,
    CompleteQuest,
    InProgressQuest,
    PlaceHolder,
}

[CreateAssetMenu(fileName = "NewDialog", menuName = "Dialog System/Dialog")]
public class DialogDataSO : ScriptableObject
{
    [TextArea(2, 5)]
    public string[] dialogLines;

    public DialogType dialogType;
    public QuestSO relatedQuest;
}
