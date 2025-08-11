using UnityEngine;
using UnityEngine.UI;

public class DestroyConfirmationPanel : MonoBehaviour
{
    public Button confirmButton;
    public Button cancelButton;

    private DraggableItemBehaviour currentDraggable;

    public void Show(DraggableItemBehaviour item)
    {
        currentDraggable = item;
        gameObject.SetActive(true);

        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();

        confirmButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            currentDraggable.OnConfirmDestroy();
        });

        cancelButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            currentDraggable.OnCancelDestroy();
        });
    }
}
