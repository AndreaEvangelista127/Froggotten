using UnityEngine;

public class PlayerVfx : MonoBehaviour
{
    [SerializeField] private ParticleSystem _jumpDustParticle;
    [SerializeField] private ParticleSystem _runParticles;
    private SpriteRenderer _spriteRenderer;


    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void PlayJumpDust()
    {
        Debug.Log("Jump dust event triggered");
        if (_jumpDustParticle != null)
        {
            Debug.Log("Jump dust particle is not null, playing particle system");
            _jumpDustParticle.Play();
        }
    }

    public void OnFootstep()
    {
        if (_spriteRenderer == null) return;

        if (_runParticles != null) _runParticles.Play();
        if(_spriteRenderer.flipX)
        {
            _runParticles.transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            _runParticles.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }

}
