using System.Collections;
using UnityEngine;

public class EnemyMushroom : EnemyBase
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 2f;

    [Header("Detection")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] private float _wallCheckDistance = 0.2f;
    [SerializeField] private float _precipiceCheckDistance = 0.5f;
    // Horizontal offset from the center of the collider to cast the precipice ray from the edge
    [SerializeField] private float _precipiceCheckOffsetX = 0.3f;

    [Header("Idle Settings")]
    [SerializeField] private float _idleDuration = 1.5f;

    private bool _isIdle = false;
    private bool _movingRight = false;

    private Collider2D _collider;

    protected override void Awake()
    {
        base.Awake();
        _collider = GetComponent<Collider2D>();

        if (_collider == null) Debug.LogWarning("EnemyMushroom: Collider2D not found!");
    }

    private void Update()
    {
        if (_isDead || _isIdle) return;

        if (HitsWall() || IsAtPrecipice())
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

        float direction = 1f; // Moving right

        if (!_movingRight) //if moving right is false, means that he needs to go left
            direction = -1f;

        _rb.linearVelocity = new Vector2(direction * _moveSpeed, _rb.linearVelocity.y);
    }

    private IEnumerator IdleAndFlip()
    {
        _isIdle = true;

        //Stop Horizontal movement
        _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
        _animator.SetBool("isRunning", false);

        yield return new WaitForSeconds(_idleDuration);

        _movingRight = !_movingRight;
        Flip();

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

        if (!_movingRight)//if moving right is false, means that he needs to go left
            direction = -1f;

        float halfWidth = _collider.bounds.extents.x;

        return Physics2D.Raycast(
            transform.position,
            new Vector2(direction, 0f),
            halfWidth + _wallCheckDistance,
            _wallLayer
        );
    }

    /// <summary>
    /// Casts a ray downward from the edge of the collider in the movement direction.
    /// Returns true if no ground is detected below, meaning a precipice is ahead.
    /// </summary>
    private bool IsAtPrecipice()
    {
        
        float direction = 1f;// Moving right

        if (!_movingRight)//if moving right is false, means that he needs to go left
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

    // ========== GIZMOS ==========

    private void OnDrawGizmos()
    {
        if (_collider == null)
            _collider = GetComponent<Collider2D>();

        if (_collider == null) return;

        // Convert bool direction to float: right = 1, left = -1
        float direction = 1f;
        if (!_movingRight)
            direction = -1f;

        float halfWidth = _collider.bounds.extents.x;
        float halfHeight = _collider.bounds.extents.y;

        // ===== Wall check ray =====
        bool wallHit = Physics2D.Raycast(
            transform.position,
            new Vector2(direction, 0f),
            halfWidth + _wallCheckDistance,
            _wallLayer
        );

        // Red if wall detected, blue if clear
        if (wallHit)
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.blue;

        Gizmos.DrawLine(
            transform.position,
            transform.position + new Vector3(direction * (halfWidth + _wallCheckDistance), 0f, 0f)
        );

        // ===== Precipice check ray =====
        Vector3 precipiceOrigin = new Vector3(
            transform.position.x + (halfWidth + _precipiceCheckOffsetX) * direction,
            transform.position.y - halfHeight,
            0f
        );

        bool groundDetected = Physics2D.Raycast(
            precipiceOrigin,
            Vector2.down,
            _precipiceCheckDistance,
            _groundLayer
        );

        // Green if ground detected, red if precipice
        if (groundDetected)
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;

        Gizmos.DrawLine(precipiceOrigin, precipiceOrigin + Vector3.down * _precipiceCheckDistance);
    }
}
