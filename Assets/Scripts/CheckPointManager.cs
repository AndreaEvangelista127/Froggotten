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

    /// <summary>
    /// Activates the checkpoint: updates the player's respawn point, triggers the flag animation,
    /// spawns confetti VFX, and plays the checkpoint sound.
    /// </summary>
    /// <param name="player">The player GameObject that triggered the checkpoint.</param>
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

        VFXConfetti.SpawnConfettiWithTimer(_confettiPrefab, _confettiSpawnPoint.position);

        if (_audioManager != null)
        {
            _audioManager.PlayCheckpointSound();
        }
    }

    /// <summary>
    /// Resets the checkpoint to its inactive state, allowing it to be triggered again.
    /// NOT USED IN THE CURRENT GAMEPLAY
    /// </summary>
    public void ResetCheckPoint()
    {
       _isActivated = false;
    }



}
