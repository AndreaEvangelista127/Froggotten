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


    private bool _hasWon = false;

    private void Start()
    {
        CountAllFlies();
        
    }

    private void Update()
    {
        _collectedFlies = _playerCollisions._currentFliesCollected;

        CheckWinCondition();
    }

    private void CountAllFlies()
    {
        _totalFlies = _allFlies.Length;
    }

    private void CheckWinCondition()
    {
        // Se il player ha raccolto tutte le mosche e non ha ancora vinto
        if (_collectedFlies >= _totalFlies && !_hasWon)
        {
            _hasWon = true;

            Debug.Log($"CollectiblesManager: ? Tutte le mosche raccolte ({_collectedFlies}/{_totalFlies})! Trofeo sbloccato!");

            if (_winManager != null)
            {
                _winManager.UnlockTrophy();
            }
            else
            {
                Debug.LogWarning("CollectiblesManager: WinManager non assegnato!");
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
