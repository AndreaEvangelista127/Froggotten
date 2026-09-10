using UnityEngine;

/// <summary>
/// Trigger collider sized to match the fire's visual area. Only enabled while
/// the trap is in its Active phase (toggled by FireTrap), so any contact
/// detected here always means damage should apply — no state check needed.
/// </summary>
public class FireHitbox : MonoBehaviour
{
    private FireTrap _fireTrap;

    private void Awake()
    {
        _fireTrap = GetComponentInParent<FireTrap>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        _fireTrap.ApplyFireContactDamage(other.gameObject);
    }
}