using UnityEngine;

public class SkyBackgroundController : MonoBehaviour
{
    public Transform player;
    public float parallaxFactor = 0.1f; // Smaller = less movement (further away feel)
    private Vector3 lastPlayerPos;

    private void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        lastPlayerPos = player.position;
    }

    private void LateUpdate()
    {
        Vector3 delta = player.position - lastPlayerPos;
        transform.position += new Vector3(delta.x * parallaxFactor, delta.y * parallaxFactor, 0f);
        lastPlayerPos = player.position;
    }
}
