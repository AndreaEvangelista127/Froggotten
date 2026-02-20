using UnityEngine;
using UnityEngine.Splines;

public class LogProjectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _bulletSpeed = 5f;
    [SerializeField] private float _damage = 0.5f;
    [SerializeField] private float _lifetime = 5f;

    [Header("Break Sprites")]
    [SerializeField] private GameObject _brokenProjectilePrefab;

    private Vector2 _direction;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, _lifetime);
    }
    void FixedUpdate()
    {
        if(_rb != null)
        {
            Vector2 newPosition = _rb.position + _direction * _bulletSpeed * Time.fixedDeltaTime;
            _rb.MovePosition(_rb.position + _direction * _bulletSpeed * Time.fixedDeltaTime);
            _rb.MovePosition(newPosition);
        }
    }

    /// <summary>
    /// Sets the movement direction of the projectile and flips the sprite accordingly.
    /// </summary>
    /// <param name="dir">The desired direction vector, which will be normalized.</param>
    public void SetDirection(Vector2 dir)
    {
        _direction = dir.normalized;

        if (_direction.x > 0)
        {
            // right
            transform.localScale = new Vector3(-1, 1, 1);  
        }
        else if (_direction.x < 0)
        {
            // Left
            transform.localScale = new Vector3(1, 1, 1);   
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerHealth>().TakeDamage(_damage);
            //BreakProjectile();
            Destroy(gameObject);
        }
        else if(collision.gameObject.layer == LayerMask.NameToLayer("Ground") || collision.gameObject.layer == LayerMask.NameToLayer("Platform") || collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            BreakProjectile();
        }
    }

    /// <summary>
    /// Spawns the broken projectile prefab at the current position and destroys this object.
    /// </summary>
    private void BreakProjectile()
    {
        if (_brokenProjectilePrefab != null)
        {
            Instantiate(_brokenProjectilePrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }





}
