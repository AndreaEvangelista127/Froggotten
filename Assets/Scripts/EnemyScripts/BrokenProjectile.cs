using UnityEngine;

public class BrokenProjectilePiece : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Vector2 _initialVelocity = new Vector2(2, 3);
    [SerializeField] private float _rotationSpeed = 360f;

    [Header("Settings")]
    [SerializeField] private float _lifetime = 2f;

    private Rigidbody2D _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        if (_rb != null)
        {
            _rb.linearVelocity = _initialVelocity;

            _rb.angularVelocity = _rotationSpeed;
        }

        Destroy(gameObject, _lifetime);
    }
}