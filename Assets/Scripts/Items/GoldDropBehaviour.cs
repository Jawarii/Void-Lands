using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldDropBehaviour : MonoBehaviour
{
    public float goldAmount = 0;
    public float direction = 1;
    private float persistDuration = 300f;
    void Start()
    {
        Destroy(gameObject, persistDuration);
    }
}
