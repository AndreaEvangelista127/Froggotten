using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    [Header("Player Component References")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] SpriteRenderer playerSpriteR;
    [SerializeField] Animator animator;
    [SerializeField] Collider2D playerCollider;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheckTransform; // Il GameObject figlio
    [SerializeField] Vector2 groundCheckSize = new Vector2(0.8f, 0.1f); // Dimensione del box
    [SerializeField] LayerMask groundLayer;

    [Header("Player Settings")]
    [SerializeField] float speed;
    [SerializeField] float jumpingForce;
    [SerializeField] float doubleJumpingForce;
    [SerializeField] private bool _canDoubleJump;
    [SerializeField] private StatePlayerMovement _statePlayerMovement;

    [Header("Variable Jump Settings")]
    [SerializeField] float maxJumpTime = 0.1f; // Tempo massimo per tenere premuto (in secondi)

    [Header("Coyote Time Settings")]
    [SerializeField] float coyoteTime = 0.15f;

    [Header("Wall Jump Settings")]
    [SerializeField] private bool _wallJumpEnabled = true;  
    [SerializeField] LayerMask wallLayer;
    [SerializeField] float wallJumpForceX = 8f; // Forza orizzontale del wall jump
    [SerializeField] float wallJumpForceY = 12f; // Forza verticale del wall jump
    [SerializeField] float wallCheckDistance = 0.2f; // Distanza per rilevare il muro
    [SerializeField] float wallJumpControlLockTime = 0.2f; // Tempo di blocco del controllo orizzontale dopo il wall jump

    [Header("Wall Slide Settings")]
    [SerializeField] private bool _wallSlideEnabled = true;
    [SerializeField] private float _wallSlideSpeed = 2f;

    [Header("Gliding Settings")]
    [SerializeField] private bool _glidingEnabled = true;
    [SerializeField] private float _glidingFallSpeed = 2f;  // Velocità caduta lenta
    [SerializeField] private float _glidingHorizontalSpeed = 5f;  // Velocità movimento orizzontale
    [SerializeField] private GameObject _lilypadSprite;  // Sprite lilypad

    [Header("Knockback Settings")]
    [SerializeField] private float _knockbackControlLockTime = 0.3f; // Tempo di blocco dopo knockback

    private float _knockbackControlTimer = 0f; // Timer per il knockback

    //Player sprite dimension
    private float _playerHalfHeight;
    private float _playerHalfWidth;
    private float _rayLength;

    //Player movement
    private float _moveValue;

    //Jump Variables
    private bool _isJumping; // Se sta saltando
    private float _jumpTimeCounter; // Quanto tempo sta tenendo premuto
    private float _currentJumpForce;
    private float _coyoteTimeCounter;
    private float _wallJumpControlTimer;

    // Gliding
    private bool _isGliding = false;
    private bool _isJumpButtonHeld = false;

    //public varibales used for animation
    public bool IsGrounded => GetIsGrounded();
    public bool IsMoving => Mathf.Abs(_moveValue) > 0.01f;
    public float VelocityY => rb.linearVelocityY;
    public float VelocityX => rb.linearVelocityX;
    public int WallDirection => GetWallJumpDirection();
    public bool IsWallSliding => CanWallSlide();
    public bool IsGliding => _isGliding;


    private void Start()
    {
        _playerHalfHeight = playerCollider.bounds.extents.y; //return the width of the sprite from the center to one side, so if it was 0.5 the total width will be 1
        _playerHalfWidth = playerCollider.bounds.extents.x;

        playerSpriteR.flipX = false;

        if(_lilypadSprite != null)
        {
            _lilypadSprite.SetActive(false); 
        }
    }

    private void FixedUpdate() //FOR PHYSICS
    {
        //Debug.Log(_knockbackControlTimer);
        // ========== Apply horizontal movement only if not in wall jump control lock or knockback control lock ==========
        if (_wallJumpControlTimer <= 0 && _knockbackControlTimer <= 0) // If we are in wall jump control lock we want to block the horizontal movement to let the player jump properly in a wall jump, otherwise the wall jump direction would have been overriden by the rb.linearVelocity = new Vector2(_moveValue * speed, rb.linearVelocity.y);
        {
            rb.linearVelocity = new Vector2(_moveValue * speed, rb.linearVelocity.y);
        }
        else
        {
            // Decrementa i timer
            if (_wallJumpControlTimer > 0)
            {
                _wallJumpControlTimer -= Time.fixedDeltaTime;
            }

            if (_knockbackControlTimer > 0) //if we are in knockback control lock we want to block all the movement to let the player be knocked back properly, otherwise the player could move during the knockback and it would feel weird and not responsive
            {
                _knockbackControlTimer -= Time.fixedDeltaTime; //blocking the player movement for a certain amount of time, as soon as we reach 0 we restore the control to the player
            }
        }

        if (CanWallSlide())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, -_wallSlideSpeed);
        }

        UpdateGliding();


        if (GetIsGrounded())
        {
            _canDoubleJump = true;
            _coyoteTimeCounter = coyoteTime; //  Reset timer when grounded

            if (_isGliding)
            {
                StopGliding();
            }
        }
        else
        {
            _coyoteTimeCounter -= Time.fixedDeltaTime; // Decremet timer while in mid air
        }

        //Debug.Log(rb.linearVelocity);
    }

    private void Update()
    {

        FlipSpriteX();

        //checking if is jumping and 
        if (_isJumping && _jumpTimeCounter > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX,_currentJumpForce); //we need to include this to avoid that the gravity force us to go down,
            _jumpTimeCounter -= Time.deltaTime; //until i press space he jumps but for certain amount of time limited to Time.deltaTime
        }
        else
        {
            _isJumping = false;
        }

        //GLIDING CHECK
        if (_glidingEnabled && _isJumpButtonHeld && CanGlide() && !_isGliding)
        {
            StartGliding();
        }

        if (_isGliding && !_isJumpButtonHeld)
        {
            StopGliding();
        }

    }

    public void Move(InputAction.CallbackContext context)
    {
        _moveValue = context.ReadValue<Vector2>().x;

        //if( _moveValue != 0)
        //{
        //    animator.SetBool("isRunning", true);
        //}
        //else
        //{
        //    animator.SetBool("isRunning", false);
        //}
    }

    public void FlipSpriteX()
    {
        //Debug.Log($"VelocityX: {rb.linearVelocityX}, flipX: {playerSpriteR.flipX}");

        // if (rb.linearVelocityX > 0)
        // {
        //    // Moving right
        //    playerSpriteR.flipX = false;
        // }
        //else if ( rb.linearVelocityX < 0) 
        // {
        //    //Moving left 
        //    playerSpriteR.flipX = true;
        //}
        float threshold = 0.1f; // Ignora velocità sotto 0.1, facendo cosi quei movimenti impercettibili dello sprite non vengono considerati ed il player non flippa a caso

        if (rb.linearVelocityX > threshold)
        {
            playerSpriteR.flipX = false; // Destra
        }
        else if (rb.linearVelocityX < -threshold)
        {
            playerSpriteR.flipX = true; // Sinistra
        }
    }


    private bool GetIsGrounded()
    {
        if (groundCheckTransform == null) return false;

        bool isGrounded = Physics2D.OverlapBox(
            groundCheckTransform.position,
            groundCheckSize,
            0f,
            groundLayer
        );

        return isGrounded;
    }

    private int GetWallJumpDirection()
    {
        if (!_wallJumpEnabled) return 0;

        if (Physics2D.Raycast(transform.position, Vector2.right, _playerHalfWidth + wallCheckDistance, wallLayer)) //wallCheckDistance as an offset to be sure
        {
            return -1; //we are jumping from a wall on the right to go left so negative value
        }
        else if(Physics2D.Raycast(transform.position, Vector2.left, _playerHalfWidth + wallCheckDistance, wallLayer)) //wallCheckDistance as an offset to be sure
        {
            return 1; //we are jumping from a wall on the left to go right so positive value
        }
        return 0;
    }

    private bool CanWallSlide()
    {
        if(!_wallSlideEnabled) return false;

        if(GetIsGrounded()) return false;

        if(rb.linearVelocityY >= 0) return false; //only when falling

        // Now we check if we are touching a wall and which side
        int wallDirection = GetWallJumpDirection();
        if(wallDirection == 0) return false; //not touching any wall

        // If we are touching a wall we need to check if we are moving towards it
        bool pressingTowardsWall = false;

        if (wallDirection == -1 && _moveValue > 0.1f)  // right wall, pressing right
        {
            pressingTowardsWall = true;
        }
        else if (wallDirection == 1 && _moveValue < -0.1f)  // left wall, pressing left
        {
            pressingTowardsWall = true;
        }

        return pressingTowardsWall;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _isJumpButtonHeld = true;
        }

        if (context.canceled)
        {
            _isJumpButtonHeld = false;
            _isJumping = false;


            if (_isGliding)
            {
                StopGliding();
            }

            if (rb.linearVelocityY > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocityX, rb.linearVelocityY * 0.5f);
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
            int wallDirection = GetWallJumpDirection();

                // WALL JUMP 
                if (wallDirection != 0 && !GetIsGrounded())
                {
                    // Salto diagonale
                    rb.linearVelocity = new Vector2(wallJumpForceX * wallDirection, wallJumpForceY); //OVERWRITTEN BY THE MOVE IN THE FIXEDUPDATE SO WE USE wallJumpControlLockTime

                    //  RESETTA il doppio salto invece di consumarlo
                    _canDoubleJump = true;

                    // If true the wall jump would be overwritten by the press and hold jump in the update method
                    _isJumping = false;

                    /* Blocking the horizontal movement in the fixedUpdate to let the player jump properly in a wall jump, otherwise the wall jump direction would
                    have been overriden by the rb.linearVelocity = new Vector2(_moveValue * speed, rb.linearVelocity.y); */
                    _wallJumpControlTimer = wallJumpControlLockTime;

                    _statePlayerMovement.SetMoveState(StatePlayerMovement.MoveState.Wall_Jump);

                    return; //without returning the player would override the wall jump with the double jump
                }
            }
            //Instead of using grounded now we check coyote timer because we want to jump when is grounded or even if the player is mid air meanwhile coyotimer is still > 0
            if (_coyoteTimeCounter > 0f)
            {
                //Debug.Log("FIRST JUMP");
                rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpingForce);

                _isJumping = true;
                _jumpTimeCounter = maxJumpTime;
                _currentJumpForce = jumpingForce;

                _coyoteTimeCounter = 0f; //exstinguish coyote time

                _statePlayerMovement.SetMoveState(StatePlayerMovement.MoveState.Jump);

            }
            // Doppio salto
            else if (_canDoubleJump)
            {
                //Debug.Log("DOUBLE JUMP");
                rb.linearVelocity = new Vector2(rb.linearVelocityX, doubleJumpingForce);

                _isJumping = true;
                _jumpTimeCounter = maxJumpTime;
                _currentJumpForce = doubleJumpingForce;

                _canDoubleJump = false;

                _statePlayerMovement.SetMoveState(StatePlayerMovement.MoveState.Double_Jump);

            }
        }

        //if (context.canceled)
        //{
        //    _isJumping = false;

        //    if (rb.linearVelocityY > 0)
        //    {
        //        rb.linearVelocity = new Vector2(rb.linearVelocityX, rb.linearVelocityY * 0.5f);// Cut the y speed because otherwise the player will be floating a little
        //    }
        //}
    }

    

    public bool CanGlide()
    {
        if (!_glidingEnabled) return false;

        if (GetIsGrounded()) return false;

        if (rb.linearVelocityY >= 0) return false;

        if (CanWallSlide()) return false;

        // The player can't glide during wall jump control lock
        if (_wallJumpControlTimer > 0) return false; 

        return true;
    }

    private void StartGliding()
    {
        _isGliding = true;

        // Attiva sprite lilypad
        if (_lilypadSprite != null)
        {
            _lilypadSprite.SetActive(true);
        }
    }

    private void StopGliding()
    {
        _isGliding = false;

        // Disattiva sprite lilypad
        if (_lilypadSprite != null)
        {
            _lilypadSprite.SetActive(false);
        }
    }

    private void UpdateGliding()
    {
        if (!_isGliding) return;

        // Velocità caduta ridotta
        if (rb.linearVelocityY < -_glidingFallSpeed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, -_glidingFallSpeed);
        }

        // Movimento orizzontale (leggermente ridotto rispetto a normale)
        float horizontalInput = _moveValue;
        rb.linearVelocity = new Vector2(horizontalInput * _glidingHorizontalSpeed, rb.linearVelocityY);
    }

    // https://discussions.unity.com/t/trying-to-get-a-knockback-function-to-work/951526
    public void ApplyKnockBack(Vector3 knockBackVel)
    { 
        

        //Block player control to have a proper knockback feeling, otherwise the player could move during the knockback and it would feel weird
        _knockbackControlTimer = _knockbackControlLockTime;

        //Actual knockback
        rb.linearVelocity = knockBackVel;      

    }

    private void OnDrawGizmos()
    {
        if (playerCollider == null) return;

        DrawGroundCheckGizmos();
        DrawWallCheckGizmos();
        DrawCoyoteTimeGizmo();
    }

    // ========== GROUND CHECK VISUALIZATION ==========
    private void DrawGroundCheckGizmos()
    {
        if (groundCheckTransform == null) return;

        bool isGrounded = Physics2D.OverlapBox(
            groundCheckTransform.position,
            groundCheckSize,
            0f,
            groundLayer
        );

        if (isGrounded)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        // Disegna il box
        Gizmos.DrawWireCube(groundCheckTransform.position, groundCheckSize);

        // Centro del ground check
        //Gizmos.DrawSphere(groundCheckTransform.position, 0.05f);
    }

    // ========== WALL CHECK VISUALIZATION ==========
    private void DrawWallCheckGizmos()
    {
        float wallCheckLen = playerCollider.bounds.extents.x + wallCheckDistance;

        // Right Wall Check
        bool wallRight = Physics2D.Raycast(transform.position, Vector2.right, wallCheckLen, wallLayer);
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
        bool wallLeft = Physics2D.Raycast(transform.position, Vector2.left, wallCheckLen, wallLayer);
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
