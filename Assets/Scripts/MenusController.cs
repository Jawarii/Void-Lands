using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MenusController : MonoBehaviour
{
    public List<GameObject> menus;
    public List<GameObject> activeMenus;
    public Canvas canvas;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    if (activeMenus!= null && activeMenus.Count > 0)
        //    {
        //        activeMenus[activeMenus.Count - 1].gameObject.SetActive(false);
        //        activeMenus.RemoveAt(activeMenus.Count - 1);
        //    }
        //}   
        //if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.B))
        //{
        //    if (menus[0].gameObject.activeSelf == true)
        //    {
        //        menus[0].gameObject.SetActive(false);
        //    }
        //    else
        //    {
        //        menus[0].gameObject.SetActive(true);
        //        activeMenus.Add(menus[0]);
        //    }
        //}

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (activeMenus != null && activeMenus.Count > 0)
            {
                canvas.gameObject.GetComponent<Canvas>().enabled = false;
                activeMenus.RemoveAt(activeMenus.Count - 1);
            }
        }
        if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.B))
        {
            if (canvas.gameObject.GetComponent<Canvas>().enabled == true)
            {
                canvas.gameObject.GetComponent<Canvas>().enabled = false;
                activeMenus.RemoveAt(activeMenus.Count - 1);
            }
            else
            {
                canvas.gameObject.GetComponent<Canvas>().enabled = true;
                activeMenus.Add(menus[0]);
            }
        }
    }
}
