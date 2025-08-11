using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaftMovementController : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 0.5f; // Adjust as needed for slow drifting

    void Update()
    {
        MoveNorth();
    }

    private void MoveNorth()
    {
        transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);
    }
}
