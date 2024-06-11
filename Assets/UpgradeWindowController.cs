using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeWindowController : MonoBehaviour
{
    public List<GameObject> menus;
    public Canvas canvas;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            canvas.gameObject.GetComponent<Canvas>().enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            if (canvas.gameObject.GetComponent<Canvas>().enabled == true)
            {
                canvas.gameObject.GetComponent<Canvas>().enabled = false;

            }
            else
            {
                canvas.gameObject.GetComponent<Canvas>().enabled = true;
            }
        }
    }
}
