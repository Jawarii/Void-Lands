using UnityEngine;

public class DynamicSortingOrder : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public int sortingOrderBase = 999999;
    public float offset = 0;
    private GameObject _player;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        _player = GameObject.FindGameObjectWithTag("Player");
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
        else
        {
            // For other objects, continue using their bottom edge for dynamic sorting
            float bottomEdge = transform.position.y - (spriteRenderer.bounds.size.y / 2);
            spriteRenderer.sortingOrder = (int)(sortingOrderBase - bottomEdge * 100) - (int)offset;
        }
    }

    // New method to calculate the player's sorting order based on its bottom edge
    private int CalculatePlayerSortingOrder()
    {
        float bottomEdge = _player.transform.position.y - (_player.GetComponent<SpriteRenderer>().bounds.size.y / 2);
        return (int)(sortingOrderBase - bottomEdge * 100) - (int)offset;
    }

    private float CalculateRotationOffset()
    {
        float angle = transform.eulerAngles.z + 90f;
        angle = angle > 180 ? angle - 360 : angle;
        float offset = Mathf.Sin(angle * Mathf.Deg2Rad);
        return offset * 1.5f; // Adjust this factor as needed for subtlety
    }
}
