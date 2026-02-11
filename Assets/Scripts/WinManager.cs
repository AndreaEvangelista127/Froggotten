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
    [SerializeField] private float _delayBeforeDespawnAnim = 0.1f;

    private bool _isTrophyUnlocked = false;
    private bool _hasWon = false;

    private void Start()
    {
        if (_trophyObject != null)
        {
            SetTrophyLocked();
        }
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

        // Bounce player
        PlayerCollisions playerCollisions = player.GetComponent<PlayerCollisions>();
        if (playerCollisions != null)
        {
            playerCollisions.BouncePlayer(_trophyBounceForce);
        }

        // TODO: add sound effect here

        // Despawn immediato
        StatePlayerMovement statePlayerMovement = player.GetComponent<StatePlayerMovement>();
        if (statePlayerMovement != null)
        {
            statePlayerMovement.TriggerDespawn();
        }
    }


}
