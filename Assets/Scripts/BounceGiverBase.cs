using UnityEngine;

/// <summary>
/// Base class for anything that bounces the player on contact. Concrete
/// bounce sources only need to set _bounceForce and optionally an Animator
/// trigger name — the collision handling is shared here.
/// </summary>
public abstract class BounceGiverBase : MonoBehaviour, IBounceGiver
{
    [Header("Bounce Settings")]
    [SerializeField] protected float _bounceForce = 15f;

    public float BounceForce => _bounceForce;

    /// <summary>
    /// Handles the collision with the player and applies the bounce force.
    /// </summary>
    /// <param name="collision">The collision data.</param>
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            BounceUtility.BouncePlayer(collision.gameObject, _bounceForce);
        }
    }
}
