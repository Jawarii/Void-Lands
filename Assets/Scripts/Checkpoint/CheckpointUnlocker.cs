using UnityEngine;

public class CheckpointUnlocker : MonoBehaviour
{
    [SerializeField] private CheckPoint checkPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Trigger entered by: {other.gameObject.name}");

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player touched checkpoint.");
            checkPoint.Unlock();
        }
    }
}
