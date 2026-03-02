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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ========== ENEMY HEAD ==========

        if (collision.CompareTag("EnemyHead"))
        {
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

        // ========== TRAPS (SAW, SPIKES, ETC) ==========
        if (collision.CompareTag("Saw") && _playerHealth != null)
        {
            if (_playerHealth != null)
            {
                _playerHealth.TakeDamage(_sawDamage);

                ApplyTrapKnockback(collision.transform.position);
            }
        }

        // ========== SPIKES ========== 
        if (collision.CompareTag("Spike") && _playerHealth != null)
        {
            if (_playerHealth != null)
            {
                _playerHealth.TakeDamage(_spikeDamage);
                ApplyTrapKnockback(collision.transform.position);
            }
        }
    }

    /// <summary>
    /// Applies a knockback force to the player directed away from the trap's position.
    /// </summary>
    /// <param name="trapPosition">The world position of the trap that hit the player.</param>
    private void ApplyTrapKnockback(Vector3 trapPosition)
    {
        if (_playerMovement == null) return;

        Vector3 dir = (transform.position - trapPosition); //Direction from the trap directe to player 

        Vector3 knockBackVelocity = dir.normalized * _trapKnockbackForce;

        _playerMovement.ApplyKnockBack(knockBackVelocity);
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("EnemyBody"))
        {
            if (_playerHealth != null)
            {
                Debug.Log("Player takes damage from enemy body!");
                _playerHealth.TakeDamage(1);
            }
        }

        // Moving surface
        if (collision.collider.TryGetComponent(out IMovingSurface2D surface))
        {
            _playerMovement.SetMovingSurface(surface);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent(out IMovingSurface2D surface))
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
