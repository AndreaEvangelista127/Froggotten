using System;
using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{

    [Header("Enemy Bounce")]
    [SerializeField] private float _bounceForce = 10f;

    [Header("Traps Damage")]
    [SerializeField] private float _sawDamage = 0.5f;
    [SerializeField] private float _trapKnockbackForceX = 8f;  
    [SerializeField] private float _trapKnockbackForceY = 10f;

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
        if (collision.CompareTag("Trap"))
        {
            Debug.Log($" PLAYER TRIGGER ENTERED: {collision.gameObject.name}");

            if (_playerHealth != null)
            {
                _playerHealth.TakeDamage(_sawDamage);

                ApplyTrapKnockback(collision.transform.position);
            }
        }
    }

    
    private void ApplyTrapKnockback(Vector3 trapPosition)
    {
        Vector3 dir = (transform.position - trapPosition); //Direction from the trap directe to player 

        //Vector3 dir = -_playerRb.linearVelocity; //giving dir the vector that represent the velocity of the player but flipped

        Vector3 knockBackVelocity = dir.normalized * _trapKnockbackForceX;

        _playerMovement.ApplyKnockBack(knockBackVelocity);
    }

    public void BouncePlayer(float bounceForce)
    {
        _bounceForce = bounceForce;//in case we want to set different bounce forces for different enemies in the future
        _playerRb.linearVelocity = new Vector2(_playerRb.linearVelocity.x, _bounceForce);

        Debug.Log("Bounce!");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("EnemyBody"))
        {
            PlayerHealth playerHealth = GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                Debug.Log("Player takes damage from enemy body!");
                playerHealth.TakeDamage(1);
            }
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
