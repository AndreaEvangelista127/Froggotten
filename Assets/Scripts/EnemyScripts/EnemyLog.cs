using UnityEngine;
using System.Collections;

public class EnemyLog : MonoBehaviour, IEnemy
{
    [Header("References")]
    [SerializeField] private Transform _sprite;
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _shootPoint;
    [SerializeField] private Transform _playerTf;

    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _detectionRange = 5f;

    [Header("Attack")]
    [SerializeField] private float _attackRange = 3f;
    [SerializeField] private float _timeBetweenAttacks = 1.5f;

    [Header("Audio Manager")]
    [SerializeField] private AudioManager _audioManager;

    private Animator _animator;
    private Rigidbody2D _rb;
    private bool _facingRight = true;
    private bool _isAttacking = false;
    private bool _isDead = false;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (_playerTf == null || _isDead) return;

        float distanceToPlayer = Vector2.Distance(transform.position, _playerTf.position);

        if (distanceToPlayer <= _attackRange) // we are in attack range
        {
            StopAndAttack();
        }
        else if (_detectionRange >= distanceToPlayer) // we are in detection range
        {
            ChasePlayer();
        }
        else
        {
            Idle();
        }
    }

    /// <summary>
    /// Moves the enemy toward the player and flips the sprite to match the movement direction.
    /// </summary>
    private void ChasePlayer()
    {

        _animator.SetBool("isRunning", true);

        // Move towards player
        Vector2 direction = (_playerTf.position - transform.position).normalized;
        _rb.linearVelocity = new Vector2(direction.x * _moveSpeed, _rb.linearVelocity.y);

        // Flip sprite based on movement direction
        if (direction.x > 0 && !_facingRight)
        {
            Flip();
        }
        else if (direction.x < 0 && _facingRight)
        {
            Flip();
        }
    }

    private void Idle()
    {
        _animator.SetBool("isRunning", false);
        _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
    }

    /// <summary>
    /// Stops the enemy's movement, faces the player, and triggers the attack coroutine if not already attacking.
    /// </summary>
    private void StopAndAttack()
    {
        // Stop rigidbody and animation
        _animator.SetBool("isRunning", false);
        _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);

        // Flip sprite towards player
        Vector2 direction = (_playerTf.position - transform.position).normalized;
        if (direction.x > 0 && !_facingRight)
        {
            Flip();
        }
        else if (direction.x < 0 && _facingRight)
        {
            Flip();
        }

        // attack check
        if (!_isAttacking)
        {
            StartCoroutine(AttackCoroutine());
        }
    }

    /// <summary>
    /// Handles the attack timing: sets the attacking animation state and waits before allowing the next attack.
    /// </summary>
    /// <returns>IEnumerator for coroutine execution.</returns>
    IEnumerator AttackCoroutine()
    {
        _isAttacking = true;

        _animator.SetBool("isAttacking", true);

        yield return new WaitForSeconds(_timeBetweenAttacks);

        _animator.SetBool("isAttacking", false);

        _isAttacking = false;
    }

    /// <summary>
    /// Instantiates a projectile at the shoot point and fires it in the direction the enemy is facing.
    /// Called by an animation event.
    /// </summary>
    public void ShootProjectile()
    {
        if (_isDead)
        {
            return;
        }

        if (_projectilePrefab != null && _shootPoint != null)
        {
            GameObject projectile = Instantiate(_projectilePrefab, _shootPoint.position, Quaternion.identity);

            if (_audioManager != null)
            {
                _audioManager.PlayEnemyShootSound();
            }

            LogProjectile projScript = projectile.GetComponent<LogProjectile>();
            if (projScript != null)
            {
                Vector2 dir = _facingRight ? Vector2.right : Vector2.left;
                projScript.SetDirection(dir);
            }

        }
    }

    /// <summary>
    /// Flips the enemy sprite horizontally by inverting the sprite's X scale.
    /// </summary>
    public void Flip()
    {
        _facingRight = !_facingRight;
        float scaleX;
        if (_facingRight)
        {
            scaleX = -1f;
        }
        else
        {
            scaleX = 1f;
        }
        _sprite.localScale = new Vector3(scaleX, 1f, 1f);
    }

    /// <summary>
    /// Disables the enemy, stopping all coroutines and freezing its behaviour on death.
    /// </summary>
    public void OnDeath()
    {
        _isDead = true;
        StopAllCoroutines();
        this.enabled = false;
    }

}
