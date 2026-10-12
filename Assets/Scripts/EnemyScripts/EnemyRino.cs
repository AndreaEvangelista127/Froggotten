using UnityEngine;

public class EnemyRino : GroundEnemyBase
{
    private enum RinoState
    {
        Patrol,
        Charging,
        Stunned
    }

    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 1.0f;
    [SerializeField] private float _chargeSpeed = 6f;

    [Header("Facing")]
    [SerializeField] private bool _facingRight = false;

    [Header("Sight")]
    [SerializeField] private float _playerRayLenght = 10f;
    // Player + wall layers: the first thing the ray hits decides what it sees
    [SerializeField] private LayerMask _sightLayerMask;
    [SerializeField] private float _sightRayOffsetY = 0f;

    private RinoState _currentState = RinoState.Patrol;

    protected override void Awake()
    {
        base.Awake();
        _isFacingRight = _facingRight;
        Flip(_isFacingRight);
    }

    private void Update()
    {
        if (_isDead) return;

        // 
        switch (_currentState)
        {
            case RinoState.Patrol:
                UpdatePatrol();
                break;

            case RinoState.Charging:
                UpdateCharge();
                break;

                // Stunned: nothing, so the knockback velocity is never overwritten
        }
    }

    private void UpdatePatrol()
    {
        // Player in front: charge straight away
        if (CanSeePlayer(FacingDirection))
        {
            StartCharge();
            return;
        }

        // Player behind: turn around first, then charge
        if (CanSeePlayer(-FacingDirection))
        {
            FlipDirection();
            StartCharge();
            return;
        }
    }

    private void StartCharge()
    {
        _currentState = RinoState.Charging;
        _animator.SetBool("isCharging", true);
    }

    private void UpdateCharge()
    {
        Move(_chargeSpeed);
    }

    private void Move(float speed)
    {
        _animator.SetBool("isCharging", true);
        PlayDustParticle();

        _rb.linearVelocity = new Vector2(FacingDirection * speed, _rb.linearVelocity.y);
    }


    // ========= SIGHT =========

    /// <summary>
    /// Ray origin on the collider edge that faces the given direction
    /// (+1 = right edge, -1 = left edge), so the ray never hits the rhino itself.
    /// </summary>
    private Vector2 GetSightRayOrigin(float direction)
    {
        return new Vector2(
            transform.position.x + BodyCollider.bounds.extents.x * direction,
            transform.position.y - _sightRayOffsetY
        );
    }

    /// <summary>
    /// Casts a ray toward the given direction (+1 right, -1 left) and returns
    /// true only if the FIRST thing it hits is the player.
    /// </summary>
    private bool CanSeePlayer(float direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            GetSightRayOrigin(direction),
            new Vector2(direction, 0f),
            _playerRayLenght,
            _sightLayerMask
        );

        if (hit.collider == null) return false;

        return hit.collider.CompareTag("Player");
    }


    private void DrawSightGizmo(float direction)
    {
        if (CanSeePlayer(direction))
        {
            Gizmos.color = Color.yellow;
        }
        else
        {
            Gizmos.color = Color.cyan;
        }

        Vector3 origin = GetSightRayOrigin(direction);
        Gizmos.DrawLine(origin, origin + new Vector3(direction, 0f, 0f) * _playerRayLenght);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if (BodyCollider == null) return;

        DrawSightGizmo(FacingDirection);   // front
        DrawSightGizmo(-FacingDirection);  // back
    }
}