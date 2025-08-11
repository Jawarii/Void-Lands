using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [Header("Visuals for Unlocked Checkpoint")]
    [SerializeField] public GameObject runesVisual;

    [Header("Initial State")]
    [SerializeField] public bool isUnlockedByDefault = false;

    public bool IsUnlocked { get; private set; } = false;

    private void Awake()
    {
        IsUnlocked = isUnlockedByDefault;
        UpdateVisual();

        if (isInvisible)
        {
            HideCheckpoint();
        }
    }

    [Header("Checkpoint Settings")]
    [SerializeField] public bool isInvisible = false;

    public void Unlock()
    {
        if (IsUnlocked) return;

        IsUnlocked = true;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (runesVisual != null)
            runesVisual.SetActive(IsUnlocked && !isInvisible);
    }

    private void HideCheckpoint()
    {
        foreach (var renderer in GetComponentsInChildren<SpriteRenderer>(true))
        {
            renderer.enabled = false;
        }

        foreach (var collider in GetComponentsInChildren<Collider2D>(true))
        {
            if (collider != null && collider != this.GetComponent<Collider2D>())
                collider.enabled = false;
        }
    }
}
