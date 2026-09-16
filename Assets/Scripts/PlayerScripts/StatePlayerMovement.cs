using System.Runtime.CompilerServices;
using UnityEngine;
using System;

public class StatePlayerMovement : MonoBehaviour
{

    public enum MoveState
    {
        None,
        Idle,
        Run,
        Jump,
        Fall,
        Double_Jump,
        Wall_Jump,
        Wall_Slide,
        Gliding,
    }

    [SerializeField] private Animator _animator;

    [SerializeField] private Rigidbody2D _rigidBody;

    [SerializeField] private PlayerMovement _playerMovement;

    [SerializeField] private PlayerVfx _playerVfx;


    [Header("State Thresholds")]
    [SerializeField] private float _velocityYThreshold = 0.5f; // Threshold for falling
    //[SerializeField] private float _velocityXThreshold = 0.1f; // Threshold per movement

    public MoveState currentMoveState { get; private set; }

    private const string idleAnim = "Idle";
    private const string runAnim = "Run";
    private const string jumpAnim = "Jump";
    private const string fallAnim = "Fall";
    private const string doubleJumpAnim = "Double Jump";
    private const string wallJumpAnim = "Wall Jump";
    private const string wallSlideAnim = "Wall Slide";
    private const string despawnAnim = "Despawn";

    // Is a delegate that other scripts can subscribe to in order to be notified when the player's movement state changes.
    public static Action<MoveState> OnPlayerMoveStateChanged; 

    private bool _isDespawning = false;

    private bool _explicitAirborneState = false; // Indicates if the player is in airborne state due to an explicit action (like jumping) rather than bouncing off an enemy or falling off a platform.

    private void Update()
    {
        if (_isDespawning) return;

        
        if (_playerMovement != null && _playerMovement.IsGliding)
        {
            SetMoveState(MoveState.Gliding);
            return;
        }

        if (_playerMovement != null && _playerMovement.IsWallSliding)
        {
            SetMoveState(MoveState.Wall_Slide);
            return;  
        }

        if (_playerMovement == null || _rigidBody == null) return;

        // Check with thresholds to determine if player is grounded, moving, or falling
        bool isGrounded = _playerMovement.IsGrounded;
        bool isMoving = _playerMovement.IsMoving;
        bool isFalling = _rigidBody.linearVelocity.y < -_velocityYThreshold && !isGrounded;

        bool isRisingInAir = !isGrounded && _rigidBody.linearVelocity.y > _velocityYThreshold;

        // logic to determine movement state based on velocity and player input
        if (isGrounded)
        {
            _explicitAirborneState = false;

            if (isMoving)
            {
                SetMoveState(MoveState.Run);
            }
            else
            {
                SetMoveState(MoveState.Idle);
            }
        }
        else if (isFalling)
        {
            SetMoveState(MoveState.Fall);
        }
        else if (isRisingInAir && _explicitAirborneState == false) // only true when the player is rising in the air due to an external force (like knockback or bounce pads) and not due to their own jump input
        {
            SetMoveState(MoveState.Jump);
        }
    }

    /// <summary>
    /// Transitions the player to a new movement state, triggering the appropriate
    /// animation and VFX, then notifies all subscribers via OnPlayerMoveStateChanged.
    /// </summary>
    /// <param name="moveState">The new state to transition to.</param>
    public void SetMoveState(MoveState moveState)
    {
        if (currentMoveState == moveState) return;

        switch (moveState)
        {
            case MoveState.Idle:
                HandleIdle();
                break;

            case MoveState.Run: 
                HandleRun(); 
                break;

            case MoveState.Jump:
                HandleJump();
                break;

            case MoveState.Fall:
                HandleFall();
                break;

            case MoveState.Double_Jump:
                HandleDoubleJump();
                break;

            case MoveState.Wall_Jump:
                HandleWallJump();
                break;
            case MoveState.Wall_Slide:  
                HandleWallSlide();
                break;
            case MoveState.Gliding:  
                HandleGliding();
                break;
            default:
                Debug.LogError($"Invalid movement state: {moveState}");
                break;
        }

        //The .? operator checks if there are any subscribers to the event before invoking it, preventing potential null reference exceptions.
        OnPlayerMoveStateChanged?.Invoke( moveState );
        currentMoveState = moveState;
    }


    private void HandleIdle()
    {
        if (_animator == null) return;

        _animator.Play(idleAnim);
    }

    private void HandleRun()
    {
        if (_animator == null) return;

        _animator.Play(runAnim);
    }

    private void HandleJump()
    {
        if (_animator == null) return;

        _animator.Play(jumpAnim);
        if (_playerVfx != null) _playerVfx.PlayJumpDust();
        AudioManager.Instance.PlayJumpSound();

        _explicitAirborneState = true; // claims this air session, blocks re-triggering from Update's fallback
    }

    private void HandleFall()
    {
        if (_animator == null) return;

        _animator.Play(fallAnim);

    }

    private void HandleDoubleJump()
    {
        if (_animator == null) return;

        _animator.Play(doubleJumpAnim);
        if (_playerVfx != null) _playerVfx.PlayJumpDust();
        AudioManager.Instance.PlayDoubleJumpSound();

        _explicitAirborneState = true; // claims this air session
    }

    private void HandleWallJump() 
    {
        if (_animator == null) return;

        _animator.Play(wallJumpAnim);
        if (_playerVfx != null) _playerVfx.PlayJumpDust();
        AudioManager.Instance.PlayWallJumpSound();

        _explicitAirborneState = true; // claims this air session

    }

    private void HandleWallSlide()
    {
        if (_animator == null) return;

        _animator.Play(wallSlideAnim);
    }
    private void HandleGliding()
    {
        if (_animator == null) return;

        _animator.Play(fallAnim);  
    }

    /// <summary>
    /// Triggers the despawn animation and disables player movement.
    /// Called when the player reaches the win trophy.
    /// </summary>
    public void TriggerDespawn()
    {
        _isDespawning = true;

        if (_playerMovement != null)
        {
            _playerMovement.enabled = false;
        }

        if (_animator == null) return;

        _animator.Play(despawnAnim);
    }

    /// <summary>
    /// Finalizes the despawn sequence by stopping physics, hiding the sprite, and disabling this script.
    /// Called by an animation event at the end of the despawn animation.
    /// </summary>
    public void OnDespawnComplete()
    {

        Debug.Log("StatePlayerMovement: Despawn complete, disabling player.");
        //Disable Movement
        if (_playerMovement != null) _playerMovement.enabled = false;

        // Disable physics 
        if (_rigidBody != null)
        {
            _rigidBody.linearVelocity = Vector2.zero;
            _rigidBody.gravityScale = 0;
            _rigidBody.simulated = false; 
        }

        // Disable sprite renderer, invisible
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        //Stop this script
        this.enabled = false;
    }


}
