using UnityEngine;


/// <summary>
/// Controls a platform that moves between a set of destination points in a loop.
/// Implements IMovingSurface2D so that any object standing on it (like the player)
/// can query the platform's current velocity and add it to their own movement,
/// creating the illusion of being carried by the platform without parenting.
/// </summary>
public class MovingPlatform : MonoBehaviour, ISurface2D
{
    [Header("References")]
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private GameObject _chainSpritePrefab;

    [Header("Movement")]
    [SerializeField] private Transform[] _destinationPositions;
    [SerializeField] private float _speed = 3f;

    [Header("Chain")]
    [SerializeField] private bool _useChainPath = true;
    [SerializeField] private float _chainSpacing = 0.5f;

    private Vector3[] _realDestinations;
    private int _currentDestIndex = 0;
    private const float _buffer = 0.05f;


    /// <summary>
    /// The displacement vector applied to the platform this FixedUpdate frame.
    /// Stored here so GetVelocity() can convert it back to a velocity and expose it
    /// to any object implementing IMovingSurface2D 
    /// </summary>
    private Vector2 _platformDelta;

    private void Awake()
    {
        if (_destinationPositions == null || _destinationPositions.Length == 0)
        {
            Debug.LogWarning("MovingPlatform: No destination positions assigned!");
            return;
        }

        _realDestinations = new Vector3[_destinationPositions.Length];
        for (int i = 0; i < _destinationPositions.Length; i++)
        {
            _realDestinations[i] = _destinationPositions[i].position;
        }

        if (_useChainPath && _chainSpritePrefab != null)
        {
            CreateChainPath();
        }
    }

    private void FixedUpdate()
    {
        if (_rb == null || _destinationPositions == null || _destinationPositions.Length == 0) return;
        MoveToNextDestination();

    }

    /// <summary>
    /// Moves the platform toward the current destination point using MovePosition.
    /// Calculates and stores _platformDelta — the exact displacement applied this frame —
    /// which is later used by GetVelocity() to inform the player of how fast the platform is moving.
    /// Uses a dynamic buffer to prevent the platform from overshooting at high speeds.
    /// </summary>
    private void MoveToNextDestination()
    {
        Vector3 target = _realDestinations[_currentDestIndex];
        Vector3 currentPos = _rb.position;
        Vector3 direction = target - currentPos;

        // Dynamic buffer: grows with speed to ensure the platform never overshoots the destination
        float dynamicBuffer = Mathf.Max(_buffer, _speed * Time.fixedDeltaTime);

        if (direction.sqrMagnitude < dynamicBuffer * dynamicBuffer)
        {
            _currentDestIndex = (_currentDestIndex + 1) % _realDestinations.Length;
            _platformDelta = Vector2.zero; // Platform is not moving this frame, so delta is zero
            return;
        }

        // Calculate the displacement for this frame: direction * speed * deltaTime
        // This is stored in _platformDelta so GetVelocity() can expose it to the player
        _platformDelta = (Vector2)direction.normalized * _speed * Time.fixedDeltaTime; // salvo il delta calcolato
        _rb.MovePosition((Vector2)currentPos + _platformDelta);
    }

    // --- Chain path same desription as the saw ---
    private void CreateChainPath()
    {
        for (int i = 0; i < _realDestinations.Length; i++)
        {
            Vector3 startPos = _realDestinations[i];
            Vector3 endPos = (i == _realDestinations.Length - 1)
                ? _realDestinations[0]
                : _realDestinations[i + 1];

            CreateChainsForSegment(startPos, endPos);
        }
    }

    private void CreateChainsForSegment(Vector3 startPos, Vector3 endPos)
    {
        Vector3 direction = endPos - startPos;
        float totalDistance = direction.magnitude;
        int numberOfChains = Mathf.CeilToInt(totalDistance / _chainSpacing);
        Vector3 normalizedDirection = direction.normalized;
        float actualSpacing = totalDistance / numberOfChains;

        for (int i = 0; i < numberOfChains; i++)
        {
            Vector3 chainPosition = startPos + normalizedDirection * (i * actualSpacing);
            Instantiate(_chainSpritePrefab, chainPosition, Quaternion.identity);
        }
    }


    /// <summary>
    /// Returns the current velocity of the platform in world space.
    ///
    /// HOW IT WORKS:
    /// _platformDelta is the raw displacement applied to the platform this frame (in units).
    /// Dividing by Time.fixedDeltaTime converts it from "units per frame" to "units per second",
    /// giving us a proper velocity vector.
    ///
    /// This velocity is read by PlayerMovement every FixedUpdate via SetMovingSurface(),
    /// and added directly to the player's own linearVelocity:
    ///     _rb.linearVelocity = new Vector2((_moveValue * _speed) + _surfaceVelocity.x, _rb.linearVelocity.y)
    ///
    /// The result: the player moves with the platform automatically, without any parenting,
    /// exactly like the 3D PlayerController pattern using IMovingSurface.
    /// </summary>
    public Vector2 GetVelocity()
    {
        return _platformDelta / Time.fixedDeltaTime; 
    }


    private void OnDrawGizmos()
    {
        if (_destinationPositions == null || _destinationPositions.Length == 0) return;

        for (int i = 0; i < _destinationPositions.Length; i++)
        {
            if (_destinationPositions[i] == null) continue;

            Vector3 current = _destinationPositions[i].position;
            int nextIndex = (i + 1) % _destinationPositions.Length;

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(current, 0.3f);

            if (_destinationPositions[nextIndex] != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(current, _destinationPositions[nextIndex].position);
            }
        }
    }
}