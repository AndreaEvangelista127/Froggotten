using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenuUI;
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private GameObject _pauseFirstSelected;
    [SerializeField] private FadeTransition _fadeTransition;

    //to stop the player movement when the game is paused, we can check this variable in the player movement script
    public static bool IsGamePaused { get; private set; }


    private bool _isPaused = false;

    /// <summary>
    /// Called by the Player Input component when the Pause action is triggered.
    /// Toggles between paused and resumed state.
    /// </summary>
    /// <param name="context">The input callback context from the Input System.</param>
    public void OnPause(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (_isPaused) ResumeGame();
        else PauseGame();
    }

    /// <summary>
    /// Pauses the game by freezing time, showing the pause UI, and pausing the music.
    /// </summary>
    public void PauseGame()
    {
        if (_pauseMenuUI == null) return;

        _pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        _isPaused = true;
        IsGamePaused = true;

        // Set controller focus to the first element of the pause menu
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(_pauseFirstSelected);

        if (_audioManager != null)
            _audioManager.PauseMusic();
    }

    /// <summary>
    /// Resumes the game by restoring time, hiding the pause UI, and resuming the music.
    /// </summary>
    public void ResumeGame()
    {
        if (_pauseMenuUI == null) return;

        _pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        _isPaused = false;
        IsGamePaused = false;

        // After opening the pause menu the first button where the controller is going to be, would be _pausefirstselected
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(_pauseFirstSelected);

        if (_audioManager != null)
            _audioManager.ResumeMusic();
    }

    /// <summary>
    /// Hides the pause UI, restores time, and transitions to the main menu scene.
    /// </summary>
    public void LoadMainMenu()
    {
        if (_pauseMenuUI != null)
        {
            _pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            _isPaused = false;
            IsGamePaused = false;
        }

        if (_fadeTransition != null)
            _fadeTransition.FadeToScene(0);
    }

    public void RestartLevel()
    {
        if (_pauseMenuUI != null)
        {
            _pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            _isPaused = false;
            IsGamePaused = false;
        }
        if (_fadeTransition != null)
            _fadeTransition.FadeToScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Called by the Player Input component when the Cancel action is triggered.
    /// Closes the pause menu if currently paused (B button on Xbox gamepad).
    /// </summary>
    public void OnCancel(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (_isPaused) ResumeGame();
    }

}