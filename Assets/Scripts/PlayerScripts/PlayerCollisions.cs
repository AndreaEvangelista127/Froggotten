using System;
using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{

    [Header("Enemy Bounce")]
    [SerializeField] private float _bounceForce = 10f;

    [Header("Saw Settings")]
    [SerializeField] private float _sawDamage = 0.5f;

    [Header("Spike Settings")]
    [SerializeField] private float _spikeDamage = 1f;

    [Header("Traps Settings")]
    [SerializeField] private float _trapKnockbackForce = 8f;

    private Rigidbody2D _playerRb;
    private PlayerHealth _playerHealth;
    private PlayerMovement _playerMovement;

    public int _currentFliesCollected = 0;

    private void Awake()
    {
        _playerRb = GetComponent<Rigidbody2D>();
        _playerHealth = GetComponent<PlayerHealth>();
        _playerMovement = GetComponent<PlayerMovement>();
    }

    // We use OnTrigger when we need to detect an overlap without physics. The object doesn't block movement, you go through it (collectibles, detection areas, effect areas, enemy hitbox)

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ========== ENEMY HEAD ==========

        if (collision.CompareTag("EnemyHead"))
        {
            Debug.Log("EnemyHead hit!");

            IDamageable enemy = collision.GetComponentInParent<IDamageable>();

            if (enemy != null)
            {

                enemy.Die();
                BouncePlayer(_bounceForce);
                _playerMovement.ResetJumps();
            }
        }

        // ========== COLLECTIBLES ==========
        if (collision.CompareTag("Fly"))
        {
            Collectibles collectible = collision.GetComponent<Collectibles>();
            if (collectible != null)
            {
                collectible.Collect();
                _currentFliesCollected++;
                Debug.Log($"Flies collected: {_currentFliesCollected}");
            }
        }
    }

    // We use OnCollision when we need real physics. The object is blocking the movement and generate contacts (terrain, walls, platforms, traps)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ========== ENEMYS ==========
        if (collision.gameObject.CompareTag("EnemyBody"))
        {
            if (_playerHealth != null)
            {
                Debug.Log("Player takes damage from enemy body!");
                _playerHealth.TakeDamage(1);
            }
        }

        // ========== TRAPS (SAW, SPIKES, ETC) ==========> We're using OnCollisionEnter2D instead of OnTriggerEnter2D because we can use collision.contact to have the exact point where the player collided, having a precise knockback
        if (collision.gameObject.CompareTag("Spike") && _playerHealth != null)
        {
            _playerHealth.TakeDamage(_spikeDamage);
            Vector2 knockbackDir = collision.contacts[0].normal;
            _playerMovement.ApplyKnockBack(knockbackDir * _trapKnockbackForce);
        }

        if (collision.gameObject.CompareTag("Saw") && _playerHealth != null)
        {
            _playerHealth.TakeDamage(_sawDamage);
            Vector2 knockbackDir = collision.contacts[0].normal;
            _playerMovement.ApplyKnockBack(knockbackDir * _trapKnockbackForce);
        }

        // ========== PLATFORMS ==========
        if (collision.collider.TryGetComponent(out ISurface2D surface))
        {
            _playerMovement.SetMovingSurface(surface);
        }
    }

    /// <summary>
    /// Launches the player upward with the given force, used when stomping an enemy.
    /// </summary>
    /// <param name="bounceForce">The vertical force to apply.</param>
    public void BouncePlayer(float bounceForce)
    {
        if (_playerRb == null) return;

        _bounceForce = bounceForce;//in case we want to set different bounce forces for different enemies in the future
        _playerRb.linearVelocity = new Vector2(_playerRb.linearVelocity.x, _bounceForce);

    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent(out ISurface2D surface))
        {
            if (_playerMovement != null)
                _playerMovement.SetMovingSurface(null);
        }
    }

    public int GetCurrentFliesCollected()
    {
        return _currentFliesCollected;
    }

    public void ResetFliesCollected()
    {
        _currentFliesCollected = 0;
    }

}
