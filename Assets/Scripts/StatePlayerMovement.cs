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
    }

    [SerializeField] private Animator _animator;

    [SerializeField] private Rigidbody2D rigidBody;

    [Header("State Thresholds")]
    [SerializeField] private float velocityYThreshold = 0.5f; // Threshold per caduta
    [SerializeField] private float velocityXThreshold = 0.1f; // Threshold per movimento

    public MoveState currentMoveState { get; private set; }

    private const string idleAnim = "Idle";
    private const string runAnim = "Run";
    private const string jumpAnim = "Jump";
    private const string fallAnim = "Fall";
    private const string doubleJumpAnim = "Double Jump";
    private const string wallJumpAnim = "Wall Jump";

    public static Action<MoveState> OnPlayerMoveStateChanged;


    private void Update()
    {
        // Controllo con threshold
        bool isGrounded = Mathf.Abs(rigidBody.linearVelocity.y) < velocityYThreshold;
        bool isMoving = Mathf.Abs(rigidBody.linearVelocity.x) > velocityXThreshold;
        bool isFalling = rigidBody.linearVelocity.y < -velocityYThreshold;

        // Logica stati
        if (isFalling)
        {
            SetMoveState(MoveState.Fall);
        }
        else if (isGrounded)
        {
            if (isMoving)
            {
                SetMoveState(MoveState.Run);
            }
            else
            {
                SetMoveState(MoveState.Idle);
            }
        }
    }


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
            default:
                Debug.LogError($"Invalid movement state: {moveState}");
                break;
        }

        OnPlayerMoveStateChanged?.Invoke( moveState );//If nothing matches don't throw an error
        currentMoveState = moveState;
    }

    private void HandleIdle()
    {
        _animator.Play(idleAnim);
    }

    private void HandleRun()
    {
        _animator.Play(runAnim);

    }

    private void HandleJump()
    {
        _animator.Play(jumpAnim);
    }

    private void HandleFall()
    {
        _animator.Play(fallAnim);

    }

    private void HandleDoubleJump()
    {
        _animator.Play(doubleJumpAnim);
    }

    private void HandleWallJump() 
    {
        _animator.Play(wallJumpAnim);
    }
}
