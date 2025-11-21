using System.Runtime.CompilerServices;
using Unity.Mathematics;
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
    [SerializeField] LayerMask groundLayer; // Layer del terreno
    [SerializeField] float groundCheckOffset = 0.2f; // Offset extra per sicurezza

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
    [SerializeField] float wallJumpForceX = 8f; // Forza orizzontale del wall jump
    [SerializeField] float wallJumpForceY = 12f; // Forza verticale del wall jump
    [SerializeField] float wallCheckDistance = 0.2f; // Distanza per rilevare il muro
    [SerializeField] float wallJumpControlLockTime = 0.2f; // Time to

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

    //public varibales used for animation
    public bool IsGrounded => GetIsGrounded();
    public bool IsMoving => Mathf.Abs(_moveValue) > 0.01f;
    public float VelocityY => rb.linearVelocityY;
    public float VelocityX => rb.linearVelocityX;
    public int WallDirection => GetWallJumpDirection();

    private void Start()
    {
        _playerHalfHeight = playerCollider.bounds.extents.y; //return the width of the sprite from the center to one side, so if it was 0.5 the total width will be 1
        _playerHalfWidth = playerCollider.bounds.extents.x;

        _rayLength = _playerHalfHeight + groundCheckOffset;

        playerSpriteR.flipX = false;
        Debug.Log($"Velocity iniziale: {rb.linearVelocity}");
        Debug.Log($"flipX iniziale: {playerSpriteR.flipX}");
    }

    private void FixedUpdate() //FOR PHYSICS
    {
        //  Applica movimento orizzontale SOLO se non sei in wall jump lock
        if (_wallJumpControlTimer <= 0)
        {
            rb.linearVelocity = new Vector2(_moveValue * speed, rb.linearVelocity.y);
        }
        else
        {
            // Decrementa il timer
            _wallJumpControlTimer -= Time.fixedDeltaTime;
        }

        if (GetIsGrounded())
        {
            _canDoubleJump = true;
            _coyoteTimeCounter = coyoteTime; //  Reset timer when grounded
        }
        else
        {
            _coyoteTimeCounter -= Time.fixedDeltaTime; // Decremet timer while in mid air
        }
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
        _rayLength = _playerHalfHeight + groundCheckOffset;

        Vector2 leftFootPos = new Vector2(transform.position.x - _playerHalfWidth * groundCheckOffset, transform.position.y); //vector2(Posizione centrale del player - (mezza lunghezza del player * offset)) = from where the point on the left has to start
        Vector2 rightFootPos = new Vector2(transform.position.x + _playerHalfWidth * groundCheckOffset, transform.position.y); //vector2(Posizione centrale del player + (mezza lunghezza del player * offset)) = from where the point on the right has to start

        bool leftGrounded = Physics2D.Raycast(leftFootPos, Vector2.down, _rayLength, groundLayer);
        bool rightGrounded = Physics2D.Raycast(rightFootPos, Vector2.down, _rayLength, groundLayer);

        return leftGrounded || rightGrounded;

    }

    private int GetWallJumpDirection()
    {
        if (Physics2D.Raycast(transform.position, Vector2.right, _playerHalfWidth + wallCheckDistance, groundLayer)) //wallCheckDistance as an offset to be sure
        {
            return -1; //we are jumping from a wall on the right to go left so negative value
        }
        else if(Physics2D.Raycast(transform.position, Vector2.left, _playerHalfWidth + wallCheckDistance, groundLayer)) //wallCheckDistance as an offset to be sure
        {
            return 1; //we are jumping from a wall on the left to go right so positive value
        }
        return 0;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            int wallDirection = GetWallJumpDirection();
            // WALL JUMP (priorità massima)
            if (wallDirection != 0 && !GetIsGrounded())
            {
                Debug.Log("WALL JUMP");

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
            //Instead of using grounded now we check coyote timer because we want to jump when is grounded or even if the player is mid air meanwhile coyotimer is still > 0
            if (_coyoteTimeCounter > 0f)
            {
                Debug.Log("FIRST JUMP");
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
                Debug.Log("DOUBLE JUMP");
                rb.linearVelocity = new Vector2(rb.linearVelocityX, doubleJumpingForce);

                _isJumping = true;
                _jumpTimeCounter = maxJumpTime;
                _currentJumpForce = doubleJumpingForce;

                _canDoubleJump = false;

                _statePlayerMovement.SetMoveState(StatePlayerMovement.MoveState.Double_Jump);

            }
        }

        if (context.canceled)
        {
            _isJumping = false;

            if (rb.linearVelocityY > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocityX, rb.linearVelocityY * 0.5f);// Cut the y speed because otherwise the player will be floating a little
            }
        }
    }


    private void OnDrawGizmos() 
    {
        if (playerCollider != null)
        {
            /* -----------------GROUND JUMP GIZMOS-----------------*/
            Vector2 leftFootPos = new Vector2(transform.position.x - playerCollider.bounds.extents.x * groundCheckOffset, transform.position.y);
            Vector2 rightFootPos = new Vector2(transform.position.x + playerCollider.bounds.extents.x * groundCheckOffset, transform.position.y);

            // Ray sinistro
            bool leftGrounded = Physics2D.Raycast(leftFootPos, Vector2.down, _rayLength, groundLayer);
            Gizmos.color = leftGrounded ? Color.green : Color.red;
            Gizmos.DrawLine(leftFootPos, leftFootPos + Vector2.down * _rayLength);
            Gizmos.DrawWireSphere(leftFootPos, 0.05f);

            // Ray destro
            bool rightGrounded = Physics2D.Raycast(rightFootPos, Vector2.down, _rayLength, groundLayer);
            Gizmos.color = rightGrounded ? Color.green : Color.red;
            Gizmos.DrawLine(rightFootPos, rightFootPos + Vector2.down * _rayLength);
            Gizmos.DrawWireSphere(rightFootPos, 0.05f);


            /* -----------------WALL JUMP GIZMOS-----------------*/
            float wallCheckLen = playerCollider.bounds.extents.x + wallCheckDistance;
            // Ray DESTRO(blu = nessun muro, ciano = muro rilevato)
            bool wallRight = Physics2D.Raycast(transform.position, Vector2.right, wallCheckLen, groundLayer);
            Gizmos.color = wallRight ? Color.cyan : Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.right * wallCheckLen);

            // Ray SINISTRO
            bool wallLeft = Physics2D.Raycast(transform.position, Vector2.left, wallCheckLen, groundLayer);
            Gizmos.color = wallLeft ? Color.cyan : Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.left * wallCheckLen);
        }

        if (_coyoteTimeCounter > 0 && !GetIsGrounded())
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.3f);
        }
    }

}
