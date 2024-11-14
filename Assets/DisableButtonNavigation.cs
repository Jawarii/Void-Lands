using UnityEngine;
using UnityEngine.UI;

public class DisableButtonNavigation : MonoBehaviour
{
    void Start()
    {
        Button[] buttons = FindObjectsOfType<Button>();
        foreach (Button button in buttons)
        {
            DisableNavigation(button);
        }
    }
    private void Update()
    {
        Button[] buttons = FindObjectsOfType<Button>();
        foreach (Button button in buttons)
        {
            DisableNavigation(button);
        }
    }
    void DisableNavigation(Button button)
    {
        Navigation nav = button.navigation;
        nav.mode = Navigation.Mode.None;
        button.navigation = nav;
    }
}
