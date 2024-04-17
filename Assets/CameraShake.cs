using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private float shakeDuration = 0.03f;
    private float currentShakeDuration = 0f;

    void Start()
    {
        currentShakeDuration = shakeDuration;
    }
    // Update is called once per frame
    void Update()
    {
        if (currentShakeDuration < shakeDuration)
        {
            currentShakeDuration += Time.deltaTime;
        }
        else
        {
            gameObject.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 4.0f;
        }
    }
    public void ShakeCamera()
    {
        if (gameObject.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize != 4f)
            return;
        gameObject.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 3.97f;
        currentShakeDuration = 0f;
    }

}
