using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Visual Feedback")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] protected float _deathDelay = 0.5f;

    [Header("Death Animation")]
    [SerializeField] private float _bounceForce = 4f;
    [SerializeField] private float _bounceRotationSpeed = 720f;
    [SerializeField] protected float _deathGravityScale = 3f;

    private Animator _animator;
    private Rigidbody2D _enemyRb;
    private bool _isDead = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _enemyRb = GetComponent<Rigidbody2D>();
    }

    public void Die()
    {
        if (_isDead) return;

        _isDead = true;

        if (_animator != null)
        {
            _animator.SetTrigger("hit");
        }

        // Blocca tutto
        DisableEnemy();

        // Rimbalzo Mario-style
        ApplyDeathBounce();

        // Distruggi dopo delay
        Destroy(gameObject, _deathDelay);
    }

    private void DisableEnemy() {

        // Disabilita tutti i collider
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }

        if (_enemyRb != null)
        {
            _enemyRb.linearVelocity = Vector2.zero;
            _enemyRb.angularVelocity = 0f;
        }

        IEnemy enemyScript = GetComponent<IEnemy>();
        if (enemyScript != null)
        {
            enemyScript.OnDeath();
        }
    }

    public void ApplyDeathBounce()
    {
        if (_enemyRb != null)
        {
            _enemyRb.linearVelocity = new Vector2(0, _bounceForce);
            _enemyRb.angularVelocity = _bounceRotationSpeed;
            _enemyRb.gravityScale = _deathGravityScale;
        }
    }

    

}
