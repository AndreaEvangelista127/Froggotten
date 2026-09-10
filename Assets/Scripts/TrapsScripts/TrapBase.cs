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
        ApplyDamageAndKnockback(collision);
    }

    protected void ApplyDamageAndKnockback(Collision2D collision)
    {
        ApplyDamageAndKnockback(collision.gameObject, GetKnockbackDirection(collision));
    }

    /// <summary>
    /// Overload for trigger-based contact (no Collision2D/contacts available,
    /// e.g. FireTrap's separate flame hitbox). Same damage/i-frame logic,
    /// knockback direction is passed in directly instead of computed from contacts.
    /// </summary>
    protected void ApplyDamageAndKnockback(GameObject target, Vector2 knockbackDirection)
    {
        PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
        PlayerMovement playerMovement = target.GetComponent<PlayerMovement>();

        if (playerHealth == null) return;

        bool damageApplied = playerHealth.TakeDamage(_damage);

        if (damageApplied && playerMovement != null)
        {
            playerMovement.ApplyKnockBack(knockbackDirection * _knockbackForce);
        }
    }

    /// <summary>
    /// Default knockback direction: straight away from the contact surface.
    /// Override in traps where the contact normal alone isn't reliable
    /// (e.g. flat traps the player stands directly on top of).
    /// </summary>
    protected virtual Vector2 GetKnockbackDirection(Collision2D collision)
    {
        return collision.contacts[0].normal;
    }
}