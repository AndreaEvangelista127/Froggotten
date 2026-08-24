using UnityEngine;

public abstract class TrapBase : MonoBehaviour, IDamageDealer
{
    [Header("Trap Damage")]
    [SerializeField] protected float _damage = 1f;

    [Header("Trap Knockback")]
    [SerializeField] protected float _knockbackForce = 8f;

    public float Damage => _damage;

    // We use OnCollisionEnter2D (not Trigger) because we need collision.contacts
    // to get the exact contact point and compute a correct knockback direction.
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
        PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();

        if (playerHealth == null) return;

        playerHealth.TakeDamage(_damage);

        if (playerMovement != null)
        {
            Vector2 knockbackDir = collision.contacts[0].normal;
            playerMovement.ApplyKnockBack(knockbackDir * _knockbackForce);
        }
    }
}
