using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText;

    private float _elapsedTime = 0f;
    private bool _isRunning = true;

    private void Update()
    {
        if (!_isRunning) return;

        _elapsedTime += Time.deltaTime;
        UpdateTimerText();
    }

    /// <summary>
    /// Stops the timer and returns the elapsed time as a formatted string.
    /// Called when the player wins.
    /// </summary>
    public string StopAndGetTime()
    {
        _isRunning = false;
        return GetFormattedTime();
    }

    /// <summary>
    /// Updates the timer UI text with the current elapsed time in MM:SS format.
    /// </summary>
    private void UpdateTimerText()
    {
        if (_timerText != null)
            _timerText.text = GetFormattedTime();
    }

    /// <summary>
    /// Converts elapsed seconds into a MM:SS formatted string.
    /// </summary>
    private string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(_elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(_elapsedTime % 60f);
        return $"{minutes:00}:{seconds:00}";
    }
}
