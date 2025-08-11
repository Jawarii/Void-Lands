using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public List<GameObject> menus; // Optional: Add multiple menus to manage
    public Canvas canvas; // Reference to the menu's canvas

    private void Start()
    {
        canvas = GetComponent<Canvas>();
        // Ensure the canvas is initialized as disabled
        if (canvas != null)
        {
            canvas.enabled = false;
        }
    }

    private void Update()
    {
        // Close the menu when Escape is pressed
        if (Input.GetKeyDown(KeyCode.Escape) && canvas.enabled == true)
        {
            canvas.enabled = false;
        }

        // Toggle the menu on/off when "O" is pressed
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (canvas != null)
            {
                canvas.enabled = !canvas.enabled;
            }
        }
    }
}
