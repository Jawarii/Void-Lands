using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MenusController : MonoBehaviour
{
    public List<GameObject> menus;
    public List<GameObject> activeMenus;
    public Canvas canvas;
    public AudioSource audioSource;
    public AudioClip inventoryOpenClip;
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
            canvas.gameObject.GetComponent<Canvas>().enabled = false;
        }
        if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.B))
        {
            if (canvas.gameObject.GetComponent<Canvas>().enabled == true)
            {
                canvas.gameObject.GetComponent<Canvas>().enabled = false;
                if (activeMenus.Count > 0)
                {
                    activeMenus.RemoveAt(activeMenus.Count - 1);
                }
            }
            else
            {
                canvas.gameObject.GetComponent<Canvas>().enabled = true;
                audioSource.PlayOneShot(inventoryOpenClip);
                activeMenus.Add(menus[0]);
            }
        }
    }
}
