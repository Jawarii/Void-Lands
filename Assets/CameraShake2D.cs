using Cinemachine;
using System.Collections;
using UnityEngine;

public class CameraShake2D : MonoBehaviour
{
    [SerializeField] private CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }
    public void ShakeTest()
    {
        impulseSource.GenerateImpulse();
    }

}
