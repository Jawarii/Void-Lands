using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BowController : MonoBehaviour
{
    public AudioSource source_;
    public AudioClip clip_;
    void Update()
    {
        RotateObjectToMouse();
    }

    void RotateObjectToMouse()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, Camera.main.transform.position.z * -1));
        Vector3 directionToMouse = mouseWorldPosition - transform.position;

        float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle + 90f));
    }
    public void PlayStringPullClip()
    {
        if (source_ != null)
        {
            source_.Stop();
            source_.PlayOneShot(clip_);
        }
    }
}
