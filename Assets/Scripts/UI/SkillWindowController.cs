using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillWindowController : MonoBehaviour
{
    public List<GameObject> menus;
    //public List<GameObject> activeMenus;
    public Canvas canvas;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            canvas.gameObject.GetComponent<Canvas>().enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.K))
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
