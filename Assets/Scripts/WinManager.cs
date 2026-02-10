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
    [SerializeField] private float _delayBeforeDespawn = 0.3f;

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

        Debug.Log("WinManager:  Trofeo sbloccato!");
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

                StartCoroutine(TriggerWinSequence(collision.gameObject));

            }
            else
            {
                Debug.Log("WinManager:  Il trofeo è ancora bloccato! Raccogli tutte le mosche per sbloccarlo.");
            }
        }
    }

    private void TriggerWinAnimation()
    {
        if (_winAnimator != null)
        {
            _winAnimator.SetTrigger(_winAnimationTrigger);
        }
    }

    private IEnumerator TriggerWinSequence(GameObject player)
    {
        _hasWon = true;
        TriggerWinAnimation();

        PlayerCollisions playerCollisions = player.GetComponent<PlayerCollisions>();
        if (playerCollisions != null)
        {
            playerCollisions.BouncePlayer(_trophyBounceForce);
        }

        if (_winAnimator != null)
        {
            _winAnimator.SetTrigger(_winAnimationTrigger);
        }

        //TODO : add sound effect here

        yield return new WaitForSeconds(_delayBeforeDespawn);

        StatePlayerMovement statePlayerMovement = player.GetComponent<StatePlayerMovement>();
        if (statePlayerMovement != null)
        {
            statePlayerMovement.TriggerDespawn();
        }
    }
}
