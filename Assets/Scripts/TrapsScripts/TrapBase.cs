using UnityEngine;

public abstract class TrapBase : MonoBehaviour, IDamageDealer
{
    [Header("Trap Damage")]
    [SerializeField] protected float _damage = 1f;

    [Header("Trap Knockback")]
    [SerializeField] protected float _knockbackForce = 8f;

    public float Damage => _damage;

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        Vector2 knockbackDir = collision.contacts[0].normal;
        ApplyDamage(collision.gameObject, knockbackDir);
    }

    /// <summary>
    /// Applies damage and knockback to the player. Shared by both solid traps
    /// (called from OnCollisionEnter2D, direction = contact normal) and
    /// walk-over traps (called from OnTriggerX2D, direction = a fixed vector).
    /// </summary>
    protected void ApplyDamage(GameObject player, Vector2 knockbackDirection)
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();

        if (playerHealth == null) return;

        playerHealth.TakeDamage(_damage);

        if (playerMovement != null)
        {
            playerMovement.ApplyKnockBack(knockbackDirection * _knockbackForce);
        }
    }
}