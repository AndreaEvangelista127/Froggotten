using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float _maxHealth = 3.5f; 
    private float _currentHealth;

    [Header("Lives Settings")]
    [SerializeField] private int _maxLives = 3;
    [SerializeField] private LivesUI _livesUI;
    private int _currentLives;

    [Header("UI")]
    [SerializeField] private HealthUi _healthUi;

    [Header("Invulnerability Settings")]
    [SerializeField] private int _numberOfFlashes = 2;
    private bool _isInvulnerable = false;

    [Header("Visual Feedback")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _hitFlashDuration = 0.1f;

    [Header("Respawn Settings")]
    [SerializeField] private Vector3 _startPosition; 
    private Vector3 _currentCheckpoint; 
    private bool _hasCheckpoint = false;

    [Header("References")]
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private CinemachineCamera _virtualCamera;
    [SerializeField] private GameOverManager gameOverManager;


    private Color _originalColor;
    private bool _isDead = false;
    private Rigidbody2D _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _currentLives = _maxLives;

        if (_livesUI != null)
            _livesUI.UpdateLives(_currentLives);

        // ============== Respawn System Initialization ==============
        _startPosition = transform.position;
        _currentCheckpoint = _startPosition;


        // ============== Health UI Initialization ==============
        _currentHealth = _maxHealth;

        if (_healthUi != null)
        {
            _healthUi.InitializeHealthUi(_maxHealth);
        }

        if (_spriteRenderer != null)
        {
            _originalColor = _spriteRenderer.color;
        }
    }

    /// <summary>
    /// Reduces the player's health by the given amount, triggers UI and audio feedback,
    /// and starts the hit flash effect. Calls Die() if health reaches zero.
    /// Has no effect if the player is invulnerable or already dead.
    /// </summary>
    /// <param name="damage">The amount of damage to apply.</param>
    public void TakeDamage(float damage)
    {
        if (_isInvulnerable || _isDead) return;

        _currentHealth -= damage;

        if (_currentHealth < 0)
        {
            _currentHealth = 0;
        }

        if (_healthUi != null)
        {
            _healthUi.UpdateHeartContainer(_currentHealth);
        }

        if (_audioManager != null)
        {
            _audioManager.PlayTakeDamageSound();
        }

        if (_currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(HitEffect());
        }
    }

    /// <summary>
    /// Flashes the player sprite red, then alternates transparency to signal invulnerability.
    /// Sets _isInvulnerable for the duration to prevent damage during the effect.
    /// </summary>
    IEnumerator HitEffect()
    {
        _isInvulnerable = true;

        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = Color.red;
        }

        yield return new WaitForSeconds(_hitFlashDuration); //flash red animation + duration

        float flashInterval = 0.1f;

        // Flashing effect
        for (int i = 0; i < _numberOfFlashes; i++)
        {
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = Color.clear; //Transparent
            }
            yield return new WaitForSeconds(flashInterval);

            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = _originalColor; //Back to original
            }
            yield return new WaitForSeconds(flashInterval);
        }

        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = _originalColor;
        }

        _isInvulnerable = false;
    }

    //NOT USED IN THIS PROTOTYPE, BUT IMPLEMENTED FOR FUTURE USE
    public void Heal(float amount)
    {
        _currentHealth = _currentHealth + amount;

        // Non superare il massimo
        if (_currentHealth > _maxHealth)
        {
            _currentHealth = _maxHealth;
        }

        Debug.Log($" Player healed! Health: {_currentHealth}/{_maxHealth}");

        if (_healthUi != null)
        {
            _healthUi.UpdateHeartContainer(_currentHealth);
        }
    }

    /// <summary>
    /// Kills the player: disables movement, physics, and camera, plays the death sound, then triggers a respawn.
    /// </summary>
    public void Die()
    {
        if (_isDead) return;
        _isDead = true;
        _currentLives--;


        if (_playerMovement != null)
        {
            _playerMovement.enabled = false;
        }

        if (_rb != null)
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.simulated = false; // Disable physics to prevent falling through the world during death animation 
        }

        if (_virtualCamera != null)
        {
            _virtualCamera.enabled = false;
        }

        if (_audioManager != null)
        {
            _audioManager.PlayDeathSound();
        }

        if (_livesUI != null)
            _livesUI.UpdateLives(_currentLives);

        if (_currentLives <= 0)
        {
            GameOver();
        }
        else
        {
            Respawn();
        }
    }

    /// <summary>
    /// Sets the respawn point to the given checkpoint position.
    /// </summary>
    /// <param name="newCheckpoint">The world position of the activated checkpoint.</param>
    // ============== Respawn System Methods ==============
    public void SetRespawnPoint(Vector3 newCheckpoint)
    {
        _currentCheckpoint = newCheckpoint;
        _hasCheckpoint = true;
    }

    /// <summary>
    /// Resets the player's health, position, physics, and movement to allow play to continue.
    /// Respawns at the last checkpoint if one has been activated, otherwise at the start position.
    /// </summary>
    private void Respawn()
    {
        _isDead = false;
        _isInvulnerable = false;

        if (_virtualCamera != null)
        {
            _virtualCamera.enabled = true;
        }

        _currentHealth = _maxHealth;
        if (_healthUi != null)
        {
            _healthUi.UpdateHeartContainer(_currentHealth);
        }

        transform.position = _hasCheckpoint ? _currentCheckpoint : _startPosition;

        if (_rb != null)
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.simulated = true; //re-eneble physics after respawn
        }

        if (_playerMovement != null)
        {
            _playerMovement.enabled = true;
        }
    }

    /// <summary>
    /// Triggered when the player runs out of lives.
    /// Stops the game and shows the Game Over panel.
    /// </summary>
    private void GameOver()
    {

        if (_audioManager != null)
            _audioManager.PauseMusic();

        Time.timeScale = 0f;

        if (gameOverManager != null)
            gameOverManager.ShowGameOver();
    }
}