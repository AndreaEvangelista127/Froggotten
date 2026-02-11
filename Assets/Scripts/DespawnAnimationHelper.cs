using UnityEngine;

public class DespawnAnimationHelper : MonoBehaviour
{
    private StatePlayerMovement _statePlayerMovement;

    private void Awake()
    {
        _statePlayerMovement = GetComponentInParent<StatePlayerMovement>();
    }

    public void OnDespawnComplete()
    {
        if (_statePlayerMovement != null)
        {
            _statePlayerMovement.OnDespawnComplete();
        }
    }
}
