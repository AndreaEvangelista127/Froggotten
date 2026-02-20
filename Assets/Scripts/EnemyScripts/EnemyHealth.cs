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

    [Header("Audio Source")]
    [SerializeField] private AudioManager _audioManager;

    private Animator _animator;
    private Rigidbody2D _enemyRb;
    private bool _isDead = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _enemyRb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Kills the enemy: plays the death sound and animation, disables all components,
    /// applies a bounce effect, and destroys the object after a delay.
    /// </summary>
    public void Die()
    {
        if (_isDead) return;

        _isDead = true;

        if(_audioManager != null)
        {
            Debug.Log("EnemyHealth: Playing death sound");
            _audioManager.PlayEnemyDeathSound();
        }

        if (_animator != null)
        {
            _animator.SetTrigger("hit");
        }

        DisableEnemy();

        ApplyDeathBounce();

        Destroy(gameObject, _deathDelay);
    }

    /// <summary>
    /// Disables all colliders, stops the rigidbody, and notifies the enemy behaviour script of death.
    /// </summary>
    private void DisableEnemy() {

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

    /// <summary>
    /// Applies an upward bounce and spin to the rigidbody for a Mario-style death effect.
    /// Unlocks all constraints to allow free rotation.
    /// </summary>
    //TIP FOR THE FUTURE: use this method for the player and the enemys using parameters for the bounce force, rotation speed and gravity scale, so you can have different death animations for different characteter
    public void ApplyDeathBounce()
    {

        if (_enemyRb != null)
        {
            _enemyRb.constraints = RigidbodyConstraints2D.None;//Unlock all constraints to allow free movement and rotation
            _enemyRb.linearVelocity = new Vector2(0, _bounceForce);
            _enemyRb.angularVelocity = _bounceRotationSpeed;
            _enemyRb.gravityScale = _deathGravityScale;
        }
    }

    

}
