using UnityEngine;

public class CollectiblesManager : MonoBehaviour
{
    [Header("Collectibles Tracking")]
    [SerializeField] private Fly[] _allFlies;
    private int _totalFlies = 0;
    private int _collectedFlies = 0;

    [Header("References")]
    [SerializeField] private WinManager _winManager;

    [Header("Debug")]
    [SerializeField] private bool _cheatWin = false;


    private bool _hasWon = false;

    private void Start()
    {
        CountAllFlies();
        SubscribeToFlies();
    }

    private void Update()
    {
        // Only handles the debug cheat now — real win checks happen
        // event-driven in OnFlyCollected, not polled every frame.
        if (_cheatWin)
        {
            _cheatWin = false;
            _hasWon = true;
            if (_winManager != null)
                _winManager.UnlockTrophy();
        }
    }

    private void CountAllFlies()
    {
        if (_allFlies == null)
        {
            _totalFlies = 0;
            Debug.LogWarning("CollectiblesManager: _allFlies array not assigned!");
            return;
        }
        _totalFlies = _allFlies.Length;
    }

    /// <summary>
    /// Subscribes to every Fly's Collected event, so this manager is notified
    /// immediately on pickup instead of polling PlayerCollisions every frame.
    /// </summary>
    private void SubscribeToFlies()
    {
        if (_allFlies == null) return;

        foreach (Fly fly in _allFlies)
        {
            if (fly != null)
            {
                // Whenever a fly is collected, this manager will be notified and can update the count.
                fly.Collected += OnFlyCollected;
            }
        }
    }

    private void OnFlyCollected(Fly fly)
    {
        _collectedFlies++;
        CheckWinCondition();
    }

    /// <summary>
    /// Checks if the player has collected all flies and unlocks the trophy if not already won.
    /// </summary>
    private void CheckWinCondition()
    {
        if (_collectedFlies >= _totalFlies && !_hasWon)
        {
            _hasWon = true;
            Debug.Log("CollectiblesManager: All flies collected! Trophy unlocked.");

            if (_winManager != null)
            {
                _winManager.UnlockTrophy();
            }
            else
            {
                Debug.LogWarning("CollectiblesManager: WinManager not assigned!");
            }
        }
    }


    public int GetTotalFlies()
    {
        return _totalFlies;
    }

    public int GetCollectedFlies()
    {
        return _collectedFlies;
    }

    public bool HasAllFlies()
    {
        return _collectedFlies >= _totalFlies;
    }

    private void OnDestroy()
    {
        if (_allFlies == null) return;

        foreach (Fly fly in _allFlies)
        {
            if (fly != null)
            {
                fly.Collected -= OnFlyCollected;
            }
        }
    }
}
