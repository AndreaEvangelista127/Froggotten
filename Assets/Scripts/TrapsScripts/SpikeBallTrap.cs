using System.Threading;
using UnityEngine;

public class SpikeBallTrap : TrapBase
{
    private enum MotionType
    {
        Swing,
        FullRotation
    }

    [SerializeField] private MotionType _currentMotion = MotionType.Swing;

    [Header("Swing Motion Settings")]
    [SerializeField] private float _swingAngle = 60f;
    [SerializeField] private float _swingSpeed = 2f;

    [SerializeField] private float _rotationSpeed = 90f;
    [SerializeField] private GameObject _chainPrefab;
    [SerializeField] private Transform _ballPivot;

    [SerializeField] private float _desiredChainSpacing = 0.5f;

    private Rigidbody2D _rb;
    private float _currentAngle = 0f;
    private float _timer;

    private void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody2D>();


        GameObject chainContainer = new GameObject("ChainContainer");
        chainContainer.transform.SetParent(gameObject.transform, false);

        ChainPathUtility.CreateSegment(_ballPivot.position, gameObject.transform.position,_chainPrefab, _desiredChainSpacing, chainContainer.transform);
    }

    private void FixedUpdate()
    {
        if (_rb == null)
        {
            Debug.LogWarning("Rigidboy is null");
        }

        if(_currentMotion == MotionType.Swing)
        {
            _timer += (Time.fixedDeltaTime) * _swingSpeed;
            _currentAngle = Mathf.Sin(_timer) * _swingAngle; // swing at the velocity around the angles of _swingAngles
        }
        else
        {
            _currentAngle += Time.fixedDeltaTime * _rotationSpeed;
        }

        _rb.MoveRotation(_currentAngle);
    }

    protected override Vector2 GetKnockbackDirection(Collision2D collision)
    {
        return (collision.transform.position - _ballPivot.position).normalized;
    }

    private void OnDrawGizmos()
    {
        Vector3 ballPosition = _ballPivot.transform.position;
        Vector3 jointPosition = gameObject.transform.position;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(ballPosition, jointPosition);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(ballPosition, 0.35f);
        Gizmos.DrawSphere(jointPosition, 0.35f);
       
        
    }
}
