using UnityEngine;

/// <summary>
/// Shows a hint canvas when the player enters the trigger zone,
/// but only if the player has collected fewer flies than the required threshold.
/// Place one instance near each area where players tend to miss flies.
/// </summary>
public class HintZone : MonoBehaviour
{
    [Header("Hint Settings")]
    // The hint will only appear if the player has collected strictly fewer flies than this value.
    // Example: set to 3 if this zone is after the 3rd fly — if the player has 0, 1, or 2 flies, hint shows.
    [SerializeField] private int _requiredFliesBeforeHint = 1;
    [SerializeField] private bool _alwaysShow = false;

    [Header("References")]
    [SerializeField] private PlayerCollisions _playerCollisions;

    private Canvas _hintCanvas;

    private void Start()
    {
        _hintCanvas = GetComponentInChildren<Canvas>();

        if (_hintCanvas == null)
        {
            Debug.LogWarning("HintZone: No Canvas found in children.");
            return;
        }

        _hintCanvas.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (ShouldShowHint())
        {
            if (_hintCanvas != null) _hintCanvas.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (_hintCanvas != null) _hintCanvas.gameObject.SetActive(false);
    }

    /// <summary>
    /// Returns true if the hint should be displayed.
    /// Always true when _alwaysShow is enabled.
    /// Otherwise checks the player's fly count against the threshold.
    /// </summary>
    private bool ShouldShowHint()
    {
        if (_alwaysShow) return true;

        if (_playerCollisions == null)
        {
            Debug.LogWarning("HintZone: PlayerCollisions not assigned and _alwaysShow is false.");
            return false;
        }

        return _playerCollisions._currentFliesCollected < _requiredFliesBeforeHint;
    }
}