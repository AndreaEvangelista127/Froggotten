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
    [SerializeField] private int _numberOfFlashes = 5;
    private bool _isInvulnerable = false;

    [Header("Visual Feedback")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _hitFlashDuration = 0.1f;

    private Color _originalColor;


    void Start()
    {
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

    void Die()
    {
        Debug.Log(" Player è morto!");
    }
}