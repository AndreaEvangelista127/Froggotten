using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float _maxHealth = 3.5f; 
    private float _currentHealth;

    [Header("UI")]
    [SerializeField] private HealthUi _healthUi;

    [Header("Invulnerability Settings")]
    [SerializeField] private int _numberOfFlashes = 2;
    private bool _isInvulnerable = false;

    [Header("Visual Feedback")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _hitFlashDuration = 0.1f;

    [Header("Death Fade Settings")]
    [SerializeField] private float _fadeDuration = 1f;    
    [SerializeField] private float _respawnDelay = 0.5f;

    [Header("Respawn Settings")]
    [SerializeField] private Vector3 _startPosition; 
    private Vector3 _currentCheckpoint; 
    private bool _hasCheckpoint = false;

    [Header("References")]
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private CinemachineCamera _virtualCamera;

    private Color _originalColor;
    private bool _isDead = false;
    private Rigidbody2D _rb;


    void Start()
    {
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

    public void TakeDamage(float damage)
    {
        if(_isInvulnerable) return;
        if (_isDead) return;

        _currentHealth -= damage;

        if (_currentHealth < 0)
        {
            _currentHealth = 0;
        }

        Debug.Log($" Player Health: {_currentHealth}/{_maxHealth}");

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

    public void Die()
    {
        if (_isDead) return;
        _isDead = true;

        // Blocca movimento
        if (_playerMovement != null)
        {
            _playerMovement.enabled = false;
        }

        // Blocca fisica
        if (_rb != null)
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.simulated = false; // Blocca completamente il rigidbody
        }

        // Blocca camera
        if (_virtualCamera != null)
        {
            _virtualCamera.enabled = false;
        }

        if (_audioManager != null)
        {
            _audioManager.PlayDeathSound();
        }
        Respawn();
    }

    // ============== Respawn System Methods ==============
    public void SetRespawnPoint(Vector3 newCheckpoint)
    {
        _currentCheckpoint = newCheckpoint;
        _hasCheckpoint = true;
    }

    private void Respawn()
    {
        _isDead = false;
        _isInvulnerable = false;

        if (_virtualCamera != null)
        {
            _virtualCamera.enabled = true;
        }
        // Ripristina health
        _currentHealth = _maxHealth;
        if (_healthUi != null)
        {
            _healthUi.UpdateHeartContainer(_currentHealth);
        }

        // Ripristina posizione
        transform.position = _hasCheckpoint ? _currentCheckpoint : _startPosition;

        // Ripristina velocità
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Riabilita controlli
        if (_playerMovement != null)
        {
            _playerMovement.enabled = true;
        }
    }
}