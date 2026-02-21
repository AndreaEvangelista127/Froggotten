using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LivesUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _livesText;
    [SerializeField] private Image _frogImage;

    /// <summary>
    /// Updates the lives counter text displayed next to the frog icon.
    /// </summary>
    /// <param name="currentLives">The number of lives remaining.</param>
    public void UpdateLives(int currentLives)
    {
        _livesText.text = $" x {currentLives}";
    }
}
