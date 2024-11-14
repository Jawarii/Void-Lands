using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    public Texture2D cursorTexture; // Your cursor texture
    public Vector2 hotSpot = Vector2.zero; // The hotspot is the "click point" on the cursor

    void Start()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
    }

    void OnDisable()
    {
        // Optionally reset the cursor when this script is disabled
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
