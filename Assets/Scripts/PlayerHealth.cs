using System.Collections;
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

    [Header("Respawn Settings")]
    [SerializeField] private Vector3 _startPosition; 
    private Vector3 _currentCheckpoint; 
    private bool _hasCheckpoint = false;

    [Header("Audio")]
    [SerializeField] private AudioManager _audioManager;

    private Color _originalColor;


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

        _currentHealth = _currentHealth - damage;

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
        Debug.Log(" Player è morto!");
        Respawn();

        if (_audioManager != null)
        {
            _audioManager.PlayDeathSound();
        }


    }

    // ============== Respawn System Methods ==============
    public void SetRespawnPoint(Vector3 newCheckpoint)
    {
        _currentCheckpoint = newCheckpoint;
        _hasCheckpoint = true;
    }

    private void Respawn()
    {
        _currentHealth = _maxHealth;
        if(_healthUi != null)
        {
            _healthUi.UpdateHeartContainer(_currentHealth);
        }

        // Respawn at current checkpoint if it exists, otherwise respawn at start position
        if (_hasCheckpoint)
        {
            transform.position = _currentCheckpoint;
        }
        else
        {
            transform.position = _startPosition;
        }
    }
}