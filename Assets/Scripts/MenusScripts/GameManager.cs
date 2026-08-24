using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private GameObject _firstSelected;
    [SerializeField] private FadeTransition _fadeTransition;
    [SerializeField] private int _mainMenuSceneIndex = 0;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
    }

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

        Time.timeScale = 0f;
    }

    /// <summary>
    /// Restarts the current level.
    /// </summary>
    public void Restart()
    {
        Time.timeScale = 1f;
        if (_fadeTransition != null)
            _fadeTransition.FadeToScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Returns to the main menu.
    /// </summary>
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        if (_fadeTransition != null)
            _fadeTransition.FadeToScene(_mainMenuSceneIndex);
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
