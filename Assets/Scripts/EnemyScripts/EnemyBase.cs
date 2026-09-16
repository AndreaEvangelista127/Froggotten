using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IDamageDealer
{
    [Header("References")]
    [SerializeField] protected Transform _sprite;
    [SerializeField] protected Transform _playerTf;

    [Header("Stats")]
    [SerializeField] protected float _damage = 1f; // Every enemy has a damage value, but it can be different for each enemy type. This is why we use protected and not private, so the child classes can change it.
    public virtual float Damage => _damage;

    [Header("Knockback")]
    [SerializeField] private float _knockbackForce = 5f;


    protected Animator _animator;
    protected Rigidbody2D _rb;
    protected bool _isDead = false;
    protected ParticleSystem _dustParticle;
                        

    protected virtual void Awake() // Virtual means that this is going to be the defualt behaviour but the child classes can change it (need to use override)
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();

    }

    // NOTE: All sprites face left by default (scale 1f = left).
    // _isFacingRight = true means the sprite has been flipped to face right (scale -1f).
    public virtual void Flip(bool isFacingRight)
    {
        if (_sprite == null) return;

        _sprite.localScale = new Vector3(isFacingRight ? 1f : -1f, 1f, 1f);

    }

    // Needed because OnCollisionEnter2D only fires once, at the start of the
    // contact — if the player remains pinned against the enemy past the end of
    // their i-frames, no new Enter event fires, and they'd stay "immune" forever.
    // TakeDamage() inside ApplyDamageAndKnockback already blocks re-damage during
    // i-frames, so this is safe to call every frame.
    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ApplyDamageAndKnockback(collision);
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            ApplyDamageAndKnockback(collision);
        }
    }

    protected void ApplyDamageAndKnockback(Collision2D collision)
    {
        Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
        DamageUtility.ApplyDamageAndKnockback(collision.gameObject, Damage, knockbackDirection, _knockbackForce);
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
