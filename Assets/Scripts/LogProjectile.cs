using UnityEngine;
using UnityEngine.Splines;

public class LogProjectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _bulletSpeed = 5f;
    [SerializeField] private int _damage = 1;
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

    public void SetDirection(Vector2 dir)
    {
        _direction = dir.normalized;

        if (_direction.x > 0)
        {
            // Va a destra
            transform.localScale = new Vector3(-1, 1, 1);  
        }
        else if (_direction.x < 0)
        {
            // Va a sinistra
            transform.localScale = new Vector3(1, 1, 1);   
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            //TODO: Damage player
            BreakProjectile();
        }
        else if(collision.gameObject.layer == LayerMask.NameToLayer("Ground") || collision.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            BreakProjectile();
        }
    }

    private void BreakProjectile()
    {
        if (_brokenProjectilePrefab != null)
        {
            Instantiate(_brokenProjectilePrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }





}
