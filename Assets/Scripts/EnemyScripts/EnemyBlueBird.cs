using UnityEngine;

public class EnemyBlueBird : EnemyBase
{
    [SerializeField] private float _moveSpeed = 3f; // How fast the bird moves horizontally
    [SerializeField] private float _amplitude = 1f; // How far left and right the bird moves from its starting position

    [SerializeField] private bool _moveVertically = false;

    private Vector3 _startingPosition; // The initial position of the bird, used as the center point for horizontal movement

    private bool _isFacingRight = false;


    protected override void Awake()
    {
        base.Awake(); //Rigidbody and animator are set up in the base Awake

        _startingPosition = transform.position; // Store the starting position for horizontal movement

        Flip(_isFacingRight); // Set the initial facing direction of the bird
    }

    private void Update()
    {
        if (_isDead) return;

        if (_moveVertically)
        {
            MoveVertically();
        }
        else
        {
            MoveHorizontally();
        }

    }

    private void MoveHorizontally()
    {
        transform.position = new Vector3(_startingPosition.x + Mathf.Sin(Time.time * _moveSpeed) * _amplitude, _startingPosition.y, transform.position.z);

        float cosValue = Mathf.Cos(Time.time * _moveSpeed); //when the sin is at the max or min, the cos is 0, so we can use that to flip the bird when it reaches the end of its movement range

        if (cosValue > 0f && !_isFacingRight) // If the bird is moving right and not currently facing right, flip it to face right
        {
            _isFacingRight = true;
            Flip(_isFacingRight);
        }
        else if (cosValue < 0f && _isFacingRight) // If the bird is moving left and currently facing right, flip it to face left
        {
            _isFacingRight = false;
            Flip(_isFacingRight);
        }
    }

    private void MoveVertically()
    {
        transform.position = new Vector3(_startingPosition.x, Mathf.Sin(Time.time * _moveSpeed) * _amplitude, transform.position.z);
    }
}
