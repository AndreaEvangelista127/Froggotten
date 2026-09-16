using UnityEngine;

public abstract class TrapBase : MonoBehaviour, IDamageDealer
{
    [Header("Trap Damage")]
    [SerializeField] protected float _damage = 1f;

    [Header("Trap Knockback")]
    [SerializeField] protected float _knockbackForce = 8f;

    public float Damage => _damage;


    // Same reasoning as EnemyBase: without this, staying pinned against a trap
    // past the end of i-frames would leave the player permanently immune to it.
    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        ApplyDamageAndKnockback(collision);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        ApplyDamageAndKnockback(collision);
    }

    /// <summary>
    /// Default damage/knockback application for collision-based contact. Uses the first contact point to compute knockback direction.
    /// </summary>
    /// <param name="collision"></param>
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
        DamageUtility.ApplyDamageAndKnockback(target, _damage, knockbackDirection, _knockbackForce);
    }

    /// <summary>
    /// Default knockback direction: straight away from the contact surface.
    /// Override in traps where the contact normal alone isn't reliable
    /// (e.g. flat traps the player stands directly on top of).
    /// </summary>
    protected virtual Vector2 GetKnockbackDirection(Collision2D collision)
    {
        return (collision.transform.position - transform.position).normalized;
    }
    protected virtual Vector2 GetKnockbackDirection(Transform other)
    {
        return (other.position - transform.position).normalized;
    }
}