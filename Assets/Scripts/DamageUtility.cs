using UnityEngine;

public static class DamageUtility
{
    // Toggle this off once you're done verifying knockback behavior.
    private static readonly bool _debugVisualize = true;

    public static bool ApplyDamageAndKnockback(GameObject target, float damage, Vector2 knockbackDirection, float knockbackForce)
    {
        PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
        PlayerMovement playerMovement = target.GetComponent<PlayerMovement>();

        if (playerHealth == null) return false;

        bool damageApplied = playerHealth.TakeDamage(damage);

        if (damageApplied && playerMovement != null)
        {
            playerMovement.ApplyKnockBack(knockbackDirection * knockbackForce);

            if (_debugVisualize)
            {
                // Draws a line from the player showing knockback direction + force.
                Debug.DrawRay(target.transform.position, knockbackDirection * knockbackForce * 0.1f, Color.magenta, 1f);
                //Debug.Log($"[Knockback] Applied to {target.name} | dir: {knockbackDirection} | force: {knockbackForce}");
            }
        }
        else if (_debugVisualize && !damageApplied)
        {
            // This log firing repeatedly while you stay in contact = i-frames are correctly blocking re-damage.
            //Debug.Log($"[Knockback] Blocked (i-frames or dead) on {target.name}");
        }

        return damageApplied;
    }
}
