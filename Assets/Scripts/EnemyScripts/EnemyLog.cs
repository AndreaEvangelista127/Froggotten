using UnityEngine;
using System.Collections;


// Using abstract gives us the upportunity to have a model/blueprint that the enemys are going to inherit to avoid duplicated code
public class EnemyLog : EnemyBase
{
    private enum EnemyState { Idle, Chase, Attack } //Enemy States

    [Header("References")]
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _shootPoint;

    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _detectionRange = 5f;
    [SerializeField] private float _verticalTolerance = 1f;
    [SerializeField] private bool _canMove = true;

    [Header("Attack")]
    [SerializeField] private float _attackRange = 3f;
    [SerializeField] private float _timeBetweenAttacks = 1.5f;

    [Header("Audio")]
    [SerializeField] private AudioManager _audioManager;

    private bool _facingRight = false;

    //This 2 variables are used to avoid the log to start the shooting anim but now shotting the projectile
    private bool _isAttackCoroutineRunning = false;
    private bool _attackAnimationComplete = false;

    private EnemyState _currentState = EnemyState.Idle;

    protected override void Awake()
    {
        base.Awake();
        Flip(_facingRight); 
    }

    private void Update()
    {

        if (_playerTf == null || _isDead || _animator == null || _rb == null) return;

        //Each frame we check in which state the log needs to be based on the player distance
        EnemyState newState = GetStateFromDistance();

        if (newState != _currentState)
            TransitionToState(newState);

        ExecuteCurrentState();
    }

    // ========== STATE MACHINE ==========

    /// <summary>
    /// Determines the appropriate state based on the player's distance and position.
    /// </summary>
    private EnemyState GetStateFromDistance()
    {
        float distanceX = Mathf.Abs(_playerTf.position.x - transform.position.x);
        float distanceY = Mathf.Abs(_playerTf.position.y - transform.position.y);

        bool inVerticalRange = distanceY <= _verticalTolerance;

        if (distanceX <= _attackRange && inVerticalRange)
            return EnemyState.Attack;

        if (distanceX <= _detectionRange && inVerticalRange && _canMove)
            return EnemyState.Chase;

        return EnemyState.Idle;
    }

    /// <summary>
    /// Handles state transitions, stopping ongoing actions when leaving a state.
    /// </summary>
    /// <param name="newState">The state to transition into.</param>
    private void TransitionToState(EnemyState newState)
    {
        // Exit current state
        if (_currentState == EnemyState.Attack)
            StopAttack();

        _currentState = newState;
    }

    /// <summary>
    /// Executes the logic for the current active state each frame.
    /// </summary>
    private void ExecuteCurrentState()
    {
        switch (_currentState)
        {
            case EnemyState.Idle: Idle(); break;
            case EnemyState.Chase: ChasePlayer(); break;
            case EnemyState.Attack: StopAndAttack(); break;
        }
    }

    // ========== STATE BEHAVIOURS ==========

    /// <summary>
    /// Stops the enemy's movement and plays the idle animation.
    /// </summary>
    private void Idle()
    {
        _animator.SetBool("isRunning", false);
        _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
    }

    /// <summary>
    /// Moves the enemy toward the player and flips the sprite to match the movement direction.
    /// </summary>
    private void ChasePlayer()
    {
        _animator.SetBool("isRunning", true);

        Vector2 direction = (_playerTf.position - transform.position).normalized;
        _rb.linearVelocity = new Vector2(direction.x * _moveSpeed, _rb.linearVelocity.y);

        if (direction.x > 0 && !_facingRight) 
        {
            _facingRight = true; 
            Flip(_facingRight); 
        }
        else if (direction.x < 0 && _facingRight) 
        {
            _facingRight = false; 
            Flip(_facingRight); 
        }
    }

    /// <summary>
    /// Stops the enemy's movement, faces the player, and triggers the attack coroutine if not already attacking.
    /// </summary>
    private void StopAndAttack()
    {
        _animator.SetBool("isRunning", false);
        _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);

        Vector2 direction = (_playerTf.position - transform.position).normalized;
        if (direction.x > 0 && !_facingRight) { _facingRight = true; Flip(_facingRight); }
        else if (direction.x < 0 && _facingRight) { _facingRight = false; Flip(_facingRight); }

        if (!_isAttackCoroutineRunning)
            StartCoroutine(AttackCoroutine());
    }

    // ========== ATTACK ==========

    /// <summary>
    /// Handles the attack cycle: plays the attack animation and waits for
    /// the animation to complete before resetting.
    /// </summary>
    private IEnumerator AttackCoroutine()
    {
        _isAttackCoroutineRunning = true;
        _attackAnimationComplete = false;
        _animator.SetBool("isAttacking", true);

        // Wait until the animation signals completion or timeout as fallback
        float timeout = _timeBetweenAttacks + 1f;
        float elapsed = 0f;
        while (!_attackAnimationComplete && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        _animator.SetBool("isAttacking", false);

        // Cooldown before next attack
        yield return new WaitForSeconds(_timeBetweenAttacks);
        _isAttackCoroutineRunning = false;
    }

    /// <summary>
    /// Interrupts the current attack immediately.
    /// Called when the player leaves the attack range mid-attack.
    /// </summary>
    private void StopAttack()
    {
        StopAllCoroutines();
        _isAttackCoroutineRunning = false;
        _attackAnimationComplete = false;
        _animator.SetBool("isAttacking", false);
    }

    /// <summary>
    /// Instantiates a projectile at the shoot point, fired in the direction the enemy is facing.
    /// Called by an Animation Event during the attack animation.
    /// The projectile is always spawned if the animation event fires, regardless of state.
    /// </summary>
    public void ShootProjectile()
    {
        Debug.Log($"ShootProjectile called - isDead: {_isDead} - prefab: {_projectilePrefab != null} - shootPoint: {_shootPoint != null}");

        if (_isDead) return;
        if (_projectilePrefab == null || _shootPoint == null) return;

        GameObject projectile = Instantiate(_projectilePrefab, _shootPoint.position, Quaternion.identity);

        if (_audioManager != null)
            _audioManager.PlayEnemyShootSound();

        LogProjectile projScript = projectile.GetComponent<LogProjectile>();
        if (projScript != null)
        {
            Vector2 dir = _facingRight ? Vector2.right : Vector2.left;
            projScript.SetDirection(dir);
        }
    }

    /// <summary>
    /// Called by an Animation Event at the end of the attack animation.
    /// Signals the coroutine that the animation has completed and the cycle can reset.
    /// </summary>
    public void OnAttackAnimationEnd()
    {
        _attackAnimationComplete = true;
    }

}