using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LootPopupBehaviour : MonoBehaviour
{
    private float floatDuration = 1.25f;
    void Start()
    {
        
    }

    void Update()
    {
        floatDuration -= Time.deltaTime;
        if (floatDuration <= 0.0f)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + Time.deltaTime * 0.1f, gameObject.transform.position.z);
            if ( floatDuration <= 0.25f)
            {
                Color textColor = gameObject.GetComponentInChildren<TMP_Text>().color;
                var tempColor = textColor;
                tempColor.a = floatDuration / 0.25f;
                gameObject.GetComponentInChildren<TMP_Text>().color = tempColor;
            }
        }
    }
}
