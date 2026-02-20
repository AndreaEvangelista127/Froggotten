using UnityEngine;

public class DespawnAnimationHelper : MonoBehaviour
{
    private StatePlayerMovement _statePlayerMovement;

    private void Awake()
    {
        _statePlayerMovement = GetComponentInParent<StatePlayerMovement>();
    }

    /// <summary>
    /// Forwards the despawn complete event to StatePlayerMovement.
    /// Called by the despawn animation event on the child object.
    /// </summary>
    public void OnDespawnComplete()
    {
        if (_statePlayerMovement != null)
        {
            _statePlayerMovement.OnDespawnComplete();
        }
    }
}
