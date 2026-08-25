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

    private TrapState _currentState = TrapState.Idle;
    private Coroutine _shutdownRoutine;

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

    // Damage is applied every physics frame the player stays in contact,
    // but only while the trap is actually Active.
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        if (_currentState == TrapState.Active)
        {
            Vector2 knockbackDir = collision.contacts[0].normal;
            ApplyDamage(collision.gameObject, knockbackDir); // shared method from TrapBase
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        Debug.Log("Player exited fire trap");
        Debug.Log($"Current state: {_currentState}, Shutdown routine: {_shutdownRoutine}");

        if (_currentState == TrapState.Active || _currentState == TrapState.Warning && _shutdownRoutine == null)
        {    
            _shutdownRoutine = StartCoroutine(ShutdownTrap());
        }
    }

    public IEnumerator ActivateTrap()
    {
        _currentState = TrapState.Warning;
        _fireTrapAnimator.SetTrigger("Warning");

        yield return new WaitForSeconds(_warningDuration);

        _fireTrapAnimator.SetTrigger("Burn");
        _currentState = TrapState.Active;
    }

    public IEnumerator ShutdownTrap()
    {
        yield return new WaitForSeconds(_shutdownDelay);

        _currentState = TrapState.Idle;
        _fireTrapAnimator.SetTrigger("Idle");
        _shutdownRoutine = null;
    }
}