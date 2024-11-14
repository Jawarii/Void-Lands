using NavMeshPlus.Components;
using UnityEngine;
using UnityEngine.AI;

public class DynamicNavMeshController : MonoBehaviour
{
    public NavMeshSurface Surface2D; // Reference to the NavMesh Surface
    private NavMeshData navMeshData; // Holds the NavMesh data
    private Bounds navMeshBounds = new Bounds(Vector3.zero, new Vector3(100, 100, 100)); // Adjust bounds size as needed
    private bool needsUpdate = false; // Flag to determine if NavMesh update is needed
    private float updateCooldown = 0.2f; // Cooldown time in seconds
    private float lastUpdateTime = 0f; // Tracks the last time UpdateNavMesh was called

    void Start()
    {
        // Initialize NavMesh data
        navMeshData = new NavMeshData();
        Surface2D.navMeshData = navMeshData;

        // Force sync of collider transforms to get accurate bounds
        Physics2D.SyncTransforms();

        // Build the initial NavMesh asynchronously
        Surface2D.BuildNavMeshAsync();
        FlagNavMeshUpdate();
    }

    void LateUpdate()
    {
        // Check if cooldown has passed
        if (Time.time - lastUpdateTime >= updateCooldown)
        {
            // Sync collider bounds if necessary
            Physics2D.SyncTransforms();

            // Perform NavMesh update
            UpdateNavMesh();

            // Update the last update time
            lastUpdateTime = Time.time;
        }
    }

    // Call this method to flag the NavMesh for updating
    public void FlagNavMeshUpdate()
    {
        needsUpdate = true;
    }

    // Updates the NavMesh using NavMeshSurface2D’s built-in update mechanism
    private void UpdateNavMesh()
    {
        if (needsUpdate)
        {
            Surface2D.UpdateNavMesh(Surface2D.navMeshData); 
            //needsUpdate = false; // Reset the update flag after updating
        }
    }
}
