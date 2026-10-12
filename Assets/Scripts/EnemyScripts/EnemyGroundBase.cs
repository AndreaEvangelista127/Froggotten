using UnityEngine;

/// <summary>
/// Base class for enemies that walk on the ground: owns the facing direction
/// and the shared wall / enemy / precipice detection, so Mushroom, Rino and
/// future walkers don't duplicate it. Field names match the old EnemyMushroom
/// ones so the values already set in the Inspector are kept.
/// </summary>
public abstract class GroundEnemyBase : EnemyBase
{
    [Header("Detection Layers")]
    [SerializeField] protected LayerMask _groundLayer;
    [SerializeField] protected LayerMask _wallLayer;
    [SerializeField] protected LayerMask _enemyLayer;

    [Header("Wall Detection")]
    [SerializeField] protected float _wallCheckDistance = 0.2f;
    // 0 = ray starts at the center height, higher = ray starts lower
    [SerializeField] protected float _wallRayOffsetY = 0f;

    [Header("Precipice Detection")]
    [SerializeField] protected float _precipiceCheckDistance = 0.5f;
    // Horizontal offset from the collider edge, to cast the ray from the front
    [SerializeField] protected float _precipiceCheckOffsetX = 0.3f;

    [Header("Enemy Detection")]
    [SerializeField] protected float _enemyCheckDistance = 0.5f;

    [Header("Gizmos Boolean")]
    [SerializeField] protected bool _drawWallRay = false;
    [SerializeField] protected bool _drawEnemyRay = false;
    [SerializeField] protected bool _drawPrecipiceRay = false;

    protected bool _isFacingRight = false;

    private Collider2D _bodyCollider;

    /// <summary>
    /// Lazy getter: also works in Edit Mode (gizmos), where Awake has not run.
    /// </summary>
    protected Collider2D BodyCollider
    {
        get
        {
            if (_bodyCollider == null)
            {
                _bodyCollider = GetComponent<Collider2D>();
            }

            return _bodyCollider;
        }
    }

    /// <summary>
    /// +1 when facing right, -1 when facing left. Written once, used by every check.
    /// </summary>
    protected float FacingDirection
    {
        get
        {
            if (_isFacingRight)
            {
                return -1f;
            }
            else
            {
                return +1f;
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();

        if (BodyCollider == null)
        {
            Debug.LogWarning($"{name}: Collider2D not found!");
        }

        Flip(_isFacingRight); // put the sprite in the starting direction
    }

    /// <summary>
    /// Inverts the facing direction and flips the sprite accordingly.
    /// </summary>
    protected void FlipDirection()
    {
        _isFacingRight = !_isFacingRight;
        Flip(_isFacingRight);
    }

    // ========== RAY ORIGINS (single source of truth for checks AND gizmos) ==========

    protected Vector2 GetWallRayOrigin()
    {
        return new Vector2(
            transform.position.x + BodyCollider.bounds.extents.x * FacingDirection,
            transform.position.y - _wallRayOffsetY
        );
    }

    protected Vector2 GetEnemyRayOrigin()
    {
        return new Vector2(
            transform.position.x + BodyCollider.bounds.extents.x * FacingDirection,
            transform.position.y - BodyCollider.bounds.extents.y
        );
    }

    protected Vector2 GetPrecipiceRayOrigin()
    {
        return new Vector2(
            transform.position.x + (BodyCollider.bounds.extents.x + _precipiceCheckOffsetX) * FacingDirection,
            transform.position.y - BodyCollider.bounds.extents.y
        );
    }

    // ========== DETECTION ==========

    /// <summary>
    /// True if a wall is detected in front, within _wallCheckDistance of the collider edge.
    /// </summary>
    protected bool HitsWall()
    {
        return Physics2D.Raycast(GetWallRayOrigin(), new Vector2(FacingDirection, 0f), _wallCheckDistance, _wallLayer);
    }

    /// <summary>
    /// True if another enemy is detected in front, at feet height.
    /// </summary>
    protected bool HitsEnemy()
    {
        return Physics2D.Raycast(GetEnemyRayOrigin(), new Vector2(FacingDirection, 0f), _enemyCheckDistance, _enemyLayer);
    }

    /// <summary>
    /// True if there is no ground below the front edge, meaning a precipice ahead.
    /// </summary>
    protected bool IsAtPrecipice()
    {
        bool groundDetected = Physics2D.Raycast(GetPrecipiceRayOrigin(), Vector2.down, _precipiceCheckDistance, _groundLayer);
        return !groundDetected;
    }

    // ========== GIZMOS ==========

    private void DrawRayGizmo(Vector3 origin, Vector3 direction, float distance, LayerMask layer, Color hitColor, Color clearColor)
    {
        bool hit = Physics2D.Raycast(origin, direction, distance, layer);

        if (hit)
        {
            Gizmos.color = hitColor;
        }
        else
        {
            Gizmos.color = clearColor;
        }

        Gizmos.DrawLine(origin, origin + direction * distance);
    }

    /// <summary>
    /// Draws the three shared rays. Child classes can override it to add their
    /// own (calling base.OnDrawGizmos() first).
    /// </summary>
    protected virtual void OnDrawGizmos()
    {
        if (BodyCollider == null) return;

        Vector3 forward = new Vector3(FacingDirection, 0f, 0f);

        if(_drawWallRay) DrawRayGizmo(GetWallRayOrigin(), forward, _wallCheckDistance, _wallLayer, Color.red, Color.blue);
        if(_drawEnemyRay) DrawRayGizmo(GetEnemyRayOrigin(), forward, _enemyCheckDistance, _enemyLayer, Color.yellow, Color.cyan);
        if(_drawPrecipiceRay) DrawRayGizmo(GetPrecipiceRayOrigin(), Vector3.down, _precipiceCheckDistance, _groundLayer, Color.green, Color.red);

    }
}