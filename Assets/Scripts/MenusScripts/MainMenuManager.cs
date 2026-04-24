using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{

    [SerializeField] private Rigidbody2D _frogRb;
    [SerializeField] private FadeTransition _fadeTransition;

    [Header("Panels")]
    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private GameObject _levelSelectPanel;

    /// <summary>
    /// Starts the game by triggering the frog fall animation followed by a scene transition.
    /// </summary>
    public void PlayGame()
    {
        StartCoroutine(FrogFallAndLoad());
    }

    /// <summary>
    /// Increases the frog's gravity to make it fall, waits briefly, then fades to the game scene.
    /// </summary>
    IEnumerator FrogFallAndLoad()
    {
        if (_frogRb != null)
            _frogRb.gravityScale = 100;

        yield return new WaitForSeconds(0.8f);

        if (_fadeTransition != null)
            _fadeTransition.FadeToScene(1);
    }

    public void OpenLevelSelect()
    {
        _mainMenuPanel.SetActive(false);
        _levelSelectPanel.SetActive(true);
    }

    public void CloseLevelSelect()
    {
        _levelSelectPanel.SetActive(false);
        _mainMenuPanel.SetActive(true);
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

}
