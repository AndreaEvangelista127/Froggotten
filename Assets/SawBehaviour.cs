using UnityEngine;

public class SawBehaviour : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 5f;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private Animator _sawAnimator;
    [SerializeField] private Transform[] _destinationPositions;

    private Vector2 _previousPosition;
    private int _currentDestIndex = 0;
    private Rigidbody _sawRb;
    const float buffer = 0.05f;


    private void Start()
    {
        _previousPosition = transform.position;
    }

    public void MoveToNextDestination(Transform[] destinations)
    {
        Vector2 target = destinations[_currentDestIndex].position;
        Vector2 currentPos = transform.position;
        Vector2 direction = target - _previousPosition; // Vector that points from the previous position to the target

        Vector2 nextPosition = direction.normalized * _speed * Time.deltaTime; // Calculate the next position based on the direction and speed

        // NextPosition now contains the vector data for the next position,
        // but we need to add it to the current position to get the actual next position in world space

        _sawRb.MovePosition(_previousPosition + nextPosition); // Move the saw to the next position

        /* 
        Pitagorean theorem : c^2 = sqr(a^2) + sqr(b^2)) so doing dir.sqrMangnitude returns a^2 + b^2  without the need to calculate the square root,
        which is computationally expensive, and we can compare it to the buffer squared to check if we are close enough to the target 
        */
        float distanceSqr = (direction).sqrMagnitude; // Calculate the squared distance from the previous position to the target (sqr(a) + sqr(b))

        if (distanceSqr < Mathf.Pow(buffer, 2f)) {
            _currentDestIndex++;

            // Se hai finito tutte le destinazioni, ricomincia (loop)
            if (_currentDestIndex >= destinations.Length)
            {
                _currentDestIndex = 0; // Torna alla prima
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (_destinationPositions == null || _destinationPositions.Length == 0) return;

        Gizmos.color = Color.cyan;

        for (int i = 0; i < _destinationPositions.Length; i++)
        {
            if (_destinationPositions[i] == null) continue; //in case one of the destinations is not assigned

            Vector2 currentPlatformPos = _destinationPositions[i].position;

            // Draw destination point
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(currentPlatformPos, 0.3f);

            // Look for the next destination
            int nextIndex;
            if (i == _destinationPositions.Length - 1)
            {
                nextIndex = 0; // Loop back to the first destination
            }
            else
            {
                nextIndex = i + 1;
            }

            if (_destinationPositions[nextIndex] != null)
            {
                Vector2 nextPos = _destinationPositions[nextIndex].position;

                // draw line to next destination
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(currentPlatformPos, nextPos);
            }
        }
    }
}



