using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    [Header("Checkpoint Settings")]
    [SerializeField] private Transform _respawnPoint; 

    [Header("Visual Feedback")]
    [SerializeField] private Animator _flagAnimator;
    [SerializeField] private string _activationAnimationTrigger = "Activate";

    [Header("VFX")]
    [SerializeField] private GameObject _confettiPrefab; 
    [SerializeField] private Transform _confettiSpawnPoint;

    [Header("Audio")]
    [SerializeField] private AudioManager _audioManager;

    private bool _isActivated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !_isActivated)
        {
            ActivateCheckpoint(collision.gameObject); //passing the player game object to the method
        }
    }

    private void ActivateCheckpoint(GameObject player)
    {
        _isActivated = true;

        Vector3 respawnPosition;

        if (_respawnPoint != null)
        {
            respawnPosition = _respawnPoint.position;
        }
        else
        {
            respawnPosition = transform.position; // Fallback to checkpoint's position if no respawn point is set
        }

       PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.SetRespawnPoint(respawnPosition);
        }
        if (_flagAnimator != null)
        {
            _flagAnimator.SetTrigger(_activationAnimationTrigger);
        }

        VFXConfetti.SpawnConfetti(_confettiPrefab, _confettiSpawnPoint.position);

        if (_audioManager != null)
        {
            _audioManager.PlayCheckpointSound();
        }
    }

    public void ResetCheckPoint()
    {
       _isActivated = false;
    }



}
