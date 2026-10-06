using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    [Header("Player Component References")]
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private SpriteRenderer _playerSpriteR;
    [SerializeField] private Animator _animator;
    [SerializeField] private Collider2D _playerCollider;

    [Header("Ground Check")]
    [SerializeField] private Transform _groundCheckTransform; 
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.8f, 0.1f); 
    [SerializeField] private LayerMask _groundLayer;

    [Header("Player Settings")]
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpingForce;
    [SerializeField] private float _doubleJumpingForce;
    [SerializeField] private bool _canDoubleJump;
    [SerializeField] private StatePlayerMovement _statePlayerMovement;

    [Header("Variable Jump Settings")]
    [SerializeField] private float _maxJumpTime = 0.1f; 

    [Header("Coyote Time Settings")]
    [SerializeField] private float _coyoteTime = 0.15f;

    [Header("Wall Jump Settings")]
    [SerializeField] private bool _wallJumpEnabled = true;  
    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] private float _wallJumpForceX = 8f; 
    [SerializeField] private float _wallJumpForceY = 12f; 
    [SerializeField] private float _wallCheckDistance = 0.2f;
    // Block horizontal control for x time after a wall jump to prevent overriding the wall jump velocity with the normal horizontal movement in the fixedUpdate
    [SerializeField] private float _wallJumpControlLockTime = 0.2f;

    [Header("Wall Slide Settings")]
    [SerializeField] private bool _wallSlideEnabled = true;
    [SerializeField] private float _wallSlideSpeed = 2f;

    [Header("Gliding Settings")]
    [SerializeField] private bool _glidingEnabled = true;
    [SerializeField] private float _glidingFallSpeed = 2f;  // fall speed while gliding
    [SerializeField] private float _glidingHorizontalSpeed = 5f;
    [SerializeField] private float _glidingAcceleration = 10f; 
    [SerializeField] private GameObject _lilypadSprite;

    [Header("Knockback Settings")]
    // Time during which player control is locked after knockback is applied, to let the player be knocked back properly without being able to move during the knockback
    [SerializeField] private float _knockbackControlLockTime = 0.3f;
    [Header("Wind Settings")]
    [SerializeField] private float _movementAcceleration = 50f; // How quickly the player accelerates to the target speed
    [SerializeField] private float _movementDeceleration = 60f; // How quickly the player decelerates to a stop

    private float _knockbackControlTimer = 0f;

    [Header("Platforms")]
    private ISurface2D _movingSurface;
    private Vector2 _surfaceVelocity;

    private Vector2 _windForce;

    //Player sprite dimension
    private float _playerHalfWidth;

    //Player movement
    private float _moveValue;

    //Jump Variables
    private bool _isJumping; 
    private float _jumpTimeCounter; 
    private float _currentJumpForce;
    private float _coyoteTimeCounter;
    private float _wallJumpControlTimer;

    // Gliding
    private bool _isGliding = false;
    private bool _isJumpButtonHeld = false;

    //The same as:
    // public bool IsGrounded{get { return GetIsGrounded(); } }
    public bool IsGrounded => GetIsGrounded();
    public bool IsMoving => Mathf.Abs(_moveValue) > 0.01f;
    public float VelocityY => _rb.linearVelocityY;
    public float VelocityX => _rb.linearVelocityX;
    public int WallDirection => GetJumpDirectionFromWall();
    public bool IsWallSliding => CanWallSlide();
    public bool IsGliding => _isGliding;
    


    private void Start()
    {
        if (_rb == null) Debug.LogWarning("PlayerMovement: Rigidbody2D not assigned!");
        if (_playerCollider == null) Debug.LogWarning("PlayerMovement: Collider2D not assigned!");

        // Calculate player half width using the collider bounds, we will use this for the wall check to be sure to start the raycast from the edge of the player and not from the center, otherwise we could have some issues with the wall jump when the player is close to a wall but not touching it because the raycast starts from the center and not from the edge
        _playerHalfWidth = _playerCollider.bounds.extents.x;

        if (_playerSpriteR != null) _playerSpriteR.flipX = false;

        if (_lilypadSprite != null)
        {
            _lilypadSprite.SetActive(false); 
        }
    }

    private void FixedUpdate() //FOR PHYSICS
    {
        if (PauseMenuManager.IsGamePaused) return;

        if (_rb == null) return;


        // =========================================================
        // HORIZONTAL MOVEMENT
        // =========================================================
        // ========== Apply horizontal movement only if not in wall jump control lock or knockback control lock ==========
        if (_wallJumpControlTimer <= 0 && _knockbackControlTimer <= 0)
        {
            // If the player is on a moving surface, get its velocity to add it to the player's movement
            if (_movingSurface != null)
                _surfaceVelocity = _movingSurface.GetVelocity();
            else
                _surfaceVelocity = Vector2.zero;

            // Handle horizontal movement based on input and surface velocity
            HandleHorizontalMovement();
        }
        else
        {
            // If in wall jump control lock or knockback control lock, decrement the timers
            if (_wallJumpControlTimer > 0)
                _wallJumpControlTimer -= Time.fixedDeltaTime;

            if (_knockbackControlTimer > 0)
                _knockbackControlTimer -= Time.fixedDeltaTime;
        }

        // =========================================================
        // WIND FORCE
        // =========================================================
        // If (windForce/ mass) roughly equals _movementAcceleration, the player will feel the wind as a constant force that will push him in the direction of the
        // wind, but he will still be able to move against it if he wants to.
        // If (windForce/ mass) is > than _movementAcceleration, the player will be pushed by the wind and will not be able to move against it.
        // If (windForce/ mass) is < than _movementAcceleration, the player will be able to move against the wind and will not be pushed by it.
        ApplyWind(); 

        // =========================================================
        // OTHER SYSTEMS
        // =========================================================

        UpdateGroundedSurface();

        if (CanWallSlide())
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocityX, -_wallSlideSpeed);
        }

        UpdateGliding();

        if (GetIsGrounded())
        {
            _canDoubleJump = true;
            _coyoteTimeCounter = _coyoteTime; //  Reset timer when grounded

            if (_isGliding)
            {
                StopGliding();
            }
        }
        else
        {
            _coyoteTimeCounter -= Time.fixedDeltaTime; // Decremet timer while in mid air
        }
    }

    /// <summary>
    /// Moves horizontal velocity toward the target speed using acceleration/
    /// deceleration rather than an instant assignment. NOTE: this per-frame
    /// correction can be counteracted by external continuous forces (see
    /// ApplyWind below) — if their magnitudes are comparable, the player
    /// effectively "runs in place" against the opposing force instead of
    /// reaching _speed.
    /// </summary>
    private void HandleHorizontalMovement()
    {
        float targetVelocityX = (_moveValue * _speed) + _surfaceVelocity.x;

        float currentVelocityX = _rb.linearVelocityX;

        float acceleration;

        if (Mathf.Abs(_moveValue) > 0.01f) // If the player is providing input, use acceleration; otherwise, use deceleration
        {
            acceleration = _movementAcceleration;
        }
        else
        {
            acceleration = _movementDeceleration;
        }

        // Smoothly move the current velocity towards the target velocity using MoveTowards
        float newVelocityX = Mathf.MoveTowards(
            currentVelocityX,
            targetVelocityX,
            acceleration * Time.fixedDeltaTime // How rapidly to change the velocity, higher values means less frames to reach the target velocity, that means more responsive movement but less smooth (snappy)
        );

        _rb.linearVelocity = new Vector2(newVelocityX, _rb.linearVelocityY);
    }

    private void ApplyWind()
    {
        if (_windForce == Vector2.zero)
            return;

        _rb.AddForce(_windForce, ForceMode2D.Force); // 2° Newton's law: F = m * a, so a = F / m, so higher mass means less acceleration(player less affected), lower mass means more acceleration (player more affected)

        // USE IN CASE OF UNBOUNDED ACCELERATION DUE TO WIND FORCE
        //float maxWindSpeed = 15f; 
        //if (_rb.linearVelocity.magnitude > maxWindSpeed)
        //{
        //    _rb.linearVelocity = _rb.linearVelocity.normalized * maxWindSpeed;
        //}
    }

    public void SetWindForce(Vector2 windForce)
    {
        _windForce = windForce;
    }

    private void Update()
    {
        if (PauseMenuManager.IsGamePaused) return;

        if (_rb == null) return;

        FlipSpriteX();

        
        if (_isJumping && _jumpTimeCounter > 0)
        {
            // Hold jump button to extend jump height up to _maxJumpTime
            _rb.linearVelocity = new Vector2(_rb.linearVelocityX,_currentJumpForce); //we need to include this to avoid that the gravity force us to go down,
            _jumpTimeCounter -= Time.deltaTime; //until i press space he jumps but for certain amount of time limited to Time.deltaTime
        }
        else
        {
            _isJumping = false;
        }

        // ========== Gliding Logic ==========
        if (_glidingEnabled && _isJumpButtonHeld && CanGlide() && !_isGliding)
        {
            StartGliding();
        }

        if (_isGliding && !_isJumpButtonHeld)
        {
            StopGliding();
        }

    }

    private void UpdateGroundedSurface()
    {
        if(_groundCheckTransform == null) return;

        Collider2D groundHit = Physics2D.OverlapBox(_groundCheckTransform.position, _groundCheckSize, 0f, _groundLayer);

        if(groundHit != null && groundHit.TryGetComponent(out ISurface2D surface))
        {
            SetMovingSurface(surface);
        }
        else
        {
            SetMovingSurface(null);
        }
    }



    /// <summary>
    /// Reads horizontal input from the Input System and stores it for use in FixedUpdate.
    /// </summary>
    public void Move(InputAction.CallbackContext context)
    {
        //to avoid the player to be able to move during the pause
        if (PauseMenuManager.IsGamePaused) return; 
        _moveValue = context.ReadValue<Vector2>().x;
    }

    public void FlipSpriteX()
    {
        if (_playerSpriteR == null) return;

        float threshold = 0.1f; // Threshold for considering the player as moving in a direction (to avoid flipping when the player is almost still)

        if (_moveValue > threshold)
        {
            _playerSpriteR.flipX = false; // Right
        }
        else if (_moveValue < -threshold)
        {
            _playerSpriteR.flipX = true; // Left
        }
    }


    private bool GetIsGrounded()
    {
        if (_groundCheckTransform == null) return false;

        bool isGrounded = Physics2D.OverlapBox(
            _groundCheckTransform.position,
            _groundCheckSize,
            0f,
            _groundLayer
        );

        return isGrounded;
    }

    /// <summary>
    /// Casts rays left and right to detect adjacent walls.
    /// </summary>
    /// <returns>-1 if wall is on the right (jump left), 1 if wall is on the left (jump right), 0 if no wall.</returns>
    private int GetJumpDirectionFromWall()
    {
        if (!_wallJumpEnabled) return 0;

        if (Physics2D.Raycast(transform.position, Vector2.right, _playerHalfWidth + _wallCheckDistance, _wallLayer)) //wallCheckDistance as an offset to be sure
        {
            return -1; //we are jumping from a wall on the right to go left so negative moving value
        }
        else if(Physics2D.Raycast(transform.position, Vector2.left, _playerHalfWidth + _wallCheckDistance, _wallLayer)) //wallCheckDistance as an offset to be sure
        {
            return 1; //we are jumping from a wall on the left to go right so positive value
        }
        return 0;
    }

    /// <summary>
    /// Returns true if the player is pressing the input directionally towards the detected wall.
    /// wallDirection: -1 = wall on right, 1 = wall on left (same convention as GetJumpDirectionFromWall)
    /// </summary>
    private bool IsPushingTowardsWall(int wallDirection)
    {
        if (wallDirection == -1 && _moveValue > 0.1f)  // wall on right, pressing right
            return true;

        if (wallDirection == 1 && _moveValue < -0.1f)  // wall on left, pressing left
            return true;

        return false;
    }

    /// <summary>
    /// Returns true when the player is falling against a wall while pressing toward it,
    /// enabling the wall slide state.
    /// </summary>
    private bool CanWallSlide()
    {
        if(!_wallSlideEnabled) return false;

        if(GetIsGrounded()) return false;

        if(_rb.linearVelocityY >= 0) return false; //only when falling

        // Now we check if we are touching a wall and which side
        int wallDirection = GetJumpDirectionFromWall();
        if(wallDirection == 0) return false; //not touching any wall

        return IsPushingTowardsWall(wallDirection);
    }

    /// <summary>
    /// Handles jump input: normal jump with coyote time, double jump, and wall jump.
    /// Also manages jump button hold for variable jump height and gliding activation.
    /// </summary>
    public void Jump(InputAction.CallbackContext context)
    {
        if (PauseMenuManager.IsGamePaused) return;

        if (context.started)
        {
            _isJumpButtonHeld = true; //Used for Gliding
        }

        if (context.canceled)
        {
            _isJumpButtonHeld = false;
            _isJumping = false;

            if (_isGliding)
            {
                StopGliding();
            }

            //If we are holding the space button we're going to have a full jump but as soon as we stop pressing the space button we cut the velocity
            if (_rb.linearVelocityY > 0)
            {
                _rb.linearVelocity = new Vector2(_rb.linearVelocityX, _rb.linearVelocityY * 0.5f);
            }
        }

        if (context.performed)
        {
            // If gliding, stop gliding when jump is pressed
            if (_isGliding)
            {
                StopGliding();
            }

            if (_wallJumpEnabled)
            {
            int wallDirection = GetJumpDirectionFromWall();

                /* ---- WALL JUMP ---- */
                if (wallDirection != 0 && !GetIsGrounded() && IsPushingTowardsWall(wallDirection))
                {
                    // Diagonal wall jump velocity
                    _rb.linearVelocity = new Vector2(_wallJumpForceX * wallDirection, _wallJumpForceY); //OVERWRITTEN BY THE MOVE IN THE FIXEDUPDATE SO WE USE wallJumpControlLockTime

                    // Reset double jump when wall jumping to allow the player to double jump after a wall jump, otherwise the player would have to touch the ground to reset the double jump and it would not be possible to do multiple wall jumps in a row without touching the ground
                    _canDoubleJump = true;

                    // If true the wall jump would be overwritten by the press and hold jump in the update method
                    _isJumping = false;

                    /* Blocking the horizontal movement in the fixedUpdate to let the player jump properly in a wall jump, otherwise the wall jump direction would
                    have been overriden by the rb.linearVelocity = new Vector2(_moveValue * speed, rb.linearVelocity.y); */
                    _wallJumpControlTimer = _wallJumpControlLockTime;

                    if (_statePlayerMovement != null)
                        _statePlayerMovement.SetMoveState(StatePlayerMovement.MoveState.Wall_Jump);

                    return; //without returning the player would override the wall jump with the double jump
                }
            }
            //Instead of using grounded now we check coyote timer because we want to jump when is grounded or even if the player is mid air meanwhile coyotimer is still > 0
            /* ---- JUMP ---- */
            if (_coyoteTimeCounter > 0f)
            {
                _rb.linearVelocity = new Vector2(_rb.linearVelocityX, _jumpingForce);

                _isJumping = true;
                _jumpTimeCounter = _maxJumpTime;
                _currentJumpForce = _jumpingForce;

                _coyoteTimeCounter = 0f; //exstinguish coyote time

                if (_statePlayerMovement != null)
                    _statePlayerMovement.SetMoveState(StatePlayerMovement.MoveState.Jump);

            }
            /* ---- DOUBLE JUMP  ---- */
            else if (_canDoubleJump)
            {
                _rb.linearVelocity = new Vector2(_rb.linearVelocityX, _doubleJumpingForce);

                _isJumping = true;
                _jumpTimeCounter = _maxJumpTime;
                _currentJumpForce = _doubleJumpingForce;

                _canDoubleJump = false;

                if (_statePlayerMovement != null)
                    _statePlayerMovement.SetMoveState(StatePlayerMovement.MoveState.Double_Jump);

            }
        }
    }

    /// <summary>
    /// Returns true when the player is airborne, falling, not wall sliding,
    /// and outside the wall jump control lock window.
    /// </summary>
    public bool CanGlide()
    {
        if (!_glidingEnabled) return false;

        if (GetIsGrounded()) return false;

        if (_rb.linearVelocityY >= 0) return false;

        if (CanWallSlide()) return false;

        // The player can't glide during wall jump control lock
        if (_wallJumpControlTimer > 0) return false; 

        return true;
    }

    private void StartGliding()
    {
        _isGliding = true;

        if (_lilypadSprite != null)
        {
            _lilypadSprite.SetActive(true);
        }
    }

    private void StopGliding()
    {
        _isGliding = false;

        if (_lilypadSprite != null)
        {
            _lilypadSprite.SetActive(false);
        }
    }

    /// <summary>
    /// Caps the fall speed and applies gliding horizontal movement while the player is gliding.
    /// </summary>
    private void UpdateGliding()
    {
        if (!_isGliding) return;

        if (_rb.linearVelocityY < -_glidingFallSpeed)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocityX, -_glidingFallSpeed);
        }

        float targetHorizontalVelocity = _moveValue * _glidingHorizontalSpeed;

        _rb.linearVelocity = new Vector2(targetHorizontalVelocity, _rb.linearVelocityY);
    }

    /// <summary>
    /// The player has the possibility to double jump during the bounce after killing an enemy
    /// </summary>
    public void ResetJumps()
    {
        _canDoubleJump = true;
        _isJumping = false;
    }

    /// <summary>
    /// Applies an instant velocity to knock the player back and locks horizontal input
    /// for a short duration to preserve the knockback feel.
    /// </summary>
    /// <param name="knockBackVel">The velocity vector to apply as knockback.</param>
    public void ApplyKnockBack(Vector3 knockBackVel)
    { 
        //Block player control to have a proper knockback feeling, otherwise the player could move during the knockback and it would feel weird
        _knockbackControlTimer = _knockbackControlLockTime;

        //Actual knockback
        _rb.linearVelocity = knockBackVel;      
    }

    /// <summary>
    /// Setter for the MovingSurface to ensure correct movement for the player
    /// when he is on it
    /// </summary>
    /// <param name="surface">Current Platform that the player is stepping on</param>
    public void SetMovingSurface(ISurface2D surface)
    {
        _movingSurface = surface;
        if (surface == null) _surfaceVelocity = Vector2.zero;
    }

    private void OnDrawGizmos()
    {
        if (_playerCollider == null) return;

        DrawGroundCheckGizmos();
        DrawWallCheckGizmos();
        DrawCoyoteTimeGizmo();
    }

    // ========== GROUND CHECK VISUALIZATION ==========
    private void DrawGroundCheckGizmos()
    {
        if (_groundCheckTransform == null) return;

        bool isGrounded = Physics2D.OverlapBox(
            _groundCheckTransform.position,
            _groundCheckSize,
            0f,
            _groundLayer
        );

        if (isGrounded)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawWireCube(_groundCheckTransform.position, _groundCheckSize);
    }

    // ========== WALL CHECK VISUALIZATION ==========
    private void DrawWallCheckGizmos()
    {
        float wallCheckLen = _playerCollider.bounds.extents.x + _wallCheckDistance;

        // Right Wall Check
        bool wallRight = Physics2D.Raycast(transform.position, Vector2.right, wallCheckLen, _wallLayer);
        if (wallRight)
        {
            Gizmos.color = Color.cyan;
        }
        else
        {
            Gizmos.color = Color.blue;
        }
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * wallCheckLen);

        //Left Wall Check
        bool wallLeft = Physics2D.Raycast(transform.position, Vector2.left, wallCheckLen, _wallLayer);
        if (wallLeft)
        {
            Gizmos.color = Color.cyan;
        }
        else
        {
            Gizmos.color = Color.blue;
        }
        Gizmos.DrawLine(transform.position, transform.position + Vector3.left * wallCheckLen);
    }

    // ========== COYOTE TIME VISUALIZATION ==========
    private void DrawCoyoteTimeGizmo()
    {
        bool isCoyoteTimeActive = _coyoteTimeCounter > 0 && !GetIsGrounded();

        if (isCoyoteTimeActive)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.3f);
        }
    }


}
