using Unity.VisualScripting;
using UnityEngine;

public class DynamicSortingOrder : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public int sortingOrderBase = 999999;
    public float offset = 0;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        UpdateSortingOrder();
    }

    private void UpdateSortingOrder()
    {
        if (gameObject.CompareTag("BowObject"))
        {
            // For the bow, use the player's sorting order directly with a small rotation-based offset
            int playerSortingOrder = CalculatePlayerSortingOrder();
            float rotationOffset = CalculateRotationOffset();
            spriteRenderer.sortingOrder = playerSortingOrder + (int)rotationOffset;
        }
        else if (gameObject.CompareTag("Player"))
        {
            // For the player, use the player's sorting order directly with a small rotation-based offset
            int playerSortingOrder = CalculatePlayerSortingOrder();
            spriteRenderer.sortingOrder = playerSortingOrder;
        }
        else
        {
            // For other objects, use their collider top point for dynamic sorting
            float topEdge = GetColliderTopPoint();
            spriteRenderer.sortingOrder = (int)(sortingOrderBase - topEdge * 100) - (int)offset;
                                          
        }
    }

    // Method to calculate the sorting order based on the top of the collider
    private float GetColliderTopPoint()
    {
        Collider2D pivotCollider = transform.Find("PivotPoint")?.GetComponent<Collider2D>();
        if (pivotCollider != null)
        {
            float topEdge = pivotCollider.bounds.center.y;
            return topEdge;
        }
        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null)
        {
            collider = GetComponentInParent<Collider2D>();
            if (collider == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                return player.transform.position.y + 2f; // Use transform position if no collider found
            }
        }
        return collider.bounds.center.y;
    }

    // Method to calculate the player's sorting order based on the top of the "PivotPoint" collider
    private int CalculatePlayerSortingOrder()
    {
        if (!gameObject.CompareTag("Player") && !gameObject.CompareTag("BowObject"))
            return sortingOrderBase;

        Collider2D pivotCollider;
        if (gameObject.CompareTag("Player"))
        {
            pivotCollider = transform.Find("PivotPoint")?.GetComponent<Collider2D>();
        }
        else
        {
            pivotCollider = transform.parent.Find("PivotPoint")?.GetComponent<Collider2D>();
        }

        if (pivotCollider == null)
            return sortingOrderBase;

        float topEdge = pivotCollider.bounds.center.y;
        return (int)(sortingOrderBase - topEdge * 100) - (int)offset;
    }

    private float CalculateRotationOffset()
    {
        float angle = transform.eulerAngles.z + 90f;
        angle = angle > 180 ? angle - 360 : angle;
        float offset = Mathf.Sin(angle * Mathf.Deg2Rad);
        return offset * 1.5f; // Adjust this factor as needed for subtlety
    }
}
