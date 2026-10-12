using UnityEngine;

public class EnemyRino : GroundEnemyBase
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 1.0f;

    [Header("Facing")]
    // Starting direction, set from the Inspector
    [SerializeField] private bool _facingRight = false;

    [Header("Sight")]
    [SerializeField] private float _playerRayLenght = 5f;
    // Must contain the Player layer AND the wall layer: the ray stops at the
    // first thing it hits, so a wall between rhino and player blocks the view
    [SerializeField] private LayerMask _sightLayerMask;
    // 0 = ray starts at the center height, higher = starts lower
    [SerializeField] private float _sightRayOffsetY = 0f;

    protected override void Awake()
    {
        base.Awake();
        _isFacingRight = _facingRight;
        Flip(_isFacingRight);
    }

    private void Update()
    {
        if (_isDead) return;

        if (CanSeePlayer())
        {
            Debug.Log("Rino sees the player!"); // TEMP: replace with the switch to Charging
        }
    }

    /// <summary>
    /// Starting point of the sight ray: front edge of the collider, so the ray
    /// never hits the rhino's own collider.
    /// </summary>
    private Vector2 GetSightRayOrigin()
    {
        return new Vector2(
            transform.position.x + BodyCollider.bounds.extents.x * FacingDirection,
            transform.position.y - _sightRayOffsetY
        );
    }

    /// <summary>
    /// Casts a ray forward and returns true only if the FIRST thing it hits is
    /// the player. A wall in between returns false, because the ray stops there.
    /// </summary>
    private bool CanSeePlayer()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            GetSightRayOrigin(),
            new Vector2(FacingDirection, 0f),
            _playerRayLenght,
            _sightLayerMask
        );

        // hit is a struct, never null: the collider inside it is what tells
        // whether the ray hit something
        if (hit.collider == null) return false;

        return hit.collider.CompareTag("Player");
    }

    // Applies the Facing checkbox immediately in the Editor (no Play needed),
    // so sprite and gizmos follow the checkbox while you set up the level.
    private void OnValidate()
    {
        _isFacingRight = _facingRight;
        Flip(_facingRight);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos(); // shared wall / enemy / precipice rays

        if (BodyCollider == null) return;

        // Yellow when the player is visible, cyan otherwise
        if (CanSeePlayer())
        {
            Gizmos.color = Color.yellow;
        }
        else
        {
            Gizmos.color = Color.cyan;
        }

        Vector3 origin = GetSightRayOrigin();
        Vector3 direction = new Vector3(FacingDirection, 0f, 0f);

        Gizmos.DrawLine(origin, origin + direction * _playerRayLenght);
    }
}