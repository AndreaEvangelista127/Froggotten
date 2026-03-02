using UnityEngine;

public class CollectiblesManager : MonoBehaviour
{
    [Header("Collectibles Tracking")]
    [SerializeField] private Collectibles[] _allFlies;
    private int _totalFlies = 0;
    private int _collectedFlies = 0;

    [Header("References")]
    [SerializeField] private WinManager _winManager;
    [SerializeField] private PlayerCollisions _playerCollisions;

    [Header("Debug")]
    [SerializeField] private bool _cheatWin = false;


    private bool _hasWon = false;

    private void Start()
    {
        CountAllFlies();
        
    }

    private void Update()
    {
        if (_cheatWin)
        {
            _cheatWin = false;
            _hasWon = true;
            if (_winManager != null)
                _winManager.UnlockTrophy();
            return;
        }

        if (_playerCollisions == null) return;

        _collectedFlies = _playerCollisions._currentFliesCollected;

        CheckWinCondition();
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


}
