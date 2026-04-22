using UnityEngine;
using UnityEngine.EventSystems;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private GameObject _firstSelected;
    [SerializeField] private FadeTransition _fadeTransition;

    private void Start()
    {
        if (_gameOverPanel != null)
            _gameOverPanel.SetActive(false);
    }

    /// <summary>
    /// Shows the Game Over panel and sets controller focus on the first button.
    /// </summary>
    public void ShowGameOver()
    {
        if (_gameOverPanel != null)
            _gameOverPanel.SetActive(true);

        AudioManager.Instance.StopFootsteps();

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(_firstSelected);
    }

    /// <summary>
    /// Restarts the current level.
    /// </summary>
    public void Restart()
    {
        Time.timeScale = 1f;
        if (_fadeTransition != null)
            _fadeTransition.FadeToScene(1);
    }

    /// <summary>
    /// Returns to the main menu.
    /// </summary>
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        if (_fadeTransition != null)
            _fadeTransition.FadeToScene(0);
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
