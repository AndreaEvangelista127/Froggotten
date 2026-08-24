using UnityEngine;

/// <summary>
/// Fire trap. Behaves like a standard TrapBase but can add its own VFX/animation
/// on top of the shared damage logic.
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

    // Write on the animator 3 trigger parameters: "Idle", "Warning", "Burn"

    private TrapState _currentState = TrapState.Idle;
    private Coroutine _shutdownRoutine;

    /// <summary>
    /// Check if the player collides with the trap. If so, trigger the "Burn" animation but
    /// if the trap was already in the "Burn" state, stop the shutdown routine and restart it 
    /// to keep the trap active for a longer time.
    /// </summary>
    /// <param name="collision"></param>
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision); // keep shared damage/knockback behaviour

        if (collision.gameObject.CompareTag("Player") && _fireTrapAnimator != null)
        {
            _fireTrapAnimator.SetTrigger("Burn");
        }
    }

    // if the player is in the active state, call the base OnCollisionEnter2D to apply damage and knockback.
    private void OnCollisionStay2D(Collision2D collision)
    {
        
    }

    // If the player leaves the trap, check if the trap is in the active state
    // and if the shutdown routine == null, start the shutdown routine to return to the idle state after a delay.
    private void OnCollisionExit2D(Collision2D collision)
    {
        
    }

    //Activation sequence

    //Shutdown sequence
}
