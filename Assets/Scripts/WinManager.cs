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
    private bool _isPaused = false;


    private void Start()
    {
        if (_trophyObject != null)
        {
            SetTrophyLocked();
        }

        if (_winPanelUI != null)
            _winPanelUI.SetActive(false);
    }

    public void UnlockTrophy()
    {
        _isTrophyUnlocked = true;

        if (_trophyObject != null)
        {
            SetTrophyUnlocked();
        }
    }

    private void SetTrophyLocked()
    {
        if (_trophySpriteRenderer != null)
        {
            _trophySpriteRenderer.color = _lockedColor;
        }
    }

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
            Debug.Log($"WinManager: Player toccato il trofeo. Unlocked: {_isTrophyUnlocked}, HasWon: {_hasWon}");

            if (_isTrophyUnlocked && !_hasWon)
            {

                TriggerWinSequence(collision.gameObject);

            }
            else
            {
                Debug.Log("WinManager:  Il trofeo è ancora bloccato! Raccogli tutte le mosche per sbloccarlo.");
            }
        }
    }

    private void TriggerWinSequence(GameObject player)
    {
        _hasWon = true;

        // Animazione trofeo
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

        // Despawn immediato
        StatePlayerMovement statePlayerMovement = player.GetComponent<StatePlayerMovement>();
        if (statePlayerMovement != null)
        {
            statePlayerMovement.TriggerDespawn();
        }

        StartCoroutine(ShowWinPanelDelayed());
    }

    private IEnumerator ShowWinPanelDelayed()
    {
        yield return new WaitForSeconds(_winPanelDelay);

        //spawn continuous confetti all around the panel

        if (_winPanelUI != null)
        {
            _winPanelUI.SetActive(true);

            Time.timeScale = 0f;
            _isPaused = true;
        }
            

        if (_audioManager != null)
        {
            _audioManager.PauseMusic();
        }

        if (_audioManager != null) _audioManager.PlayWinSound();
    }

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

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("WinManager: Quit Game called ");
    }


}
