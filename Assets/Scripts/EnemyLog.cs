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

    IEnumerator AttackCoroutine()
    {
        _isAttacking = true;

        _animator.SetBool("isAttacking", true);

        yield return new WaitForSeconds(_timeBetweenAttacks);

        _animator.SetBool("isAttacking", false);

        _isAttacking = false;
    }

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
                Debug.Log($"Chiamo SetDirection con: {dir}, FacingRight: {_facingRight}");
                projScript.SetDirection(dir);
            }

        }
    }
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

    public void OnDeath()
    {
        _isDead = true;
        StopAllCoroutines();
        this.enabled = false;
    }

}
