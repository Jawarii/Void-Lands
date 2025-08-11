using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public Vector3 GetClosestUnlockedCheckPoint(Vector3 playerPosition)
    {
        CheckPoint[] points = FindObjectsOfType<CheckPoint>();
        CheckPoint closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (CheckPoint point in points)
        {
            if (!point.IsUnlocked) continue;

            float distance = Vector3.Distance(playerPosition, point.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = point;
            }
        }

        return closest != null ? closest.transform.position : Vector3.zero;
    }
}
