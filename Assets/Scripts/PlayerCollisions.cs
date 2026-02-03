using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{

    [Header("Enemy Bounce")]
    [SerializeField] private float _enemyBounceForce = 10f;

    private Rigidbody2D _playerRb;

    private void Awake()
    {
        _playerRb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.CompareTag("EnemyHead"))
        {
            IDamageable enemy = collision.GetComponentInParent<IDamageable>();
            if (enemy != null)
            {
                enemy.Die();
                BouncePlayer();
            }
        }

        if (collision.CompareTag("Fly"))
        {
            Collectibles collectible = collision.GetComponent<Collectibles>();
            if (collectible != null)
            {
                collectible.Collect();
            }
        }
    }

    private void BouncePlayer()
    {
        _playerRb.linearVelocity = new Vector2(_playerRb.linearVelocity.x, _enemyBounceForce);

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

}
