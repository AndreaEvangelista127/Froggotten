using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{

    [SerializeField] private Rigidbody2D _frogRb;
    [SerializeField] private FadeTransition _fadeTransition;

    public void PlayGame()
    {
        StartCoroutine(FrogFallAndLoad());
    }

    IEnumerator FrogFallAndLoad()
    {
        _frogRb.gravityScale = 100;

        yield return new WaitForSeconds(0.8f);

        _fadeTransition.FadeToMainMenu(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
