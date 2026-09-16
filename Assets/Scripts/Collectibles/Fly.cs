using System;
using UnityEngine;

public class Fly : MonoBehaviour
{

    [Header("Movement")]
    [SerializeField] private float _verticalSpeed = 2f;
    [SerializeField] private float _horizontalSpeed = 1.5f;
    [SerializeField] private float _verticalDistance = 0.3f;
    [SerializeField] private float _horizontalDistance = 0.5f;

    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private Vector3 _startPosition;
    private float _previousX;
    private bool _isCollected = false;

    public event Action<Fly> Collected;


    public void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _startPosition = transform.position;
        _previousX = _startPosition.x;
    }

    private void Update()
    {
        if (_isCollected)
            return;

        float newY = _startPosition.y +
                     Mathf.Sin(Time.time * _verticalSpeed) * _verticalDistance;

        float newX = _startPosition.x +
                     Mathf.Sin(Time.time * _horizontalSpeed) * _horizontalDistance;

        transform.position = new Vector3(newX, newY, _startPosition.z);

        FlipSprite(newX);

        _previousX = newX;
    }

    /// <summary>
    /// Flips the sprite horizontally based on the horizontal movement direction.
    /// Compares the current X position to the previous frame's X to determine direction.
    /// </summary>
    /// <param name="currentX">The current X position of the collectible this frame.</param>
    public void FlipSprite(float currentX)
    {
        if (_spriteRenderer == null) return;

        // Imagine that we start from previus = 0 and newX = 0, and we are moving to the right. At the last frame, we are going to have: previous >= 3.0 and newX 2.9, so now we flip
        if (currentX > _previousX) 
        {
            _spriteRenderer.flipX = false;
        }
        else if (currentX < _previousX)
        {
            _spriteRenderer.flipX = true;
        }
    }

    /// <summary>
    /// Triggers the collect animation, plays the collectible sound, and destroys the object after a short delay.
    /// </summary>
    public void Collect()
    {
        if (_isCollected)
            return;

        _isCollected = true;

        _animator?.SetTrigger("Collect");

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayCollectibleSound();

        Collected?.Invoke(this); // Notify subscribers that this collectible has been collected

        Destroy(gameObject, 0.5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Collect();
        }
    }

}
