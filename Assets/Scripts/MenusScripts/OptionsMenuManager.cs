using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionsMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject _optionsPanel;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private GameObject _firstMainMenuButton;

    /// <summary>
    /// Opens the options panel and sets controller focus on the music slider.
    /// Called by the Options button OnClick event.
    /// </summary>
    public void OpenOptions()
    {
        _optionsPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_musicSlider.gameObject);
    }

    /// <summary>
    /// Closes the options panel and restores controller focus to the main menu.
    /// Called by the Back button OnClick event.
    /// </summary>
    public void CloseOptions()
    {
        _optionsPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(_firstMainMenuButton);
    }
}