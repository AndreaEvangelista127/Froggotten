using UnityEngine;
using System.Collections;

public class WinManager : MonoBehaviour
{
    [Header("Win Animation Settings")]
    [SerializeField] private string _winAnimationTrigger = "Win";
    [SerializeField] private Animator _winAnimator;

    [Header("Trophy Settings")]
    [SerializeField] private GameObject _trophyObject; 
    [SerializeField] private SpriteRenderer _trophySpriteRenderer;
    [SerializeField] private Color _lockedColor = new Color(0.5f, 0.5f, 0.5f, 0.5f); 
    [SerializeField] private Color _unlockedColor = Color.white;

    [Header("Player Win Settings")]
    [SerializeField] private float _trophyBounceForce = 15f;

    [Header("VFX")]
    [SerializeField] private GameObject _confettiPrefab;
    [SerializeField] private Transform _confettiSpawnPoint;

    [Header("Win UI")]
    [SerializeField] private GameObject _winPanelUI;          
    [SerializeField] private float _winPanelDelay = 1f;
    [SerializeField] private Transform _continuosConfettiSpawnPoint;

    [Header("References")]
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private FadeTransition _fadeTransition;


    private bool _isTrophyUnlocked = false;
    private bool _hasWon = false;


    private void Start()
    {
        if (_trophyObject != null)
        {
            SetTrophyLocked();
        }

        if (_winPanelUI != null)
            _winPanelUI.SetActive(false);
    }

    /// <summary>
    /// Unlocks the trophy, updating its visual appearance to the unlocked state.
    /// Should be called once all required collectibles have been gathered.
    /// </summary>
    public void UnlockTrophy()
    {
        _isTrophyUnlocked = true;

        if (_trophyObject != null)
        {
            SetTrophyUnlocked();
        }
    }

    /// <summary>
    /// Sets the trophy sprite color to the locked appearance.
    /// </summary>
    private void SetTrophyLocked()
    {
        if (_trophySpriteRenderer != null)
        {
            _trophySpriteRenderer.color = _lockedColor;
        }
    }

    /// <summary>
    /// Sets the trophy sprite color to the unlocked appearance.
    /// </summary>
    private void SetTrophyUnlocked()
    {
        if (_trophySpriteRenderer != null)
        {
            _trophySpriteRenderer.color = _unlockedColor;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log($"WinManager: Player jumped on the trophy. Unlocked: {_isTrophyUnlocked}, HasWon: {_hasWon}");

            if (_isTrophyUnlocked && !_hasWon)
            {

                TriggerWinSequence(collision.gameObject);

            }
            else
            {
                Debug.Log("WinManager:  Trophy still blocked. Collect all the flies!");
            }
        }
    }

    /// <summary>
    /// Triggers the full win sequence: plays the trophy animation, spawns confetti,
    /// bounces the player, triggers the player despawn, and schedules the win panel display.
    /// </summary>
    /// <param name="player">The player GameObject that reached the trophy.</param>
    private void TriggerWinSequence(GameObject player)
    {
        _hasWon = true;

        // Trophy animation
        if (_winAnimator != null)
        {
            _winAnimator.SetTrigger(_winAnimationTrigger);
        }

        VFXConfetti.SpawnConfettiWithTimer(_confettiPrefab, _confettiSpawnPoint.position);

        // Bounce player
        PlayerCollisions playerCollisions = player.GetComponent<PlayerCollisions>();
        if (playerCollisions != null)
        {
            playerCollisions.BouncePlayer(_trophyBounceForce);
        }

        // Despawn 
        StatePlayerMovement statePlayerMovement = player.GetComponent<StatePlayerMovement>();
        if (statePlayerMovement != null)
        {
            statePlayerMovement.TriggerDespawn();
        }

        StartCoroutine(ShowWinPanelDelayed());
    }

    /// <summary>
    /// Waits for a short delay, then displays the win panel, pauses the game, and plays the win sound.
    /// </summary>
    /// <returns>IEnumerator for coroutine execution.</returns>
    private IEnumerator ShowWinPanelDelayed()
    {
        yield return new WaitForSeconds(_winPanelDelay);

        //spawn continuous confetti all around the panel (todo)

        if (_winPanelUI != null)
        {
            _winPanelUI.SetActive(true);

            Time.timeScale = 0f;
        }
            

        if (_audioManager != null)
        {
            _audioManager.PauseMusic();
        }

        if (_audioManager != null) _audioManager.PlayWinSound();
    }

    /// <summary>
    /// Hides the win panel and transitions to the main menu scene.
    /// </summary>
    public void LoadMainMenu()
    {
        if (_winPanelUI != null)
        {
            _winPanelUI.SetActive(false);
            Time.timeScale = 1f;
        }

        if (_fadeTransition != null)
        {
            _fadeTransition.FadeToScene(0);
        }
    }

    /// <summary>
    /// Hides the win panel and restarts the current level.
    /// </summary>
    public void RestartLevel()
    {
        if (_winPanelUI != null)
        {
            _winPanelUI.SetActive(false);
            Time.timeScale = 1f;
        }
        if (_fadeTransition != null)
        {
            _fadeTransition.FadeToScene(1);
        }
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("WinManager: Quit Game called ");
    }


}
