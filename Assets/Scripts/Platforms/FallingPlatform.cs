using System.Collections;
using UnityEngine;

public class FallingPlatform : MonoBehaviour, ISurface2D
{
    [Header("References")]
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody2D _rb;

    [Header("Fall Settings")]
    [SerializeField] private float _fallDelay = 1.5f;
    [SerializeField] private float _fallGravityScale = 3f;  // gravity scale when falling

    [Header("Hovering Settings")]
    [SerializeField] private float _amplitudeValue = 0.2f;  // how high it bobs up and down
    [SerializeField] private float _frequencyValue = 1.5f;  // how fast it bobs

    [Header("Squish Settings")]
    [SerializeField] private float _squishAmount = 0.3f;
    [SerializeField] private float _squishDuration = 0.1f;
    [SerializeField] private float _recoverDuration = 0.3f;

    private Vector3 _startPosition;
    private Vector2 _platformDelta;
    private float _squishOffset = 0f;


    private bool _isActivated = false;  // true when player landed, starts fall sequence
    private bool _isFalling = false;    // true only when actually falling

    private void Awake()
    {
        if (_animator == null) Debug.LogWarning("FanPlatform: Animator not found!");
        if (_rb == null) Debug.LogWarning("FanPlatform: Rigidbody2D not found!");
    }

    private void Start()
    {
        _startPosition = transform.position;
    }
    private void Update()
    {
        Debug.Log($"isActivated: {_isActivated} | pos: {transform.position.y} | startPos: {_startPosition.y}");
    }

    private void FixedUpdate()
    {
        if (_isFalling) return;

        // Synusodial Movement
        Vector3 previousPosition = transform.position;

        float newY = _startPosition.y + Mathf.Sin(Time.time * _frequencyValue) * _amplitudeValue + _squishOffset; // adding the squishOffset here would modify the amount of force on the y creating this squish effect
        transform.position = new Vector3(_startPosition.x, newY, _startPosition.z);

        // Calculate delta for ISurface2D
        _platformDelta = (Vector2)(transform.position - previousPosition);
    }

    /// <summary>
    /// Detects when the player lands on top of the platform using the trigger collider.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isActivated) return;

        if (collision.CompareTag("Player"))
        {
            Activate();
        }
    }

    /// <summary>
    /// Turns off the fan, plays the off animation, and switches to dynamic physics for falling.
    /// </summary>
    private void Activate()
    {
        _isActivated = true;
        _platformDelta = Vector2.zero;

        StartCoroutine(FallAfterDelay());
    }

    /// <summary>
    /// Moves the platform down then back up using _squishOffset to simulate
    /// the weight of the player landing on it.
    /// </summary>
    private IEnumerator SquishEffect()
    {
        // Go down
        float elapsed = 0f; // how much passed after the "Squish"
        while (elapsed < _squishDuration) // Continue until we don't reach the falling duration
        {
            // Gradually move _squishOffset from 0 to -_squishAmount over _squishDuration seconds
            _squishOffset = Mathf.Lerp(0f, -_squishAmount, elapsed / _squishDuration); // Mathf.Lerp(a, b, t) interpolate between "a" and "b" based on t that goes from 0 to 1, so t = 0 return 0f, t = 0,5 return the value halfway between a and b, t = 1 return -_squishAmount
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // Come back up
        elapsed = 0f;
        while (elapsed < _recoverDuration)
        {
            _squishOffset = Mathf.Lerp(-_squishAmount, 0f, elapsed / _recoverDuration);
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        _squishOffset = 0f;
    }

    private IEnumerator FallAfterDelay()
    {
        yield return StartCoroutine(SquishEffect()); // wait for squish to finish

        _animator.SetTrigger("TurnOff");

        yield return new WaitForSeconds(_fallDelay);
        DisablePlatform();
    }

    private void DisablePlatform()
    {
        _isFalling = true;

        // Disable all colliders so the platform falls through everything
        foreach (Collider2D col in GetComponents<Collider2D>())
            col.enabled = false;

        _rb.bodyType = RigidbodyType2D.Dynamic;
        _rb.gravityScale = _fallGravityScale;

        Destroy(gameObject, 3f);
    }



    /// <summary>
    /// Returns the current velocity of the platform in world space.
    /// Used by PlayerMovement to move the player with the platform while hovering.
    /// </summary>
    public Vector2 GetVelocity()
    {
        if (_isActivated) return Vector2.zero;

        return _platformDelta / Time.fixedDeltaTime;
    }
}
