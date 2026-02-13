using System.Collections.Generic;
using UnityEngine;

public class SawBehaviour : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private Animator _sawAnimator;
    [SerializeField] private Transform[] _destinationPositions;
    [SerializeField] private Rigidbody2D _sawRb;
    [SerializeField] private GameObject _sawChainSpritePrefab;
    [SerializeField] private float _chainSpacing = 0.5f;

    private Vector3[] _realDestinations;
    private int _currentDestIndex = 0;
    const float buffer = 0.05f;

    private void Awake() 
    {
        _realDestinations = new Vector3[_destinationPositions.Length];
        for (int i = 0; i < _destinationPositions.Length; i++)
        {
            _realDestinations[i] = _destinationPositions[i].position;
        }
        CreateChainPath();
    }


    private void CreateChainPath()
    {
        /* For each segment between two destinations, we need to calculate how many chain sprites we need to create and where to place them.
         * 
         * For example, if we have two destinations A and B that are 5 units apart, and we want to place a chain sprite every 0.5 units, we would need 10 chain sprites to cover the distance from A to B.
         * We would then calculate the position of each chain sprite along the line from A to B and instantiate the prefab at those positions.
         */
        for (int i = 0; i < _realDestinations.Length; i++)
        {
            Vector3 startPos = _realDestinations[i];

            Vector3 endPos;
            if (i == _realDestinations.Length - 1)
            {
                endPos = _realDestinations[0]; // if we are at the last destination, the next one is the first one to create a loop
            }
            else
            {
                endPos = _realDestinations[i + 1];
            }

            CreateChainsForSegment(startPos, endPos); // Create the chain sprites for the segment between startPos and endPos of this loop iteration
        }
    }

    private void CreateChainsForSegment(Vector3 startPos, Vector3 endPos)
    {
        Vector3 direction = endPos - startPos; // Calculate the direction vector from startPos to endPos

        float totalDistance = direction.magnitude; // Calculate the total distance between startPos and endPos by doing the sqrt(x^2 + y^2) of the direction vector

        int numberOfChains = Mathf.CeilToInt(totalDistance / _chainSpacing); // Calculate how many chain sprites we need to create by dividing the total distance by the desired spacing between chains and rounding up to the nearest whole number

        Vector3 normalizedDirection = direction.normalized; // Here we normalize the direction vector to get a unit vector that points in the same direction but has a magnitude of 1. This will allow us to easily calculate the position of each chain sprite along the line from startPos to endPos.

        /* Here we calculate the actual spacing between the chain sprites based on the total distance and the number of chains we need to create. 
         * This is important because if the total distance is not perfectly divisible by the desired spacing, we want to adjust the spacing so that the chain sprites are evenly distributed along the segment from startPos to endPos.
         * So that when we have for example a distance of 10.8 units and we want to place a chain every 0.5 units, we would have 22 chains (10.8 / 0.5 = 21.6, rounded up to 22)
         * but the actual spacing between the chains would be 10.8 / 22 = 0.4909 units instead of 0.5 units, to ensure that the chains are evenly spaced along the entire distance from startPos to endPos.
         */
        float actualSpacing = totalDistance / numberOfChains;


        /* Now we can loop through the number of chains we need to create and calculate the position of each chain sprite along the line from startPos to endPos.
         * Now we can calcuate 0.5 * 0, 0.5 * 1, 0.5 * 2, 0.5 * 3, etc... and add that to the startPos to get the position of each chain sprite along the line from startPos to endPos.
         */
        for (int i = 0; i < numberOfChains; i++)
        {
            float distanceFromStart = i * actualSpacing;

            // Calculate the position of the chain sprite by starting from startPos and adding the normalized direction vector multiplied by the distance from the start position for this chain sprite
            Vector3 chainPosition = startPos + (normalizedDirection * distanceFromStart);

            Instantiate(_sawChainSpritePrefab, chainPosition, Quaternion.identity);

        }
    }

    private void FixedUpdate()
    {
        if (_destinationPositions != null)
        {
            MoveToNextDestination(_realDestinations);
        }
    }

    public void MoveToNextDestination(Vector3[] destinations)
    {

        Vector3 target = destinations[_currentDestIndex];
        Vector3 currentPos = transform.position;
        Vector3 direction = target - currentPos; // Vector that points from the previous position to the target

        Vector3 nextPosition = direction.normalized * _speed * Time.fixedDeltaTime; // Calculate the next position based on the direction and speed

        //transform.position = transform.position + direction.normalized * _speed * Time.deltaTime;
        // NextPosition now contains the vector data for the next position,
        // but we need to add it to the current position to get the actual next position in world space

        _sawRb.MovePosition(currentPos + nextPosition); // Move the saw to the next position

        /* 
        Pitagorean theorem : c^2 = sqr(a^2) + sqr(b^2)) so doing dir.sqrMangnitude returns a^2 + b^2  without the need to calculate the square root,
        which is computationally expensive, and we can compare it to the buffer squared to check if we are close enough to the target 
        */
        float distanceSqr = direction.sqrMagnitude; // Calculate the squared distance from the previous position to the target (sqr(a) + sqr(b))

        if (distanceSqr < Mathf.Pow(buffer, 2f)) 
        {
            _currentDestIndex++;

            // When we reach the last one we need to loop back
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

            Vector3 currentPlatformPos = _destinationPositions[i].position;

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
                Vector3 nextPos = _destinationPositions[nextIndex].position;

                // draw line to next destination
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(currentPlatformPos, nextPos);
            }
        }
    }

    
}



