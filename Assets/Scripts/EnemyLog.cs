using UnityEngine;

public class EnemyLog : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _sprite;
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _shootPoint;

    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _detectionRange = 5f;

    [Header("Attack")]
    [SerializeField] private float _attackRange = 3f;
    [SerializeField] private float _attackCooldown = 2f;

    private Animator _animator;
    private Rigidbody2D _rb;
    private Transform _playerTf;
    private float _lastAttackTime;
    private bool _facingRight = true;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _playerTf = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if(_playerTf == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, _playerTf.position);

        float coolDown = _lastAttackTime + _attackCooldown;

        if (_attackRange >= distanceToPlayer && Time.time >= coolDown) // if in attack range and time.time reached the cooldown
        {
            Attack();
        }
        else if(_detectionRange >= distanceToPlayer) // if in detection range
        {
            ChasePlayer();
        }
        else
        {
            Idle();
        }
    }

    private void ChasePlayer()
    {
        _animator.SetBool("isRunning", true);

        // Move towards player
        Vector2 direction = (_playerTf.position - transform.position).normalized;
        _rb.linearVelocity = new Vector2(direction.x * _moveSpeed, _rb.linearVelocity.y);
        // Flip sprite based on movement direction
        if (direction.x > 0 && !_facingRight)
        {
            Flip();
        }
        else if (direction.x < 0 && _facingRight)
        {
            Flip();
        }
    }

    private void Idle()
    {
        _animator.SetBool("isRunning", false);
        _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
    }

    private void Attack()
    {
        _animator.SetTrigger("attack"); 
        _lastAttackTime = Time.time;
        _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
    }

    public void ShootProjectile()
    {
        // Questo metodo sarà chiamato dall'Animation Event
        if (_projectilePrefab != null && _shootPoint != null)
        {
            GameObject projectile = Instantiate(_projectilePrefab, _shootPoint.position, Quaternion.identity);

            LogProjectile projScript = projectile.GetComponent<LogProjectile>();
            if (projScript != null)
            {
                Vector2 dir = _facingRight ? Vector2.right : Vector2.left;
                Debug.Log($"Chiamo SetDirection con: {dir}, FacingRight: {_facingRight}");
                projScript.SetDirection(dir);
            }

        }
    }


    public void TakeDamage(int damage)
    {
        _animator.SetTrigger("hit");
    }

    public void Flip()
    {
        _facingRight = !_facingRight;
        float scaleX;
        if (_facingRight)
        {
            scaleX = -1f;  
        }
        else
        {
            scaleX = 1f;   
        }
        _sprite.localScale = new Vector3(scaleX, 1f, 1f);
    }


}
