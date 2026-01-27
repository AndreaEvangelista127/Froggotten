using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class FadeTransition : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

    public void FadeToMainMenu()
    {
        StartCoroutine(FadeAndLoadMenu());
    }


    /* IEnumerator that can pause and resume execution */
    IEnumerator FadeAndLoadMenu()
    {
        float timer = 0f;


        while (timer < fadeDuration) // timer less than duration? continue the fade
        {

            timer += Time.unscaledDeltaTime; // use unscaled time to ignore time scale changes (like pausing)

            //calculate alpha value from 0 to 1 based on timer progress
            float alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration); // EX: timer = 0.016 / 1 = 0.016 -> alpha = 0.016
                                                                    // EX: timer = 0.5 / 1 = 0.5 -> alpha = 0.5
                                                                    // EX: timer = 1 / 1 = 1 -> alpha = 1

            fadeImage.color = new Color(0, 0, 0, alpha); //increase alpha of black image

            yield return null; // wait for next frame
        }

        // black screen
        fadeImage.color = new Color(0, 0, 0, 1);

        // load main menu scene
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);

    }
}
