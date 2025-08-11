using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillWindowController : MonoBehaviour
{
    public List<GameObject> menus;
    public AudioSource audioSource;
    public AudioClip openClip;
    public AudioClip closeClip;
    //public List<GameObject> activeMenus;
    public Canvas canvas;

    private void Start()
    {
        audioSource = GameObject.Find("MenusSoundSource").GetComponent<AudioSource>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && canvas.gameObject.GetComponent<Canvas>().enabled == true)
        {
            audioSource.PlayOneShot(closeClip);
            canvas.gameObject.GetComponent<Canvas>().enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            if (canvas.gameObject.GetComponent<Canvas>().enabled == true)
            {
                audioSource.PlayOneShot(closeClip);
                canvas.gameObject.GetComponent<Canvas>().enabled = false;
            }
            else
            {
                audioSource.PlayOneShot(openClip);
                canvas.gameObject.GetComponent<Canvas>().enabled = true;
            }
        }
    }
}
