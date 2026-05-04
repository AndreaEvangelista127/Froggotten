using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Automatically selects the first interactable Selectable (Button, Slider,
/// Toggle, etc.) in this panel when it becomes active, enabling gamepad
/// navigation without manual setup.
/// </summary>
public class AutoSelectFirstButton : MonoBehaviour
{
    private void OnEnable()
    {
        // Find the first active and interactable UI element of any type
        Selectable firstSelectable = GetComponentInChildren<Selectable>();

        if (firstSelectable != null && firstSelectable.interactable)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelectable.gameObject);
        }
    }
}
