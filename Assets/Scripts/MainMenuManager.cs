using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{

    [SerializeField] private Rigidbody2D _frogRb;
    [SerializeField] private float fallDelay = 4f;

    public void PlayGame()
    {
        StartCoroutine(FrogFallAndLoad());
    }

    IEnumerator FrogFallAndLoad()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);

        operation.allowSceneActivation = false; // Load but not activate

        // Fai cadere la rana
        _frogRb.gravityScale = 100; 

        // Aspetta che cada
        yield return new WaitForSeconds(fallDelay);

        operation.allowSceneActivation = true;

        yield return new WaitUntil(()=> operation.isDone); //wait until the scene is activated

        operation = SceneManager.UnloadSceneAsync(0); // and then unload menu

    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
