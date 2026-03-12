using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected Transform _sprite;
    [SerializeField] protected Transform _playerTf;

    protected Animator _animator;
    protected Rigidbody2D _rb;
    protected bool _isDead = false;
    protected bool _facingRight = false;

    //public abstract void attack(); has no body but all the enemys have to implement it

    protected virtual void Awake() // Virtual means that this is going to be the defualt behaviour but the child classes can change it (need to use override)
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
    }

    public virtual void Flip()
    {
        _facingRight = !_facingRight;
        if (_sprite == null) return;

        _sprite.localScale = new Vector3(_facingRight ? -1f : 1f, 1f, 1f);
    }

    public virtual void OnDeath()
    {
        _isDead = true;
        StopAllCoroutines();
        this.enabled = false;
    }


}
