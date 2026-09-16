using System.Collections;
using UnityEngine;

/// <summary>
/// Fire trap with a solid (non-trigger) collider — the player physically rests
/// on top of it, like Spike/Saw. Warning/Active/Idle phases are driven by
/// Animator triggers; damage is only applied while in the Active phase.
/// </summary>
public class FireTrap : TrapBase
{
    private enum TrapState
    {
        Idle,
        Warning,
        Active
    }

    [Header("Fire Trap Timing")]
    [SerializeField] private float _warningDuration = 0.5f;
    [SerializeField] private float _shutdownDelay = 1f;

    [Header("Fire Trap Animation")]
    [SerializeField] private Animator _fireTrapAnimator;

    [Header("Fire Trap Hitbox")]
    [SerializeField] private Collider2D _fireHitboxCollider;

    private TrapState _currentState = TrapState.Idle;
    private Coroutine _shutdownRoutine;

    private void Awake()
    {
        if (_fireHitboxCollider != null)
        {
            _fireHitboxCollider.enabled = false; // starts off: no fire, no damage
        }
    }

    protected override void OnCollisionStay2D(Collision2D collision)
    {
        
    }

    // Only handles state transitions here. No damage is applied on Enter:
    // the player might just be stepping on an Idle or Warning trap.
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        if (_currentState == TrapState.Idle)
        {
            StartCoroutine(ActivateTrap());
        }
        else if (_currentState == TrapState.Active && _shutdownRoutine != null)
        {
            // Player came back before the fire turned off: cancel the shutdown.
            StopCoroutine(_shutdownRoutine);
            _shutdownRoutine = null;
        }
    }

    //// Damage is applied every physics frame the player stays in contact,
    //// but only while the trap is actually Active. Uses TrapBase's shared
    //// logic so i-frames are respected and knockback isn't spammed.
    //private void OnCollisionStay2D(Collision2D collision)
    //{
    //    if (!collision.gameObject.CompareTag("Player")) return;

    //    if (_currentState == TrapState.Active)
    //    {
    //        ApplyDamageAndKnockback(collision); // was: ApplyDamage(...)
    //    }
    //}

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        bool shouldShutdown = (_currentState == TrapState.Active || _currentState == TrapState.Warning)
                               && _shutdownRoutine == null;

        if (shouldShutdown)
        {
            _shutdownRoutine = StartCoroutine(ShutdownTrap());
        }
    }

    public void ApplyFireContactDamage(GameObject player)
    {
        Vector2 knockbackDir = GetKnockbackDirection(player.transform);
        ApplyDamageAndKnockback(player, knockbackDir);
    }

    public IEnumerator ActivateTrap()
    {
        _currentState = TrapState.Warning;
        _fireTrapAnimator.SetTrigger("Warning");

        yield return new WaitForSeconds(_warningDuration);

        _fireTrapAnimator.SetTrigger("Burn");
        _currentState = TrapState.Active;

        if (_fireHitboxCollider != null)
        {
            _fireHitboxCollider.enabled = true; // fire is now dangerous
        }
    }

    public IEnumerator ShutdownTrap()
    {
        yield return new WaitForSeconds(_shutdownDelay);

        _currentState = TrapState.Idle;
        _fireTrapAnimator.SetTrigger("Idle");
        _shutdownRoutine = null;

        if (_fireHitboxCollider != null)
        {
            _fireHitboxCollider.enabled = false; // fire is off, stop dealing damage
        }
    }

    /// <summary>
    /// The player usually stands directly on top of this trap, so the collision
    /// normal points almost straight up and just drops them back onto the same spot.
    /// Push up AND sideways instead, so they actually leave the danger zone.
    /// </summary>
    protected override Vector2 GetKnockbackDirection(Transform other)
    {
        // Example: Fire trap at x = 5 and player at x = 3: offsetX = 3 - 5 = -2, horizontalSign = -1 (push left)
        float offsetX = other.position.x - transform.position.x;

        float horizontalSign;
        if (Mathf.Abs(offsetX) < 0.1f) // Player is almost exactly above the trap, so pick a random horizontal direction
        {
            horizontalSign = (Random.value < 0.5f) ? -1f : 1f;
        }
        else
        {
            horizontalSign = Mathf.Sign(offsetX); // horizontalSign will be -1 if player is left of trap, +1 if right
        }

        return new Vector2(horizontalSign, 1f).normalized; // Without normalization, the knockback would have been much stronger because later is multiplied by the knockback force
    }
}