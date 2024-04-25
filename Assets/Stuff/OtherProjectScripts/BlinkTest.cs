using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkTest : MonoBehaviour
{
    public float blinkCd = 2.0f;
    public float blinkDur = 0.1f;
    private bool canBlink = true;

    void Update()
    {
        blinkCd -= Time.deltaTime;
        if (blinkCd <= 0)
        {
            if (canBlink)
            {
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y / 4.0f);
                canBlink = false;
                blinkDur = 0.1f;
            }
            blinkDur -= Time.deltaTime;
            if (blinkDur <= 0f)
            {
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 4.0f);
                blinkCd = 2.0f;
                canBlink = true;
            }
        }
    }
}
