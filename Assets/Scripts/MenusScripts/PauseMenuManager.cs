using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenuUI;
    [SerializeField] private AudioManager _audioManager;

    private bool _isPaused = false;

    private void Update()
    {
        //Escape = escape key on keyboard and "Cancel" is the default input name for start/pause
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Cancel"))
        {
            if (_isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    /// <summary>
    /// Pauses the game by freezing time, showing the pause UI, and pausing the music.
    /// </summary>
    public void PauseGame()
    {

        if(_pauseMenuUI != null)
        {
            _pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            _isPaused = true;

            if(_audioManager != null)
            {
                _audioManager.PauseMusic();
            }
        }

    }

    /// <summary>
    /// Resumes the game by restoring time, hiding the pause UI, and resuming the music.
    /// </summary>
    public void ResumeGame()
    {
        if(_pauseMenuUI != null)
        {
            _pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            _isPaused = false;

            if(_audioManager != null)
            {
                _audioManager.ResumeMusic();
            }
        }
    }

    /// <summary>
    /// Hides the pause UI, restores time, and transitions to the main menu scene.
    /// </summary>
    public void LoadMainMenu()
    {
        if(_pauseMenuUI != null)
        {
            _pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            _isPaused = false;
        }

        FadeTransition fadeTransition = FindFirstObjectByType<FadeTransition>();
        if (fadeTransition != null)
        {
            fadeTransition.FadeToScene(0);
        }
    }
}
