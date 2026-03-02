using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionsMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject _optionsPanel;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private GameObject _firstMainMenuButton;


    private void Start()
    {
        // Deselect any preselected button on scene load
        // The player's mouse movement will handle hover naturally
        EventSystem.current.SetSelectedGameObject(null);
    }
    /// <summary>
    /// Opens the options panel and sets controller focus on the music slider.
    /// Called by the Options button OnClick event.
    /// </summary>
    public void OpenOptions()
    {
        if (_optionsPanel == null) return;

        _optionsPanel.SetActive(true);
        if (EventSystem.current != null && _musicSlider != null)
            EventSystem.current.SetSelectedGameObject(_musicSlider.gameObject);
    }

    /// <summary>
    /// Closes the options panel and restores controller focus to the main menu.
    /// Called by the Back button OnClick event.
    /// </summary>
    public void CloseOptions()
    {
        if (_optionsPanel == null) return;

        _optionsPanel.SetActive(false);
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(_firstMainMenuButton);
    }
}