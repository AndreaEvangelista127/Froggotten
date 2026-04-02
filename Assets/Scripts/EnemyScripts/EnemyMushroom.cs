using System.Collections;
using UnityEngine;

public class EnemyMushroom : EnemyBase
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 2f;

    [Header("Wall Detection")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] private float _wallCheckDistance = 0.2f;
    [SerializeField] private float _precipiceCheckDistance = 0.5f;
    // Horizontal offset from the center of the collider to cast the precipice ray from the edge
    [SerializeField] private float _precipiceCheckOffsetX = 0.3f;

    [Header("Enemy Detection")]
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private float _enemyCheckDistance = 0.5f;

    [Header("Idle Settings")]
    [SerializeField] private float _idleDuration = 1.5f;

    private bool _isIdle = false;

    private bool _isFacingRight = false;

    private Collider2D _collider;

    protected override void Awake()
    {
        base.Awake();
        _collider = GetComponent<Collider2D>();

        if (_collider == null) Debug.LogWarning("EnemyMushroom: Collider2D not found!");

        
        Flip(_isFacingRight); //Put the sprite in the correct facing direction dictacted by this script
    }

    private void Update()
    {
        if (_isDead || _isIdle) return;

        if (HitsWall() || IsAtPrecipice() || HitsEnemy())
        {
            StartCoroutine(IdleAndFlip());
            return;
        }

        Move();
    }


    // ======== MOVEMENT ========

    /// <summary>
    /// Moves the mushroom horizontally in the current movement direction at a fixed speed.
    /// </summary>
    private void Move()
    {
        _animator.SetBool("isRunning", true);
        PlayDustParticle();

        float direction = _isFacingRight ? 1f : -1f;

        _rb.linearVelocity = new Vector2(direction * _moveSpeed, _rb.linearVelocity.y);
    }

    private IEnumerator IdleAndFlip()
    {
        _isIdle = true;

        //Stop Horizontal movement
        _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
        _animator.SetBool("isRunning", false);

        StopDustParticle();

        yield return new WaitForSeconds(_idleDuration);

        _isFacingRight = !_isFacingRight;

        Flip(_isFacingRight); //Flip the sprite

        _isIdle = false;
    }

    // ========== DETECTION ==========

    /// <summary>
    /// Casts a ray forward in the movement direction to detect walls.
    /// Returns true if a wall is detected within _wallCheckDistance.
    /// </summary>
    private bool HitsWall()
    {
        
        float direction = 1f;// Moving right

        if (_isFacingRight == false)//if moving right is false, means that he needs to go left
            direction = -1f;

        float halfWidth = _collider.bounds.extents.x;

        return Physics2D.Raycast(
            transform.position,
            new Vector2(direction, 0f),
            halfWidth + _wallCheckDistance,
            _wallLayer
        );
    }

    private bool HitsEnemy()
    {
        float direction = 1f;

        if(_isFacingRight == false) direction = -1f;
        
        float halfWidth = _collider.bounds.extents.x;
        float halfHeight = _collider.bounds.extents.y;

        Vector2 rayOrigin = new Vector2(transform.position.x + halfWidth * direction, transform.position.y - halfHeight);

        return Physics2D.Raycast(
            rayOrigin,
            new Vector2(direction, 0f),
            _enemyCheckDistance,
            _enemyLayer
        );

    }

    /// <summary>
    /// Casts a ray downward from the edge of the collider in the movement direction.
    /// Returns true if no ground is detected below, meaning a precipice is ahead.
    /// </summary>
    private bool IsAtPrecipice()
    {
        float direction = 1f;// Moving right

        if (_isFacingRight == false)//if moving right is false, means that he needs to go left
            direction = -1f;

        float halfWidth = _collider.bounds.extents.x;
        float halfHeight = _collider.bounds.extents.y;

        // Cast from the bottom edge of the collider, offset forward in the movement direction
        Vector2 origin = new Vector2(
            transform.position.x + (halfWidth + _precipiceCheckOffsetX) * direction,
            transform.position.y - halfHeight
        );

        // No ground detected below means there is a precipice ahead
        bool groundDetected = Physics2D.Raycast(origin, Vector2.down, _precipiceCheckDistance, _groundLayer);
        return !groundDetected;
    }

    //Helper method to draw rays in the editor for debugging
    private void DrawRayGizmo(Vector3 origin, Vector3 direction, float distance, LayerMask layer, Color hitColor, Color clearColor)
    {
        bool hit = Physics2D.Raycast(origin, direction, distance, layer);
        Gizmos.color = hit ? hitColor : clearColor;
        Gizmos.DrawLine(origin, origin + direction * distance);
    }

    // ========== GIZMOS ==========

    private void OnDrawGizmos()
    {
        if (_collider == null)
            _collider = GetComponent<Collider2D>();
        if (_collider == null) return;

        float dir = _isFacingRight ? 1f : -1f;
        float halfWidth = _collider.bounds.extents.x;
        float halfHeight = _collider.bounds.extents.y;

        // Wall check
        DrawRayGizmo(transform.position, new Vector3(dir, 0f), halfWidth + _wallCheckDistance, _wallLayer, Color.red, Color.blue);

        // Enemy check
        Vector3 enemyRayOrigin = new Vector3(transform.position.x + halfWidth * dir, transform.position.y - halfHeight, 0f);
         DrawRayGizmo(enemyRayOrigin, new Vector3(dir, 0f), _enemyCheckDistance, _enemyLayer, Color.yellow, Color.cyan);

        // Precipice check
        Vector3 precipiceOrigin = new Vector3(
            transform.position.x + (halfWidth + _precipiceCheckOffsetX) * dir,
            transform.position.y - halfHeight,
            0f
        );
        DrawRayGizmo(precipiceOrigin, Vector3.down, _precipiceCheckDistance, _groundLayer, Color.green, Color.red);
    }
}
