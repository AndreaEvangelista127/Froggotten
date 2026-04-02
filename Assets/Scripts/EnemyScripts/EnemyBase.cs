using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected Transform _sprite;
    [SerializeField] protected Transform _playerTf;
    

    protected Animator _animator;
    protected Rigidbody2D _rb;
    protected bool _isDead = false;
    protected ParticleSystem _dustParticle;


    protected virtual void Awake() // Virtual means that this is going to be the defualt behaviour but the child classes can change it (need to use override)
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _dustParticle = GetComponentInChildren<ParticleSystem>();
        Debug.Log(_dustParticle);
    }

    // NOTE: All sprites face left by default (scale 1f = left).
// _isFacingRight = true means the sprite has been flipped to face right (scale -1f).
    public virtual void Flip(bool isFacingRight)
    {
        if (_sprite == null) return;

        _sprite.localScale = new Vector3(isFacingRight ? 1f : -1f, 1f, 1f);

    }

    public virtual void OnDeath()
    {
        _isDead = true;
        StopAllCoroutines();
        this.enabled = false;
    }

    public void PlayDustParticle()
    {
        if (_dustParticle != null) _dustParticle.Play();
    }

    public void StopDustParticle()
    {
        if(_dustParticle != null) _dustParticle.Stop();
    }




}
