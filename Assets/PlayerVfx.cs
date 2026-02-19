using UnityEngine;

public class PlayerVfx : MonoBehaviour
{
    [SerializeField] private ParticleSystem _dustParticle;

    public void PlayJumpDust()
    {
        if (_dustParticle != null) _dustParticle.Play();
    }

}
