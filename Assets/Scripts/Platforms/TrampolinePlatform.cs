using UnityEngine;

public class TrampolinePlatform : BounceGiverBase
{
    [SerializeField] private Animator _trampolineAnimator;

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        if (collision.gameObject.CompareTag("Player"))
        {
            // Trigger the trampoline animation
            if (_trampolineAnimator != null)
            {
                _trampolineAnimator.SetTrigger("Bounce");
            }
        }
    }
}
